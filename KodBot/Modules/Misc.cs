using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KodBot.Core.UserAccounts;
using NReco.ImageGenerator;
using System.IO;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights.DataContracts;

namespace KodBot.Modules
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        //[RequireBotPermission(GuildPermission.Administrator)]
        private const string configFolder = "Resources";
        private const string configFile = "config.json";
        //private static string filePath = "Resources/accounts.json";


        //Public commands
        #region echo
        [Command("Echo")]
        public async Task Echo([Remainder]string msg)
        {
            var embed = new EmbedBuilder() {
                Title = "Echo ",
                Description = msg,
                Color = Color.Red
            }.Build();

            Utilities.SendEvent(embed.Title + msg);
            await Context.Channel.SendMessageAsync("",false,embed);
        }
        #endregion

        #region userinfo
        [Command("UserInfo")]
        public async Task UserInfo(SocketUser user)
        {
            //var account = UserAccounts.GetAccount((SocketUser)userTag);
            var avatarUrl = user.GetAvatarUrl(Discord.ImageFormat.Gif, 64); 
            var embed = new EmbedBuilder()
            {
                //ImageUrl = avatarUrl,
                Title = "UserInfo",
                Description = "This command will show your info\n\n" 
                + user + " has the following information:\n"
                + "Current status: " + user.Status + "\n" 
                + "UserID: " + user.Id + "\n"
                + "UserTag: " + user.Mention,
                Color = Color.Green
            }.Build();
            Utilities.SendEvent(embed.Title);
            await Context.Channel.SendMessageAsync("",false, embed);
            return;
        }
        #endregion

        #region role
        [Command("Role")]
        public async Task Role(SocketGuildUser user)
        {
            var roles = new List<string>();
            foreach (SocketRole role in ((SocketGuildUser) user).Roles)
            {
                roles.Add(role.Name);
            }            

            var embed = new EmbedBuilder()
            {
                Title = "Role",
                Description = $"{user.Username} has the following roles**{"\n" + String.Join("\n",roles)}**",
                Color = Color.Blue
            }.Build();
            Console.WriteLine(user.Roles);
            Utilities.SendEvent(embed.Title);
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

        #region stats
        [Command("stats")]
        public async Task stats(SocketUser user)
        {
            var account = UserAccounts.GetAccount(user);
            var avatarUrl = user.GetAvatarUrl(Discord.ImageFormat.Jpeg, 64);
            uint level = (uint)Math.Sqrt(account.XP / 10);

            var embed = new EmbedBuilder()
            {
                Title = $"{user} | Level **{level}**\n",
                ImageUrl = avatarUrl,
                Description = $"\nExp [{account.XP}]"
                            + $"\nExp for next level [{Convert.ToInt32((((level+1)*(level+1))*10)-account.XP)}]",
                Color = Color.LightOrange
            }.Build();
            Utilities.SendEvent(embed.Title);
            await Context.Channel.SendMessageAsync("",false,embed);
            return;
        }
        #endregion

        #region vote
        /*[Command("vote")]
        public async Task Vote(SocketMessage msg)
        {
            string cmd = "Vote";
            var t = new Emoji("✔️");
            var f = new Emoji("❌");
            var rMessage = (RestUserMessage)await msg.Channel.GetMessageAsync(msg.Id);
            Utilities.SendEvent(cmd + Convert.ToString(msg));
            //await Context.Channel.SendMessageAsync("The vote has started!\n" + msg, false);
            //Context.Message.AddReactionAsync(t);
        }*/
        #endregion

        #region Ping
        [Command("Ping")]
        public async Task ping()
        {
            await Context.Channel.SendMessageAsync("Pong!", false);
            string userPing = Program.SendPing();
            var embed = new EmbedBuilder()
            {
                Title = ":clock1: " + userPing + "ms ",
                Color = Color.DarkGrey
            }.Build();

            Utilities.SendEvent("ping" + userPing);
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

        #region turgido
        
        [Command("turgido")]
        public async Task turgido(SocketUser user)
        {
            var avatarUrl = user.GetAvatarUrl();
            int h = 300;
            int w = 300;
            string css = "<style>\nbody\n{margin: auto;\n background: trasparent \n}</style>\n";
            string html = String.Format("<html>\n<img style='width:{0}px; height:{1}px;' src='{2}' style='z-index:-9999;'>\n<img src='https://upload.wikimedia.org/wikipedia/commons/thumb/f/fb/Rainbow-gradient-fully-saturated-diagonal.svg/1024px-Rainbow-gradient-fully-saturated-diagonal.svg.png' style='background-color:red; width:{0}px;height:{1}px;position: absolute;top:0px;left:0px;opacity: 0.7;'/>\n</html>", w,h,avatarUrl);
            var convert = new HtmlToImageConverter
            {
                Width = h,
                Height = w
            };
            var jpg = convert.GenerateImage(css + html, NReco.ImageGenerator.ImageFormat.Jpeg);
            Utilities.SendEvent("turgido");
            await Context.Channel.SendFileAsync(new MemoryStream(jpg), Convert.ToString(user + ".jpg"));
        }

        #endregion

        #region question
        [Command("qs")]
        public async Task qs([Remainder]string msg)
        {
            var answer = new List<string>() { "Yes", "Maybe", "Are you an idiot?", "WTF?!", "I have to answer?", "Nah", "Are you gay?", "Why are you running? :runner:" };
            var index = new Random();
            int n = index.Next(answer.Count);
            Utilities.SendEvent("question");
            await Context.Channel.SendMessageAsync(answer[n]);
        }
        #endregion

        #region menu
        [Command("menu")]
        public async Task menu([Remainder]string msg)
        {
            var user = UserAccounts.GetAccount(Context.User);
            char[] sep = { ',' };
            string[] menu = msg.Split(sep);

            if (menu[0] == "") user.P = "";
            if (menu[1] == "") user.S = "";
            if (menu[2] == "") user.C = "";

            user.P = menu[0];
            user.S = menu[1];
            user.C = menu[2];
            UserAccounts.SaveAccounts();

            Utilities.SendEvent("menu");
            await Context.Channel.SendMessageAsync("Menu salvato!");
        }
        #endregion

        #region getmenu
        [Command("getmenu")]
        public async Task getmenu(SocketUser user)
        {
            var account = UserAccounts.GetAccount(user);

            var embed = new EmbedBuilder()
            {
                Title = $"Menu di {user}",
                Description = $"1.{account.P}\n2.{account.S}\n3.{account.C}",
                Color = Color.Orange
            }.Build();

            Utilities.SendEvent("getmenu");
            await Context.Channel.SendMessageAsync("",false,embed);
        }
        #endregion

        #region getgod
        [Command("getgod")]
        public async Task getgod()
        {
            string[] txt = (File.ReadAllText(configFolder + "/dpoint.txt",Encoding.Default)).Split(separator:':');
            string[] id = new string[txt.GetLength(0)/2];
            int[] dpoint = new int[txt.GetLength(0)/2];
            int cu = 0;
            int cp = 0;

            for (int i = 0; i < txt.GetLength(0); i++)
            {
                if (i % 2 == 0)
                {
                    id[cu] = txt[i];
                    cu++;
                }

                if (i % 2 != 0)
                {
                    dpoint[cp] = Convert.ToInt32(txt[i]);
                    cp++;
                }
            }

            int temp = 0;
            string IDtemp = "";

            for (int write = 0; write < dpoint.Length; write++)
            {
                for (int sort = 0; sort < dpoint.Length - 1; sort++)
                {
                    if (dpoint[sort] < dpoint[sort + 1])
                    {
                        IDtemp = id[sort + 1];
                        id[sort + 1] = id[sort];
                        id[sort] = IDtemp;

                        temp = dpoint[sort + 1];
                        dpoint[sort + 1] = dpoint[sort];
                        dpoint[sort] = temp;
                    }
                }
            }

            var embed = new EmbedBuilder()
            {
                Title = $"God Leaderboards",
                Description = $":first_place: <@{id[0]}> | {dpoint[0]}\n\n:second_place: <@{id[1]}> | {dpoint[1]}\n\n:third_place: <@{id[2]}> | {dpoint[2]}",
                Color = Color.DarkPurple
            }.Build();

            Utilities.SendEvent("getgod");
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

        #region commands
        [Command("commands")]
        public async Task Commands()
        {
            var cmds = new List<string>() {"commands","echo","getgod","getmenu","menu","ping","stats","turgido","userinfo","qs"/*,"vote"*/};
            var embed = new EmbedBuilder()
            {
                Title = "Commands",
                Description = $"{Config.bot.cmdPrefix}{String.Join("\n"+ Config.bot.cmdPrefix, cmds)}",
                Color = Color.DarkerGrey
            }.Build();

            Utilities.SendEvent("Commands");
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

        //Admin commands
        #region commandsAdmin
        [Command("ca")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ca()
        {
            var cmds = new List<string>() { "addrole","addxp", "changePrefix","removerole", "restart", "stop","test"};
            var embed = new EmbedBuilder()
            {
                Title = "CommandsAdmin",
                Description = $"{Config.bot.cmdPrefix}{String.Join("\n" + Config.bot.cmdPrefix, cmds)}",
                Color = Color.DarkerGrey
            }.Build();

            Utilities.SendEvent("Commands");
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

        #region addXP
        [Command("addXP")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task addxp(SocketUser user, uint xp)
        {
            var account = UserAccounts.GetAccount(user);
            account.XP += xp;
            account.Lvl = (uint)Math.Sqrt(account.XP / 5);
            UserAccounts.SaveAccounts();

            var embed = new EmbedBuilder()
            {
                Title = "AddXP ",
                Description = $"{user.Mention} gained {xp} xp!\nNow {user.Mention} has {account.XP}!",
                Color = Color.Blue
            }.Build();

            Utilities.SendEvent(embed.Title);
            await Context.Channel.SendMessageAsync("",false,embed);
        }
        #endregion

        #region changePrefix
        [Command("changePrefix")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task changePrefix(string prefix)
        {
            string json = File.ReadAllText(configFolder + "/" + configFile);
            var bot = JsonConvert.DeserializeObject<BotConfig>(json);
            string newP = json.Replace(bot.cmdPrefix, prefix);

            File.WriteAllText(configFolder + "/" + configFile, newP);

            Utilities.SendEvent("changePrefix");
            await Context.Channel.SendMessageAsync($"Bot prefix is chenged! Now you have to use <{prefix}> to run a command!");
            Console.WriteLine("Restart!");
            Utilities.Restart();
        }
        #endregion

        #region restart
        [Command("restart")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task restart()
        {
            Utilities.SendEvent("Restart");
            await Context.Channel.SendMessageAsync($"Bot is restarting!");
            Console.WriteLine("Restart!");
            Utilities.Restart();
        }
        #endregion

        #region stop
        [Command("stop")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task stop()
        {
            Utilities.SendEvent("Stop");
            await Context.Channel.SendMessageAsync($"Bye Bye!");
            Console.WriteLine("Stop!");
            Environment.Exit(1);
        }
        #endregion

        #region test
        [Command("TestAI")]
        [RequireBotPermission(GuildPermission.Administrator)]
        public async Task TestAI()
        {
            Utilities.SendEvent("Test");
            Utilities.SendMetric("MetricTest", 1);
            Utilities.SendPageView("TestPageView");
            Utilities.SendTrace("TestTrace", SeverityLevel.Information, new Dictionary<string, string> { { "InformationTrace","1"} });
            Console.WriteLine("Test");
            await Context.Channel.SendMessageAsync("Sent some information...");
        }
        #endregion

        #region addrole
        [Command("addrole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task addrole(SocketUser user, string role)
        {
            var nrole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == role);
            await (user as IGuildUser).AddRoleAsync(nrole);

            var embed = new EmbedBuilder()
            {
                Title = "AddRole ",
                Description = $"{Context.User.Username} add role {role} to {user.Mention}!",
                Color = Color.Purple
            }.Build();

            Utilities.SendEvent(embed.Title);
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

        #region removerole
        [Command("removerole")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task removerole(SocketUser user, string role)
        {
            var nrole = Context.Guild.Roles.FirstOrDefault(x => x.Name.ToString() == role);
            await (user as IGuildUser).RemoveRoleAsync(nrole);

            var embed = new EmbedBuilder()
            {
                Title = "AddRole ",
                Description = $"{Context.User.Username} remove role {role} from {user.Mention}!",
                Color = Color.Gold
            }.Build();

            Utilities.SendEvent(embed.Title);
            await Context.Channel.SendMessageAsync("", false, embed);
        }
        #endregion

    }
}