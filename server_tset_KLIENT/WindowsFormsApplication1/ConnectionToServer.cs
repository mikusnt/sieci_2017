using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server_test_KLIENT {
    class ConnectionToServer {
        public OdseparowaneIP IP { get; private set; }
        private OknoGlowne oknoGlowne;
        private ToServerBuffer toServerBuffer;
        //private FromServerBuffer fromServerBuffer;
        public String info { get; private set; }

        public ConnectionToServer(OdseparowaneIP IP) {
            this.IP = IP;
        }

        public int TestIP() {
            toServerBuffer = new ToServerBuffer(OrderCode.eConnection, null, 0, null, DataConst.defaultMaxPing, 80);

            return -1;
        }
        public void Login() { }
        public void Logout() { }

        public void AddServer() { }
        public void RemoveServer() {}
        public void RemoveAllServers() { }
        public void TurnOnReceiveFromServer(OknoGlowne oknoGlowne) {
            this.oknoGlowne = oknoGlowne;
            // osobny watek do odbierania wiadomosci i ladowania danych do tabeli z OknoGlowne
        }

        //private void Send
    }
}
