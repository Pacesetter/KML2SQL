using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace KML2SQL
{
    class SettingsPersister
    {
        private static string FileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KML2SQL.settings");

        public void Persist(Settings settings)
        {
            var settingsText = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
            File.WriteAllText(FileName, settingsText);
        }
        public Settings Retrieve()
        {

            if (File.Exists(FileName))
            {
                var settingsText = File.ReadAllText(FileName);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(settingsText);
            }
            return null;
        }

    }
}