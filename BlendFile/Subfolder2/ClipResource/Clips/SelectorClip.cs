using static Logger;

public class SelectorClip : ClipData
{
    public uint mTrackIndex;
    public uint mNumPairs;
    public SelectorClipPair[] mPairs; // Name made up
    public class SelectorClipPair: Writable
    {
        uint mClipID;
        float mProbability;
        public override void Read(BinaryReader br)
        {
            base.Read(br);
            mClipID = ReadUInt32(br, $"{nameof(mClipID)}");
            mProbability = ReadSingle(br, $"{nameof(mProbability)}");
        }
        public override void Write(BinaryWriter bw)
        {
            base.Write(bw);
            WriteUInt32(bw, mClipID, $"{nameof(mClipID)}");
            WriteSingle(bw, mProbability, $"{nameof(mProbability)}");
        }
    };
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mTrackIndex = ReadUInt32(br, $"{nameof(mTrackIndex)}");
        mNumPairs = ReadUInt32(br, $"{nameof(mNumPairs)}");
        mPairs = ReadArr(br, null, mNumPairs, br => Read<SelectorClipPair>(br, $"<{nameof(SelectorClipPair)}>"), $"{nameof(mPairs)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eSelector;

        base.Write(bw);

        WriteUInt32(bw, mTrackIndex, $"{nameof(mTrackIndex)}");
        WriteUInt32(bw, mNumPairs, $"{nameof(mNumPairs)}");
        WriteArr(bw, null, mPairs, (bw, v) => Write<SelectorClipPair>(bw, v, $"<{nameof(SelectorClipPair)}>"), $"{nameof(mPairs)}");
    }
}