using System.Collections.Generic;
using System.Diagnostics;
using SPT.Launcher.MiniCommon;
using SPT.Launcher.Models.Fika;
using SPT.Launcher.Models.SPT;

namespace SPT.Launcher.Controllers
{
    public class FikaController
    {
        public static FikaPlayer[] GetOnlinePlayers()
        {
            try
            {
                string json = RequestHandler.RequestOnlinePlayers();

                return Json.Deserialize<FikaPlayer[]>(json);
            }
            catch
            {
                return new FikaPlayer[] { };
            }
        }

        public static FikaDedicatedData GetDedicatedData() 
        {
            try
            {
                string json = RequestHandler.RequestDedicatedClientStatus();

                return Json.Deserialize<FikaDedicatedData>(json);
            }
            catch
            {
                return new FikaDedicatedData { };
            }
        }
    }
}
