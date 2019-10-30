using System.Net.WebSockets;
using System.Threading.Tasks;

namespace TimoStamm.WebSockets.Controller
{
    public abstract class AWebsocketController : IWebsocketController
    {
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