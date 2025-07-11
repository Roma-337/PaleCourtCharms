using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Modding;
using UnityEngine;
using SFCore;
using SFCore.Utils;
using ItemChanger;
using ItemChanger.Items;
using ItemChanger.Locations;
using ItemChanger.Placements;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using UnityEngine.SceneManagement;
using PaleCourtCharms.Rando;

namespace PaleCourtCharms
{
    public class PaleCourtCharms : Mod,
    ILocalSettings<SaveModSettings>,
    IGlobalSettings<GlobalSettings>,    IMenuMod
    {
        public static PaleCourtCharms Instance;

        public static readonly List<CharmDefinition> Charms = new();
        public static readonly string[] CharmKeys = { "PURITY", "LAMENT", "BOON", "BLOOM" };
        public static readonly int[] CharmCosts = { 3, 2, 4, 5, 2 };

        public static List<int> CharmIDs = new();
        private static Dictionary<int, int> CharmCostsByID = new();

        public static Dictionary<string, AudioClip> Clips { get; } = new();
        public static Dictionary<string, AnimationClip> AnimClips { get; } = new();
        public static Dictionary<string, GameObject> preloadedGO { get; } = new();
        public static readonly Dictionary<string, Sprite> SPRITES = new();

        private SaveModSettings localSettings = new();
        public SaveModSettings OnSaveLocal() => localSettings;
        public void OnLoadLocal(SaveModSettings s) => localSettings = s;
public static SaveModSettings Settings => Instance?.localSettings;

        public override string GetVersion() => "1.0.0";

        public PaleCourtCharms() : base("PaleCourtCharms")
        {
            if (ModHooks.GetMod("FiveKnights") is Mod)
            {
                Log("[Warning]Detected PaleCourt,disabling mod.Note:you'll see a few more errors,that's normal.");
                return;
            }

            Instance = this;
            Log("PaleCourtCharms initialized.");

            LoadEmbeddedSprites();

            Charms.Add(new CharmDefinition { InternalName = "MarkOfPurity", DisplayName = "Mark of Purity", Description = "Description of Mark of Purity.", Icon = SPRITES["Mark_of_Purity"], NotchCost = CharmCosts[0] });
            Charms.Add(new CharmDefinition { InternalName = "VesselsLament", DisplayName = "Vessel's Lament", Description = "Description of Vessel's Lament.", Icon = SPRITES["Vessels_Lament"], NotchCost = CharmCosts[1] });
            Charms.Add(new CharmDefinition { InternalName = "BoonOfHallownest", DisplayName = "Boon of Hallownest", Description = "Description of Boon of Hallownest.", Icon = SPRITES["Boon_of_Hallownest"], NotchCost = CharmCosts[2] });
            Charms.Add(new CharmDefinition { InternalName = "AbyssalBloom", DisplayName = "Abyssal Bloom", Description = "Description of Abyssal Bloom.", Icon = SPRITES["Abyssal_Bloom"], NotchCost = CharmCosts[3] });

            CharmIDs = SFCore.CharmHelper.AddSprites(
                SPRITES["Mark_of_Purity"],
                SPRITES["Vessels_Lament"],
                SPRITES["Boon_of_Hallownest"],
                SPRITES["Abyssal_Bloom"]
            );

            InitializeCharmCosts();
        }

        private void InitializeCharmCosts()
        {
            CharmCostsByID.Clear();
            for (int i = 0; i < CharmIDs.Count; i++)
                CharmCostsByID[CharmIDs[i]] = Charms[i].NotchCost;
        }

        private void LoadEmbeddedSprites()
        {
            var asm = Assembly.GetExecutingAssembly();
            foreach (var res in asm.GetManifestResourceNames())
            {
                if (!res.EndsWith(".png")) continue;
                using var stream = asm.GetManifestResourceStream(res);
                if (stream == null) continue;

                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);

                var tex = new Texture2D(1, 1);
                tex.LoadImage(buffer);

                var split = res.Split('.');
                var resName = split.Length >= 3
                    ? split[split.Length - 2] : Path.GetFileNameWithoutExtension(res);

                SPRITES[resName] = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );

                Log($"Loaded sprite: {resName}");
            }
        }

       public override List<(string, string)> GetPreloadNames()
{
    return new List<(string, string)>
    {
        ("GG_Hive_Knight", "Battle Scene/Hive Knight/Slash 1"), ("GG_Hollow_Knight", "Battle Scene/HK Prime"), ("GG_Hollow_Knight", "Battle Scene/HK Prime/Counter Flash"), ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast/Blast"), ("Abyss_05", "Dusk Knight/Dream Enter 2"), ("Abyss_05", "Dusk Knight/Idle Pt"), ("Abyss_05", "Dream Dialogue"), ("GG_Failed_Champion", "False Knight Dream"), ("GG_Failed_Champion", "Ceiling Dust"), ("White_Palace_09", "White King Corpse/Throne Sit"), ("Fungus1_12", "Plant Turret"), ("Fungus1_12", "simple_grass"), ("Fungus1_12", "green_grass_2"), ("Fungus1_12", "green_grass_3"), ("Fungus1_12", "green_grass_1 (1)"), ("Fungus1_19", "Plant Trap"), ("White_Palace_01", "WhiteBench"), ("White_Palace_01", "White_ Spikes"), ("GG_Workshop", "GG_Statue_ElderHu"), ("GG_Lost_Kin", "Lost Kin"), ("GG_Soul_Tyrant", "Dream Mage Lord"), ("GG_Workshop", "GG_Statue_TraitorLord"), ("Room_Mansion", "Heart Piece Folder/Heart Piece/Plink"), ("Fungus3_23_boss", "Battle Scene/Wave 3/Mantis Traitor Lord"), ("GG_White_Defender", "Boss Scene Controller"), ("GG_Atrium_Roof", "Land of Storms Doors"), ("GG_White_Defender", "GG_Arena_Prefab/Godseeker Crowd"), ("Dream_04_White_Defender", "_SceneManager"), ("Dream_04_White_Defender", "Battle Gate (1)"), ("Dream_04_White_Defender", "Dream Entry"), ("Dream_04_White_Defender", "White Defender"), ("Dream_04_White_Defender", "Dream Fall Catcher"), ("Dream_Final_Boss", "Boss Control/Radiance/Death/Knight Split/Knight Ball"), ("Dream_Final_Boss", "Boss Control/Radiance"), ("GG_Nosk", "Mimic Spider"), ("GG_Hornet_1", "Boss Holder/Hornet Boss 1"), ("Fungus3_13", "Thorn Collider"), ("Abyss_05", "Dusk Knight/Shield"), ("Mines_31", "crystal_barrel_02/mines_cryst_barrel_short/Pt Crystal (1)"), ("Room_Queen", "UI Msg Get WhiteCharm"), ("Room_Queen", "Queen Item"), ("White_Palace_03_hub", "dream_nail_base"), ("White_Palace_03_hub", "dream_beam_animation"), ("White_Palace_03_hub", "doorWarp"), ("Dream_Room_Believer_Shrine", "Plaque_statue_01 (1)"), ("Room_Mansion", "Heart Piece Folder/Heart Piece"), ("Room_Mansion", "Xun NPC/White Flash"), ("GG_Radiance", "Boss Control/Plat Sets/Hazard Plat/Radiant Plat Small (1)"), ("GG_Atrium", "gg_roof_door_pieces"), ("Tutorial_01", "_Props/Tut_tablet_top/Glows"), ("Ruins1_23", "Mage"), ("Fungus2_03", "Mushroom Turret (2)"), ("Crossroads_46", "Tram Main"), ("Deepnest_East_11", "Breakable Wall top"), ("Deepnest_38", "Collapser Small"), ("Abyss_10", "higher_being/shadow_gate"), ("Abyss_10", "Ruins Fossil"), ("Abyss_10", "Tute Pole 2"), ("Abyss_10", "hanging_cords_01"), ("Abyss_10", "_SceneManager"), ("Abyss_09", "abyss_black-water/abyss_water_top (3)"), ("Abyss_10", "Surface Water Region"), ("Abyss_10", "Darkness Region"), ("Waterways_05", "Dream Gate Set Lock"), ("Abyss_10", "Hollow_Shade Marker"), ("Mines_28", "Soul Totem 5"), ("Mines_28", "Mini_totems_0000_7")
    };
}

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            ModHooks.AfterSavegameLoadHook += _ => ICShiny.ResetPlacementsFlag();
            On.UIManager.StartNewGame += HandleNewGame;
            ModHooks.LanguageGetHook += LangGet;
            ModHooks.GetPlayerIntHook += OnGetPlayerIntHook;
            ModHooks.GetPlayerBoolHook += ModHooks_GetPlayerBool;
            ModHooks.SetPlayerBoolHook += OnSetPlayerBool;
            On.GameManager.StartNewGame += GameManager_StartNewGame;
            ModHooks.AfterSavegameLoadHook += OnAfterSave;
            On.HeroController.Awake += HeroController_Awake;
            preloadedGO["PV"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            preloadedGO["Blast"] = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast/Blast"];
            preloadedGO["Knight Ball"] = preloadedObjects["Dream_Final_Boss"]["Boss Control/Radiance/Death/Knight Split/Knight Ball"];
            preloadedGO["Radiance"] = preloadedObjects["Dream_Final_Boss"]["Boss Control/Radiance"];
            preloadedGO["SoulTwister"] = preloadedObjects["Ruins1_23"]["Mage"];
            preloadedGO["SoulEffect"] = preloadedObjects["Tutorial_01"]["_Props/Tut_tablet_top/Glows"];
            ABManager.LoadAll();
            var bloomAnim = ABManager.LoadFromCharms<GameObject>("BloomAnim");
            if (bloomAnim != null) preloadedGO["Bloom Anim Prefab"] = bloomAnim;
            else Log("Warning: BloomAnim not found in AssetBundle.");
            var abyssalBloom = ABManager.LoadFromCharms<GameObject>("AbyssalBloom");
            if (abyssalBloom != null) preloadedGO["Bloom Sprite Prefab"] = abyssalBloom;
            else Log("Warning: AbyssalBloom not found in AssetBundle.");
            Instance = this;
            if (ModHooks.GetMod("DebugMod") is Mod)
            {
                DebugModHook.GiveAllCharms(() =>
                {
                    ToggleAllCharms(true);
                    PlayerData.instance.CountCharms();
                });
                DebugModHook.RemoveAllCharms(() =>
                {
                    ToggleAllCharms(false);
                    PlayerData.instance.CountCharms();
                });
            }

           
            ICShiny.Hook();
            var honourItem = new HonourUpgradeItem();
            Finder.DefineCustomItem(honourItem);
            SetupProgressionChain();
         if (ModHooks.GetMod("Randomizer 4")is Mod)
             ConnectionMenu.Hook();
        }
        
        private static bool randoInitialized = false;
         public bool ToggleButtonInsideMenu => false;
          public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggle)
    {
       
        return new List<IMenuMod.MenuEntry> {
            new IMenuMod.MenuEntry {
                Name        = "Add Pale Court Charms",
                Description = "Adds your custom charms into the randomizer pool.",
                Values      = new[] { "Off", "On" },
                Saver       = i => globalSettings.AddCharms     = (i == 1),
                Loader      = () => globalSettings.AddCharms ? 1 : 0
            },
            new IMenuMod.MenuEntry {
                Name        = "Randomize Charm Costs",
                Description = "Shuffle how many notches each charm costs.",
                Values      = new[] { "Off", "On" },
                Saver       = i => globalSettings.RandomizeCosts = (i == 1),
                Loader      = () => globalSettings.RandomizeCosts ? 1 : 0
            }
        };
    }

