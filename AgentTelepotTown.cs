using System;
using System.Linq;
using Clio.Utilities;
using ff14bot;
using ff14bot.Managers;
using LlamaLibrary.Memory.Attributes;
using LlamaLibrary.RemoteAgents;

namespace NavigationTest
{
    public class AgentTelepotTown : AgentInterface<AgentTelepotTown>, IAgent
    {
        private static class Offsets
        {
            //0x18C9FC0
            [Offset("Search 48 8D 05 ? ? ? ? 48 C7 43 ? ? ? ? ? 48 89 03 48 8B C3 48 83 C4 ? 5B C3 ? ? ? ? ? ? ? 48 8D 05 ? ? ? ? 48 89 01 E9 ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? 40 53 48 83 EC ? 32 C0 Add 3 TraceRelative")]
            internal static IntPtr Vtable;
            
            [Offset("Search 48 89 5C 24 ? 57 48 83 EC ? 0F B6 FA 48 8B D9 48 8B 51 ?")]
            internal static IntPtr Teleport;
            
            //0x28
            [Offset("Search 48 8B 77 ? 80 7E ? ? 0F 84 ? ? ? ? Add 3 Read8")]
            internal static int PointerOffset;
            
            //0x5
            [Offset("Search 80 7E ? ? 0F 84 ? ? ? ? 0F B6 86 ? ? ? ? Add 2 Read8")]
            internal static int Count;
            
            //0x4
            [Offset("Search 40 38 7A ? 75 ? 48 8B 49 ? Add 3 Read8")]
            internal static int CurrentLocation;
        }

        public IntPtr RegisteredVtable => Offsets.Vtable;

        public IntPtr AEStructPointer => Core.Memory.Read<IntPtr>(Pointer + Offsets.PointerOffset);

        public int AECount => AEStructPointer == IntPtr.Zero ? 0 : Core.Memory.Read<byte>(AEStructPointer + Offsets.Count)-1;
        
        public byte CurrentLocation => (byte) (AEStructPointer == IntPtr.Zero ? 0 : Core.Memory.Read<byte>(AEStructPointer + Offsets.CurrentLocation));

        public TownAE[] TownAEs => Core.Memory.ReadArray<TownAE>(AEStructPointer, AECount);

        public bool TeleportToIndex(int index)
        {
            if (index == CurrentLocation) return true;
            
            lock (Core.Memory.Executor.AssemblyLock)
            {
                return Core.Memory.CallInjected64<uint>(
                    Offsets.Teleport,
                    Pointer,
                    (byte)index) == 1;
            }
        }

        public bool TeleportByAetheryteId(uint id)
        {
            var list = TownAEs;

            if (list.Any(i => i.AEKey == id))
            {
                return TeleportToIndex(list.IndexOf(list.First(i => i.AEKey == id)));
            }

            return false;
        }

        protected AgentTelepotTown(IntPtr pointer) : base(pointer)
        {
        }
    }
}