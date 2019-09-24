using DiscordWebhookInfo.Classes;
using ModAPI.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordWebhookInfo
{
    public class StartDiscordWebhookInfoMod
    {
        [ExecuteOnGameStart]
        private static void Load()
        {
            // Only for first start to copy the Config file if not exist
            Config myConf = new Config();
        }
    }
}
