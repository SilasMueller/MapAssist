using MapAssist.D2Assist.Builds;
using MapAssist.Types;
using System.Threading;

namespace MapAssist.D2Assist.Runs
{
    internal class Summoner : Run
    {
        public override void Execute(Build build)
        {
            /* Summoner: */
            Town.DoTownChores();

            Movement.TakeWaypoint(1, 7);

            build.DoPrebuffs();

            Movement.MoveToQuest(false);

            build.KillSingleTarget(Npc.Summoner);

            build.ClearArea(25);
            Thread.Sleep(1000);

            Movement.LootItemsOnGround();
            Thread.Sleep(1000);

            Movement.ToTownViaPortal();
            Thread.Sleep(5000);
        }

        public override string GetName()
        {
            return "Summoner";
        }
    }
}
