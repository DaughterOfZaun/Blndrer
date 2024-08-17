public class UpdaterData : Resource
{
    public ushort mInputType;
    public ushort mOutputType;
    public byte mNumTransforms;
    public AnimValueProcessorData[] mProcessors;
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