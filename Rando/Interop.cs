

namespace PaleCourtCharms.Rando {
  internal static class Interop {
    public static void Setup(GlobalSettings gs, PaleCourtCharms main)
    {
      
      ItemHandler.Hook();
        ConnectionMenu.Hook();
    }
  }
}
