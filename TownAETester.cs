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
using ff14bot.Objects;
using ff14bot.Pathing;
using ff14bot.Pathing.Service_Navigation;
using LlamaLibrary.Logging;
using LlamaLibrary.Memory;
using LlamaLibrary.Memory.Attributes;
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

        private static class Offsets
        {
            [Offset("Search 48 39 0D ?? ?? ?? ?? 48 8B F9 Add 3 TraceRelative")]
            internal static IntPtr GatheringCallBack;

            [Offset("Search 48 8B AE ?? ?? ?? ?? 48 85 ED 74 ?? 48 89 7C 24 ?? Add 3 Read32")]
            internal static int GatheringItemOffset;

            [Offset(
                "Search 48 81 EF ?? ?? ?? ?? 48 8B CF E8 ?? ?? ?? ?? 48 83 EB ?? 75 ?? BA ?? ?? ?? ?? Add 3 Read32")]
            internal static int GatheringItemSize;

            [Offset("Search 48 8D BD ? ? ? ? 66 0F 1F 84 00 ? ? ? ? 48 81 EF ? ? ? ? Add 3 Read32")]
            internal static int GatheringEnd;

            [Offset("Search 48 8B AE ? ? ? ? 48 85 ED 74 ? 48 89 7C 24 ? Add 3 Read32")]
            internal static int GatheringStart;

            [Offset("Search BB ? ? ? ? 48 8D BD ? ? ? ? 66 0F 1F 84 00 ? ? ? ? 48 81 EF ? ? ? ? Add 1 Read8")]
            internal static int GatheringCount;
        }


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

        public static List<GatheringPointObject> NodeList => GameObjectManager.GetObjectsOfType<GatheringPointObject>()
            .OrderBy(r => r.Distance()).ToList();


        internal static IntPtr CallbackOffset => Core.Memory.Read<IntPtr>(Offsets.GatheringCallBack);

        internal static IntPtr GatheringItemOffset =>
            Core.Memory.Read<IntPtr>(CallbackOffset + Offsets.GatheringItemOffset) +
            (Offsets.GatheringEnd - (Offsets.GatheringItemSize * Offsets.GatheringCount));

        public async Task testGather()
        {
            var GatherLock = Core.Memory.Read<uint>(LlamaLibrary.Memory.Offsets.Conditions + 0x2A);
            Log.Information("in Test Gather");

            if (GatheringManager.WindowOpen)
            {
                GatheringItem items =
                    GatheringManager.GatheringWindowItems.FirstOrDefault(i => i.IsFilled && i.CanGather);

                Log.Information($"Gathering: {items}");

                while (GatheringManager.SwingsRemaining > 0)
                {
                    items.GatherItem();
                    await Coroutine.Wait(20000,
                        () => Core.Memory.Read<uint>(LlamaLibrary.Memory.Offsets.Conditions + 0x2A) != 0);
                    await Coroutine.Wait(20000,
                        () => Core.Memory.Read<uint>(LlamaLibrary.Memory.Offsets.Conditions + 0x2A) == 0);
                }
            }
        }

        private async Task<bool> Run()
        {
            if (GatheringManager.WindowOpen)
            {
                Log.Information($"{Offsets.GatheringItemSize.ToString("X")}");
                
                for (uint num = 0; num < Offsets.GatheringCount; num++)
                {
                    var pointer = GatheringItemOffset + (int) (num * Offsets.GatheringItemSize);
                    var item = Core.Memory.Read<GatheringStruct>(pointer);

                    if (!item.isFilled)
                    {
                        continue;
                    }

                    Log.Information($"{pointer.ToString("X")}");


                    Log.Information($"{num} {ff14bot.Helpers.Utils.DynamicString(item)}");
                }

            }

            TreeRoot.Stop("Stop Requested");

            return true;
        }

        private async Task<bool> Run2()
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