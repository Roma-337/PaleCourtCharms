using System.Linq;
using System.Collections.Generic;
using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.Tags;
using RandomizerMod.RC;
using RandomizerMod.RandomizerData; 
using RandomizerMod.Settings;
using RandomizerCore.Logic;
using RandomizerCore.Json;        

namespace PaleCourtCharms.Rando
{
    internal static class ItemHandler
    {
        public static void Hook()
        {
            DefineObjects();
            RequestBuilder.OnUpdate.Subscribe(0f, AddToPool);
            RequestBuilder.OnUpdate.Subscribe(1f, ScaleNotchBudget);
            RequestBuilder.OnUpdate.Subscribe(50f, RandomizeNotchCosts);
            RCData.RuntimeLogicOverride.Subscribe(50f, InjectLogic);
        }

        private static void DefineObjects()
        {
            for (int i = 0; i < PaleCourtCharms.CharmKeys.Length; i++)
            {
                string key = PaleCourtCharms.CharmKeys[i];

                if (Finder.GetItemInternal(key) == null)
                {
                    Finder.DefineCustomItem(new PC_CharmItem(i));
                }

                var scene = i < 4 ? ICShiny.CharmScenes[i] : ICShiny.HonourScene;
                var pos = i < 4 ? ICShiny.CharmPositions[i] : ICShiny.HonourPos;

                if (Finder.GetLocationInternal(key) == null)
                {
                    var loc = new CoordinateLocation
                    {
                        name = key,
                        sceneName = scene,
                        x = pos.x,
                        y = pos.y,
                        elevation = 0
                    };

                    var tag = loc.AddTag<InteropTag>();
                    tag.Message = "RandoSupplementalMetadata";
                    tag.Properties["ModSource"] = PaleCourtCharms.Instance.GetName();
                    tag.Properties["PoolGroup"] = PoolNames.Charm;
                    tag.Properties["VanillaItem"] = key;

                    Finder.DefineCustomLocation(loc);
                }
                else
                {
                    var existingLoc = Finder.GetLocationInternal(key);
                    if (existingLoc != null && existingLoc.GetTag<InteropTag>() == null)
                    {
                        var tag = existingLoc.AddTag<InteropTag>();
                        tag.Message = "RandoSupplementalMetadata";
                        tag.Properties["ModSource"] = PaleCourtCharms.Instance.GetName();
                        tag.Properties["PoolGroup"] = PoolNames.Charm;
                        tag.Properties["VanillaItem"] = key;
                    }
                }
            }
        }

        private static void AddToPool(RequestBuilder rb)
        {
            if (!RandoInterop.IsRando || !rb.gs.PoolSettings.Charms) return;

            foreach (var key in PaleCourtCharms.CharmKeys)
            {
                rb.AddItemByName(key);
                rb.EditItemRequest(key, info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = key,
                        Pool = PoolNames.Charm,
                        MajorItem = false,
                        PriceCap = 2000
                    };
                });
                rb.AddLocationByName(key);
            }

          
            Finder.GetItemOverride += args =>
{
    if (args.ItemName == "Defenders_Crest")
    {
        args.Current.AddTag(new ItemChainTag { successor = ICShiny.HonourName });
    }
};

      Modding.Logger.Log("[PaleCourtCharms] AddToPool called"); }

        private static void ScaleNotchBudget(RequestBuilder rb)
        {
            if (!RandoInterop.IsRando) return;
            float scale = (90f + PaleCourtCharms.CharmKeys.Length * 2) / 90f;
            rb.gs.MiscSettings.MinRandomNotchTotal =
                (int)(rb.gs.MiscSettings.MinRandomNotchTotal * scale);
            rb.gs.MiscSettings.MaxRandomNotchTotal =
                (int)(rb.gs.MiscSettings.MaxRandomNotchTotal * scale);
        }

        private static void RandomizeNotchCosts(RequestBuilder rb)
        {
            if (!RandoInterop.IsRando || !rb.gs.MiscSettings.RandomizeNotchCosts) return;

            var rng = rb.rng;
            int total = rb.ctx.notchCosts.Sum() + PaleCourtCharms.CharmKeys.Length * 2;
            var costs = new int[PaleCourtCharms.CharmKeys.Length];
            for (int i = 0; i < total; i++)
                costs[rng.Next(costs.Length)]++;
            PaleCourtCharms.Settings.notchCosts = costs.ToList();
        }

        private static void InjectLogic(GenerationSettings gs, LogicManagerBuilder lmb)
{
    
    for (int i = 0; i < PaleCourtCharms.CharmKeys.Length; i++)
    {
        lmb.StateManager.GetOrAddBool("CHARM" + i);
        lmb.StateManager.GetOrAddBool("noCHARM" + i);
    }

    LoadAdditionalLogicFiles(lmb);
}


        private static void LoadAdditionalLogicFiles(LogicManagerBuilder lmb)
        {
            var modDir = System.IO.Path.GetDirectoryName(typeof(ItemHandler).Assembly.Location);
            var jsonFmt = new JsonLogicFormat();

            var macroLoc = System.IO.Path.Combine(modDir, "LogicMacros.json");
            if (System.IO.File.Exists(macroLoc))
            {
                using var macros = System.IO.File.OpenRead(macroLoc);
                lmb.DeserializeFile(LogicFileType.MacroEdit, jsonFmt, macros);
            }

            var waypointLoc = System.IO.Path.Combine(modDir, "LogicWaypoints.json");
            if (System.IO.File.Exists(waypointLoc))
            {
                using var waypoints = System.IO.File.OpenRead(waypointLoc);
                lmb.DeserializeFile(LogicFileType.Waypoints, jsonFmt, waypoints);
            }

            var connLogicLoc = System.IO.Path.Combine(modDir, "ConnectionLogicPatches.json");
            if (System.IO.File.Exists(connLogicLoc))
            {
                using var patches = System.IO.File.OpenRead(connLogicLoc);
                PatchConnectionLogic(lmb, jsonFmt, patches);
            }
        }

        private static void PatchConnectionLogic(LogicManagerBuilder lmb, ILogicFormat fmt, System.IO.Stream logicFile)
        {
            var logicDefs = fmt.LoadLogicEdits(logicFile);

            foreach (var def in logicDefs)
            {
                if (lmb.LogicLookup.ContainsKey(def.name))
                {
                    lmb.DoLogicEdit(def);
                }
            }
        }
    }
}