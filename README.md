# apac-breachers-ranked-bot

A development version of the bot is setup and running in this server: https://discord.gg/TjxMW4em

DM me (jasonp2009) for the bot token, discord automatically resets the token if I commit it to this repo.

If you make any changes which affect the database schema you will have to either delete all tables in the database with

EXEC sp_MSforeachtable @command1 = "DROP TABLE ?"

Or manually update the schema to match your changes.
