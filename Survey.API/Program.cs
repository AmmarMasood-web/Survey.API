global using Serilog;
global using ILogger = Serilog.ILogger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog.Events;
using Serilog.Formatting.Json;
using Survey.API.Classes;
using Survey.API.Database;
using Survey.API.Exceptions;
using Survey.API.Helpers;
using Survey.API.Interfaces;
using Survey.API.Services;
using SurveyAPI.Services;
using System.Text;

namespace Survey.API;

public class Program
{
    public IConfiguration Configuration { get; }
    public Program(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            //Logger Configuration
            var LoggingPath = builder.Configuration.GetSection("LogPath");
            string path = LoggingPath.GetValue<string>("logPath")!;
            Log.Logger = new LoggerConfiguration()
                       .WriteTo.File(new JsonFormatter(),
                           "important-logs.json",
                           restrictedToMinimumLevel: LogEventLevel.Debug)
                       .WriteTo.File(path,
                       rollingInterval: RollingInterval.Day)
                   .MinimumLevel.Debug()
                   .CreateLogger();

            //Token Configuration
            var tokenConfiguration = builder.Configuration.GetSection("tokenConfiguration");
            string clientId = tokenConfiguration.GetValue<string>("clientId")!;
            string tokenSecret = tokenConfiguration.GetValue<string>("tokenSecret")!;

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var exceptionType = context.Exception.GetType();
                        if (exceptionType == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.StatusCode = 403;
                            context.Response.WriteAsync(
                            JsonConvert.SerializeObject(new
                            {
                                error = "Token Expired"
                            }));
                        }
                        else if (exceptionType == typeof(SecurityTokenInvalidSignatureException))
                        {
                            context.Response.StatusCode = 401;
                            context.Response.WriteAsync(
                            JsonConvert.SerializeObject(new
                            {
                                error = "Invalid Token"
                            }));
                        }
                        return Task.CompletedTask;
                    },
                };
            });

            // Api versioning middleware
            builder.Services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(majorVersion: 1, minorVersion: 0);
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.ReportApiVersions = true;
                config.ApiVersionReader = new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader();
            });

            builder.Services.AddVersionedApiExplorer(config =>
            {
                config.GroupNameFormat = "'v'VVV";
                config.SubstituteApiVersionInUrl = true;
            });
            //Enable CORS 
            builder.Services.AddCors(config =>
            {
                config.AddDefaultPolicy(
                    bldr =>
                    {
                        bldr.WithOrigins("http://127.0.0.1:4200") //todo
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddCors();


            builder.Services.AddSingleton<IAuthentication>(new AuthenticationService(tokenSecret, clientId));
            builder.Services.AddSingleton<INanoIdInitializer, NanoIdInitializer>();
            builder.Services.AddScoped<ISurvey, SurveyService>();

            //Automapper
            builder.Services.AddAutoMapper(typeof(DTOsMapper));

            //Exception Middleware
            builder.Services.AddExceptionHandler<AppExceptionHandler>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            //Database Context Handling
            builder.Services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));

            var app = builder.Build();

            //Nanoid Initialization
            var nanoIdInitializer = app.Services.GetRequiredService<INanoIdInitializer>();
            nanoIdInitializer.Initialize();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Exception Handler
            app.UseExceptionHandler(_ => { });

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            Log.Information("Application has been Initialized successfully with id ===> " + nanoIdInitializer.ApplicationId);

            app.Run();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
