using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Serilog;
using Hangfire;
using Serilog.Exceptions;
using ServicePortal.Middleware;
using ServicePortals.Application.Interfaces.Auth;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Application.Interfaces.Role;
using ServicePortals.Application.Interfaces.TypeLeave;
using ServicePortals.Application.Interfaces.LeaveRequest;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Services.Auth;
using ServicePortals.Infrastructure.Services.Role;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Services.TimeKeeping;
using ServicePortal.Infrastructure.Cache;
using ServicePortals.Infrastructure.Excel;
using ServicePortals.Application.Interfaces.TimeKeeping;
using ServicePortals.Application.Interfaces.UserConfig;
using ServicePortals.Application.ScheduleJob;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Services.OrgUnit;
using Microsoft.OpenApi.Models;
using ServicePortals.Application.Interfaces.SystemConfig;
using ServicePortals.Application.Services.SystemConfig;
using ServicePortals.Application.Interfaces.DelegatedTemp;
using ServicePortals.Application.Services.DelegatedTemp;
using ServicePortals.Application.Services.Role;
using ServicePortals.Application.Services.User;
using ServicePortals.Application.Services.MemoNotification;
using ServicePortals.Application.Services.LeaveRequest;
using ServicePortals.Application.Interfaces.RequestType;
using ServicePortals.Application.Services.RequestType;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Application.Services.Approval;
using ServicePortals.Application.Services.TypeLeave;
using ServicePortals.Application.Interfaces.ApprovalFlow;
using ServicePortals.Application.Services.ApprovalFlow;
using ServicePortals.Application.Services.Auth;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Application.Services.ITForm;
using ServicePortals.Application.Interfaces.Priority;
using ServicePortals.Application.Services.Priority;
using ServicePortals.Application.Interfaces.CostCenter;
using ServicePortals.Application.Services.CostCenter;
using ServicePortals.Application.Interfaces.Purchase;
using ServicePortals.Application.Services.Purchase;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace ServicePortal
{
    public class Program
    {
        public static void Main(string[] args)
        {
            #region Config Serilog

            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", DateTimeOffset.UtcNow.ToString("yyyy"), DateTimeOffset.UtcNow.ToString("MM"));

            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }

            var logFile = Path.Combine(logDir, "log-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.File(
                    path: logFile,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] ===========> {Message:lj}{NewLine}{Exception}",
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 90,
                    shared: true
                )
                .WriteTo.Console()
                .CreateLogger();

            #endregion

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();

            builder.Services.AddHealthChecks();

            #region Config SqlServer

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("StringConnectionDb"))
                        .LogTo(Console.WriteLine, LogLevel.Information)
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            #endregion

            #region Config hangfire
            builder.Services.AddHangfire(config =>
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(builder.Configuration.GetConnectionString("StringConnectionDb")));

            builder.Services.AddHangfireServer();

            #endregion

            builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("SmtpSettings"));

            #region DI

            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IRoleService, RoleService>();

            builder.Services.AddScoped<ITypeLeaveService, TypeLeaveService>();

            builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();

            builder.Services.AddScoped<IEmailService, SmtpEmailService>();

            builder.Services.AddScoped<IMemoNotificationService, MemoNotificationService>();

            builder.Services.AddSingleton<ICacheService, CacheService>();

            builder.Services.AddScoped<IUserConfigService, UserConfigService>();

            builder.Services.AddScoped<ITimeKeepingService, TimeKeepingService>();

            builder.Services.AddScoped<JwtService>();

            builder.Services.AddScoped<ExcelService>();

            builder.Services.AddScoped<IOrgUnitService, OrgUnitService>();

            builder.Services.AddScoped<IApprovalFlowService, ApprovalFlowService>();

            builder.Services.AddScoped<ISystemConfigService, SystemConfigService>();

            builder.Services.AddScoped<IDelegatedTempService, DelegatedTempService>();

            builder.Services.AddScoped<IPermissionService, PermissionService>();

            builder.Services.AddScoped<IRequestTypeService, RequestTypeService>();

            builder.Services.AddScoped<IApprovalService, ApprovalService>();

            builder.Services.AddScoped<IOrgPositionService, OrgPositionService>();

            builder.Services.AddScoped<ITFormService, ITFormServiceImpl>();

            builder.Services.AddScoped<ITCategoryService, ITCategoryServiceImpl>();

            builder.Services.AddScoped<IPriorityService, PriorityService>();

            builder.Services.AddScoped<ICostCenterService, CostCenterService>();

            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
            #endregion

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddControllers();

            #region Multiple language

            builder.Services.AddLocalization(options => options.ResourcesPath = "Application/Resources");

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("vi"),
                };

                options.DefaultRequestCulture = new RequestCulture("en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            #endregion

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            #region Cors

            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowPrivate",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins ?? new[] { "http://localhost:5173" })
                              .AllowAnyMethod()
                              .AllowAnyHeader()
                              .AllowCredentials();
                    });
            });

            #endregion

            #region JWT

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")),
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Log.Error($"JWT error: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },
                    };
                });

            builder.Services.AddAuthorization();

            #endregion

            #region Configure validation

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .Select(e => new
                        {
                            Field = e.Key,
                            Errors = e.Value!.Errors.Select(e => e.ErrorMessage)
                        });

                    return new UnprocessableEntityObjectResult(new
                    {
                        status = 422,
                        message = "Validation failed",
                        errors = errors
                    });
                };
            });

            #endregion

            builder.Services.AddSignalR();

            //memory cache
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            //global exception
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseRouting();

            //check app is running
            app.MapHealthChecks("/health");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("AllowPrivate");

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // Add localization middleware
            app.UseRequestLocalization();

            app.MapControllers();

            //default redirect to swagger
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger/index.html", permanent: false);
                    return;
                }
                await next();
            });

            app.UseHangfireDashboard();

            JobScheduleConfig.Configure();

            app.Run();
        }
    }
}
