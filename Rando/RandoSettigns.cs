namespace PaleCourtCharms.Rando
{
    public class RandoSettings
    {
        public bool Enabled;
        public bool RandomizeCosts;

        public static RandoSettings FromSaveSettings(SaveModSettings s)
        {
            return new RandoSettings
            {
                Enabled = s != null && s.EnabledCharms.Count > 0,
                RandomizeCosts = s != null && s.RandomizeCosts
            };
        }

        public void ApplyTo(SaveModSettings s)
        {
            if (!Enabled)
                s.EnabledCharms.Clear();
            s.RandomizeCosts = RandomizeCosts;
        }

        public bool IsEnabled() => Enabled;
    }
}
