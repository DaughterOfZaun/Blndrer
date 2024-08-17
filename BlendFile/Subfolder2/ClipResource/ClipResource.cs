using static Logger;

public class ClipResource: Resource
{
    public Flags mFlags;
    public enum Flags: int
    {
        eNone_0 = 0x0,
        eMain = 0x1,
        eLoop = 0x2,
        eContinue = 0x4,
        ePlayOnce = 0x8,
    }
    public uint mUniqueID;
    public string mName;
    public ClipData mClipData;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mFlags = (Flags)ReadUInt32(br, $"{nameof(mFlags)}");
        mUniqueID = ReadUInt32(br, $"{nameof(mUniqueID)}");
        long mNameOffset = ReadAddr(br, baseAddr, $"{nameof(mNameOffset)}");
        long mClipDataOffset = ReadAddr(br, baseAddr, $"{nameof(mClipDataOffset)}");

        long prevPosition = br.BaseStream.Position;
        br.BaseStream.Position = mNameOffset;
        if(mNameOffset != 0) mName = ReadCString(br, $"{nameof(mName)}");
        br.BaseStream.Position = mClipDataOffset;
        if(mClipDataOffset != 0) mClipData = ClipData.ReadStatic(br);
        br.BaseStream.Position = prevPosition;
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteUInt32(bw, (uint)mFlags, $"{nameof(mFlags)}");
        WriteUInt32(bw, mUniqueID, $"{nameof(mUniqueID)}");
        long mNameOffset = WriteAddr(bw, baseAddr, $"{nameof(mNameOffset)}");
        long mClipDataOffset = WriteAddr(bw, baseAddr, $"{nameof(mClipDataOffset)}");

        WriteCString(bw, mNameOffset, mName, $"{nameof(mName)}");
        Write<ClipData>(bw, mClipDataOffset, mClipData, $"<{mClipData.GetType().Name}>");
    }
}
