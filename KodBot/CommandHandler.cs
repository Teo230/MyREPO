using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using System.IO;
using System.Reflection;
using KodBot.Core.LevelingSystem;
using Microsoft.ApplicationInsights.DataContracts;

namespace KodBot
{
    class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeASync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(),null);
            _client.MessageReceived += HandleCommandASync;
        }

        private async Task HandleCommandASync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(_client, msg);

            //Lvl Up
            if (context.User.IsBot) return;
            Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel, context.Message);
            Utilities.SendPageView(context.Channel.Name);

            int argPos = 0;
            if (msg.HasStringPrefix(Config.bot.cmdPrefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos,null);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                    Utilities.SendTrace("CommandError", SeverityLevel.Error, new Dictionary<string, string> { { "Description", Convert.ToString(result.ErrorReason) } });

                    var embed = new EmbedBuilder()
                    {
                        Title = "Error: " + Convert.ToString(result.Error),
                        Description = result.ErrorReason,
                        Color = Color.Red,
                    }.Build();
                    await context.Channel.SendMessageAsync("",false,embed);
                }
            }
            
        }
    }
}