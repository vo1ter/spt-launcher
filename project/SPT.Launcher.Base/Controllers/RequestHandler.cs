/* RequestHandler.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 */


using SPT.Launcher.MiniCommon;

namespace SPT.Launcher
{
    public static class RequestHandler
    {
        private static Request request = new Request(null, "");

        public static string GetBackendUrl()
        {
            return request.RemoteEndPoint;
        }

        public static void ChangeBackendUrl(string remoteEndPoint)
        {
            request.RemoteEndPoint = remoteEndPoint;
        }

        public static void ChangeSession(string session)
        {
            request.Session = session;
        }

        public static string RequestConnect()
        {
            return request.GetJson("/launcher/server/connect");
        }

        public static string RequestLogin(LoginRequestData data)
        {
            return request.PostJson("/launcher/profile/login", Json.Serialize(data));
        }

        public static string RequestRegister(RegisterRequestData data)
        {
            return request.PostJson("/launcher/profile/register", Json.Serialize(data));
        }

        public static string RequestRemove(LoginRequestData data)
        {
            return request.PostJson("/launcher/profile/remove", Json.Serialize(data));
        }

        public static string RequestAccount(LoginRequestData data)
        {
            return request.PostJson("/launcher/profile/get", Json.Serialize(data));
        }

        public static string RequestProfileInfo(LoginRequestData data)
        {
            return request.PostJson("/launcher/profile/info", Json.Serialize(data));
        }

        public static string RequestExistingProfiles()
        {
            return request.GetJson("/launcher/profiles");
        }

        public static string RequestChangeUsername(ChangeRequestData data)
        {
            return request.PostJson("/launcher/profile/change/username", Json.Serialize(data));
        }

        public static string RequestChangePassword(ChangeRequestData data)
        {
            return request.PostJson("/launcher/profile/change/password", Json.Serialize(data));
        }

        public static string RequestWipe(RegisterRequestData data)
        {
            return request.PostJson("/launcher/profile/change/wipe", Json.Serialize(data));
        }

        public static string SendPing()
        {
            return request.GetJson("/launcher/ping");
        }

        public static string RequestServerVersion()
        {
            return request.GetJson("/launcher/server/version");
        }

        public static string RequestCompatibleGameVersion()
        {
            return request.GetJson("/launcher/profile/compatibleTarkovVersion");
        }

        public static string RequestLoadedServerMods()
        {
            return request.GetJson("/launcher/server/loadedServerMods");
        }

        public static string RequestProfileMods()
        {
            return request.GetJson("/launcher/server/serverModsUsedByProfile");
        }

        public static string RequestOnlinePlayers() 
        {
            return request.GetJson("/fika/presence/get");
        }

        public static string RequestDedicatedClientStatus()
        {
            return request.GetJson("/fika/headless/available");
        }
    }
}
