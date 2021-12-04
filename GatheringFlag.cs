using System;

namespace NavigationTest
{
    [Flags]
    public enum GatheringFlag : sbyte
    {
        GatheringUp = 1,
        GathersBoon = 2,
        Unused = 4,
        Hidden = 8,
        Rare = 16, // 0x10
        Bonus = 32, // 0x20
        Undiscovered = 64, // 0x40
    }
}