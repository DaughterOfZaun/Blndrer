using static Logger;

public class AnimationResource : AnimResourceBase
{
    public uint mVersion;
    public uint mFlags;
    public uint mNumChannels;
    public uint mNumTicks;
    public float mTickDuration;
    private long mJointNameHashesOffset;
    public string mAssetName;
    private long mTimeOffset;
    private long mVectorPaletteOffset;
    private long mQuatPaletteOffset;
    private long mTickDataOffset;
    private uint[] mExtBuffer;
    private const int mExtBufferSize = 3;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mVersion = ReadUInt32(br, $"{nameof(mVersion)}");
        mFlags = ReadUInt32(br, $"{nameof(mFlags)}");
        mNumChannels = ReadUInt32(br, $"{nameof(mNumChannels)}");
        mNumTicks = ReadUInt32(br, $"{nameof(mNumTicks)}");
        mTickDuration = ReadUInt32(br, $"{nameof(mTickDuration)}");
        long mJointNameHashesOffset = ReadAddr(br, baseAddr, $"{nameof(mJointNameHashesOffset)}");
        long mAssetNameOffset = ReadAddr(br, baseAddr, $"{nameof(mAssetNameOffset)}");
        long mTimeOffset = ReadAddr(br, baseAddr, $"{nameof(mTimeOffset)}");
        long mVectorPaletteOffset = ReadAddr(br, baseAddr, $"{nameof(mVectorPaletteOffset)}");
        long mQuatPaletteOffset = ReadAddr(br, baseAddr, $"{nameof(mQuatPaletteOffset)}");
        long mTickDataOffset = ReadAddr(br, baseAddr, $"{nameof(mTickDataOffset)}");
        mExtBuffer = new uint[]
        {
            ReadUInt32(br, $"{nameof(mExtBuffer)}[{0}]"),
            ReadUInt32(br, $"{nameof(mExtBuffer)}[{1}]"),
            ReadUInt32(br, $"{nameof(mExtBuffer)}[{2}]"),
        };

        long prevPosition = br.BaseStream.Position;
        br.BaseStream.Position = mAssetNameOffset;
        mAssetName = ReadCString(br, $"{nameof(mAssetName)}");
        br.BaseStream.Position = prevPosition;
    }
};
