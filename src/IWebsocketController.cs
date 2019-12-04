using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TimoStamm.WebSockets.Controller
{
    public interface IWebsocketController
    {
        Task<Client> OnWebSocketRequest(HttpContext context);

        Task OnOpen(Client client);

        Task OnTextMessage(Client client, string text);

        Task OnBinaryMessage(Client client, byte[] bytes);

        Task OnClose(Client client, WebSocketCloseStatus closeStatus, string closeStatusDescription);
    }
}