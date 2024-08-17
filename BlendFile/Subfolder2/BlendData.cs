using static Logger;

public class BlendData : Writable
{
    public uint mFromAnimId;
    public uint mToAnimId;
    public uint mBlendFlags;
    public float mBlendTime;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mFromAnimId = ReadUInt32(br, $"{nameof(mFromAnimId)}");
        mToAnimId = ReadUInt32(br, $"{nameof(mToAnimId)}");
        mBlendFlags = ReadUInt32(br, $"{nameof(mBlendFlags)}");
        mBlendTime = ReadSingle(br, $"{nameof(mBlendTime)}");
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteUInt32(bw, mFromAnimId, $"{nameof(mFromAnimId)}");
        WriteUInt32(bw, mToAnimId, $"{nameof(mToAnimId)}");
        WriteUInt32(bw, mBlendFlags, $"{nameof(mBlendFlags)}");
        WriteSingle(bw, mBlendTime, $"{nameof(mBlendTime)}");
    }
};