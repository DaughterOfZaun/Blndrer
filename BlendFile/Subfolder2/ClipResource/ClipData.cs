using static Logger;

public class ClipData : Writable
{
    protected ClipTypes mClipTypeID;
    protected enum ClipTypes: int
    {
        eInvalid = 0x0,
        eAtomic = 0x1,
        eSelector = 0x2,
        eSequencer = 0x3,
        eParallel = 0x4,
        eMultiChildClip = 0x5,
        eParametric = 0x6,
        eConditionBool = 0x7,
        eConditionFloat = 0x8,
    };
    protected long baseAddr;
    public override void Read(BinaryReader br)
    {
        baseAddr = br.BaseStream.Position;
        mClipTypeID = (ClipTypes)ReadUInt32(br, $"{nameof(mClipTypeID)}");
        Assert((int)mClipTypeID > 0 && (int)mClipTypeID < 9);
    }
    public static ClipData ReadStatic(BinaryReader br)
    {
        var prevPosition = br.BaseStream.Position;
        //ClipTypes mClipTypeID = (ClipTypes)ReadUInt32(br, $"{nameof(mClipTypeID)}");
        ClipTypes mClipTypeID = (ClipTypes)br.ReadUInt32();
        br.BaseStream.Position = prevPosition;
        if(mClipTypeID == ClipTypes.eAtomic)
        {
            return Read<AtomicClip>(br, $"<{nameof(AtomicClip)}>");
        }
        else if(mClipTypeID == ClipTypes.eSelector)
        {
            return Read<SelectorClip>(br, $"<{nameof(SelectorClip)}>");
        }
        else if(mClipTypeID == ClipTypes.eSequencer)
        {
            return Read<SequencerClip>(br, $"<{nameof(SequencerClip)}>");
        }
        else if(mClipTypeID == ClipTypes.eParallel)
        {
            return Read<ParallelClip>(br, $"<{nameof(ParallelClip)}>");
        }
        else if(mClipTypeID == ClipTypes.eMultiChildClip)
        {
            //return Log<MultiChildClip>($"<{nameof(MultiChildClip)}>");
            Console.WriteLine("  MultiChildClip UNIMPLEMENTED");
            throw new Exception("MultiChildClip UNIMPLEMENTED");
        }
        else if(mClipTypeID == ClipTypes.eParametric)
        {
            //return Read<ParametricClip>(br, $"<{nameof(ParametricClip)}>");
            Console.WriteLine("  ParametricClip UNIMPLEMENTED");
            throw new Exception("ParametricClip UNIMPLEMENTED");
        }
        else if(mClipTypeID == ClipTypes.eConditionBool)
        {
            //return Read<ConditionBoolClip>(br, $"<{nameof(ConditionBoolClip)}>");
            Console.WriteLine("  ConditionBoolClip UNIMPLEMENTED");
            throw new Exception("ConditionBoolClip UNIMPLEMENTED");
        }
        else if(mClipTypeID == ClipTypes.eConditionFloat)
        {
            //return Read<ConditionFloatClip>(br, $"<{nameof(ConditionFloatClip)}>");
            Console.WriteLine("  ConditionFloatClip UNIMPLEMENTED");
            throw new Exception("ConditionFloatClip UNIMPLEMENTED");
        }
        else
            throw new Exception("Invalid clip type");
    }
    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteUInt32(bw, (uint)mClipTypeID, $"{nameof(mClipTypeID)}");
    }
}