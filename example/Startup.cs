using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TimoStamm.WebSockets.Controller;

namespace example
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseWebSockets();

            app.Map("/ws-chat", a =>
            {
                var handler = WebSocketHandler.CreateFor<ChatController>(app.ApplicationServices);
                a.Run(handler.HandleRequest);
            });
            
            app.Map("/ws-my", a =>
            {
                var handler = WebSocketHandler.CreateFor<MyController>(app.ApplicationServices);
                a.Run(handler.HandleRequest);
            });
            
        }
    }
}