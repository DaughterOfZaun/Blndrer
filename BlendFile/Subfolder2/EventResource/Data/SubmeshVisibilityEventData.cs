using static Logger;

public class SubmeshVisibilityEventData : BaseEventData
{
    public float mEndFrame;
    public uint mShowSubmeshHash;
    public uint mHideSubmeshHash;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mEndFrame = ReadSingle(br, $"{nameof(mEndFrame)}");
        mShowSubmeshHash = ReadUInt32(br, $"{nameof(mShowSubmeshHash)}");
        mHideSubmeshHash = ReadUInt32(br, $"{nameof(mHideSubmeshHash)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mEventTypeId = EventType.SubmeshVisibilityEventData;

        base.Write(bw);

        throw new NotImplementedException();
    }
}