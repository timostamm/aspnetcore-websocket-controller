using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using TimoStamm.WebSockets.Controller;

namespace example
{
    public class ChatController : AWebsocketController
    {
        private readonly ClientCollection _clients;
        private static int _clientCounter;

        public ChatController(ClientCollection clients)
        {
            _clients = clients;
        }


        public override async Task OnOpen(Client client)
        {
            var nick = client.Context.Items["nick"] = ++_clientCounter;
            var msg = $"{nick} has entered the chat.";
            if (_clients.Count == 1)
            {
                msg += " You are alone. Open another browser window...";
            }
            await Task.WhenAll(_clients.Select(c => c.SendAsync(msg)));
        }


        public override async Task OnClose(Client client, WebSocketCloseStatus closeStatus,
            string closeStatusDescription)
        {
            var nick = client.Context.Items["nick"];
            var msg = $"{nick} has left the chat.";
            await Task.WhenAll(_clients.Select(c => c.SendAsync(msg)));
        }


        public override async Task OnTextMessage(Client client, string text)
        {
            var nick = client.Context.Items["nick"];
            var msg = $"{nick}: {text}";
            await Task.WhenAll(_clients.Select(c => c.SendAsync(msg)));
        }
        
    }
}