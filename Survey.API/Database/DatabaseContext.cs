using Microsoft.EntityFrameworkCore;
using Survey.API.Interfaces;
using Survey.API.Models;

namespace Survey.API.Database;

public class DatabaseContext : DbContext
{
    private readonly INanoIdInitializer _nanoId;
    public DatabaseContext(DbContextOptions<DatabaseContext> opt, INanoIdInitializer nanoId) : base(opt)
    {
        _nanoId = nanoId;
        try
        {
            Database.EnsureCreated();

        }
        catch (Exception ex)
        {
            Log.Error("{0} :", _nanoId.ApplicationId + "Error Occured in Database Context with message" + ex.Message);
        }
    }
    public DbSet<SurveyModel> Surveys { get; set; }
}
