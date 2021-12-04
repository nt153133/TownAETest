using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using Buddy.Coroutines;
using Clio.Utilities;

using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using LlamaLibrary.Logging;
using LlamaLibrary.Memory;
using TreeSharp;

namespace NavigationTest
{
    public class TownAETester : BotBase
    {
        private static readonly LLogger Log = new LLogger("TownAE", Colors.Pink);

        private Composite _root;

        public override string Name => "TownAE";
        public override PulseFlags PulseFlags => PulseFlags.All;

        public override bool IsAutonomous => true;
        public override bool RequiresProfile => false;

        public override Composite Root => _root;

        public override bool WantButton { get; } = true;

        public TownAETester()
        {
            OffsetManager.Init();
            OffsetManager.SetOffsetClassesAndAgents();
        }

        public override void Start()
        {
            //Navigator.PlayerMover = new SlideMover();
            //Navigator.NavigationProvider = new ServiceNavigationProvider();
            _root = new ActionRunCoroutine(r => Run());
        }

        public override void Stop()
        {
            _root = null;
            //(Navigator.NavigationProvider as IDisposable)?.Dispose();
            //Navigator.NavigationProvider = null;
        }

        private async Task<bool> Run()
        {
            Log.Information("Nothing to test, this does nothing right now");

            if (TelepotTown.Instance.IsOpen)
            {
                Log.Information($"{AgentTelepotTown.Instance.AEStructPointer.ToString("X")}");

                var aes = AgentTelepotTown.Instance.TownAEs;

                foreach (var townAe in aes)
                {
                    Log.Information(townAe.ToString());
                }
                
                Log.Information($"Current Location: {aes[AgentTelepotTown.Instance.CurrentLocation].ToString()}");

                //Test Teleport to The Rostra in old sharlayan
                var result = AgentTelepotTown.Instance.TeleportByAetheryteId(186);
                
                Log.Information($"Result {result}");
            }
            
            
            TreeRoot.Stop("Stop Requested");

            return true;
        }
        

        
    }
}