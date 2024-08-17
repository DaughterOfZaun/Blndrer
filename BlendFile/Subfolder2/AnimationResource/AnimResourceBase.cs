using static Logger;

public class AnimResourceBase : Resource
{
    public uint formatToken;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        formatToken = ReadUInt32(br, $"{nameof(formatToken)}");
    }
    public static AnimResourceBase ReadStatic(BinaryReader br)
    {
        long prevPosition = br.BaseStream.Position;
        uint formatToken = ReadUInt32(br, $"{nameof(formatToken)}");
        br.BaseStream.Position = prevPosition;
        //return Read<AnimationResource>(br, $"<{nameof(AnimationResource)}>");
        //return Read<CompressedAnimResource>(br, $"<{nameof(CompressedAnimResource)}>");
        throw new NotImplementedException();
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        throw new NotImplementedException();
    }
};