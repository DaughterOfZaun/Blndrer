public class BlendFile: Writable
{
    public BinaryHeader Header = new();
    public PoolData Pool = new();
    public override void Prewrite()
    {
        base.Prewrite();
        Header.Prewrite();
        Pool.Prewrite();
    }
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        Header.Read(br);
        Pool.Read(br);
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        Header.Write(bw);
        Pool.Write(bw);
        
        Logger.Afterwrite(bw, 0, this); //HACK:
    }
    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
        Header.Afterwrite(bw);
        Pool.Afterwrite(bw);
    }
}