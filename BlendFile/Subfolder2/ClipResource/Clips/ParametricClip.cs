using static Logger;

public class ParametricClip : ClipData
{
    public uint mNumPairs;
    public uint mUpdaterType;

    public int mMaskDataIndex = -1;
    private MaskResource? mMaskData; // Deffered
    public int mTrackDataIndex = -1;
    private TrackResource? mTrackData; // Deffered
    public override void Read(BinaryReader br)
    {
        base.Read(br);

        mNumPairs = ReadUInt32(br, $"{nameof(mNumPairs)}");
        mUpdaterType = ReadUInt32(br, $"{nameof(mUpdaterType)}");
        
        long mMaskDataAddr = ReadAddr(br, $"{nameof(mMaskDataAddr)}");
        long mTrackDataAddr = ReadAddr(br, $"{nameof(mTrackDataAddr)}");

        mMaskData = Read<MaskResource>(br, mMaskDataAddr, $"{nameof(mMaskData)}");
        mTrackData = Read<TrackResource>(br, mTrackDataAddr, $"{nameof(mTrackData)}");
    }
    public override void Prewrite()
    {
        base.Prewrite();

        var f = Logger.CurrentFile;
        Connect(f?.mMaskDataAry, ref mMaskDataIndex, ref mMaskData);
        Connect(f?.mBlendTrackAry, ref mTrackDataIndex, ref mTrackData);
    }
    public override void Write(BinaryWriter bw)
    {
        mClipTypeID = ClipTypes.eParametric;

        base.Write(bw);

        throw new NotImplementedException();
    }
}