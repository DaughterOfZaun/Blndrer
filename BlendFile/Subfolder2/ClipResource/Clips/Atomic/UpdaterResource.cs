public class UpdaterResource : Resource
{
    public uint mVersion;
    public ushort mNumUpdaters;
    public UpdaterData mUpdaters;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        throw new NotImplementedException();
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        throw new NotImplementedException();
    }
}