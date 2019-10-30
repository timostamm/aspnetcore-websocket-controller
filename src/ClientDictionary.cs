using System.Collections.Concurrent;

namespace TimoStamm.WebSockets.Controller
{
    
    public class ClientDictionary : ConcurrentDictionary<string, Client>
    {


        public readonly ClientCollection ClientCollection;

        
        public ClientDictionary()
        {
            ClientCollection = new ClientCollection(this);
        }

        
    }
}