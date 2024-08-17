using System.Text;
using static Logger;

public class Skin
{
    public int Id;
    public string Name;
    public string IdName => (Id == 0) ? "Base" : $"Skin{Id}";
    public string Skeleton;
    public string? Texture;
    public string? SimpleSkin;
    public string? EmissiveTexture;
    public Character Character;
    public List<AnimationsListEntry> Animations = [];
    public class AnimationsListEntry
    {
        public int Id;
        public string Name;
        public string Animation;
        public float FPS;
        public bool DisableRootBone = false;
        public bool Loops => false
            || Name == "Idle1"
            || Name.StartsWith("Run")
            || Name.StartsWith("Dance")
            || Name.StartsWith("Channel");
        public int Frames;
    }
    public List<AnimEffectsListEntry> AnimEffects = [];
    public class AnimEffectsListEntry
    {
        public int Id;
        public string Name;
        public string Bone;
        public string Effect;
        public float StartFrame = 0;
        public float EndFrame = -1;
        public bool Loops = false;
    }

    private string[] sortedAry =
    [
        "Attack1", "Attack2", "Attack3",
        "Channel", "Channel_WNDUP",
        "Crit",
        "Death",
        "Idle1", "Idle2", "Idle3", "Idle4", "Idle5",
        "Joke",
        "Laugh",
        "Run", "Run1", "Run2",
        "Spell1", "Spell2", "Spell3", "Spell4",
        "Taunt",
        "Dance",
    ];
    public void SortAndEnumerateAnimations()
    {
        //Animations.Sort((a, b) => a.Name.CompareTo(b.Name));
        //AnimEffects.Sort((a, b) => a.Name.CompareTo(b.Name));
        Animations.Sort((a, b) => Array.IndexOf(sortedAry, a.Name) - Array.IndexOf(sortedAry, b.Name));
        AnimEffects.Sort((a, b) => Array.IndexOf(sortedAry, a.Name) - Array.IndexOf(sortedAry, b.Name));

        for(int i = 0; i < Animations.Count; i++)
            Animations[i].Id = i;
        for(int i = 0; i < AnimEffects.Count; i++)
            AnimEffects[i].Id = i;
    }
    
    public void MergeWithBase(Skin @base)
    {
        Animations.AddRange(
            @base.Animations.Where(
                ba => Animations.Find(
                    a => a.Name == ba.Name
                ) == null
            )
        );
        AnimEffects.AddRange(
            @base.AnimEffects.Where(
                bae => AnimEffects.Find(
                    ae => ae.Name == bae.Name
                ) == null
            )
        );
    }

    // Based on LeagueToolkit
    public void AssignFpsAndNumFrames()
    {
        foreach(var anim in new List<AnimationsListEntry>(Animations))
        {
            var path = Character.GetAbsoluteAnimationPath(IdName, anim.Animation);
            if(path == null)
            {
                Console.WriteLine($"{Character.Name}: {IdName}: Removing {anim.Name}");
                Animations.Remove(anim);
                continue;
            }

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader(fs, Encoding.UTF8);

            string magic = Encoding.ASCII.GetString(br.ReadBytes(8));
            uint version = br.ReadUInt32();

            float fps;
            int frameCount;
            float frameDuration;
            switch(magic)
            {
                case "r3d2anmd":
                    switch(version)
                    {
                        case 3u:
                            /*uint skeletonId =*/ br.ReadUInt32();
                            /*int trackCount =*/ br.ReadInt32();
                            frameCount = br.ReadInt32();
                            fps = br.ReadInt32();

                            frameDuration = 1f / fps;
                        break;
                        case 4u or 5u:
                            /*uint resourceSize =*/ br.ReadUInt32();
                            /*uint formatToken =*/ br.ReadUInt32();
                            /*uint version =*/ br.ReadUInt32();
                            /*uint flags =*/ br.ReadUInt32();
                            /*int trackCount =*/ br.ReadInt32();
                            frameCount = br.ReadInt32();
                            frameDuration = br.ReadSingle();
                            
                            fps = 1f / frameDuration;
                        break;
                        default:
                            throw new Exception();
                    }
                    break;
                case "r3d2canm":
                    switch(version)
                    {
                        case 1u or 2u or 3u:
                            /*uint resourceSize =*/ br.ReadUInt32();
                            /*uint formatToken =*/ br.ReadUInt32();
                            /*uint flags =*/ br.ReadUInt32();
                            /*int jointCount =*/ br.ReadInt32();
                            frameCount = br.ReadInt32();
                            /*int jumpCacheCount =*/ br.ReadInt32();
                            /*float duration =*/ br.ReadSingle();
                            fps = br.ReadSingle();
                            
                            frameDuration = 1f / fps;
                        break;
                        default:
                            throw new Exception();
                    }
                    break;
                default:
                    throw new Exception();
            }

            anim.FPS = fps;
            anim.Frames = frameCount;
        }
    }

