# Discord Ping Bot

### Configuration
1. Get a discord bot token
2. Setup a sql database with discordpingbot.sql
3. Enter required environment variables
	- `DISCORD_PINGBOT_TOKEN` token generated from discord
	- `DISCORD_PINGBOT_DB_PASSWORD` password for sql database
4. Open the .sln and run!

### Bot Commands

| Command  |  Description |
| :------------ | :------------ |
| `^pingbot | shows the configuration |

### Bot About

Warn: Current only works for a single channel on a single server 

Selects a random user to ping with a message

To configure you need to directly modify the database