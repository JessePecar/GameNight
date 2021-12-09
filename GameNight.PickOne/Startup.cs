using GameNight.API.Utilities;
using GameNight.API.Utilities.Interfaces;
using GameNight.Lobby.Hubs;
using Microsoft.AspNetCore.Builder;

namespace GameNight.API
{
    public class Startup
    {
        public Startup(WebApplicationBuilder? builder)
        {
            AddServices(builder.Services);

            ConfigureApp(builder.Build());
        }

        private void AddServices(IServiceCollection services)
        {
            services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


            services.AddSingleton<ILobbyKeyGenerator, LobbyKeyGenerator>();

            services.AddMemoryCache();

            services.AddSignalR();
        }

        private void ConfigureApp(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<LobbyHub>("/lobby");
            });

            app.Run();
        }
    }
}
