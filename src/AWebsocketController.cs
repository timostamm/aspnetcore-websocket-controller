using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TimoStamm.WebSockets.Controller
{
    public abstract class AWebsocketController : IWebsocketController
    {
        public virtual async Task<Client> OnWebSocketRequest(HttpContext context)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            return new Client(socket, context);
        }

        public virtual Task OnOpen(Client client)
        {
            return Task.CompletedTask;
        }

        public virtual async Task OnTextMessage(Client client, string text)
        {
            await client.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot process text message.");
        }

        public virtual async Task OnBinaryMessage(Client client, byte[] bytes)
        {
            await client.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "Cannot process binary message.");
        }

        public virtual Task OnClose(Client client, WebSocketCloseStatus closeStatus, string closeStatusDescription)
        {
            return Task.CompletedTask;
        }
    }
}