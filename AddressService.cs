using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KirkServer
{
    class AddressService
    {
        public async Task<IPAddress> getIP(string hostName)
        {

            string name = Dns.GetHostName();
            try
            {
                IPAddress[] addrs = Dns.Resolve(name).AddressList;
                foreach (IPAddress addr in addrs)
                    Console.WriteLine("{0}/{1}", name, addr);
                return addrs[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