    public void CreateBlendFile()
    {
        var path = Path.Join(Character.absoluteCharacterDir, $"{IdName}.blnd");
        //Assert(!Path.Exists(path));

        var file = new BlendFile();
        
        CreateBlendDataAry(file);
        CreateBlendTrackAry(file);
        CreateEventDataAry(file);
        CreateClassAry(file);
        CreateAnimDataAry(file);
        CreateAnimNames(file);
        CreateSkeleton(file);
        
        WriteBLND(file, path);
        //LogToFile($"{path}.txt");
        //LogStructureToFile($"{path}.s.txt");
        //WriteJSON(file, $"{path}.json");
    }

    private void CreateBlendDataAry(BlendFile file)
    {
        var blendDataAry = new List<BlendData>();
        var deathAnimId = Animations.FindIndex(a => a.Name == "Death");
        for(uint from = 0; from < Animations.Count; from++)
        {
            for(uint to = from; to < Animations.Count; to++)
            {
                blendDataAry.Add(new(){
                    mFromAnimId = from,
                    mToAnimId = to,
                    mBlendFlags = 0,
                    mBlendTime = (to == deathAnimId) ? 0 : 0.2f,
                });
            }
        }
        file.Pool.mBlendDataAry = [..blendDataAry];
    }

    private void CreateBlendTrackAry(BlendFile file)
    {
        file.Pool.mBlendTrackAry =
        [
            new()
            {
                mIndex = 0,
                mName = "Default",
                mBlendMode = 0,
                mBlendWeight = 1f,
            }
        ];
    }

    private void CreateEventDataAry(BlendFile file)
    {
        var animEffects = new Dictionary<string, List<AnimEffectsListEntry>>();
        foreach(var eff in AnimEffects)
        {
            (animEffects.GetValueOrDefault(eff.Name, null!) ??
                (animEffects[eff.Name] = []))
            .Add(eff); 
        }
        var eventDataAry = new List<EventResource>();
        foreach(var (effName, effs) in animEffects)
        {
            var i = 0;
            var id = eventDataAry.Count;
            eventDataAry.Add(new(){
                mFormatToken = 0,
                mVersion = 0,
                mFlags = 0,
                mName = effName,
                mUniqueID = (uint)id,
                mEventArray = effs.Select(
                    eff => new ParticleEventData()
                    {
                        mEffectName = eff.Effect,
                        mBoneName = eff.Bone,
                        mTargetBoneName = "",
                        mFrame = eff.StartFrame,
                        mEndFrame = eff.EndFrame,
                        mName = $"Pfx{i++}",
                        mFlags = 2 | (eff.Loops ? 1u : 0), //TODO: Find flags
                    }
                ).ToArray(),
            });
        }
        file.Pool.mEventDataAry = [..eventDataAry];
    }

    private void CreateClassAry(BlendFile file)
    {
        var classAry = new List<ClipResource>();
        foreach(var anim in Animations)
        {
            classAry.Add(new()
            {
                mFlags = anim.Loops ? ClipResource.Flags.eLoop : 0,
                mUniqueID = (uint)anim.Id,
                mName = anim.Name,
                mClipData = new AtomicClip()
                {
                    mStartTick = 0,
                    mEndTick = (uint)anim.Frames,
                    mTickDuration = 1f / anim.FPS,
                    mAnimDataIndex = anim.Id,
                    mEventDataIndex = Array.FindIndex(
                        file.Pool.mEventDataAry,
                        ev => ev.mName == anim.Name
                    ),
                    mMaskDataIndex = -1,
                    mTrackDataIndex = 0, /*Array.FindIndex(
                        file.Pool.mBlendTrackAry,
                        bt => bt.Name == "Default"
                    ),*/
                    mUpdaterData = null,
                    mSyncGroupName = null,
                    mSyncGroup = 0,
                },
            });
        }
        file.Pool.mClassAry = [..classAry];
    }

    private void CreateAnimDataAry(BlendFile file)
    {
        file.Pool.mAnimDataAry = new AnimResourceBase[Animations.Count];
    }

    private void CreateAnimNames(BlendFile file)
    {
        file.Pool.mAnimNames = Animations.Select(
            anim => new PathRecord()
            {
                path = Character.GetRelativeAnimationPath(IdName, anim.Animation).ToLower()
            }
        ).ToArray();
    }

    private void CreateSkeleton(BlendFile file)
    {
        file.Pool.mSkeleton = new PathRecord()
        {
            path = Character.GetRelativeSkeletonPath(IdName, Skeleton).ToLower()
        };
    }
}