
namespace StatusMicroService
{
    // ==============================================================================
    // 1. APPLICATION SETUP
    // This section configures the web server. It tells ASP.NET to look for 
    // "Controllers" (classes that handle web traffic) and route traffic to them.
    // To see how the controller responds to a simple list query, enter the address
    // you see in your terminal window into a browser, and add /api/Status to the url
    // It will look like: http://localhost:5104/api/Status 
    // ==============================================================================
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
