using static Logger;

public class MaskResource : Resource
{
    public uint mFormatToken;
    public uint mVersion;
    public ushort mFlags;
    public ushort mNumElement;
    public uint mUniqueID;
    public float[] mWeight;
    public JointHash[] mJointHash;
    public class JointHash : Writable
    {
        public int mWeightID;
        public uint mJointHash;
        public override void Read(BinaryReader br)
        {
            base.Read(br);
            mWeightID = ReadInt32(br, $"{nameof(mWeightID)}");
            mJointHash = ReadUInt32(br, $"{nameof(mJointHash)}");
        }
        public override void Write(BinaryWriter bw)
        {
            base.Write(bw);
            WriteInt32(bw, mWeightID, $"{nameof(mWeightID)}");
            WriteUInt32(bw, mJointHash, $"{nameof(mJointHash)}");
        }
    }
    public JointNdx[] mJointNdx;
    public class JointNdx : Writable
    {
        public int mWeightID;
        public int mJointNdx;
        public override void Read(BinaryReader br)
        {
            base.Read(br);
            mWeightID = ReadInt32(br, $"{nameof(mWeightID)}");
            mJointNdx = ReadInt32(br, $"{nameof(mJointNdx)}");
        }
        public override void Write(BinaryWriter bw)
        {
            base.Write(bw);
            WriteInt32(bw, mWeightID, $"{nameof(mWeightID)}");
            WriteInt32(bw, mJointNdx, $"{nameof(mJointNdx)}");
        }
    }
    public string mName;
    private long mWeightOffset;
    private long mJointHashOffset;
    private long mJointNdxOffset;
    private uint[] mExtBuffer;
    private const int mExtBufferSize = 2;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mFormatToken = ReadUInt32(br, $"{nameof(mFormatToken)}");
        mVersion = ReadUInt32(br, $"{nameof(mVersion)}");
        mFlags = ReadUInt16(br, $"{nameof(mFlags)}");
        mNumElement = ReadUInt16(br, $"{nameof(mNumElement)}");
        mUniqueID = ReadUInt32(br, $"{nameof(mUniqueID)}");
        mWeightOffset = ReadAddr(br, baseAddr, $"{nameof(mWeightOffset)}");
        mJointHashOffset = ReadAddr(br, baseAddr, $"{nameof(mJointHashOffset)}");
        mJointNdxOffset = ReadAddr(br, baseAddr, $"{nameof(mJointNdxOffset)}");
        mName = ReadCString(br, 32, $"{nameof(mName)}");
        mExtBuffer = new uint[]
        {
            ReadUInt32(br, $"{nameof(mExtBuffer)}[{0}]"),
            ReadUInt32(br, $"{nameof(mExtBuffer)}[{1}]"),
        };

        mWeight = ReadArr(br, mWeightOffset, mNumElement, br => ReadSingle(br, "Single"), $"{nameof(mWeight)}");
        mJointHash = ReadArr(br, mJointHashOffset, mNumElement, br => Read<JointHash>(br, $"<{nameof(JointHash)}>"), $"{nameof(mJointHash)}");
        mJointNdx = ReadArr(br, mJointNdxOffset, mNumElement, br => Read<JointNdx>(br, $"<{nameof(JointNdx)}>"), $"{nameof(mJointNdx)}");
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);

        mExtBuffer = new uint[mExtBufferSize];

        WriteUInt32(bw, mFormatToken, $"{nameof(mFormatToken)}");
        WriteUInt32(bw, mVersion, $"{nameof(mVersion)}");
        WriteUInt16(bw, mFlags, $"{nameof(mFlags)}");
        WriteUInt16(bw, mNumElement, $"{nameof(mNumElement)}");
        WriteUInt32(bw, mUniqueID, $"{nameof(mUniqueID)}");
        mWeightOffset = WriteAddr(bw, baseAddr, $"{nameof(mWeightOffset)}");
        mJointHashOffset = WriteAddr(bw, baseAddr, $"{nameof(mJointHashOffset)}");
        mJointNdxOffset = WriteAddr(bw, baseAddr, $"{nameof(mJointNdxOffset)}");
        WriteCString(bw, mName, 32, $"{nameof(mName)}");
        WriteUInt32(bw, mExtBuffer[0], $"{nameof(mExtBuffer)}[{0}]");
        WriteUInt32(bw, mExtBuffer[1], $"{nameof(mExtBuffer)}[{1}]");
    }

    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
        WriteArr(bw, mWeightOffset, mWeight, (bw, v) => WriteSingle(bw, v, "Single"), $"{nameof(mWeight)}");
        WriteArr(bw, mJointHashOffset, mJointHash, (bw, v) => Write<JointHash>(bw, v, $"<{nameof(JointHash)}>"), $"{nameof(mJointHash)}");
        WriteArr(bw, mJointNdxOffset, mJointNdx, (bw, v) => Write<JointNdx>(bw, v, $"<{nameof(JointNdx)}>"), $"{nameof(mJointNdx)}");
    }
};