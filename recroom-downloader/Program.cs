using Newtonsoft.Json;
using recroom_downloader;
using System;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace recroom.downloader
{
    public class Program
    {
        public static List<BuildInfo> buildInfos = new List<BuildInfo>();
        static void Main(string[] args)
        {
            Console.Title = "RecRoom Downloader by ImagineHacking";
            if (!File.Exists("DepotDownloaderSettings.json"))
            {
                Console.Write("Please enter your steam username: ");
                string username = Console.ReadLine();
                Console.Write("\nPlease enter your password for this account: ");
                string password = Console.ReadLine();
                File.WriteAllText("DepotDownloaderSettings.json", JsonConvert.SerializeObject(new SteamLoginInfo
                {
                    Username = username,
                    Password = password,
                    Preferred_MaxDownloads = 8
                }));
            }
            if (!File.Exists("DepotDownloader.exe"))
            {
                Console.WriteLine("Downloading Steam DepotDownloader");
                new WebClient().DownloadFile("https://github.com/SteamRE/DepotDownloader/releases/download/DepotDownloader_3.0.0/DepotDownloader-windows-x64.zip", AppContext.BaseDirectory + "temp.zip");
                ZipFile.ExtractToDirectory(AppContext.BaseDirectory + "temp.zip", AppContext.BaseDirectory);
                File.Delete(AppContext.BaseDirectory + "LICENSE");
                File.Delete(AppContext.BaseDirectory + "temp.zip");
            }
            Console.WriteLine("Downloading manifest data from https://raw.githubusercontent.com/Rec-cs/recroom-downloader/refs/heads/main/data/builds.json");
            if (!Directory.Exists("_downloadedBuilds/"))
            {
                Directory.CreateDirectory("_downloadedBuilds/");
            }
            var rawInfo = JsonConvert.DeserializeObject<List<BuildInfoDownloaded>>(new WebClient().DownloadString("https://raw.githubusercontent.com/Rec-cs/recroom-downloader/refs/heads/main/data/builds.json"));
            foreach (var info in rawInfo)
            {
                buildInfos.Add(new BuildInfo
                {
                    Manifest = info.Manifest,
                    Date = DateTime.ParseExact(info.Date, "d MMMM yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                });
            }
            buildInfos.OrderBy(x => x.Date).ToList();
            while (true)
            {
                Downloader.Download();
            }
        }
        public static string BetterNavigation(List<string> strings, int pgsize = 10)
        {
            int selected = 0;
            int pageSize = pgsize;
            ConsoleKeyInfo keyinfo;
            int startY = Console.CursorTop;
            int totalPages = (int)Math.Ceiling(strings.Count / (double)pageSize);
            while (true)
            {
                int pageStart = (selected / pageSize) * pageSize;
                int pageEnd = Math.Min(pageStart + pageSize, strings.Count);
                Console.SetCursorPosition(0, startY);
                for (int i = pageStart; i < pageEnd; i++)
                {
                    Console.ForegroundColor = (i == selected) ? ConsoleColor.White : ConsoleColor.DarkGray;
                    if (i == selected)
                        Console.WriteLine("> " + strings[i] + new string(' ', Console.WindowWidth - strings[i].Length - 2));
                    else
                        Console.WriteLine(strings[i] + new string(' ', Console.WindowWidth - strings[i].Length));
                }
                for (int i = pageEnd; i < pageStart + pageSize; i++)
                {
                    Console.WriteLine(new string(' ', Console.WindowWidth));
                }
                Console.ResetColor();
                if (totalPages > 1)
                {
                    Console.WriteLine($"\n({pageStart / pageSize + 1}/{totalPages})            ");
                    Console.ResetColor();
                }
                Console.WriteLine("\n\n[Use arrow keys or WASD to navigate menus]");
                Console.ResetColor();
                keyinfo = Console.ReadKey(true);

                if (keyinfo.Key == ConsoleKey.UpArrow || keyinfo.Key == ConsoleKey.W)
                {
                    selected = (selected - 1 + strings.Count) % strings.Count;
                }
                else if (keyinfo.Key == ConsoleKey.DownArrow || keyinfo.Key == ConsoleKey.S)
                {
                    selected = (selected + 1) % strings.Count;
                }
                else if ((keyinfo.Key == ConsoleKey.LeftArrow || keyinfo.Key == ConsoleKey.A) && totalPages > 1)
                {
                    selected -= pageSize;
                    if (selected < 0)
                        selected = 0;
                }
                else if ((keyinfo.Key == ConsoleKey.RightArrow || keyinfo.Key == ConsoleKey.D) && totalPages > 1)
                {
                    selected += pageSize;
                    if (selected >= strings.Count)
                        selected = strings.Count - 1;
                }
                else if (keyinfo.Key == ConsoleKey.Enter)
                {
                    return strings[selected];
                }
            }
        }
    }
    public class BuildInfo
    {
        public DateTime Date { get; set; }
        public string Manifest { get; set; }
    }
    public class BuildInfoDownloaded
    {
        public string Date { get; set; }
        public string Manifest { get; set; }
    }
    public class SteamLoginInfo
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public int Preferred_MaxDownloads { get; set; }
    }
}