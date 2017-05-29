using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace publishCommon.Domain
{
    public class ClientClass
    {
        public ClientClass()
        {
            client_id = "58eb3188a78df92264ed8ebd";
            client_key = "58eb3190a78df92264ed8ebe";
        }
        public string client_id { get; set; }
        public string client_key { get; set; }

        public string Client_secret
        {
            get
            {

                return client_secret;
            }

            set
            {
                client_secret = value;
            }
        }

        private string client_secret;

    }
}
