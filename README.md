# apac-breachers-ranked-bot

## Tools

Visual Studio 2022 (Community Edition): https://visualstudio.microsoft.com/

Github Desktop: https://desktop.github.com/

SQL Server Management Studio: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16

## Getting setup

A development version of the bot is setup in this server: https://discord.gg/TjxMW4em

All the appsettings.Development.json config points to the above server/bot/dev database.

The only thing you'll need is the bot token, DM me (jasonp2009) for this. Discord automatically resets the token if I commit it to this repo.

## Contributing

Create a new branch off the develop branch.

Make any changes you'd like, test it out on the dev version of the bot.

Raise a pull request to the develop branch and I will review it.

## Architecture

The code follows clean architecture/domain-driven design concepts.

The solution is broken into 4 projects:

 - Domain: Has the responsibilities of being a breachers ranked system. Is not aware of discord or that it is a discord bot.
 - Application: Has the responsibilities of interfacing with the domain and orchestrating commands, events, etc. Is aware of discord, but not know that it is a bot.
 - Bot (ApacBreachersRanked project): Has the responsibility of mapping bot commands, components interactions, etc. to the application layer. In a traditional API application, this would be the API layer with all the controllers. Modules are the discord bot version of controllers.
 - Infrastructure: Has the responsibility of implementing the infrastructure dependencies of the Domain/Application layer (defined in those layers as interfaces). At the moment the only infrastructure dependency is the database, but there may be more in the future (eg if I decide to decouple event/INotification handling into a separate consumer application so the bot is more responsive to interactions).
