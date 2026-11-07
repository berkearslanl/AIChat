using _14_15_AIChat.Context;
using _14_15_AIChat.Entities;
using _14_15_AIChat.Helpers;
using _14_15_AIChat.Services.ChatServices;
using _14_15_AIChat.Services.VoiceServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;

namespace _14_15_AIChat.Registration
{
    public static class ServiceRegistration
    {
        public static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddHttpClient<IAiChatService, AiChatService>(client =>
            {
                client.BaseAddress = new Uri("https://api.openai.com/v1/");

                var apiKey = configuration["ApiKey:OpenAI"] ?? throw new InvalidOperationException("ApiKey:OpenAI ayarı bulunamadı.");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            });

            services.AddHttpClient<IVoiceService, VoiceService>(client =>
            {
                client.BaseAddress = new Uri("https://api.elevenlabs.io/v1/");

                var apiKey = configuration["ApiKey:ElevenLabs"] ?? throw new InvalidOperationException("ApiKey:ElevenLabs ayarı bulunamadı.");
                client.DefaultRequestHeaders.Add("xi-api-key", apiKey);
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>()
                .AddErrorDescriber<TurkishIdentityErrorDescriber>();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(7);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    else
                        context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            services.AddSignalR();
        }
    }
}