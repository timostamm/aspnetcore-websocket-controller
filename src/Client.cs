using System;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TimoStamm.WebSockets.Controller
{
    public class Client
    {

        private readonly WebSocket _webSocket;

        private readonly HttpContext _context;


        public Client(WebSocket webSocket, HttpContext context)
        {
            _webSocket = webSocket;
            _context = context;
            
        }

        
        public ClaimsPrincipal User => _context.User;

        public WebSocket WebSocket => _webSocket;
        
        public HttpContext Context => _context;

        public string Id => _context.TraceIdentifier;

        
        public Task CloseAsync(
            WebSocketCloseStatus closeStatus = WebSocketCloseStatus.NormalClosure,
            string statusDescription = "Closing",
            CancellationToken cancellationToken = default)
        {
            return _webSocket.CloseAsync(closeStatus, statusDescription, cancellationToken);
        }
        

        public Task SendAsync(byte[] bytes, CancellationToken cancellationToken = default)
        {
            return _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
        }

        
        public Task SendAsync(string text, CancellationToken cancellationToken = default)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellationToken);
        }


        public override string ToString()
        {
            return $"{base.ToString()}[{Id}]";
        }
        
    }
}