using GameOverlay.Drawing;
using MapAssist.D2Assist.Builds;
using MapAssist.Types;
using System.Linq;
using System.Threading;

namespace MapAssist.D2Assist.Runs
{
    internal class Diablo : Run
    {

        public override void Execute(Build build)
        {
            Town.DoTownChores();

            Movement.TakeWaypoint(3, 2);

            build.DoPrebuffs();

            Movement.MoveToNextArea();//TODO funktioniert nicht immer

            var areaData = Core.GetAreaData();

            var Seal1 = areaData.Objects[GameObject.DiabloSeal1][0];
            var Seal2 = areaData.Objects[GameObject.DiabloSeal2][0];
            var Seal3 = areaData.Objects[GameObject.DiabloSeal3][0];
            var Seal4 = areaData.Objects[GameObject.DiabloSeal4][0];
            var Seal5 = areaData.Objects[GameObject.DiabloSeal5][0];
            var DiabloStart = areaData.Objects[GameObject.DiabloStartPoint][0];

            // Seal 1
            ClearSeal(build, Seal1);
            Thread.Sleep(1500);
            build.KillSingleTargetSuperUnique(Npc.VenomLord); //TODO waitForSuperUnique function
            Movement.LootItemsOnGround();

            // Seal 2
            ClearSeal(build, Seal2);

            // Seal 3
            ClearSeal(build, Seal3);
            Thread.Sleep(2500);
            Movement.MoveToPoint(new Point(7773, 5173));
            build.KillSingleTargetSuperUnique(Npc.OblivionKnight);//TODO waitForSuperUnique function
            Movement.LootItemsOnGround();

            // Seal 4
            ClearSeal(build, Seal4);

            // Seal 5
            ClearSeal(build, Seal5);
            Thread.Sleep(2000);
            //Movement.MoveToPoint(new Point(7680, 5291));
            build.KillSingleTargetSuperUnique(Npc.StormCaster);//TODO waitForSuperUnique function
            Movement.LootItemsOnGround();

            // Diablo
            Movement.MoveToPoint(DiabloStart);
            WaitForDiablo();
            build.KillSingleTarget(Npc.Diablo);
            Thread.Sleep(500);
            Movement.LootItemsOnGround();

            Movement.ToTownViaPortal(build);
            Thread.Sleep(5000);
        }

        private void WaitForDiablo()
        {
            UnitMonster diablo = null;
            var counter = 0;

            while(diablo == null && counter < 150)
            {
                Thread.Sleep(100);
                diablo = Core.GetGameData().Monsters.FirstOrDefault(x => x.Npc == Npc.Diablo);
            }
        }

        private void ClearSeal(Build build, Point seal)
        {
            Movement.MoveToPoint(seal);
            build.ClearArea(15);
            Movement.MoveToPoint(seal);
            Movement.Interact(seal, UnitType.Object);
            Movement.LootItemsOnGround();
        }

        public override string GetName()
        {
            return "Diablo";
        }
    }
}
