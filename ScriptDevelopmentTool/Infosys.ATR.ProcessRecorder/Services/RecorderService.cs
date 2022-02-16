using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Infosys.ATR.ProcessRecorder.Services
{
    internal class RecorderService
    {
        /// <summary>
        /// 
        /// Method to read the appsettings information of Recorder.exe configuration file
        /// </summary>
        /// <param name="configPath">File location of the recorder config</param>
        /// <returns>Dictionary<string, string></returns>
        internal static Dictionary<string, string> GetRecorderAppSetting(string configPath)
        {

            Dictionary<string, string> dictionaryCollection = new Dictionary<string, string>();
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = configPath
            };
            var config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
            var sectionNode = (AppSettingsSection)config.GetSection("appSettings");

            foreach (var collection in sectionNode.Settings)
            {
                var element = ((System.Configuration.KeyValueConfigurationElement)(collection));
                dictionaryCollection.Add(element.Key, element.Value);
            }

            return dictionaryCollection;
        }

    }

}
