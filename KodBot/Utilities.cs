using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.DependencyCollector;
using Discord.WebSocket;
using Discord.Net;
using System.Diagnostics;
using System.Threading;

namespace KodBot
{
    class Utilities
    {
        private static Dictionary<string, string> Alerts;
        static Utilities()
        {
            string path = "C:/Users/matteo.fantin/source/Repos/KodBot/";
            string json = File.ReadAllText(path + "SystemLang/Alerts.json");
            var data = JsonConvert.DeserializeObject<dynamic>(json);
            Alerts = data.ToObject<Dictionary<string, string>>();
        }

        public static string GetAlerts(string key)
        {
            if (Alerts.ContainsKey(key))
            {
                return Alerts[key];
            }
            string msg = "Error: key not found or not exist";
            SendTrace(msg, SeverityLevel.Error, new Dictionary<string, string> { { "Description", key } });
            return msg;
        }

        public static void AddDpoint(SocketGuildUser user)
        {
            string text = File.ReadAllText("Resources/dpoint.txt");
            string[] txt = (File.ReadAllText("Resources/dpoint.txt", Encoding.Default)).Split(separator: ':');
            for(int i = 0; i<txt.GetLength(0); i++)
            {
                if (txt[i] == Convert.ToString(user.Id))
                {
                    int temp = Convert.ToInt32(txt[i + 1]);
                    string newvalue = Convert.ToString(Convert.ToInt32(txt[i + 1]) + 1);
                    Convert.ToString(txt[i + 1]);
                    text = text.Replace(Convert.ToString(user.Id + ":" + temp), Convert.ToString(user.Id + ":" + newvalue));
                    File.WriteAllText("Resources/dpoint.txt", text);
                }
            }
        }

        public static void SendEvent(string msg)
        {
            TelemetryConfiguration conf = new TelemetryConfiguration("37c0c49a-223d-450f-b4c2-776d69cf3956");
            TelemetryClient client = new TelemetryClient(conf);
            client.TrackEvent(msg);
        }

        public static void SendMetric(string msg, uint value)
        {
            TelemetryConfiguration conf = new TelemetryConfiguration("37c0c49a-223d-450f-b4c2-776d69cf3956");
            TelemetryClient client = new TelemetryClient(conf);

            var sample = new MetricTelemetry();
            sample.Name = msg;
            sample.Sum = value;
            client.TrackMetric(sample);
        }

        public static void SendPageView(string msg)
        {
            TelemetryConfiguration conf = new TelemetryConfiguration("37c0c49a-223d-450f-b4c2-776d69cf3956");
            TelemetryClient client = new TelemetryClient(conf);

            client.TrackPageView(msg);
        }

        public static void SendTrace(string msg, SeverityLevel type, IDictionary<string,string>prop)
        {
            TelemetryConfiguration conf = new TelemetryConfiguration("37c0c49a-223d-450f-b4c2-776d69cf3956");
            TelemetryClient client = new TelemetryClient(conf);

            client.TrackTrace(msg, type, prop);
        }

        public static void Restart()
        {
            SendTrace("Restart App", SeverityLevel.Information, new Dictionary<string, string> { { "Information", "restart" } });
            Thread.Sleep(500);
            var proc = new Process();
            proc.StartInfo.FileName = "restart.bat";
            proc.StartInfo.CreateNoWindow = true;
            proc.Start();
        }

        /*public static void SendDep()
        {
            TelemetryConfiguration conf = new TelemetryConfiguration("37c0c49a-223d-450f-b4c2-776d69cf3956");
            TelemetryClient client = new TelemetryClient(conf);

            var success = false;
            var startTime = DateTime.UtcNow;
            var timer = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                success = dependency.Call();
            }
            catch (Exception ex)
            {
                success = false;
                client.TrackException(ex);
                throw new Exception("Operation went wrong", ex);
            }
            finally
            {
                timer.Stop();
                client.TrackDependency("DependencyType", "myDependency", "myCall", startTime, timer.Elapsed, success);
            }
        }
        */
    }
}