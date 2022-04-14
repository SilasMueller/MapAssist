using GameOverlay.Windows;
using MapAssist.D2Assist.Builds;
using MapAssist.D2Assist.Runs;
using MapAssist.Helpers;
using MapAssist.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MapAssist.D2Assist
{
    public static class D2RMosBot
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private static Thread _mainThread = null;
        private static bool _running = false;
        private static Build _build;
        private static List<Run> _runs = new List<Run>();

        //TODO kauft manchmal ein town portal auch wenn buch voll ist
        //TODO send log to file

        public static Build GetBuild()
        {
            return _build;
        }

        private static void Run()
        {
            while(true)
            {

                while(_running)
                {
                    
                    if (_build.UseLifeguard) Lifeguard.Start();
                    var success = Common.StartGame(2);
                    if(!success)
                    {
                        _log.Error("Failed to start the game. Aborting");
                        _running = false;
                        break;
                    }

                    for (var i = 0; i < _runs.Count(); i++)
                    {
                        if (!_running) break;
                        BotStats.CurrentRunInSequence = i + 1;
                        BotStats.LogNewRun(_runs[i]);
                        try
                        {
                            _runs[i].Execute(_build);
                        }
                        catch(LifeguardException ex)
                        {
                            _log.Error(ex.ToString());
                            BotStats.LogFailedRun(_runs[i]);
                            break;
                        }
                        catch (Exception ex)
                        {
                            _log.Error(ex.ToString());
                            BotStats.LogFailedRun(_runs[i]);
                            try
                            {
                                Movement.ToTownViaPortal();
                            }
                            catch (Exception ex2)
                            {
                                _log.Error(ex2.ToString());
                                break;
                            }
                        }
                    }

                    if(_running) Common.ExitGame();
                    if (_build.UseLifeguard) Lifeguard.Stop();
                    Thread.Sleep(5000);
                }

                Thread.Sleep(3000);
            }
        }

        public static void Start(Build build, List<Run> runs)
        {
            if (_mainThread == null)
            {
                _mainThread = new Thread(new ThreadStart(Run));
                _mainThread.Start();
            }

            if (_running) return;

            _build = build;
            _runs = runs;

            _running = true;

            BotStats.Running = true;
            BotStats.NewRunSequence(runs);
        }

        public static void Stop()
        {
            _running = false;
            BotStats.Running = false;
        }


        public static void KeyDownHandler(object sender, KeyEventArgs args)
        {
            if (GameManager.IsGameInForeground)
            {
                if (args.KeyCode == Keys.F9)
                {
                    try
                    {
                        Movement.MoveToQuest(true);
                    }
                    catch (Exception ex)
                    {
                        _log.Info(ex.Message);
                    }
                }

                if (args.KeyCode == Keys.F10)
                {
                    Start(new Hammerdin(), new List<Run> { new Mephisto(), new Countess(), new Summoner(), new Nihlathak() });
                    //Start(new Hammerdin(), new List<Run> { new Andariel() });
                    
                    /*var build = new Hammerdin();
                    var andy = new Andariel();
                    andy.Execute(build);*/
                }

                if (args.KeyCode == Keys.F11)
                {
                    Stop();
                }

                if (args.KeyCode == Keys.F12)
                {
                    WindowHelper.GetWindowClientBounds(GameManager.MainWindowHandle, out WindowBounds rect);
                    _log.Info(Pathing.GetMovementMode());

                    var gameData = Core.GetGameData();
                    var areaData = Core.GetAreaData();
                    var pointsOfInterest = Core.GetPois();

                    var npcs = areaData.NPCs;

                    var eqItems = gameData.AllItems.Where(x => x.ItemMode == ItemMode.EQUIP);

                    var vendorItems = gameData.AllItems.Where(x => x.ItemModeMapped == ItemModeMapped.Vendor);

                    var portals = gameData.Objects.Where(x => x.IsPortal);

                    _log.Info("\n###### Portals ######");
                    foreach (var portal in portals)
                    {
                        _log.Info(portal);
                    }
                    
                    _log.Info("\n###### AllItems ######");
                    foreach (var item in gameData.AllItems)
                    {
                        _log.Info(item.ItemBaseName);
                    }

                    _log.Info("\n###### Equipped Items ######");
                    foreach (var item in eqItems)
                    {
                        _log.Info(item.ToString());
                    }

                    _log.Info("\n###### Vendor Items ######");
                    foreach (var item in vendorItems)
                    {
                        _log.Info(item.ItemBaseName);
                    }

                    _log.Info("###### NPCs ######");
                    foreach(var npc in npcs)
                    {
                        _log.Info(npc.Key + "(" + npc.Value.Count() + ", " + npc.Value[0] +")");
                    }

                    _log.Info("\n###### Items ######");
                    foreach (var item in gameData.Items)
                    {
                        _log.Info(item.ItemBaseName);
                    }

                    _log.Info("\n###### Monsters ######");
                    foreach (var monster in gameData.Monsters)
                    {
                        _log.Info(monster.Npc + "(" + monster.UnitId + ", " + monster.Position + ")");
                    }

                    var hoverData = GameMemory.GetCurrentHoverData();
                    var hoveredUnit = gameData.Objects.FirstOrDefault(obj => obj.UnitId == hoverData.UnitId);
                    _log.Info("\n###### HoverData ######");
                    _log.Info(hoverData.UnitType);
                    _log.Info(hoverData.UnitId);
                    _log.Info(hoverData.IsHovered);
                    _log.Info(hoverData.IsItemTooltip);
                    


                }
            }
        }

    }

}






/*var bestSuccessCount = 0;
var bestXOffset = -0.10f;
var bestYOffset = -0.10f;

for (var y = -0.1f; y < 0.11f; y += 0.01f)
{
    for (var x = -0.1f; x < 0.11f; x += 0.01f)
    {
        var successCount = 0;

        var xOffset = rect.Right * x;
        var yOffset = rect.Bottom * y;

        foreach (var item in itemsOnGround)
        {
            //_log.Info("#### Selecting next item ####");
            //_log.Info("(" + item.UnitId + ") " + item.ItemBaseName);
            //_log.Info("Distance: " + Pathing.CalculateDistance(gameData.PlayerPosition, item.Position));

            Common.SetCursorPos(0, 0);
            Thread.Sleep(50);
            var (_, screenCoord) = Common.WorldToScreen(gameData, areaData, item.Position, gameData.PlayerPosition);

            Common.SetCursorPos(screenCoord.X + xOffset, screenCoord.Y + yOffset);

            Thread.Sleep(50);

            var hoverData = GameMemory.GetCurrentHoverData();
            //_log.Info("Hovering unitId: " + hoverData.UnitId);
            //_log.Info("IsHovered: " + hoverData.IsHovered);

            if (hoverData.IsHovered && hoverData.UnitId == item.UnitId) successCount++;

            Thread.Sleep(100);
        }
        _log.Info("Done, SuccessCount: " + successCount + "/" + itemsOnGround.Count());
        if (successCount > bestSuccessCount)
        {
            bestSuccessCount = successCount;
            bestXOffset = x;
            bestYOffset = y;
        }

    }
}

_log.Info("############## DONE ############");
_log.Info("bestSuccessCount: " + bestSuccessCount);
_log.Info("bestXOffset: " + bestXOffset);
_log.Info("bestYOffset: " + bestYOffset);
*/

/*

bestSuccessCount: 8
bestXOffset: -0,01
bestYOffset: 0,01

 */
