# apac-breachers-ranked-bot

A development version of the bot is setup and running in this server: https://discord.gg/TjxMW4em

DM me (jasonp2009) for the bot token, discord automatically resets the token if I commit it to this repo.

If you make any changes which affect the database schema you will have to either delete all tables in the database with

EXEC sp_MSforeachtable @command1 = "DROP TABLE ?"

Run it a few times until there are no errors. On restart of the bot, EFCore will rebuild the tables/schemas.

You will need to run it from SSMS which can be downloaded here: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16

Login credentials can be found under ApacBreachersRanked\ApacBreachersRanked\appsettings.Development.json
This will only give you access to the dev db btw, not the one that is used by the bot in the BETA server.

Or manually update the schema to match your changes.
