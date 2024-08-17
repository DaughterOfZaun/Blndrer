using static Logger;

public class SequencerClip : ClipData
{
    public uint mTrackIndex;
    public uint mNumPairs;
    public uint[] mPairs; // Name made up
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mTrackIndex = ReadUInt32(br, $"{nameof(mTrackIndex)}");
        mNumPairs = ReadUInt32(br, $"{nameof(mNumPairs)}");
        mPairs = ReadArr(br, null, mNumPairs, br => ReadUInt32(br, "UInt32"), $"{nameof(mPairs)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eSequencer;

        base.Write(bw);

        WriteUInt32(bw, mTrackIndex, $"{nameof(mTrackIndex)}");
        WriteUInt32(bw, mNumPairs, $"{nameof(mNumPairs)}");
        mPairs = WriteArr(bw, null, mPairs, (bw, v) => WriteUInt32(bw, v, "UInt32"), $"{nameof(mPairs)}");
    }
}