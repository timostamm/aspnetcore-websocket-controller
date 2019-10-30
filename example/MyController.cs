using System.Threading.Tasks;
using TimoStamm.WebSockets.Controller;

namespace example
{
    public class MyController : AWebsocketController
    {
        
        public override async Task OnOpen(Client client)
        {
            await client.SendAsync("Hello there");
        }

    }
}