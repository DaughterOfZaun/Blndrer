using static Logger;

public class TrackResource: Resource
{
    public float mBlendWeight;
    public uint mBlendMode;
    public uint mIndex;
    public string mName;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mBlendWeight = ReadSingle(br, $"{nameof(mBlendWeight)}");
        mBlendMode = ReadUInt32(br, $"{nameof(mBlendMode)}");
        mIndex = ReadUInt32(br, $"{nameof(mIndex)}");
        mName = ReadCString(br, 32, $"{nameof(mName)}");
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteSingle(bw, mBlendWeight, $"{nameof(mBlendWeight)}");
        WriteUInt32(bw, mBlendMode, $"{nameof(mBlendMode)}");
        WriteUInt32(bw, mIndex, $"{nameof(mIndex)}");
        WriteCString(bw, mName, 32, $"{nameof(mName)}");
    }
}