aspnetcore-websocket-controller
===============================

Easy WebSockets for AspNetCore 3.0.


Run `dotnet add package TimoStamm.WebSockets.Controller` to install.

Implement your websocket controller:
````c#
public class MyController : AWebsocketController
{
    public override async Task OnOpen(Client client)
    {
        await client.SendAsync("Hello there");
    }
    // OnTextMessage(Client client, string text)
    // OnBinaryMessage(...)
    // OnClose(...)
}
````

And add it in your `Startup.cs`
````c#
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseWebSockets();
    var myHandler = WebSocketHandler.CreateFor<MyController>(app.ApplicationServices);
    app.Run(myHandler.HandleRequest);
}
````

See example for a basic websocket chat. To run, `cd example` and `dotnet run`, then open http://localhost:5000/
