using System;
using System.Diagnostics;
using System.Configuration;

namespace IVAO_Frequency
{
    class IVAO_frequency
    {
        static void conf()
        {
            string vid, ivan, path;
            Console.WriteLine("Configuration Sequence");
            Console.WriteLine("***********************");
            Console.Write("Your personal VID: ");
            vid = Console.ReadLine();

            Console.Write("Your IVAN Password: ");
            ivan = Console.ReadLine();

            Console.Write("Your TeamSpeak2 Path (double \\ instead of one): ");
            path = Console.ReadLine();

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add("VID", vid);
            config.AppSettings.Settings.Add("PW", ivan);
            config.AppSettings.Settings.Add("PATH", path);
            config.AppSettings.Settings.Remove("RUN");
            config.AppSettings.Settings.Add("RUN", "1");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        static void tsremote(string server, string callsign, string vid, string ivan, string station, string path)
        {
            Process ts = new Process();
            ts.StartInfo.UseShellExecute = true;
            ts.StartInfo.CreateNoWindow = true;
            ts.StartInfo.FileName = String.Format("{0}\\TeamSpeak.exe", path);
            ts.StartInfo.Arguments = String.Format("{0}.ts.ivao.aero?nickname={1}?loginname={2}?password={3}?channel={4}", server, callsign, vid, ivan, station);
            ts.Start();
        }
        static void reconf(out bool su)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove("VID");
            config.AppSettings.Settings.Remove("PW");
            config.AppSettings.Settings.Remove("PATH");
            config.AppSettings.Settings.Remove("RUN");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            Console.WriteLine("***********************");
            Console.WriteLine("Reenter the Configuration");
            Console.WriteLine("***********************");
            su = true;
            conf();
        }

        static void Main(string[] args)
        {
            //global variable decleration
            string callsign, server, station = "-", vid, ivan, path;
            bool su = false;
            string[] aserv = { "eu3", "eu4", "eu6", "eu16", "eu17" };
            
            Console.SetWindowSize(52, 16);
            Console.Title = "IVAO-Frequency-Changer - " + station;
            Console.WriteLine("*************************************");
            Console.WriteLine("IVAO Frequency Switcher by Mich21050");
            Console.WriteLine("*************************************");
            if (Convert.ToInt32(ConfigurationManager.AppSettings["RUN"]) == 1)
            {
                Console.WriteLine("To reenter the configuration");
                Console.WriteLine("at any moment just type: reconf ");
                Console.WriteLine("********************************");
            }
            //initial setup, designed to run just once
            if (Convert.ToInt32(ConfigurationManager.AppSettings["RUN"]) == 0) conf();

            //callsign request, just at launch
            Console.Write("Callsign: ");
            callsign = Console.ReadLine();
            while (callsign.Length > 7 || callsign.Length < 3)
            {
                Console.Write("Your Callsign is not valid: ");
                callsign = Console.ReadLine();
            }
            if (callsign.Contains("reconf") == true) reconf(out su);

            //reading variables from config
            vid = ConfigurationManager.AppSettings["VID"];
            ivan = ConfigurationManager.AppSettings["PW"];
            path = ConfigurationManager.AppSettings["PATH"];

            //looping connection part
            while(true)
            {
                Console.Write("Server: ");
                server = Console.ReadLine();
                while(!Array.Exists(aserv, element => element == server))
                {
                    Console.Write("Not a valid IVAO Server: ");
                    server = Console.ReadLine();
                }
                Console.Write("Station: ");
                station = Console.ReadLine();
                if (station.Contains("reconf") == true || server.Contains("reconf") == true) reconf(out su);
                if(su == true)
                {
                    vid = ConfigurationManager.AppSettings["VID"];
                    ivan = ConfigurationManager.AppSettings["PW"];
                    path = ConfigurationManager.AppSettings["PATH"];
                }
                Console.Title = "IVAO-Frequency-Changer - " + station;
                
                //ts2 connection with parameteres
                tsremote(server, callsign, vid, ivan, station, path);

            }
        }
    }
}
