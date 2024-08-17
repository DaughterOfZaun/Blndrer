using static Logger;

public class JointSnapEventData : BaseEventData
{
    public float mEndFrame;
    public ushort mJointToOverrideIdx;
    public ushort mJointToSnapToIdx;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mEndFrame = ReadSingle(br, $"{nameof(mEndFrame)}");
        mJointToOverrideIdx = ReadUInt16(br, $"{nameof(mJointToOverrideIdx)}");
        mJointToSnapToIdx = ReadUInt16(br, $"{nameof(mJointToSnapToIdx)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mEventTypeId = EventType.JointSnapEventData;

        base.Write(bw);

        WriteSingle(bw, mEndFrame, $"{nameof(mEndFrame)}");
        WriteUInt16(bw, mJointToOverrideIdx, $"{nameof(mJointToOverrideIdx)}");
        WriteUInt16(bw, mJointToSnapToIdx, $"{nameof(mJointToSnapToIdx)}");
    }
}