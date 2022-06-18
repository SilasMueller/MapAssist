using GameOverlay.Drawing;
using MapAssist.D2Assist.GameInteraction;
using MapAssist.Types;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MapAssist.D2Assist.Builds
{
    internal class Hammerdin : Build
    {
        public override bool DoPrebuffs()
        {
            _log.Info("Prebuffs");
            if (!Movement.CanUseSkill(Skill.BattleCommand))
            {
                Input.KeyPress(Keys.W);
                Thread.Sleep(500);
                if (!Movement.CanUseSkill(Skill.BattleCommand))
                {
                    return false;
                }
            }

            Input.KeyPress(SkillConfig.BattleCommand);
            Thread.Sleep(500);
            Input.KeyPress(SkillConfig.BattleOrders);
            Thread.Sleep(500);

            Input.KeyPress(Keys.W);
            Thread.Sleep(500);

            Input.KeyPress(SkillConfig.HolyShield);
            Thread.Sleep(500);
            Input.KeyPress(SkillConfig.Concentration);
            Thread.Sleep(500);

            return true;
        }

        public override void KillSingleTarget(UnitMonster monster)
        {
            //TODO pruefen ob der mode auf cast steht. wenn lange nicht auf cast, dann erneut casten
            //TODO limit total runtime to x minutes
            //TODO move around if target not hit

            _log.Info("Killing single target " + monster.Npc);

            var gameData = Core.GetGameData();
            var areaData = Core.GetAreaData();

            if (Pathing.CalculateDistance(gameData.PlayerPosition, monster.Position) > 5)
            {
                Movement.MoveToPoint(new Point(monster.Position.X + 2, monster.Position.Y + 2));
                Movement.WaitForNeutral();
                gameData = Core.GetGameData();
                monster = gameData.Monsters.FirstOrDefault(x => x.UnitId == monster.UnitId && !x.IsCorpse);
                if (monster == null) return;
            }

            Movement.WaitForNeutral();

            Point screenCoord;
            (_, screenCoord) = Common.WorldToScreen(gameData, areaData, new Point(monster.Position.X + 2, monster.Position.Y + 2), gameData.PlayerPosition);
            Input.SetCursorPos(screenCoord);
            Thread.Sleep(50);
            Input.KeyPress(SkillConfig.Teleport);
            Movement.WaitForNeutral();
            gameData = Core.GetGameData();

            var counter = 0;
            var notHitCounter = 0;
            while (monster != null && monster.HealthPercentage > 0 && counter < 40 && notHitCounter < 15)
            {
                counter++;
                // reposition if we are too far away from the monster of if we have been in a suboptimal position for 2.5 seconds
                if (Pathing.CalculateDistance(gameData.PlayerPosition, monster.Position) > 4
                       || (counter % 5 == 0 && gameData.PlayerPosition != new Point(monster.Position.X + 2, monster.Position.Y + 2)))
                {
                    Input.KeyUp(SkillConfig.BlessedHammer);
                    Movement.WaitForNeutral();
                    (_, screenCoord) = Common.WorldToScreen(gameData, areaData, new Point(monster.Position.X + 2, monster.Position.Y + 2), gameData.PlayerPosition);
                    Input.SetCursorPos(screenCoord);
                    Thread.Sleep(50);
                    Input.KeyPress(SkillConfig.Teleport);
                    Movement.WaitForNeutral();
                    Input.KeyDown(SkillConfig.BlessedHammer);
                    Thread.Sleep(50);
                }

                for (var i = 0; i < 10; i++)
                {
                    Input.KeyDown(SkillConfig.BlessedHammer);
                    Thread.Sleep(50);
                }

                var oldHealth = monster.HealthPercentage;
                gameData = Core.GetGameData();
                monster = gameData.Monsters.FirstOrDefault(x => x.UnitId == monster.UnitId);
                if (monster != null && monster.HealthPercentage >= oldHealth)
                {
                    notHitCounter++;
                    if (notHitCounter % 5 == 0)
                        _log.Info("Did not hit monster (" + notHitCounter + ")");
                }
            }
            Input.KeyUp(SkillConfig.BlessedHammer);
            Thread.Sleep(50);
        }
    }
}
