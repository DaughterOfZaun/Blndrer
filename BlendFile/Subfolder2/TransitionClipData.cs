using static Logger;

public class TransitionClipData : Writable
{
    public uint mFromAnimId;
    public uint mTransitionToCount;
    public TransitionToData[] mTransitionToArray;
    public class TransitionToData : Writable
    {
        public uint mToAnimId;
        public uint mTransitionAnimId;
        public override void Read(BinaryReader br)
        {
            base.Read(br);
            mToAnimId = ReadUInt32(br, $"{nameof(mToAnimId)}");
            mTransitionAnimId = ReadUInt32(br, $"{nameof(mTransitionAnimId)}");
        }

        public override void Write(BinaryWriter bw)
        {
            base.Write(bw);
            WriteUInt32(bw, mToAnimId, $"{nameof(mToAnimId)}");
            WriteUInt32(bw, mTransitionAnimId, $"{nameof(mTransitionAnimId)}");
        }
    }
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        mFromAnimId = ReadUInt32(br, $"{nameof(mFromAnimId)}");
        mTransitionToCount = ReadUInt32(br, $"{nameof(mTransitionToCount)}");

        uint mOffsetFromSelf = ReadUInt32(br, $"{nameof(mOffsetFromSelf)}");
        mTransitionToArray = new TransitionToData[mTransitionToCount];
        for(int i = 0; i < mTransitionToCount; i++)
            mTransitionToArray[i] = Read<TransitionToData>(br, $"<{nameof(TransitionToData)}>");
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        WriteUInt32(bw, mFromAnimId, $"{nameof(mFromAnimId)}");
        WriteUInt32(bw, mTransitionToCount, $"{nameof(mTransitionToCount)}");

        long mOffsetFromSelf = WriteAddr(bw, $"{nameof(mOffsetFromSelf)}");
        WriteArr(bw, mOffsetFromSelf, mTransitionToArray, (bw, v) => Write<TransitionToData>(bw, v, $"<{nameof(TransitionToData)}>"), $"{nameof(mTransitionToCount)}");
        
        throw new NotImplementedException();
    }
}