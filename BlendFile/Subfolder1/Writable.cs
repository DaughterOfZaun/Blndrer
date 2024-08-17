using static Logger;

public class Writable
{
    public int Id = 0;
    #if DEFFERED
    public List<WriteArgs> Deffered = new();
    #endif
    public Writable()
    {
        Logger.AllWritables.Add(this);
    }
    public virtual void Prewrite(){}
    public virtual void Read(BinaryReader br)
    {
        //throw new NotImplementedException();
    }
    public virtual void Write(BinaryWriter bw)
    {
        //throw new NotImplementedException();
    }
    public virtual void Afterwrite(BinaryWriter bw){}
    #if MANUAL
    public virtual void ManualAfterwrite(BinaryWriter bw){}
    #endif

    protected static void Connect<T>(T?[]? ary, ref int index, ref T? data) where T: class
    {
        var temp = data;
        if(ary == null || ary.Length == 0)
        {
            index = -1;
            data = null;
        }
        else if(data == null && index != -1)
        {
            data = ary[index];
            Assert(data != null);
        }
        else if(data != null && index == -1)
        {
            index = Array.FindIndex(ary, v => v == temp);
            Assert(index != -1);
        }
    }
}