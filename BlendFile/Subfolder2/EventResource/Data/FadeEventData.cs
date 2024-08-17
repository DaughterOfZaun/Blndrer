using static Logger;

public class FadeEventData : BaseEventData
{
    public float mTimeToFade;
    public float mTargetAlpha;
    public float mEndFrame;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mTimeToFade = ReadSingle(br, $"{nameof(mTimeToFade)}");
        mTargetAlpha = ReadSingle(br, $"{nameof(mTargetAlpha)}");
        mEndFrame = ReadSingle(br, $"{nameof(mEndFrame)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mEventTypeId = EventType.FadeEventData;

        base.Write(bw);

        throw new NotImplementedException();
    }
}