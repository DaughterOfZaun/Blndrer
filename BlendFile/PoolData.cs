using static Logger;

public class PoolData : Resource
{
    public uint mFormatToken = 1514195421;
    public uint mVersion;
    public bool mUseCascadeBlend;
    public float mCascadeBlendValue;
    
    public BlendData[] mBlendDataAry = [];
    public TransitionClipData[] mTransitionData = [];

    public TrackResource[] mBlendTrackAry = []; // Deffered from mClassAry
    public ClipResource[] mClassAry = [];
    public MaskResource[] mMaskDataAry = []; // Deffered from mClassAry
    public EventResource[] mEventDataAry = []; // Deffered from mClassAry
    public AnimResourceBase?[] mAnimDataAry = [];
    public PathRecord[] mAnimNames = [];
    public PathRecord mSkeleton;

    private uint[] mExtBuffer = [ 0 ];
    private const int mExtBufferSize = 1;

    long mBlendDataAryAddr;
    long mTransitionDataOffset;
    long mBlendTrackAryAddr;
    long mClassAryAddr;
    long mMaskDataAryAddr;
    long mEventDataAryAddr;
    long mAnimDataAryAddr;
    long mAnimNamesOffset;

    public override void Read(BinaryReader br)
    {
        base.Read(br);

        mFormatToken = ReadUInt32(br, $"{nameof(mFormatToken)}");
        mVersion = ReadUInt32(br, $"{nameof(mVersion)}");

        Assert(mFormatToken == 1514195421 && mVersion == 0);

        uint mNumClasses = ReadUInt32(br, $"{nameof(mNumClasses)}");
        uint mNumBlends = ReadUInt32(br, $"{nameof(mNumBlends)}");
        uint mNumTransitionData = ReadUInt32(br, $"{nameof(mNumTransitionData)}");
        uint mNumTracks = ReadUInt32(br, $"{nameof(mNumTracks)}");
        uint mNumAnimData = ReadUInt32(br, $"{nameof(mNumAnimData)}");
        uint mNumMaskData = ReadUInt32(br, $"{nameof(mNumMaskData)}");
        uint mNumEventData = ReadUInt32(br, $"{nameof(mNumEventData)}");
        mUseCascadeBlend = ReadUInt32(br, $"{nameof(mUseCascadeBlend)}") != 0;
        mCascadeBlendValue = ReadSingle(br, $"{nameof(mCascadeBlendValue)}");

        mBlendDataAryAddr = ReadAddr(br, $"{nameof(mBlendDataAryAddr)}");
        mTransitionDataOffset = ReadAddr(br, $"{nameof(mTransitionDataOffset)}");
        mBlendTrackAryAddr = ReadAddr(br, $"{nameof(mBlendTrackAryAddr)}");
        mClassAryAddr = ReadAddr(br, $"{nameof(mClassAryAddr)}");
        mMaskDataAryAddr = ReadAddr(br, $"{nameof(mMaskDataAryAddr)}");
        mEventDataAryAddr = ReadAddr(br, $"{nameof(mEventDataAryAddr)}");
        mAnimDataAryAddr = ReadAddr(br, $"{nameof(mAnimDataAryAddr)}");
        uint mAnimNameCount = ReadUInt32(br, $"{nameof(mAnimNameCount)}");
        mAnimNamesOffset = ReadAddr(br, $"{nameof(mAnimNamesOffset)}");

        mSkeleton = Read<PathRecord>(br, $"{nameof(mSkeleton)}");
        mExtBuffer = ReadArr(br, null, mExtBufferSize, br => ReadUInt32(br, "UInt32"), $"{nameof(mExtBuffer)}");

        mBlendDataAry = ReadArr(br, mBlendDataAryAddr, mNumBlends, br => Read<BlendData>(br, $"<{nameof(BlendData)}>"), $"{nameof(mBlendDataAry)}");
        mTransitionData = ReadArr(br, mTransitionDataOffset, mNumTransitionData, br => Read<TransitionClipData>(br, $"<{nameof(TransitionClipData)}>"), $"{nameof(mTransitionData)}");
        mBlendTrackAry = ReadArr(br, mBlendTrackAryAddr, mNumTracks, br => Read<TrackResource>(br, $"<{nameof(TrackResource)}>"), $"{nameof(mBlendTrackAry)}");

        mClassAry = ReadArr2(br, mClassAryAddr, mNumClasses, br => Read<ClipResource>(br, $"<{nameof(ClipResource)}>"), $"{nameof(mClassAry)}");
        mMaskDataAry = ReadArr2(br, mMaskDataAryAddr, mNumMaskData, br => Read<MaskResource>(br, $"<{nameof(MaskResource)}>"), $"{nameof(mMaskDataAry)}");
        mEventDataAry = ReadArr2(br, mEventDataAryAddr, mNumEventData, br => Read<EventResource>(br, $"<{nameof(EventResource)}>"), $"{nameof(mEventDataAry)}");
        mAnimDataAry = ReadArr2(br, mAnimDataAryAddr, mNumAnimData, br => AnimResourceBase.ReadStatic(br), $"{nameof(mAnimDataAry)}");
        
        mAnimNames = ReadArr(br, mAnimNamesOffset, mAnimNameCount, br => Read<PathRecord>(br, $"<{nameof(PathRecord)}>"), $"{nameof(mAnimNames)}");
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);

