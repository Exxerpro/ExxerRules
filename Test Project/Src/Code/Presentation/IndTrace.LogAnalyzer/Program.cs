using IndTrace.LogAnalyzer.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace IndTrace.LogAnalyzer
{
    /// <summary>
    /// Represents the entry point for the IndTrace Log Analyzer application.
    /// Configures and starts the web application with Blazor Server components.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main entry point for the application. Configures services and starts the web application.
        /// </summary>
        /// <param name="args">Command line arguments passed to the application.</param>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}

//TODO [BEST PRACTICE][CURSOR][20/JUNE/2025] - Validate program startup and log analyzer logic and handle edge cases defensively. Ensure all required properties are set and handle missing/invalid values gracefully.
