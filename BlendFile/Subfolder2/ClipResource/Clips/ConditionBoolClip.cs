using static Logger;

public class ConditionBoolClip : ClipData
{
    public uint mTrackIndex;
    public uint mNumPairs;
    public uint mUpdaterType;
    public bool mChangeAnimationMidPlay;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mTrackIndex = ReadUInt32(br, $"{nameof(mTrackIndex)}");
        mNumPairs = ReadUInt32(br, $"{nameof(mNumPairs)}");
        mUpdaterType = ReadUInt32(br, $"{nameof(mUpdaterType)}");
        mChangeAnimationMidPlay = ReadUInt32(br, $"{nameof(mChangeAnimationMidPlay)}") != 0;
    }
    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eConditionBool;

        base.Write(bw);

        throw new NotImplementedException();
    }
}