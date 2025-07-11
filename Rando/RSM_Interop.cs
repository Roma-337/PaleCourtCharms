using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;
using PaleCourtCharms.Rando;

namespace PaleCourtCharms.Interop
{
    internal static class RSM_Interop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new PaleCourtSettingsProxy());
        }
    }

    internal class PaleCourtSettingsProxy : RandoSettingsProxy<RandoSettings, string>
    {
        public override string ModKey => PaleCourtCharms.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; }
            = new EqualityVersioningPolicy<string>(PaleCourtCharms.Instance.GetVersion());

        public override void ReceiveSettings(RandoSettings settings)
        {
            if (settings != null)
            {
                var save = PaleCourtCharms.Settings;
                settings.ApplyTo(save);
                
            }
            else
            {
             
                PaleCourtCharms.Settings.EnabledCharms.Clear();
            }
        }

        public override bool TryProvideSettings(out RandoSettings settings)
        {
            settings = RandoSettings.FromSaveSettings(PaleCourtCharms.Settings);
            return settings.IsEnabled();
        }
    }
}
