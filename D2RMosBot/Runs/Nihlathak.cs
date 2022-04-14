using MapAssist.D2Assist.Builds;
using MapAssist.Types;
using System.Threading;

namespace MapAssist.D2Assist.Runs
{
    internal class Nihlathak : Run
    {
        public override void Execute(Build build)
        {
            /* Nihlathak */
            Town.DoTownChores();

            Movement.TakeWaypoint(4, 5);

            build.DoPrebuffs();

            Movement.MoveToNextArea();

            Movement.MoveToQuest(false);

            build.KillSingleTarget(Npc.Nihlathak);

            build.ClearArea(25);
            Thread.Sleep(1000);

            Movement.LootItemsOnGround();
            Thread.Sleep(1000);

            Movement.ToTownViaPortal();
        }

        public override string GetName()
        {
            return "Nihlathak";
        }
    }
}
