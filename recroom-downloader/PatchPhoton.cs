using Newtonsoft.Json;
using recroom.downloader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace recroom_downloader
{
    internal class PatchPhoton
    {
        public static string RecRoomRealtime = "9372aa8d-d3f4-44a0-986d-419e145a2b83";
        public static string RecRoomVoice = "e93ae440-f238-4b6c-848f-1df89faf14f5";
        public static void Patch(string resourcesassetspath)
        {
            var appsettings = JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText("DepotDownloaderSettings.json"));
            byte[] assets = File.ReadAllBytes(resourcesassetspath);
            assets = replaceByteArray(assets, Encoding.UTF8.GetBytes(RecRoomRealtime), Encoding.UTF8.GetBytes(appsettings.PhotonRealtimeId));
            assets = replaceByteArray(assets, Encoding.UTF8.GetBytes(RecRoomVoice), Encoding.UTF8.GetBytes(appsettings.PhotonVoiceId));
            File.WriteAllBytes(resourcesassetspath, assets);
        }
        public static byte[] replaceByteArray(byte[] data, byte[] toreplace, byte[] replacewith)
        {
            for (int i = 0; i <= data.Length - toreplace.Length; i++)
            {
                if (data.AsSpan(i, toreplace.Length).SequenceEqual(toreplace))
                {
                    replacewith.CopyTo(data.AsSpan(i));
                    i += toreplace.Length - 1;
                }
            }
            return data;
        }
    }
}
