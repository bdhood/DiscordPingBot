using Discord_Bot.src.Modules.Commands;
using DiscordRPC;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;


namespace DiscordPingBot
{
    public class Bot
    {

        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public static DbSettings dbSettings { get => _dbSettings; }
        private static DbSettings _dbSettings;
        public static string prefix;
        private Random rand;

        public async Task RunAsync()
        {
            rand = new Random(Environment.TickCount);

            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<Configjson>(json);

            var botkey = Environment.GetEnvironmentVariable("DISCORD_PINGBOT_TOKEN");
            if (string.IsNullOrEmpty(botkey)) { 
                throw new Exception("Environment variable DISCORD_PINGBOT_TOKEN is not set!");
            }

            string mysql_password = Environment.GetEnvironmentVariable("DISCORD_PINGBOT_DB_PASSWORD");
            if (string.IsNullOrEmpty(mysql_password))
            {
                throw new Exception("Environment variable DISCORD_PINGBOT_DB_PASSWORD is not set!");
            }

            _dbSettings = configJson.dbSettings;
            _dbSettings.password = mysql_password;

            var config = new DiscordConfiguration
            {
                Token = botkey,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Debug,
                UseInternalLogHandler = true
            };

            Client = new DiscordClient(config);

            Client.Heartbeated += Client_Heartbeated;

            if (configJson.Prefix.Length > 0)
            {
                prefix = configJson.Prefix[0];
            }

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = configJson.Prefix,
                EnableDms = true,
                EnableMentionPrefix = true,
                EnableDefaultHelp = false,
                IgnoreExtraArguments = true,
            };
            Commands = Client.UseCommandsNext(commandsConfig);

            //REGRISTRATION FOR COMMAND FILES:
            Commands.RegisterCommands<Commands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Heartbeated(HeartbeatEventArgs e)
        {
            Int32 now = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            DbConnection conn = new DbConnection(Bot.dbSettings);
            DbApi dbApi = new DbApi(conn);

            Int32 last_sent = Int32.Parse(dbApi.GetLastSent());
            Int32 cooldown = Int32.Parse(dbApi.GetCooldown());

            Int32 next_send = last_sent + cooldown;
            if (next_send <= now)
            {
                string message = dbApi.GetMessage();
                ulong channelId = ulong.Parse(dbApi.GetChannel());
                DiscordChannel channel = await e.Client.GetChannelAsync(channelId);
                var users = channel.Users.ToArray();
                DiscordUser user;
                while (true)
                {
                    user = users[rand.Next(users.Count())];
                    if (!user.IsBot)
                    {
                        break;
                    }
                }
                message = message.Replace("<user>", user.Mention);
                await e.Client.SendMessageAsync(channel, message);
                dbApi.SetLastSent(now.ToString());
            }
        }
    }
}
