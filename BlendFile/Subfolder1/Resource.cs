using static Logger;

public class Resource : Writable
{
    protected long baseAddr;
    public uint mResourceSize;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        baseAddr = br.BaseStream.Position;
        mResourceSize = ReadUInt32(br, $"{nameof(mResourceSize)}");
        //Assert(mResourceSize > 0); // PoolData, ParticleEventData...
        mResourceSize = 0; // It doesn't really matter
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        baseAddr = bw.BaseStream.Position;
        WriteUInt32(bw, mResourceSize, $"{nameof(mResourceSize)}");
    }
}