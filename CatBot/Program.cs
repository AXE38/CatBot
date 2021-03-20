using System;
using System.Configuration;

namespace CatBot
{
    class Program
    {
        private static Configuration configManager = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        private static KeyValueConfigurationCollection confCollection = configManager.AppSettings.Settings;
        static void Main(string[] args)
        {
            var bot_token = confCollection["bot_token"].Value;
            Utils.sConn = confCollection["connection_string"].Value;
            Utils.catToken = confCollection["cat_token"].Value;
        }
    }
}
