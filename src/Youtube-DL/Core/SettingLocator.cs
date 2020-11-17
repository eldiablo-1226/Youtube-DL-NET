namespace Youtube_DL.Core
{
    public static class SettingLocator
    {
        public static Settings settings { get; private set; }

        static SettingLocator()
        {
            settings = new Settings();
        }
    }
}