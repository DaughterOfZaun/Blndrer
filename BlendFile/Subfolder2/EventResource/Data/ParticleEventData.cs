using static Logger;

public class ParticleEventData : BaseEventData
{
    public string? mEffectName;
    public string? mBoneName;
    public string? mTargetBoneName;
    public float mEndFrame;

    private long mEffectNameOffset;
    private long mBoneNameOffset;
    private long mTargetBoneNameOffset;
    public override void Read(BinaryReader br)
    {
        base.Read(br);

        mEffectNameOffset = ReadAddr(br, baseAddr, $"{nameof(mEffectNameOffset)}");
        mBoneNameOffset = ReadAddr(br, baseAddr, $"{nameof(mBoneNameOffset)}");
        mTargetBoneNameOffset = ReadAddr(br, baseAddr, $"{nameof(mTargetBoneNameOffset)}");
        mEndFrame = ReadSingle(br, $"{nameof(mEndFrame)}");
        
        mEffectName = ReadCString(br, mEffectNameOffset, $"{nameof(mEffectName)}");
        mBoneName = ReadCString(br, mBoneNameOffset, $"{nameof(mBoneName)}");
        mTargetBoneName = ReadCString(br, mTargetBoneNameOffset, $"{nameof(mTargetBoneName)}");
    }
    public override void Write(BinaryWriter bw)
    {
        mEventTypeId = EventType.ParticleEventData;

        base.Write(bw);

        mEffectNameOffset = WriteAddr(bw, baseAddr, $"{nameof(mEffectNameOffset)}");
        mBoneNameOffset = WriteAddr(bw, baseAddr, $"{nameof(mBoneNameOffset)}");
        mTargetBoneNameOffset = WriteAddr(bw, baseAddr, $"{nameof(mTargetBoneNameOffset)}");
        WriteSingle(bw, mEndFrame, $"{nameof(mEndFrame)}");
    }
    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
        WriteCString(bw, mEffectNameOffset, mEffectName, $"{nameof(mEffectName)}");
        WriteCString(bw, mBoneNameOffset, mBoneName, $"{nameof(mBoneName)}");
        WriteCString(bw, mTargetBoneNameOffset, mTargetBoneName, $"{nameof(mTargetBoneName)}");
    }
}