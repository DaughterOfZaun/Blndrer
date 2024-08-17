public class MultiChildClip : ClipData
{
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        throw new NotImplementedException();
    }

    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eMultiChildClip;
        
        base.Write(bw);

        throw new NotImplementedException();
    }
}