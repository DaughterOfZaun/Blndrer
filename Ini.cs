public class Ini(string path)
{
    public string Path = path;
    public string[] Lines = [];
    public Dictionary<string, Dictionary<string, string>> Data = [];

    public Ini Read()
    {
        Lines = File.ReadAllLines(Path);
        Dictionary<string, string>? currentSection = null;
        foreach (var line in Lines)
        {
            if (line != "")
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                Data[line[1..^1]] = currentSection = [];
            }
            else
            {
                var pair = line.Split('=', 2);
                //Debug.Assert(pair.Length == 2);
                var (key, value) = (pair[0], pair[1]);
                if (key.StartsWith('\'')) continue;
                if (value.StartsWith('"') && value.EndsWith('"'))
                    value = value[1..^1];
                //Debug.Assert(currentSection != null);
                currentSection![key] = value;
            }
        }
        return this;
    }
}