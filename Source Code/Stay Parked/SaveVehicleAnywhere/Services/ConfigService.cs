using System;
using System.IO;
using System.Windows.Forms;
using GTA;
using SaveVehicleAnywhere.Config;

namespace SaveVehicleAnywhere.Services
{
    public static class ConfigService
    {
        private const string ConfigFileName = "SaveVehicleAnywhere.ini";

        public static ModConfig Load()
        {
            string path = Path.Combine(
                "scripts",
                ConfigFileName
            );

            ScriptSettings settings = ScriptSettings.Load(path);

            ModConfig config = new ModConfig();

            string keyName = settings.GetValue(
                "Settings",
                "SaveKey",
                "Y"
            );

            Keys parsedKey;

            if (Enum.TryParse(
                    keyName,
                    true,
                    out parsedKey))
            {
                config.SaveKey = parsedKey;
            }
            else
            {
                config.SaveKey = Keys.Y;
            }

            config.BlipsEnabled = settings.GetValue(
                "Settings",
                "Blips",
                true
            );

            return config;
        }
    }
}