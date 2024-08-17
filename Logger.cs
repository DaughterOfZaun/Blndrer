using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

static partial class Logger
{
    //^    (\w+) static ([^(]*?) (\w+)(?=;| =)
    private static List<string> prefixes = [];
    private static List<AnnotatedByte> bytes = [];
    private static Dictionary<long, Writable> readCache = [];
    private static BinaryReader? prevBR;
    private static Dictionary<Writable, long> writeCache = [];
    private static BinaryWriter? prevBW;
    private static int gid = 0;
    public static PoolData? CurrentFile; //HACK:
    public static List<Writable> AllWritables = []; //HACK:
    private record AfterwriteArgs(Writable Value, List<string> prefixes);
    private static List<AfterwriteArgs> DefferedAfterwrites = [];
    private static readonly Dictionary<long, long> baseAddrForPointerAt = [];
    private static JsonSerializerSettings settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto
    };
    private static void Clear()
    {
        bytes.Clear();
        prefixes.Clear();
        readCache.Clear();
        prevBR = null;
        writeCache.Clear();
        prevBW = null;
        gid = 0;
        CurrentFile = null;
        //AllWritables.Clear(); //TODO: Fix memory leak
        DefferedAfterwrites.Clear();
        baseAddrForPointerAt.Clear();
    }

    private static void Pop(this List<string> list, string expdesc)
    {
        int i = list.Count - 1;
        var desk = list[i];
        Assert(desk == expdesc);
        list.RemoveAt(i);
    }

    public static void Assert(bool cond)
    {
        if(!cond)
        {
            throw new Exception();
        }
    }

    private static void Swap(BinaryReader br, ref long? addr)
    {
        if(addr != null)
        {
            (addr, br.BaseStream.Position) =
            (br.BaseStream.Position, (long)addr);
        }
    }

    private class AnnotatedByte
    {
        public byte? Byte;
        public List<string> Annotations = [];
    }
    private static void Grow(int to)
    {
        for(int i = bytes.Count; i < to; i++)
        {
            bytes.Add(new AnnotatedByte());
        }
    }
    private static string Desc(string desc)
    {
        string newdesc;
        prefixes.Add(desc);
        {
            newdesc = string.Join('.', prefixes)
                .Replace(".<", "<")
                .Replace(".[", "[")
                .Replace("..", ".");
        }
        prefixes.Pop(desc);
        return newdesc;
    }
    public static void Log(long position, string desc, byte[] value)
    {
        Assert(position < int.MaxValue);
        int pos = (int)position;

        Grow(pos + value.Length);
        desc = Desc(desc);

        for(int i = 0; i < value.Length; i++)
        {
            var ab = bytes[pos + i]!;
            var b = ab.Byte;
            var vb = value[i];

            Assert(b == null || b == vb);
            ab.Byte = vb;
            ab.Annotations.Add(desc);
        }
    }
    public static void LogToFile(string path, int from = 0, int to = int.MaxValue)
    {
        var sb = new StringBuilder();
        for(int i = from; i < bytes.Count && i < to; i++)
        {
            var ab = bytes[i];
            sb.Append((ab.Byte != null) ? Convert.ToHexString([ (byte)ab.Byte ]) : "??");
            sb.Append("; ");
            foreach(var desc in ab.Annotations)
            {
                sb.Append(desc);
                sb.Append("; ");
            }
            sb.AppendLine();
        }
        File.WriteAllText(path, sb.ToString());
    }
    [GeneratedRegex(@"\[(?!0\])")]
    private static partial Regex NonZeroIndexRegex();
    public static void LogStructureToFile(string path, int from = 0, int to = int.MaxValue)
    {
        var prevLine = "";
        var sb = new StringBuilder();
        for(int i = from; i < bytes.Count && i < to; i++)
        {
            var ab = bytes[i];
            var line = string.Join("; ", ab.Annotations);
            if(line == prevLine) continue;
            prevLine = line;
            //if(NonZeroIndexRegex().IsMatch(line)) continue;
            sb.AppendLine(line);
        }
        File.WriteAllText(path, sb.ToString());
    }

    public static long ReadAddr(BinaryReader br, string desc)
    {
        long pos = br.BaseStream.Position;
        return ReadAddr(br, pos, desc);
    }
    public static long ReadAddr(BinaryReader br, long pos, string desc)
    {
        int addr = ReadInt32(br, desc);
        return (addr != 0) ? pos + addr : 0; 
    }
    public static long WriteAddr(BinaryWriter bw, string desc)
    {
        return WriteAddr(bw, null, desc);
    }
    public static long WriteAddr(BinaryWriter bw, long? baseAddr, string desc)
    {
        long pos = bw.BaseStream.Position;

        baseAddrForPointerAt[pos] = baseAddr ?? pos;

        bw.Write((int)0);
        Grow((int)(pos + 4));
        desc = Desc(desc);
        for(int i = 0; i < 4; i++)
        {
            bytes[(int)(pos + i)].Annotations.Add(desc);
        }
        return pos;
    }
    public static void WriteAddr(BinaryWriter bw, long? pos, long? val)
    {
        if(pos == null) return;

        var prevPos = bw.BaseStream.Position;
        bw.BaseStream.Position = (long)pos;
        
        long value = (val != null) ?
            (long)val - baseAddrForPointerAt[(long)pos] : 0;

        bw.Write((int)value);
        var valueBytes = BitConverter.GetBytes(value);
        for(int i = 0; i < 4; i++)
        {
            bytes[(int)(pos + i)].Byte = valueBytes[i];
        }
        bw.BaseStream.Position = prevPos;
        //return value;
    }
    public static uint ReadUInt32(BinaryReader br, string desc)
    {
        var pos = br.BaseStream.Position;
        var value = br.ReadUInt32();
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static uint WriteUInt32(BinaryWriter bw, uint value, string desc)
    {
        var pos = bw.BaseStream.Position;
        bw.Write((uint)value);
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static ushort ReadUInt16(BinaryReader br, string desc)
    {
        var pos = br.BaseStream.Position;
        var value = br.ReadUInt16();
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static ushort WriteUInt16(BinaryWriter bw, ushort value, string desc)
    {
        var pos = bw.BaseStream.Position;
        bw.Write((ushort)value);
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static int ReadInt32(BinaryReader br, string desc)
    {
        var pos = br.BaseStream.Position;
        var value = br.ReadInt32();
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static int WriteInt32(BinaryWriter bw, int value, string desc)
    {
        var pos = bw.BaseStream.Position;
        bw.Write((int)value);
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static float ReadSingle(BinaryReader br, string desc)
    {
        var pos = br.BaseStream.Position;
        var value = br.ReadSingle();
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }
    public static float WriteSingle(BinaryWriter bw, float value, string desc)
    {
        var pos = bw.BaseStream.Position;
        bw.Write((float)value);
        Log(pos, desc, BitConverter.GetBytes(value));
        return value;
    }

    const int intMaxValueAligned = 214748360;
    public static string ReadCString(BinaryReader br, string desc)
    {
        return ReadCStringImpl(br, null, desc)!;
    }
    public static string ReadCString(BinaryReader br, int length, string desc)
    {
        return ReadCStringImpl(br, null, desc, length)!;
    }
    public static string? ReadCString(BinaryReader br, long? addr, string desc)
    {
        return ReadCStringImpl(br, addr, desc);
    }
    public static string? ReadCString(BinaryReader br, long? addr, int length, string desc)
    {
        return ReadCStringImpl(br, addr, desc, length);
    }
    private static string? ReadCStringImpl(BinaryReader br, long? addr, string desc, int length = intMaxValueAligned)
    {
        Assert(length % 4 == 0);

        if(addr == 0) return default;

        var ret = "";

        Swap(br, ref addr);
        {
            int i;
            for(i = 0; i < length; i++)
            {
                var c = ReadChar(br, $"{desc}[{i}]");
                if(c == '\0')
                {
                    i++; // important!
                    break;
                }
                ret += c;
            }
            if(length != intMaxValueAligned)
            {
                //br.BaseStream.Position = prevPosition + length;
                for(; i < length; i++)
                {
                    var c = ReadChar(br, $"{desc}[x]");
                    Assert(c == '\0');
                }
                //Assert(br.BaseStream.Position == prevPosition + length);
            }
            else
            {
                for(; i % 4 != 0; i++)
                {
                    var c = ReadChar(br, $"{desc}[y]");
                    Assert(c == '\0');
                }
            }
        }
        Swap(br, ref addr);

        return ret;
    }
    public static char ReadChar(BinaryReader br, string desc)
    {
        var pos = br.BaseStream.Position;
        var value = br.ReadByte();
        Log(pos, desc, [ value ]);
        return (char)(value);
    }

    public static string? WriteCString(BinaryWriter bw, string? value, string desc)
    {
        return WriteCStringImpl(bw, null, value, desc);
    }
    public static string? WriteCString(BinaryWriter bw, string? value, int length, string desc)
    {
        return WriteCStringImpl(bw, null, value, desc, length);
    }
    public static string? WriteCString(BinaryWriter bw, long? addr, string? value, string desc)
    {
        return WriteCStringImpl(bw, addr, value, desc);
    }
    public static string? WriteCString(BinaryWriter bw, long? addr, string? value, int length, string desc)
    {
        return WriteCStringImpl(bw, addr, value, desc, length);
    }
    private static string? WriteCStringImpl(BinaryWriter bw, long? addr, string? value, string desc, int length = intMaxValueAligned)
    {
        Assert(length % 4 == 0);

        long? pointer = null;
        if(value != null)
        {
            pointer = bw.BaseStream.Position;

            int i;
            for(i = 0; i < value.Length; i++)
            {
                WriteChar(bw, value[i], $"{desc}[{i}]");
            }
            if(length != intMaxValueAligned)
            {
                if(i < length)
                {
                    WriteChar(bw, '\0', $"{desc}[{i}]");
                    i++;
                }
                for(; i < length; i++)
                {
                    WriteChar(bw, '\0', $"{desc}[x]");
                }
            }
            else
            {
                WriteChar(bw, '\0', $"{desc}[{i}]");
                i++;
                
                for(; i % 4 != 0; i++)
                {
                    WriteChar(bw, '\0', $"{desc}[y]");
                }
            }
        }
        WriteAddr(bw, addr, pointer);
        return value;
    }
    public static char WriteChar(BinaryWriter bw, char value, string desc)
    {
        var pos = bw.BaseStream.Position;
        bw.Write((byte)value);
        Log(pos, desc, [ (byte)value ]);
        return value;
    }

    public static T Read<T>(BinaryReader br, string desc) where T: Writable
    {
        return Read<T>(br, null, desc)!;
    }
    public static T? Read<T>(BinaryReader br, long? addr, string desc) where T: Writable
    {
        if(addr == 0) return default;

        //if(prevBR != br) readCache.Clear(); prevBR = br;

        Writable? readed;

        Swap(br, ref addr);
        {
            long pos = br.BaseStream.Position;
            readed = readCache.GetValueOrDefault(pos);
            if(readed != null)
            {
                Assert(readed.GetType() == typeof(T));
                return (T)readed;
            }
            int id = gid++;
            desc = $"{desc}#{id}";
            prefixes.Add(desc);
            {
                readed = (Writable)Activator.CreateInstance(typeof(T))!;
                readCache[pos] = readed;
                readed.Id = id; //HACK:
                readed.Read(br);
            }
            prefixes.Pop(desc);
        }
        Swap(br, ref addr);

        return (T)readed;
    }

    // Must be called before writing and after reading JSON
    private static void Prewrite(BlendFile file)
    {
        CurrentFile = file.Pool;
        Assert(AllWritables.Count != 0);
        foreach(var v in AllWritables)
        {
            v.Prewrite();
        }
    }

    public record WriteArgs(long? addr, string desc, List<string> prefixes);
    public static void DefferedWrite<T>(BinaryWriter bw, T? value, string desc) where T: Writable
    {
        DefferedWrite(bw, null, value, desc);
    }
    public static void DefferedWrite<T>(BinaryWriter bw, long? addr, T? value, string desc) where T: Writable
    {
        #if !DEFFERED
        Write<T>(bw, addr, value, desc);
        #else
        long? pointer = null;
        if(value == null || (pointer = writeCache.GetValueOrDefault(value, null)) != null)
        {
            WriteAddr(bw, addr, pointer);
            return;
        }
        value.Deffered.Add(new(addr, desc, new(prefixes)));
        #endif
    }

    public static long? GetValueOrDefault(this Dictionary<Writable, long> dict, Writable? key, long? def = null)
    {
        return (key != null && dict.TryGetValue(key, out long value)) ? value : null;
    }
    public static void Write<T>(BinaryWriter bw, T? value, string desc) where T: Writable
    {
        Write<T>(bw, null, value, desc);
    }
    public static void Write<T>(BinaryWriter bw, long? addr, T? value, string desc) where T: Writable
    {
        #if DEFFERED
        if(value != null && value.Deffered.Count > 0)
        {
            // Cache and clear
            var deffered = new List<WriteArgs>();
            (deffered, value.Deffered) =
                (value.Deffered, deffered);
            value.Deffered.Clear();

            foreach(var args in deffered)
            {
                var (_addr, _desc, temp) = args;
                (prefixes, temp) = (temp, prefixes);
                Write<T>(bw, _addr, value, _desc);
                (prefixes, temp) = (temp, prefixes);
            }
        }
        #endif

        long? pointer = null;
        if(value != null && (pointer = writeCache.GetValueOrDefault(value, null)) == null)
        {
            pointer = bw.BaseStream.Position;
            writeCache[value] = (long)pointer;
            
            desc = $"{desc}#{value.Id}";
            prefixes.Add(desc);
            {
                value.Write(bw);
                Afterwrite(bw, addr, value);
            }
            prefixes.Pop(desc);
        }
        WriteAddr(bw, addr, pointer);
    }

    public static void Afterwrite(BinaryWriter bw, long? addr, Writable? value)
    {
        if(addr != null) // The cursor is outside the object
        {
            // Cache and clear
            var defferedAfterwrites = new List<AfterwriteArgs>();
            (defferedAfterwrites, DefferedAfterwrites) =
                (DefferedAfterwrites, defferedAfterwrites);
            DefferedAfterwrites.Clear();

            foreach(var args in defferedAfterwrites)
            {
                var temp = args.prefixes;
                (prefixes, temp) = (temp, prefixes);
                args.Value.Afterwrite(bw);
                (prefixes, temp) = (temp, prefixes);
            }

            value?.Afterwrite(bw);
        }
        else if(value != null)
        {
            DefferedAfterwrites.Add(new(value, new(prefixes)));
        }
    }

    #if MANUAL
    public static void ManualAfterwrite<T>(BinaryWriter bw, T? value, string desc) where T: Writable
    {
        ManualAfterwrite<T>(bw, null, value, desc);
    }
    public static void ManualAfterwrite<T>(BinaryWriter bw, long? addr, T? value, string desc) where T: Writable
    {
        long? pointer = null;
        if(value != null)
        {
            pointer = bw.BaseStream.Position;

            desc = $"{desc}#{value.Id}";
            prefixes.Add(desc);
            {
                value.ManualAfterwrite(bw);
            }
            prefixes.Pop(desc);
        }
        WriteAddr(bw, addr, pointer);
    }
    #endif
    
    public static T[] ReadArr<T>(BinaryReader br, long? addr, uint num, Func<BinaryReader, T> ctr, string desc)
    {
        Assert(addr != 0 || num == 0);
        if(addr == 0) return [];

        var arr = new T[num];

        Swap(br, ref addr);
        {
            prefixes.Add(desc);
            {
                for(int i = 0; i < num; i++)
                {
                    var idesc = $"[{i}]";
                    prefixes.Add(idesc);
                    {
                        arr[i] = ctr(br);
                    }
                    prefixes.Pop(idesc);
                }
            }
            prefixes.Pop(desc);
        }
        Swap(br, ref addr);

        return arr;
    }

    public static T[] WriteArr<T>(BinaryWriter bw, long? addr, T[] arr, Action<BinaryWriter, T> ctr, string desc)
    {
        int num = arr.Length;

        long? pointer = null;
        if(num > 0)
        {
            pointer = bw.BaseStream.Position;

            prefixes.Add(desc);
            {
                for(int i = 0; i < num; i++)
                {
                    var idesc = $"[{i}]";
                    prefixes.Add(idesc);
                    {
                        ctr(bw, arr[i]);
                    }
                    prefixes.Pop(idesc);
                }
                Afterwrite(bw, addr, null);
            }
            prefixes.Pop(desc);
        }
        WriteAddr(bw, addr, pointer);
        return arr;
    }

    public static T[] ReadArr2<T>(BinaryReader br, long addr, uint num, Func<BinaryReader, T> ctr, string desc)
    {
        return ReadArr2Impl<T>(br, addr, num, ctr, desc);
    }
    public static T[] ReadArr2<T>(BinaryReader br, long addr, uint num, Func<BinaryReader, T> ctr, long baseAddr, string desc)
    {
        return ReadArr2Impl<T>(br, addr, num, ctr, desc, baseAddr);
    }
    private static T[] ReadArr2Impl<T>(BinaryReader br, long addr, uint num, Func<BinaryReader, T> ctr, string desc, long? baseAddr = null)
    {
        Assert(addr != 0 || num == 0);
        
        if(addr == 0) return [];
        
        var arr = new T[num];

        var pos = br.BaseStream.Position;
        br.BaseStream.Position = addr;
        {
            prefixes.Add(desc);
            {
                var addrs = new long[num];
                for(int i = 0; i < num; i++)
                    addrs[i] = ReadAddr(br, baseAddr ?? addr, $"[{i}].BASE_ADDR");
                for(int i = 0; i < num; i++)
                {
                    addr = addrs[i];
                    if(addr != 0) //TODO: throw an Exception otherwise?
                    {
                        br.BaseStream.Position = addr;

                        var idesc = $"[{i}]";
                        prefixes.Add(idesc);
                        {
                            arr[i] = ctr(br);
                        }
                        prefixes.Pop(idesc);
                    }
                }
            }
            prefixes.Pop(desc);
        }
        br.BaseStream.Position = pos;

        return arr;
    }

    public static T[] WriteArr2<T>(BinaryWriter bw, long? addr, T[] arr, Action<BinaryWriter, long, T> ctr, string desc)
    {
        return WriteArr2Impl<T>(bw, addr, arr, ctr, desc);
    }
    public static T[] WriteArr2<T>(BinaryWriter bw, long? addr, T[] arr, Action<BinaryWriter, long, T> ctr, long baseAddr, string desc)
    {
        return WriteArr2Impl<T>(bw, addr, arr, ctr, desc, baseAddr);
    }
    private static T[] WriteArr2Impl<T>(BinaryWriter bw, long? addr, T[] arr, Action<BinaryWriter, long, T> ctr, string desc, long? baseAddr = null)
    {
        int num = arr.Length;

        long? pointer = null;
        if(num > 0)
        {
            pointer = bw.BaseStream.Position;

            prefixes.Add(desc);
            {
                var addrs = new long[num];
                for(int i = 0; i < num; i++)
                    addrs[i] = WriteAddr(bw, baseAddr ?? pointer, $"[{i}].BASE_ADDR");
                for(int i = 0; i < num; i++)
                {
                    var v = arr[i];
                    if(v == null)
                        WriteAddr(bw, addrs[i], (long?)null);
                    else
                    {
                        var idesc = $"[{i}]";
                        prefixes.Add(idesc);
                        {
                            ctr(bw, addrs[i], v);
                        }
                        prefixes.Pop(idesc);
                    }
                }
            }
            prefixes.Pop(desc);
        }
        WriteAddr(bw, addr, pointer);
        return arr;
    }

    public static BlendFile ReadBLND(string path)
    {
        Clear(); // Clear before read/write blnd
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var br = new BinaryReader(fs, Encoding.Default);
        var file = Read<BlendFile>(br, $"<{nameof(BlendFile)}>");
        return file;
    }
    public static void WriteBLND(BlendFile file, string path)
    {
        Clear(); // Clear before read/write blnd
        Prewrite(file); // Prewrite before writing
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        Write<BlendFile>(bw, file, $"<{nameof(BlendFile)}>");
        File.WriteAllBytes(path, ms.ToArray());
    }
    public static BlendFile ReadJSON(string path)
    {
        return JsonConvert.DeserializeObject<BlendFile>(File.ReadAllText(path, Encoding.UTF8), settings)!;
    }
    public static void WriteJSON(BlendFile file, string path)
    {
        Prewrite(file); // Prewrite before writing
        File.WriteAllText(path, JsonConvert.SerializeObject(file, Formatting.Indented, settings), Encoding.UTF8);
    }
}