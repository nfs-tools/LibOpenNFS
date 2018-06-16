using System.IO;

namespace LibOpenNFS.Games.MW.Database.Types
{
    public class AirSupport : VltType
    {
        public uint ChopperType;
        public uint Unknown1;
        public float FuelTime;
        
        public override void Read(BinaryReader br)
        {
            ChopperType = br.ReadUInt32();
            Unknown1 = br.ReadUInt32();
            FuelTime = br.ReadSingle();
        }

        public override void Write(BinaryWriter bw)
        {
            throw new System.NotImplementedException();
        }

        public override string ToString() => $"Type: {ChopperType} - FuelTime: {FuelTime}";
    }
}