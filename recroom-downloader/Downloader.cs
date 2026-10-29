using Newtonsoft.Json;
using recroom.downloader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace recroom_downloader
{
    internal class Downloader
    {
        public static void Download()
        {
            Console.Clear();
            Console.WriteLine("Select a build to download.\n");
            BuildInfo selected = Program.buildInfos.FirstOrDefault(x => x.Date.ToString("d MMMM yyyy HH:mm:ss") == Program.BetterNavigation(Program.buildInfos.Select(b => b.Date.ToString("d MMMM yyyy HH:mm:ss")).ToList()));
            var DownloadInfo = JsonConvert.DeserializeObject<SteamLoginInfo>(File.ReadAllText("DepotDownloaderSettings.json"));
            Process p = new Process();
            p.StartInfo.FileName = "DepotDownloader.exe";
            p.StartInfo.Arguments = $"-app 471710 -depot 471711 -manifest {selected.Manifest} -username {DownloadInfo.Username} -password {DownloadInfo.Password} -dir _downloadedBuilds/{selected.Date.ToString("d MMMM yyyy HH:mm:ss")}/ -max-downloads {DownloadInfo.Preferred_MaxDownloads}";
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();
            File.WriteAllText($"_downloadedBuilds/{selected.Date.ToString("d MMMM yyyy HH:mm:ss")}/steam_appid.txt", "471710");
        }
    }
}
