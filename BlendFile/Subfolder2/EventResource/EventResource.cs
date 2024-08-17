using static Logger;
using static HashFunctions;

public class EventResource: Resource
{
    public uint mFormatToken;
    public uint mVersion;
    public ushort mFlags;
    public uint mUniqueID;
    public BaseEventData[] mEventArray = [];
    private BaseEventData? mEventData = null; // mEventArray[0]
    private EventNameHash[] mEventNameHash = []; // Hash(mEventArray[i].mName)
    public class EventNameHash : Writable
    {
        public uint mDataID;
        public uint mNameHash;
        public EventNameHash(): base() {}
        public EventNameHash(uint id, uint hash) : base()
        {
            mDataID = id;
            mNameHash = hash;
        }
        public override void Read(BinaryReader br)
        {
            base.Read(br);
            mDataID = ReadUInt32(br, $"{nameof(mDataID)}");
            mNameHash = ReadUInt32(br, $"{nameof(mNameHash)}");
        }
        public override void Write(BinaryWriter bw)
        {
            base.Write(bw);
            WriteUInt32(bw, mDataID, $"{nameof(mDataID)}");
            WriteUInt32(bw, mNameHash, $"{nameof(mNameHash)}");
        }
    }
    private EventFrame[] mEventFrame; // mEventArray[i].mFrame
    public class EventFrame : Writable
    {
        public uint mDataID;
        public float mFrame;
        public EventFrame() : base() {}
        public EventFrame(uint id, float frame) : base()
        {
            mDataID = id;
            mFrame = frame;
        }
        public override void Read(BinaryReader br)
        {
            base.Read(br);
            mDataID = ReadUInt32(br, $"{nameof(mDataID)}");
            mFrame = ReadSingle(br, $"{nameof(mFrame)}");
        }
        public override void Write(BinaryWriter bw)
        {
            base.Write(bw);
            WriteUInt32(bw, mDataID, $"{nameof(mDataID)}");
            WriteSingle(bw, mFrame, $"{nameof(mFrame)}");
        }
    }
    public string? mName;
    private uint[] mExtBuffer;
    private const int mExtBufferSize = 2;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mFormatToken = ReadUInt32(br, $"{nameof(mFormatToken)}");
        mVersion = ReadUInt32(br, $"{nameof(mVersion)}");
        mFlags = ReadUInt16(br, $"{nameof(mFlags)}");
        ushort mNumEvents = ReadUInt16(br, $"{nameof(mNumEvents)}");
        mUniqueID = ReadUInt32(br, $"{nameof(mUniqueID)}");
        long mEventOffsetArrayOffset = ReadAddr(br, baseAddr, $"{nameof(mEventOffsetArrayOffset)}");
        long mEventDataOffset = ReadAddr(br, baseAddr, $"{nameof(mEventDataOffset)}");
        long mEventNameHashOffset = ReadAddr(br, baseAddr, $"{nameof(mEventNameHashOffset)}");
        long mEventFrameOffset = ReadAddr(br, baseAddr, $"{nameof(mEventFrameOffset)}");
        long mNameOffset = ReadAddr(br, baseAddr, $"{nameof(mNameOffset)}");
        mExtBuffer = ReadArr(br, null, mExtBufferSize, br => ReadUInt32(br, "UInt32"), $"{nameof(mExtBuffer)}");

        long prevPosition = br.BaseStream.Position;
        mEventArray = ReadArr2(br, mEventOffsetArrayOffset, mNumEvents, br => BaseEventData.ReadStatic(br), baseAddr, $"{nameof(mEventArray)}");
        br.BaseStream.Position = mEventDataOffset; if(mEventDataOffset != 0) mEventData = BaseEventData.ReadStatic(br);
        mEventNameHash = ReadArr(br, mEventNameHashOffset, mNumEvents, br => Read<EventNameHash>(br, $"<{nameof(EventNameHash)}>"), $"{nameof(mEventNameHash)}");
        mEventFrame = ReadArr(br, mEventFrameOffset, mNumEvents, br => Read<EventFrame>(br, $"<{nameof(EventFrame)}>"), $"{nameof(mEventFrame)}");
        mName = ReadCString(br, mNameOffset, $"{nameof(mName)}");
        br.BaseStream.Position = prevPosition;
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);

        mEventData = null;
        mEventFrame = [];
        mEventNameHash = [];

        mExtBuffer = new uint[mExtBufferSize];
        
        if(mEventArray != null && mEventArray.Length > 0)
        {
            mEventData = mEventArray[0];
            Array.Resize(ref mEventFrame, mEventArray.Length);
            Array.Resize(ref mEventNameHash, mEventArray.Length);
            for(uint i = 0; i < mEventArray.Length; i++)
            {
                var e = mEventArray[i];
                mEventFrame[i] = new(i, e.mFrame);
                mEventNameHash[i] = new(i, HashStringFNV1a(e.mName));
            }
        }

        WriteUInt32(bw, mFormatToken, $"{nameof(mFormatToken)}");
        WriteUInt32(bw, mVersion, $"{nameof(mVersion)}");
        WriteUInt16(bw, mFlags, $"{nameof(mFlags)}");
        ushort mNumEvents = WriteUInt16(bw, (ushort)mEventArray.Length, $"{nameof(mNumEvents)}");
        WriteUInt32(bw, mUniqueID, $"{nameof(mUniqueID)}");
        long mEventOffsetArrayOffset = WriteAddr(bw, baseAddr, $"{nameof(mEventOffsetArrayOffset)}");
        long mEventDataOffset = WriteAddr(bw, baseAddr, $"{nameof(mEventDataOffset)}");
        long mEventNameHashOffset = WriteAddr(bw, baseAddr, $"{nameof(mEventNameHashOffset)}");
        long mEventFrameOffset = WriteAddr(bw, baseAddr, $"{nameof(mEventFrameOffset)}");
        long mNameOffset = WriteAddr(bw, baseAddr, $"{nameof(mNameOffset)}");
        WriteArr(bw, null, mExtBuffer, (bw, v) => WriteUInt32(bw, v, "UInt32"), $"{nameof(mExtBuffer)}");

        WriteArr2(bw, mEventOffsetArrayOffset, mEventArray, (bw, addr, v) => Write<BaseEventData>(bw, addr, v, $"<{v.GetType().Name}>"), baseAddr, $"{nameof(mEventArray)}");
        Write<BaseEventData>(bw, mEventDataOffset, mEventData, $"{nameof(mEventData)}");
        WriteArr(bw, mEventNameHashOffset, mEventNameHash, (bw, v) => Write<EventNameHash>(bw, v, $"<{nameof(EventNameHash)}>"), $"{nameof(mEventNameHash)}");
        WriteArr(bw, mEventFrameOffset, mEventFrame, (bw, v) => Write<EventFrame>(bw, v, $"<{nameof(EventFrame)}>"), $"{nameof(mEventFrame)}");
        WriteCString(bw, mNameOffset, mName, $"{nameof(mName)}");
    }
}
