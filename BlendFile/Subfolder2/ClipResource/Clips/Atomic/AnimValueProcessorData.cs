using static Logger;

public class AnimValueProcessorData : Resource
{
    public ushort mProcessorType;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mProcessorType = ReadUInt16(br, $"{nameof(mProcessorType)}");
        ReadUInt16(br, "mExtBuffer");
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        throw new NotImplementedException();
    }
}