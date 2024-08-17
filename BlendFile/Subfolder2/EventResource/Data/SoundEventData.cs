using static Logger;

public class SoundEventData : BaseEventData
{
    public string? mSoundName;
    private long mSoundNameOffset;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mSoundNameOffset = ReadAddr(br, $"{nameof(mSoundNameOffset)}");
        mSoundName = ReadCString(br, mSoundNameOffset, $"{nameof(mSoundName)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mEventTypeId = EventType.SoundEventData;

        base.Write(bw);

        mSoundNameOffset = WriteAddr(bw, $"{nameof(mSoundNameOffset)}");
    }
    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
        WriteCString(bw, mSoundNameOffset, mSoundName, $"{nameof(mSoundName)}");
    }
}