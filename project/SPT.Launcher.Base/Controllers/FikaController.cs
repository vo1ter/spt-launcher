using System;
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
                Debug.WriteLine("Received dedicated data JSON: " + json);

                // First deserialize as a list of dedicated status objects
                var statusList = Json.Deserialize<List<DedicatedStatus>>(json);

                // Create and return a FikaDedicatedData object with the first status if available
                var result = new FikaDedicatedData();
                if (statusList != null && statusList.Count > 0)
                {
                    result.Available = statusList[0];
                }
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deserializing dedicated data: {ex.Message}");
                return new FikaDedicatedData();
            }
        }
    }
}
