using System.Collections;
using System.Collections.Generic;

namespace TimoStamm.WebSockets.Controller
{
    public class ClientCollection : IReadOnlyCollection<Client>
    {
        private readonly ClientDictionary _clientDictionary;

        public ClientCollection(ClientDictionary clientDictionary)
        {
            _clientDictionary = clientDictionary;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Client> GetEnumerator()
        {
            foreach (var pair in _clientDictionary)
            {
                yield return pair.Value;
            }
        }

        public int Count => _clientDictionary.Count;
    }
}