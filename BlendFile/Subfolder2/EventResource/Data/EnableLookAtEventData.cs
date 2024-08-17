using static Logger;

public class EnableLookAtEventData : BaseEventData
{
    public float mEndFrame;
    public uint mEnableLookAt;
    public uint mLockCurrentValues;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mEndFrame = ReadSingle(br, $"{nameof(mEndFrame)}");
        mEnableLookAt = ReadUInt32(br, $"{nameof(mEnableLookAt)}");
        mLockCurrentValues = ReadUInt32(br, $"{nameof(mLockCurrentValues)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mEventTypeId = EventType.EnableLookAtEventData;

        base.Write(bw);

        throw new NotImplementedException();
    }
}