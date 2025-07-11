using ItemChanger;  
using ItemChanger.Locations;  
using ItemChanger.Tags;  
using RandomizerMod;  
using RandomizerMod.RC;  
using RandomizerMod.RandomizerData;  
using RandomizerMod.Settings;  
using RandomizerCore.Logic;

namespace PaleCourtCharms.Rando {
  internal static class RandoInterop {
    public static void Hook() {
    
      Events.AfterStartNewGame += () => PaleCourtCharms.Instance.StartGame();

      
    }

    public static bool IsRando => RandomizerMod.RandomizerMod.IsRandoSave;
  }
}
