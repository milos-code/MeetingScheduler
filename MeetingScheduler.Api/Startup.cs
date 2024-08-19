using MeetingScheduler.Bussines.Mappings;
using MeetingScheduler.Bussines.Services.Interfaces;
using MeetingScheduler.Bussines.Services;
using MeetingScheduler.Infrastructure.AppContext;
using MeetingScheduler.Infrastructure.Models;
using MeetingScheduler.Infrastructure.Repositories.Interfaces;
using MeetingScheduler.Infrastructure.Repositories;
using MeetingScheduler.Infrastructure.Seeder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MeetingScheduler.Bussines.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MeetingScheduler.Bussines.Services.JwtTokenGenerator;
using MeetingScheduler.Api.ExceptionMiddleware;
using Serilog;
using Hangfire;
using MeetingScheduler.Bussines.Services.BackgroundJobs;
using System.Diagnostics.CodeAnalysis;

namespace MeetingScheduler.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup(IConfiguration configuration)
    {
        private readonly IConfiguration _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMeetingRepository, MeetingRepository>();
            services.AddScoped<IMeetingRoomRepository, MeetingRoomRepository>();
            services.AddScoped<IMeetingNotesRepository, MeetingNotesRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMeetingUserRepository, MeetingUserRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMeetingService, MeetingService>();
            services.AddScoped<IMeetingRoomService, MeetingRoomService>();
            services.AddScoped<IMeetingNotesService, MeetingNotesService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IUserHelperService, UserHelperService>();
            services.AddHttpContextAccessor();

            services.AddExceptionHandler<BadRequestExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddScoped<IMeetingReminderService, MeetingReminderService>();
            services.AddScoped<IMeetingCompleteService, MeetingCompleteService>();

            services.AddHangfire(configuration =>
                configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(_configuration.GetConnectionString("MeetingScheduler")));

            services.AddHangfireServer();
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            services.AddFluentEmail(_configuration.GetSection("EmailSettings").GetSection("From").Value)
                .AddSmtpSender(_configuration.GetSection("EmailSettings").GetSection("Host").Value, Convert.ToInt32(_configuration.GetSection("EmailSettings").GetSection("Port").Value));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddDbContext<MeetingSchedulerContext>(options => options.UseSqlServer(_configuration.GetConnectionString("MeetingScheduler")));

            services.AddTransient<SeedData>();

            services.AddIdentity<User, IdentityRole<Guid>>(opt =>
            {
                opt.Password.RequiredLength = 6;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireNonAlphanumeric = false;

                opt.User.RequireUniqueEmail = true;

                opt.SignIn.RequireConfirmedEmail = true;
            })
             .AddEntityFrameworkStores<MeetingSchedulerContext>()
             .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(MappingProfile));


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = _configuration["Jwt:Audience"],
                        ValidIssuer = _configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretForKey"]))
                    };
                });


            services.Configure<IdentityOptions>(options =>
            {
                // Configure Customize password requirements, lockout settings, etc.
            });

            services.AddAuthorization();
        }

        public void Configure(WebApplication app, IWebHostEnvironment env)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var seeder = scope.ServiceProvider.GetRequiredService<SeedData>();
                seeder.Seed().Wait();
            }

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            app.UseExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllers();

            RecurringJob.AddOrUpdate<IMeetingReminderService>(serivce => serivce.ProcessMeetingBackgroundJob(), Cron.MinuteInterval(5));

            RecurringJob.AddOrUpdate<IMeetingCompleteService>(cancel => cancel.CompleteMeetingBackgroundJob(), Cron.MinuteInterval(15));

            app.Run();
        }
    }
}
