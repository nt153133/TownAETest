using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace NavigationTest
{
    [StructLayout(LayoutKind.Explicit)]
    public struct GatheringStruct
    {
        //This is the key to the GatheringItems exd sheet leave this in
        [FieldOffset(0x68)] 
        public readonly ushort GatheringItemKey;

        [FieldOffset(0x6c)] 
        public readonly uint ItemID;
        
        [FieldOffset(0x78)] 
        public readonly byte Chance;

        //This replaced HQ and if it's 255 it means it doesn't get a bonus like crystals or things that couldn't be HQ before
        [FieldOffset(0x79)] 
        public readonly byte GatherersBoonChance;

        //Couldn't find an amount option in the window at all
        // [FieldOffset(0x7e)]
        // public readonly byte Amount;

        [FieldOffset(0x7e)] 
        public readonly byte Level;

        [FieldOffset(0x7f)] 
        public readonly byte Stars;
        
        //Flags are slightly different than RB enum
        [FieldOffset(0x7c)] 
        public readonly GatheringFlag GatheringFlags;

        //This one only seems to be 1 if the item is for collectable use only, could only find one example to test
        [FieldOffset(0x80)]
        public readonly byte Collectable;

        public bool isFilled => GatheringItemKey != 0;

        public string Item => DataManager.GetItem(ItemID).CurrentLocaleName;
    }
}