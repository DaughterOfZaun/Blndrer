using static Logger;
using System.Text.RegularExpressions;

public partial class Character
{
    public string Name;
    public Dictionary<int, Skin> Skins = [];

    public string? absoluteDataDir;
    public string? relativeCharacterDir;
    public string? absoluteCharacterDir;

    public Character Read(string absoluteDataDir, string name)
    {
        this.Name = name;
        this.absoluteDataDir = absoluteDataDir;
        this.relativeCharacterDir = Path.Join("Characters", Name);
        this.absoluteCharacterDir = Path.Join(absoluteDataDir, relativeCharacterDir);
        ReadMeshSkins();
        if(Skins.Count > 0)
        {
            ReadAnimations();
            ReadAnimEffects();
            Postprocess();
        }
        else
        {
            //Console.WriteLine($"{Name}: No skins found");
        }
        return this;
    }

    public void CreateBlendFiles()
    {
        foreach(var (_, skin) in Skins)
        {
            skin.CreateBlendFile();
        }
    }

    private Ini? ini;
    private void ReadMeshSkins()
    {
        var iniPath = CaseSensify(absoluteCharacterDir!, $"{Name}.ini", true);
        if(iniPath == null)
        {
            Console.WriteLine($"{Name}: INI not found");
            return;
        }
        ini = new Ini(iniPath).Read();
        for (int i = 0; i < 100; i++)
        {
            var sectionName = $"MeshSkin{(i == 0 ? "" : i)}";
            if (ini.Data.TryGetValue(sectionName, out var section))
            {
                Skins.Add(i, new()
                {
                    Id = i,
                    Skeleton = section["Skeleton"],
                    Name = section.GetValueOrDefault("CharacterSkinName") ?? "", // Optional
                    EmissiveTexture = section.GetValueOrDefault("EmissiveTexture") ?? "", // Optional
                    SimpleSkin = section.GetValueOrDefault("SimpleSkin") ?? "", // Optional
                    Texture = section.GetValueOrDefault("Texture") ?? "", // Optional
                    Character = this,
                });
                continue;
            }
            break;
        }
    }

    private string GetRawRelativeAnimationPath(string idName, string animName)
    {
        return Path.Join(relativeCharacterDir, "Animations", $"{animName}.anm");
        //return Path.Join(relativeCharacterDir, "Skins", idName, "Animations", $"{animName}.anm");
    }

    public string? GetRelativeAnimationPath(string idName, string animName)
    {
        var
        path = GetRawRelativeAnimationPath(idName, animName);
        path = CaseSensify(absoluteDataDir!, path);
        Assert(File.Exists(Path.Join(absoluteDataDir, path)));
        return path;
    }

    public string? GetAbsoluteAnimationPath(string idName, string animName)
    {
        var
        path = GetRawRelativeAnimationPath(idName, animName);
        path = CaseSensify(absoluteDataDir!, path, true);
        return path;
    }

    public string? GetRelativeSkeletonPath(string idName, string skeleton)
    {
        var
        path = Path.Join(relativeCharacterDir, skeleton);
        //path = Path.Join(relativeCharacterDir, "Skins", idName, skeleton);
        path = CaseSensify(absoluteDataDir!, path);
        return path;
    }

    public static string? CaseSensify(string spath, string ipath, bool join = false)
    {
        if(ipath.StartsWith('/'))
            ipath = ipath[1..];
        if(ipath.EndsWith('/'))
            ipath = ipath[..^1];
        if(spath.EndsWith('/'))
            spath = spath[..^1];

        var path = spath;
        var parts = ipath.Split('/');
        foreach(var ipart in parts)
        {
            var pathPlusPart = $"{path}/{ipart}";
            if(Path.Exists(pathPlusPart))
            {
                path = pathPlusPart;
            }
            else
            {
                bool found = false;
                foreach(var entry in Directory.EnumerateFileSystemEntries(path))
                {
                    var spart = entry[(path.Length + 1)..];
                    if(spart.Equals(ipart, StringComparison.InvariantCultureIgnoreCase))
                    {
                        path = $"{path}/{spart}";
                        found = true;
                        break;
                    }
                }
                if(!found)
                    return null;
            }
        }
        return join ? path : path[(spath.Length + 1)..];
    }

