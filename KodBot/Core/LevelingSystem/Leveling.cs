using Discord.WebSocket;
using System;
using Discord;
using System.Collections.Generic;

namespace KodBot.Core.LevelingSystem
{
    internal static class Leveling 
    {
        internal static async void UserSentMessage(SocketGuildUser user, SocketTextChannel channel, SocketMessage message)
        {
            Utilities.SendEvent("Message by " + user); //AI

            var userAccount = UserAccounts.UserAccounts.GetAccount(user);

            userAccount.Nick = user.Username;
            char[] sep = { ',' };
            IMessage msg = await message.Channel.GetMessageAsync(message.Id);
            string[] msgArray = msg.ToString().Split(sep);

            List<string> holy = new List<string>();
            holy.Add("god");
            holy.Add("dio");
            holy.Add("jesus");
            holy.Add("gesù");
            holy.Add("gesu");

            foreach (string word in msgArray)
            {
                if (holy.Contains(word))
                {                  
                    Console.WriteLine("God Find!");
                    Utilities.SendEvent("GodFind");
                    Utilities.AddDpoint(user);
                    Utilities.SendMetric(Convert.ToString(user.Username) + "DPoint",1);
                }
            }

            uint oldLvl = userAccount.Lvl;
            userAccount.XP += 5;
            UserAccounts.UserAccounts.SaveAccounts();
            uint newLvl = userAccount.Lvl;

            if(oldLvl != newLvl)
            {
                var embed = new EmbedBuilder()
                {
                    Color = Color.Green,
                    Title = $"{user.Username} level up!",
                    Description = $"{user.Username} has reached lvl **{newLvl}**"
                }.Build();
                await channel.SendMessageAsync("", false, embed);
                Utilities.SendMetric("lvl." + user, newLvl); //AI
            }
        }
    }
}