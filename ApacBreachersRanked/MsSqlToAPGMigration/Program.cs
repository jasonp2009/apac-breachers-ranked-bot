// See https://aka.ms/new-console-template for more information

using ApacBreachersRanked.Domain.MatchQueue.Entities;
using ApacBreachersRanked.Infrastructure.Config;
using ApacBreachersRanked.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

var msSqlOptions = new RdsOptions()
{
    DatabaseEngine = DatabaseEngine.SqlServer,
    UserName = "admin",
    Password = "0DqVZeNtyPaCuOSJ",
    DbName = "ApacBreachersDb-Live",
    HostName = "abr-db.c8nom3bphtyr.ap-southeast-2.rds.amazonaws.com"
};

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var msSqlDbContext = new BreachersDbContext(msSqlOptions);

var apgOptions = new RdsOptions()
{
    DatabaseEngine = DatabaseEngine.Postgress,
    UserName = "postgres",
    Password = "39Cn3XT].$+_lSvp:OksOx478nY]",
    DbName = "ApacBreachersDb-Live",
    HostName = "abr-db-apg.cluster-c8nom3bphtyr.ap-southeast-2.rds.amazonaws.com"
};


var apgDbContext = new BreachersDbContext(apgOptions);

try
{
    var entities = await msSqlDbContext.PendingMatchScores
        .Include(x => x.Score)
        .Include(x => x.Players)
        .ToListAsync();

    foreach (var entity in entities)
    {
        apgDbContext.PendingMatchScores.Add(entity);
    }
    await apgDbContext.SaveChangesAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}
