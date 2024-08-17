using static Logger;

public class ParallelClip : ClipData
{
    public uint mClipFlag;
    public uint mNumClips;
    public uint[] mClips; // Name made up
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        long mClipFlagPtr = ReadAddr(br, $"{nameof(mClipFlagPtr)}");
        mNumClips = ReadUInt32(br, $"{nameof(mNumClips)}");
        mClips = ReadArr(br, null, mNumClips, br => ReadUInt32(br, "UInt32"), $"{nameof(mClips)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eParallel;

        base.Write(bw);

        WriteUInt32(bw, 0, $"mClipFlagPtr"); //TODO:
        WriteUInt32(bw, mNumClips, $"{nameof(mNumClips)}");
        WriteArr(bw, null, mClips, (bw, v) => WriteUInt32(bw, v, "UInt32"), $"{nameof(mClips)}");
    }
}