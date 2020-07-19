
using DiscordRPC;
using DiscordPingBot;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Modes;
using System.Linq;
using Renci.SshNet.Messages;

namespace Discord_Bot.src.Modules.Commands
{
    public class Commands : BaseCommandModule
    {

        [Command("help")]
        public async Task PingHelp(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("```\n" + Bot.prefix + string.Join(Bot.prefix,
                new string[] {
                    "pingbot\n",
                }) + "```");
        }

        [Command("pingbot")]
        public async Task PingBot(CommandContext ctx)
        {
            Int32 now = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            DbConnection conn = new DbConnection(Bot.dbSettings);
            DbApi dbApi = new DbApi(conn);

            Int32 last_sent = Int32.Parse(dbApi.GetLastSent());
            Int32 cooldown = Int32.Parse(dbApi.GetCooldown());

            Int32 next_send = last_sent + cooldown;
            if (now > next_send)
            {
                next_send = now;
            }

            string last_sent_str = UnixTimeStampToDateTime(last_sent).ToString();
            if (last_sent == 0)
            {
                last_sent_str = "Never";
            }

            string message = dbApi.GetMessage();
            string channel = dbApi.GetChannel();
            await ctx.Channel.SendMessageAsync(
                "message: " + message + "\n" + 
                "channel: <#" + channel + ">\n" +
                "next send: " + UnixTimeStampToDateTime(next_send).ToString() + "\n" +
                "last sent: " + last_sent_str + "\n" +
                "cooldown: " + cooldown.ToString() + " sec"); ;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {

            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

    }
}
