using static Logger;

public class BinaryHeader : Writable
{
    public uint mEngineType = 845427570u;
    public uint mBinaryBlockType = 1684958306u;
    public uint mBinaryBlockVersion = 1u;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mEngineType = ReadUInt32(br, $"{nameof(mEngineType)}");
        mBinaryBlockType = ReadUInt32(br, $"{nameof(mBinaryBlockType)}");
        mBinaryBlockVersion = ReadUInt32(br, $"{nameof(mBinaryBlockVersion)}");
        Assert(
            mEngineType == 845427570u &&
            mBinaryBlockType == 1684958306u &&
            mBinaryBlockVersion == 1u
        );
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteUInt32(bw, mEngineType, $"{nameof(mEngineType)}");
        WriteUInt32(bw, mBinaryBlockType, $"{nameof(mBinaryBlockType)}");
        WriteUInt32(bw, mBinaryBlockVersion, $"{nameof(mBinaryBlockVersion)}");
    }
}