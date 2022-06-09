using GameOverlay.Drawing;
using MapAssist.D2Assist.Builds;
using MapAssist.Types;
using System.Threading;

namespace MapAssist.D2Assist.Runs
{
    internal class Andariel : Run
    {
        public override void Execute(Build build)
        {
            Town.DoTownChores();

            Movement.TakeWaypoint(0, 8);

            build.DoPrebuffs();

            Movement.MoveToNextArea();

            Movement.MoveToNextArea();

            build.KillSingleTarget(Npc.Andariel);

            Thread.Sleep(1000);

            build.ClearArea();

            Movement.LootItemsOnGround();

            Movement.ToTownViaPortal(build);

            Thread.Sleep(5000);
        }

        public override string GetName()
        {
            return "Andariel";
        }
    }
}