        mExtBuffer = new uint[mExtBufferSize];

        WriteUInt32(bw, mFormatToken, $"{nameof(mFormatToken)}");
        WriteUInt32(bw, mVersion, $"{nameof(mVersion)}");
        uint mNumClasses = WriteUInt32(bw, (uint)mClassAry.Length, $"{nameof(mNumClasses)}");
        uint mNumBlends = WriteUInt32(bw, (uint)mBlendDataAry.Length, $"{nameof(mNumBlends)}");
        uint mNumTransitionData = WriteUInt32(bw, (uint)mTransitionData.Length, $"{nameof(mNumTransitionData)}");
        uint mNumTracks = WriteUInt32(bw, (uint)mBlendTrackAry.Length, $"{nameof(mNumTracks)}");
        uint mNumAnimData = WriteUInt32(bw, (uint)mAnimDataAry.Length, $"{nameof(mNumAnimData)}");
        uint mNumMaskData = WriteUInt32(bw, (uint)mMaskDataAry.Length, $"{nameof(mNumMaskData)}");
        uint mNumEventData = WriteUInt32(bw, (uint)mEventDataAry.Length, $"{nameof(mNumEventData)}");
        WriteUInt32(bw, Convert.ToUInt32(mUseCascadeBlend), $"{nameof(mUseCascadeBlend)}");
        WriteSingle(bw, mCascadeBlendValue, $"{nameof(mCascadeBlendValue)}");

        mBlendDataAryAddr = WriteAddr(bw, $"{nameof(mBlendDataAryAddr)}");
        mTransitionDataOffset = WriteAddr(bw, $"{nameof(mTransitionDataOffset)}");
        mBlendTrackAryAddr = WriteAddr(bw, $"{nameof(mBlendTrackAryAddr)}");
        mClassAryAddr = WriteAddr(bw, $"{nameof(mClassAryAddr)}");
        mMaskDataAryAddr = WriteAddr(bw, $"{nameof(mMaskDataAryAddr)}");
        mEventDataAryAddr = WriteAddr(bw, $"{nameof(mEventDataAryAddr)}");
        mAnimDataAryAddr = WriteAddr(bw, $"{nameof(mAnimDataAryAddr)}");
        uint mAnimNameCount = WriteUInt32(bw, (uint)mAnimNames.Length, $"{nameof(mAnimNameCount)}");
        mAnimNamesOffset = WriteAddr(bw, $"{nameof(mAnimNamesOffset)}");

        Write<PathRecord>(bw, mSkeleton, $"{nameof(mSkeleton)}");
        WriteArr(bw, null, mExtBuffer, (bw, v) => WriteUInt32(bw, v, "UInt32"), $"{nameof(mExtBuffer)}");
    }

    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);

        WriteArr(bw, mBlendDataAryAddr, mBlendDataAry, (bw, v) => Write<BlendData>(bw, v, $"<{nameof(BlendData)}>"), $"{nameof(mBlendDataAry)}");
        WriteArr(bw, mTransitionDataOffset, mTransitionData, (bw, v) => Write<TransitionClipData>(bw, v, $"<{nameof(TransitionClipData)}>"), $"{nameof(mTransitionData)}");
        WriteArr(bw, mBlendTrackAryAddr, mBlendTrackAry, (bw, v) => Write<TrackResource>(bw, v, $"<{nameof(TrackResource)}>"), $"{nameof(mBlendTrackAry)}");

        WriteArr2(bw, mClassAryAddr, mClassAry, (bw, addr, v) => Write<ClipResource>(bw, addr, v, $"<{nameof(ClipResource)}>"), $"{nameof(mClassAry)}");
        WriteArr2(bw, mMaskDataAryAddr, mMaskDataAry, (bw, addr, v) => Write<MaskResource>(bw, addr, v, $"<{nameof(MaskResource)}>"), $"{nameof(mMaskDataAry)}");
        WriteArr2(bw, mEventDataAryAddr, mEventDataAry, (bw, addr, v) => Write<EventResource>(bw, addr, v, $"<{nameof(EventResource)}>"), $"{nameof(mEventDataAry)}");
        WriteArr2(bw, mAnimDataAryAddr, mAnimDataAry, (bw, addr, v) => Write<AnimResourceBase>(bw, addr, v, $"<{v?.GetType().Name}>"), $"{nameof(mAnimDataAry)}");
        
        WriteArr(bw, mAnimNamesOffset, mAnimNames, (bw, v) => Write<PathRecord>(bw, v, $"<{nameof(PathRecord)}>"), $"{nameof(mAnimNames)}");

        #if MANUAL
        WriteArr(bw, null, mAnimNames, (bw, v) => ManualAfterwrite<PathRecord>(bw, v, $"<{nameof(PathRecord)}>"), $"{nameof(mAnimNames)}");
        ManualAfterwrite<PathRecord>(bw, mSkeleton, $"{nameof(mSkeleton)}");
        #endif
    }
}