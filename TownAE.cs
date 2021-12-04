using System.Runtime.InteropServices;
using ff14bot.Managers;

namespace NavigationTest
{
    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    public struct TownAE
    {
        [FieldOffset(0x0)]
        public ushort ZoneId;

        [FieldOffset(0x3)]
        public byte HaveIt1;

        [FieldOffset(0x5)]
        public byte HaveIt2;

        [FieldOffset(0x8)]
        public ushort AEKey;

        [FieldOffset(0x10)]
        public ushort PlaceName;

        [FieldOffset(0x12)]
        public ushort PlaceName2;

        public string Name => DataManager.AetheryteCache[AEKey].CurrentLocaleAethernetName;

        public bool HaveIt => WorldManager.HasAetheryteId(AEKey);

        public string ZoneName => DataManager.ZoneNameResults[ZoneId].CurrentLocaleName;

        public override string ToString()
        {
            return $"Name: {Name}, HaveIt: {HaveIt}, ZoneName: {ZoneName}";
        }
    }
}