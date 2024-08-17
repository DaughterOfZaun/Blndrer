using static Logger;

public class BaseEventData : Resource
{
    protected EventType mEventTypeId;
    protected enum EventType: int
    {
        SoundEventData = 0x1,
        ParticleEventData = 0x2,
        SubmeshVisibilityEventData = 0x3,
        FadeEventData = 0x4,
        JointSnapEventData = 0x5,
        EnableLookAtEventData = 0x6,
    }
    public uint mFlags;
    public float mFrame;
    public string? mName;

    private long mNameOffset;
    public override void Read(BinaryReader br)
    {
        base.Read(br);

        mEventTypeId = (EventType)ReadUInt32(br, $"{nameof(mEventTypeId)}");
        mFlags = ReadUInt32(br, $"{nameof(mFlags)}");
        mFrame = ReadSingle(br, $"{nameof(mFrame)}");
        mNameOffset = ReadAddr(br, baseAddr, $"{nameof(mNameOffset)}");

        mName = ReadCString(br, mNameOffset, $"{nameof(mName)}");
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteUInt32(bw, (uint)mEventTypeId, $"{nameof(mEventTypeId)}");
        WriteUInt32(bw, mFlags, $"{nameof(mFlags)}");
        WriteSingle(bw, mFrame, $"{nameof(mFrame)}");
        mNameOffset = WriteAddr(bw, baseAddr, $"{nameof(mNameOffset)}");
    }
    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
        WriteCString(bw, mNameOffset, mName, $"{nameof(mName)}");
    }
    public static BaseEventData ReadStatic(BinaryReader br)
    {
        long prevPosition = br.BaseStream.Position;
        br.BaseStream.Position += 4; // mResourceSize
        //EventType mEventTypeId = (EventType)ReadUInt32(br, $"{nameof(mEventTypeId)}");
        EventType mEventTypeId = (EventType)br.ReadUInt32();
        br.BaseStream.Position = prevPosition;

        if(mEventTypeId == EventType.SoundEventData)
            return Read<SoundEventData>(br, $"<{nameof(SoundEventData)}>");
        if(mEventTypeId == EventType.ParticleEventData)
            return Read<ParticleEventData>(br, $"<{nameof(ParticleEventData)}>");
        if(mEventTypeId == EventType.SubmeshVisibilityEventData)
            return Read<SubmeshVisibilityEventData>(br, $"<{nameof(SubmeshVisibilityEventData)}>");
        if(mEventTypeId == EventType.FadeEventData)
            return Read<FadeEventData>(br, $"<{nameof(FadeEventData)}>");
        if(mEventTypeId == EventType.JointSnapEventData)
            return Read<JointSnapEventData>(br, $"<{nameof(JointSnapEventData)}>");
        if(mEventTypeId == EventType.EnableLookAtEventData)
            return Read<EnableLookAtEventData>(br, $"<{nameof(EnableLookAtEventData)}>");
        else
            //return Read<BaseEventData>(br, $"<{nameof(BaseEventData)}>");
            throw new Exception("Invalid event type");
    }
}