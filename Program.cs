using System.CommandLine;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using static Logger;

public partial class Program
{
    private static async Task<int> Main(string[] args)
    {
        var testCmdCharArg = new Argument<string>("CHARACTER");
        var testCmd = new Command("test")
        {
            testCmdCharArg
        };
        testCmd.SetHandler(Test, testCmdCharArg);

        var upgradeCmdDataArg = new Argument<DirectoryInfo>("DATA");
        var upgradeCmd = new Command("upgrade")
        {
            upgradeCmdDataArg
        };
        upgradeCmd.SetHandler(Upgrade, upgradeCmdDataArg);

        var rootCmd = new RootCommand()
        {
            testCmd,
            upgradeCmd,
        };
        return await rootCmd.InvokeAsync(args);
    }

    async static void Test(string n)
    {
        var c = $"inout/{n}/{n}";

        var f1 = ReadBLND($"{c}.blnd");
        LogToFile($"{c}.blnd.r.txt");
        LogStructureToFile($"{c}.blnd.r.s.txt");

        WriteBLND(f1, $"{c}.w.blnd");
        LogToFile($"{c}.blnd.w.txt");
        LogStructureToFile($"{c}.blnd.w.s.txt");

        WriteJSON(f1, $"{c}.blnd.json");
        var f2 = ReadJSON($"{c}.blnd.json");
        WriteJSON(f2, $"{c}.blnd.2.json");

        WriteBLND(f2, $"{c}.ww.blnd");
        LogToFile($"{c}.blnd.ww.txt");
        LogStructureToFile($"{c}.blnd.ww.s.txt");
    }

    [GeneratedRegex(@"(\[MeshSkin(\d*)\])(?:\nAnimations=.*)?")]
    private static partial Regex MeshSkinRegex();
    async static void Upgrade(DirectoryInfo dataDirInfo)
    {
        var absoluteDataPath = dataDirInfo.FullName;
        var absoluteCharactersPath = Path.Join(absoluteDataPath, "Characters");
        var dirs = Directory.GetDirectories(absoluteCharactersPath);
        //int i = 0;
        foreach(var absoluteCharacterPath in dirs)
        {
            var characterName = absoluteCharacterPath[(absoluteCharactersPath.Length + 1)..];

            //Console.WriteLine($"[{++i}/{dirs.Length}] {characterName}");
            
            new Character().Read(absoluteDataPath, characterName).CreateBlendFiles();
            
            var iniPath = Character.CaseSensify(absoluteCharacterPath, $"{characterName}.ini", true);
            if(iniPath != null)
            {
                var text = File.ReadAllText(iniPath, Encoding.UTF8);
                text = MeshSkinRegex().Replace(text, m => {
                    var id = m.Groups[2].Value;
                    var header = m.Groups[1].Value;
                    var name = (id == "") ? "Base" : $"Skin{id}";
                    var replacement = $"{header}\nAnimations={name}.blnd";
                    return replacement;
                });
                File.WriteAllText(iniPath, text, Encoding.UTF8);
            }
        }
    }
}