using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Discord.WebSocket;
using Discord;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.DependencyCollector;
using System.Data.SqlClient;
using System.Data;

namespace KodBot
{
    class Program
    {
        DiscordSocketClient _client;
        CommandHandler _handler;
        public string log;
        public static string reply;
        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();
        public async Task StartAsync()
        {
            #region AI
            TelemetryConfiguration configuration = new TelemetryConfiguration();
            configuration.InstrumentationKey = "37c0c49a-223d-450f-b4c2-776d69cf3956";

            DependencyTrackingTelemetryModule depModule = new DependencyTrackingTelemetryModule();
            depModule.Initialize(TelemetryConfiguration.Active);

            QuickPulseTelemetryProcessor processor = null;

            configuration.TelemetryProcessorChainBuilder
                .Use((next) =>
                {
                    processor = new QuickPulseTelemetryProcessor(next);
                    return processor;
                })
                        .Build();

            var QuickPulse = new QuickPulseTelemetryModule()
            {
                AuthenticationApiKey = "2vmbyql0gneokktc5z2d48mkcbpzfe4tcmprnxxm"
            };
            QuickPulse.Initialize(configuration);
            QuickPulse.RegisterTelemetryProcessor(processor);
            foreach (var telemetryProcessor in configuration.TelemetryProcessors)
            {
                if (telemetryProcessor is ITelemetryModule telemetryModule)
                {
                    telemetryModule.Initialize(configuration);
                }
            }
            #endregion

            if (Config.bot.token == "" || Config.bot.token == null)
            {
                return;
            }
            else
            {
                _client = new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose
                });
                _client.Log += Log;
                await _client.LoginAsync(TokenType.Bot, Config.bot.token);
                await _client.StartAsync();
                _handler = new CommandHandler();
                await _handler.InitializeASync(_client);
                await Task.Delay(-1);
            }
        }

        #region log
        private async Task Log(LogMessage msg)
        {
            log = msg.Message;
            Console.WriteLine(log);
            string[] ping = log.Split();
            ping = ping[2].Split(separator: ',');
            reply = Convert.ToString(Convert.ToInt64(ping[0])/3);
        }

        public static string SendPing()
        {
            return reply;
        }
        #endregion
    }
}