using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TimoStamm.WebSockets.Controller
{
    public class WebSocketHandler
    {
        public static WebSocketHandler CreateFor<T>(IServiceProvider serviceProvider)
            where T : IWebsocketController
        {
            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger(nameof(WebSocketHandler) + " for " + typeof(T).FullName);
            var clients = new ClientDictionary();
            var controller = ActivatorUtilities.CreateInstance<T>(serviceProvider, clients.ClientCollection);
            return ActivatorUtilities.CreateInstance<WebSocketHandler>(serviceProvider, controller, clients, logger);
        }


        private readonly IWebsocketController _controller;
        private readonly ClientDictionary _clients;
        private readonly ILogger _logger;

        
        public WebSocketHandler(IWebsocketController controller, ClientDictionary clients, ILogger logger)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _logger = logger;
        }


        public async Task HandleRequest(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await HandleNotWebSocket(context);
                return;
            }

            var cancel = context.RequestAborted;
            var client = await _controller.OnWebSocketRequest(context);
            var socket = client.WebSocket;

            _clients.TryAdd(context.TraceIdentifier, client);
            
            _logger.LogDebug($"Client id \"{client.Id}\" connected ({_clients.Count} total).");

            try
            {
                await _controller.OnOpen(client);

                while (socket.State == WebSocketState.Open)
                {
                    switch (await ReceiveMessage(client, cancel))
                    {
                        case string text:
                            await _controller.OnTextMessage(client, text);
                            break;
                        case byte[] bytes:
                            await _controller.OnBinaryMessage(client, bytes);
                            break;
                    }
                }

                _clients.TryRemove(context.TraceIdentifier, out var unused);

                _logger.LogDebug($"Client id \"{client.Id}\" closed ({_clients.Count} total).");

                await _controller.OnClose(client, socket.CloseStatus ?? WebSocketCloseStatus.Empty,
                    socket.CloseStatusDescription);

                if (socket.CloseStatus == WebSocketCloseStatus.EndpointUnavailable)
                {
                    // When the endpoint is unavailable, we abort. 
                    socket.Abort();
                }
                else if (socket.State == WebSocketState.CloseReceived)
                {
                    // If the client closed the connection, we finish the close handshake.
                    await socket.CloseOutputAsync(socket.CloseStatus ?? WebSocketCloseStatus.ProtocolError,
                        socket.CloseStatusDescription, cancel);
                }

            }
            catch (OperationCanceledException ex)
            {
                _logger.LogDebug($"Client id \"{client.Id}\" request aborted: {ex.Message}");
            }

            socket.Dispose();
        }


        private async Task HandleNotWebSocket(HttpContext context)
        {
            _logger.LogDebug($"Rejecting request id \"{context.TraceIdentifier}\" - not a websocket request.");
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("need websocket request");
        }


        private async Task<object> ReceiveMessage(Client client, CancellationToken cancel = default)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            await using var stream = new MemoryStream();
            WebSocketReceiveResult result;
            do
            {
                result = await client.WebSocket.ReceiveAsync(buffer, cancel);
                await stream.WriteAsync(buffer.Array, buffer.Offset, result.Count, cancel);
            } while (!result.EndOfMessage);

            stream.Seek(0, SeekOrigin.Begin);
            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                {
                    _logger.LogDebug($"Client id \"{client.Id}\" received text message.");
                    // Encoding UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    return await reader.ReadToEndAsync();
                }
                case WebSocketMessageType.Binary:

                    _logger.LogDebug($"Client id \"{client.Id}\" received binary message.");
                    return stream.ToArray();

                case WebSocketMessageType.Close:
                {
                    _logger.LogDebug($"Client id \"{client.Id}\" received close message.");
                    return null;
                }
                default:
                    throw new Exception();
            }
        }
    }
}