    [GeneratedRegex(@"^\s*(.*?)\s+(.*?)(?:\s+d:([01]))?(?:\s+s:(\d+))\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex AnimationsListEntryRegex();
    private void ReadAnimations()
    {
        var animationsPath = Path.Join(absoluteCharacterDir, "Animations.list");
        if (File.Exists(animationsPath))
        {
            var skin = Skins[0];
            var lines = File.ReadAllLines(animationsPath);
            foreach (var rawline in lines)
            {
                var line = rawline.Trim();
                if (line == "" || line.StartsWith(';')) continue;
                if (
                    line.StartsWith('[')
                    && line.EndsWith(']')
                    && line[1..5].Equals("skin", StringComparison.InvariantCultureIgnoreCase)
                    && int.TryParse(line[5..^1], out var j)
                ){
                    if(j < Skins.Count)
                        skin = Skins[j];
                    else
                    {
                        Console.WriteLine($"{Name}: No definition for skin {j}");
                        skin = null;
                    }
                }
                else if(skin != null)
                {
                    var m = AnimationsListEntryRegex().Match(line);
                    if(!m.Success)
                    {
                        Console.WriteLine($"{Name}: {skin.Name}: Incorrect line: ^{line}$");
                        continue;
                    }
                    var animName = m.Groups[1].Value;
                    skin.Animations.Add(new()
                    {
                        //Id = 0, // Will be assigned after sorting
                        Name = animName,
                        Animation = m.Groups[2].Value,
                        FPS = float.Parse(m.Groups[4].Value),
                        DisableRootBone = m.Groups[3].Value == "1"
                    });
                }
            }
        }
        else
        {
            Console.WriteLine($"{Name}: No animation.list found");
        }
    }

    [GeneratedRegex(@"^\s*(.*?)\s+(.*?)\s+(.*?)(?:\s+s:(\d+))?(?:\s+e:(\d+))?(?:\s+l:([01]))?\s*$", RegexOptions.IgnoreCase)]
    private static partial Regex AnimEffectsListEntryRegex();
    private void ReadAnimEffects()
    {
        var animEffectsPath = Path.Join(absoluteCharacterDir, "AnimEffects.list");
        if (File.Exists(animEffectsPath))
        {
            var skin = Skins[0];
            var lines = File.ReadAllLines(animEffectsPath);
            foreach (var line in lines)
            {
                if (line == "" || line.StartsWith(';')) continue;
                if (line.StartsWith("[Skin") && line.EndsWith(']') && int.TryParse(line[5..^1], out var j))
                    skin = Skins[j];
                else
                {
                    var m = AnimEffectsListEntryRegex().Match(line);
                    skin.AnimEffects.Add(new()
                    {
                        //Id = 0, // Will be assigned after sorting
                        Name = m.Groups[1].Value,
                        Bone = m.Groups[2].Value,
                        Effect = m.Groups[3].Value,
                        StartFrame = m.Groups[4].Value != "" ? int.Parse(m.Groups[4].Value) : 0,
                        EndFrame = m.Groups[5].Value != "" ? int.Parse(m.Groups[5].Value) : -1,
                        Loops = m.Groups[6].Value != "" && m.Groups[6].Value == "1",
                    });
                }
            }
        }
        else
        {
            //Console.WriteLine($"{Name}: No animeffects.list found");
        }
    }

    private void Postprocess()
    {
        var @base = Skins[0];
        foreach(var (_, skin) in Skins)
        {
            skin.AssignFpsAndNumFrames();
            if(skin != @base)
                skin.MergeWithBase(@base);
            skin.SortAndEnumerateAnimations();
        }
    }
}