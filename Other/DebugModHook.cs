using System;

namespace PaleCourtCharms
{
    internal static class DebugModHook
    {
        public static void GiveAllCharms(Action callback)
        {
            try
            {
                DebugMod.BindableFunctions.OnGiveAllCharms += callback;
            }
            catch (MissingMethodException)
            {
                PaleCourtCharms.Instance?.LogWarn("DebugMod missing OnGiveAllCharms. Update DebugMod to support charm integration.");
            }
        }

        public static void RemoveAllCharms(Action callback)
        {
            try
            {
                DebugMod.BindableFunctions.OnRemoveAllCharms += callback;
            }
            catch (MissingMethodException)
            {
                PaleCourtCharms.Instance?.LogWarn("DebugMod missing OnRemoveAllCharms. Update DebugMod to support charm integration.");
            }
        }
    }
}
