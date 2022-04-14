﻿using MapAssist.D2Assist.Builds;
using MapAssist.Types;
using System.Linq;
using System.Threading;

namespace MapAssist.D2Assist.Runs
{
    internal class Countess : Run
    {
        public override void Execute(Build build)
        {
            /* Countess: */
            Town.DoTownChores();

            Movement.TakeWaypoint(0, 4);

            build.DoPrebuffs();

            Movement.MoveToQuest(true);

            for (var i = 0; i < 5; i++)
            {
                Movement.MoveToNextArea();
            }

            var chest = Core.GetAreaData().PointsOfInterest.FirstOrDefault(x => x.Type == PoiType.SuperChest);

            if (chest != null)
            {
                Movement.MoveToPoint(chest.Position);

                build.KillSingleTargetSuperUnique(Npc.DarkStalker);

                build.ClearArea();
                Thread.Sleep(2000);

                Movement.LootItemsOnGround();
            }

            Movement.ToTownViaPortal();

            Thread.Sleep(5000);
        }

        public override string GetName()
        {
            return "Countess";
        }
    }
}
