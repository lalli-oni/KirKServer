using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KirkServer
{
    public class AddressService
    {
        public IPAddress getIP(string hostName)
        {
            try
            {
                IPAddress[] addrs = Dns.GetHostAddresses(hostName);
                return addrs[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<IPAddress> getIPAsync(string hostName)
        {
            try
            {
                IPAddress[] addrs = await Dns.GetHostAddressesAsync(hostName);
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