private static void HandleNewGame(On.UIManager.orig_StartNewGame orig, UIManager self, bool permaDeath, bool bossRush)
        {
            orig(self, permaDeath, bossRush);

           
            if (!randoInitialized
                && ModHooks.GetMod("Randomizer 4") is Mod rndMod
                && RandomizerMod.RandomizerMod.RS?.GenerationSettings != null)
            {
                randoInitialized = true;

             
                PaleCourtCharms.Instance.StartGame();

           
                RandoInterop.Hook();                
                Rando.Interop.Setup(
                    PaleCourtCharms.Instance.globalSettings,
                    PaleCourtCharms.Instance
                );
            }
        }
private void SetupProgressionChain()
        {
            // 1. Attach tag to custom Honour charm
            if (Finder.GetItemInternal("Kings_Honour") is AbstractItem honour)
            {
                honour.AddTag(new ItemChainTag
                {
                    predecessor = "Defenders_Crest",
                    successor = null
                });
            }

       Finder.GetItemOverride += args =>
            {
                if (args.ItemName == "Defenders_Crest")
                {
                    args.Current.AddTag(new ItemChainTag
                    {
                        predecessor = null,
                        successor = "Kings_Honour"
                    });
                }
            };
        }

        private void GameManager_StartNewGame(On.GameManager.orig_StartNewGame orig, GameManager gm, bool perma, bool bossRush)
        {
            orig(gm, perma, bossRush);

            if (bossRush)
            {
                Log("Godseeker mode detected. Unlocking Pale Court charms.");
                int count = CharmIDs.Count;
                localSettings.gotCharms = new bool[count];
                localSettings.newCharms = new bool[count];
                localSettings.equippedCharms = new bool[count];

                for (int i = 0; i < count; i++)
                    localSettings.gotCharms[i] = localSettings.newCharms[i] = true;

                localSettings.upgradedCharm_10 = true;
            }

            StartGame();
        }

        public void StartGame()
        {
            GameManager.instance.gameObject.AddComponent<Amulets>();
            Log("Amulets component added via StartGame.");
            
        }

        private int OnGetPlayerIntHook(string target, int orig)
        {
            if (target.StartsWith("charmCost_") && int.TryParse(target.Split('_')[1], out var charmNum) && CharmCostsByID.TryGetValue(charmNum, out var cost))
                return cost;
            return orig;
        }

        private bool OnSetPlayerBool(string target, bool value)
        {
            if (CharmIDs.Count == 0) return value;
            var parts = target.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[1], out var charmNum))
            {
                var idx = CharmIDs.IndexOf(charmNum);
                if (idx >= 0)
                {
                    switch (parts[0])
                    {
                        case "equippedCharm": localSettings.equippedCharms[idx] = value; return true;
                        case "gotCharm": localSettings.gotCharms[idx] = value; return true;
                        case "newCharm": localSettings.newCharms[idx] = value; return true;
                    }
                }
            }
            return value;
        }

        private bool ModHooks_GetPlayerBool(string target, bool orig)
        {
            if (CharmIDs.Count == 0) return orig;
            var parts = target.Split('_');
            if (parts.Length == 2 && int.TryParse(parts[1], out var charmNum))
            {
                var idx = CharmIDs.IndexOf(charmNum);
                if (idx >= 0)
                {
                    bool Safe(int i, bool[] arr) => i >= 0 && i < arr.Length ? arr[i] : orig;
                    return parts[0] switch
                    {
                        "gotCharm" => Safe(idx, localSettings.gotCharms),
                        "newCharm" => Safe(idx, localSettings.newCharms),
                        "equippedCharm" => Safe(idx, localSettings.equippedCharms),
                        _ => orig
                    };
                }
            }
            return orig;
        }

        private string LangGet(string key, string sheet, string orig)
        {
            if (key.StartsWith("CHARM_NAME_") || key.StartsWith("CHARM_DESC_"))
            {
                if (int.TryParse(key.Split('_')[2], out int charmNum))
                {
                    int idx = CharmIDs.IndexOf(charmNum);
                    if (idx >= 0)
                    {
                        if (key.StartsWith("CHARM_NAME_"))
                            return Charms[idx].DisplayName;
                        else if (key.StartsWith("CHARM_DESC_"))
                            return Charms[idx].Description;
                    }

                    if (charmNum == 10 && localSettings.upgradedCharm_10)
                    {
                        if (key.StartsWith("CHARM_NAME_"))
                            return Charms[4].DisplayName;
                        else if (key.StartsWith("CHARM_DESC_"))
                            return Charms[4].Description;
                    }
                }
            }

            return orig;
        }

        private static void ToggleAllCharms(bool give)
        {
            for (int i = 0; i < CharmIDs.Count; i++)
            {
                PaleCourtCharms.Instance.localSettings.gotCharms[i] = give;
                PaleCourtCharms.Instance.localSettings.newCharms[i] = give;
                PaleCourtCharms.Instance.localSettings.equippedCharms[i] = false;
            }
        }

        private void OnAfterSave(SaveGameData data)
        {
            if (CharmIDs.Count == 0 || localSettings.gotCharms == null || localSettings.gotCharms.Length != CharmIDs.Count)
                return;

            for (int i = 0; i < CharmIDs.Count; i++)
            {
                int charmNum = CharmIDs[i];
                PlayerData.instance.SetBool($"gotCharm_{charmNum}", localSettings.gotCharms[i]);
                PlayerData.instance.SetBool($"newCharm_{charmNum}", localSettings.newCharms[i]);
                PlayerData.instance.SetBool($"equippedCharm_{charmNum}", localSettings.equippedCharms[i]);
            }

            if (localSettings.upgradedCharm_10)
                PlayerData.instance.SetBool("upgradedCharm_10", true);
        }
      public static GlobalSettings GlobalSettings => Instance?.globalSettings;
private GlobalSettings globalSettings = new GlobalSettings();

    public void OnLoadGlobal(GlobalSettings s)
{
    globalSettings = s;
}
      public GlobalSettings OnSaveGlobal() {
      return globalSettings;
    }

private void HeroController_Awake(On.HeroController.orig_Awake orig, HeroController self)
        {
            orig(self);

            if (GameManager.instance != null && GameManager.instance.gameObject.GetComponent<Amulets>() == null)
            {
                GameManager.instance.gameObject.AddComponent<Amulets>();
                Log("Amulets component added during HeroController.Awake.");
            }

        }

        private new void Log(object msg) => Modding.Logger.Log("[PaleCourtCharms] " + msg);
    }

    public class CharmDefinition
    {
        public string InternalName;
        public int NotchCost;
        public string DisplayName;
        public string Description;
        public Sprite Icon;
    }
} 
