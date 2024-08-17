using static Logger;
using static HashFunctions;

public class PathRecord : Writable
{
    public string? path;
    private uint pathHash;
    private long pathOffset;
    public override void Read(BinaryReader br)
    {
        base.Read(br);
        pathHash = ReadUInt32(br, $"{nameof(pathHash)}");
        pathOffset = ReadAddr(br, $"{nameof(pathOffset)}");

        path = ReadCString(br, pathOffset, $"{nameof(path)}");
    }

    public override void Write(BinaryWriter bw)
    {
        base.Write(bw);
        pathHash = WriteUInt32(bw, HashStringFNV1a(path), $"{nameof(pathHash)}");
        pathOffset = WriteAddr(bw, $"{nameof(pathOffset)}");
    }

    #if !MANUAL
    public override void Afterwrite(BinaryWriter bw)
    {
        base.Afterwrite(bw);
    #else
    public override void ManualAfterwrite(BinaryWriter bw)
    {
        base.ManualAfterwrite(bw);
    #endif
        WriteCString(bw, pathOffset, path, $"{nameof(path)}");
    }
}