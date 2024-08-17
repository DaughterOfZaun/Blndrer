using static Logger;

public class AtomicClip : ClipData
{
    public uint mStartTick;
    public uint mEndTick;
    public float mTickDuration;

    public int mAnimDataIndex = -1;
    private AnimResourceBase? mAnimData;

    private long mEventDataAddr;
    private long mMaskDataAddr;
    private long mTrackDataAddr;
    private long mUpdaterDataOffset;
    private long mSyncGroupNameOffset;

    public int mEventDataIndex = -1;
    private EventResource? mEventData; // Deffered
    public int mMaskDataIndex = -1;
    private MaskResource? mMaskData; // Deffered
    public int mTrackDataIndex = -1;
    private TrackResource? mTrackData; // Deffered

    public UpdaterResource? mUpdaterData;
    public string? mSyncGroupName;
    public uint mSyncGroup;
    private uint[] mExtBuffer;
    private const int mExtBufferSize = 2;

    public override void Read(BinaryReader br)
    {
        base.Read(br);

        mStartTick = ReadUInt32(br, $"{nameof(mStartTick)}");
        mEndTick = ReadUInt32(br, $"{nameof(mEndTick)}");
        mTickDuration = ReadSingle(br, $"{nameof(mTickDuration)}");
        mAnimDataIndex = ReadInt32(br, $"{nameof(mAnimDataIndex)}");
        mEventDataAddr = ReadAddr(br, $"{nameof(mEventDataAddr)}");
        mMaskDataAddr = ReadAddr(br, $"{nameof(mMaskDataAddr)}");
        mTrackDataAddr = ReadAddr(br, $"{nameof(mTrackDataAddr)}");
        mUpdaterDataOffset = ReadAddr(br, $"{nameof(mUpdaterDataOffset)}");
        mSyncGroupNameOffset = ReadAddr(br, $"{nameof(mSyncGroupNameOffset)}");
        mSyncGroup = ReadUInt32(br, $"{nameof(mSyncGroup)}");
        mExtBuffer = ReadArr(br, null, mExtBufferSize, br => ReadUInt32(br, "UInt32"), $"{nameof(mExtBuffer)}");

        var prevPosition = br.BaseStream.Position;
        //br.BaseStream.Position = mAnimDataAddr;
        //if(mAnimDataAddr != 0) mAnimData = AnimResourceBase.Read(br);
        //if(mAnimDataIndex != -1) mAnimData = pool.mAnimDataAry[mAnimDataIndex];
        mEventData = Read<EventResource>(br, mEventDataAddr, $"{nameof(mEventData)}");
        mMaskData = Read<MaskResource>(br, mMaskDataAddr, $"{nameof(mMaskData)}");
        mTrackData = Read<TrackResource>(br, mTrackDataAddr, $"{nameof(mTrackData)}");
        mUpdaterData = Read<UpdaterResource>(br, mUpdaterDataOffset, $"{nameof(mUpdaterData)}");
        mSyncGroupName = ReadCString(br, mSyncGroupNameOffset, $"{nameof(mSyncGroupName)}");
        br.BaseStream.Position = prevPosition;
    }

    public override void Prewrite()
    {
        base.Prewrite();

        var f = Logger.CurrentFile;
        //Connect(f?.mAnimDataAry, ref mAnimDataIndex, ref mAnimData);
        Connect(f?.mEventDataAry, ref mEventDataIndex, ref mEventData);
        Connect(f?.mMaskDataAry, ref mMaskDataIndex, ref mMaskData);
        Connect(f?.mBlendTrackAry, ref mTrackDataIndex, ref mTrackData);
    }

    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eAtomic;
        mExtBuffer = new uint[mExtBufferSize];

        base.Write(bw);

        // Assert(mAnimData == null || (pool.mAnimDataAry?.Contains(mAnimData) ?? false));
        // Assert(mAnimDataIndex >= 0 && mAnimDataIndex < pool.mAnimDataAry.Length);
        // Assert(mEventData == null || (pool.mEventDataAry?.Contains(mEventData) ?? false));
        // Assert(mMaskData == null || (pool.mMaskDataAry?.Contains(mMaskData) ?? false));
        // Assert(mTrackData == null || (pool.mBlendTrackAry?.Contains(mTrackData) ?? false));

        WriteUInt32(bw, mStartTick, $"{nameof(mStartTick)}");
        WriteUInt32(bw, mEndTick, $"{nameof(mEndTick)}");
        WriteSingle(bw, mTickDuration, $"{nameof(mTickDuration)}");
        WriteInt32(bw, mAnimDataIndex, $"{nameof(mAnimDataIndex)}");
        mEventDataAddr = WriteAddr(bw, $"{nameof(mEventDataAddr)}");
        mMaskDataAddr = WriteAddr(bw, $"{nameof(mMaskDataAddr)}");
        mTrackDataAddr = WriteAddr(bw, $"{nameof(mTrackDataAddr)}");
        mUpdaterDataOffset = WriteAddr(bw, $"{nameof(mUpdaterDataOffset)}");
        mSyncGroupNameOffset = WriteAddr(bw, $"{nameof(mSyncGroupNameOffset)}");
        WriteUInt32(bw, mSyncGroup, $"{nameof(mSyncGroup)}");
        WriteArr(bw, null, mExtBuffer, (bw, v) => WriteUInt32(bw, v, "UInt32"), $"{nameof(mExtBuffer)}");
    }

    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
        //br.BaseStream.Position = mAnimDataAddr;
        //if(mAnimDataAddr != 0) mAnimData = AnimResourceBase.Read(br);
        //if(mAnimDataIndex != -1) mAnimData = pool.mAnimDataAry[mAnimDataIndex];
        /*(not)Deffered*/Write<EventResource>(bw, mEventDataAddr, mEventData, $"{nameof(mEventData)}");
        DefferedWrite<MaskResource>(bw, mMaskDataAddr, mMaskData, $"{nameof(mMaskData)}");
        /*(not)Deffered*/Write<TrackResource>(bw, mTrackDataAddr, mTrackData, $"{nameof(mTrackData)}");
        Write<UpdaterResource>(bw, mUpdaterDataOffset, mUpdaterData, $"{nameof(mUpdaterData)}");
        WriteCString(bw, mSyncGroupNameOffset, mSyncGroupName, $"{nameof(mSyncGroupName)}");
    }
};