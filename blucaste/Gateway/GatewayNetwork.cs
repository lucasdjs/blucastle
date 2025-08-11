using System.Net.NetworkInformation;
using System.Linq;

namespace blucaste.Gateway
{
    public class GatewayNetwork
    {
        public string GetGatewayIp(string interfaceType)
        {
            interfaceType = interfaceType.ToLower();

            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n =>
                    n.OperationalStatus == OperationalStatus.Up &&
                    n.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ((interfaceType == "ethernet" && n.NetworkInterfaceType == NetworkInterfaceType.Ethernet) ||
                     (interfaceType == "wi-fi" && n.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)));

            foreach (var ni in networkInterfaces)
            {
                var gateway = ni.GetIPProperties()?.GatewayAddresses
                    .FirstOrDefault(g => g?.Address?.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (gateway != null)
                {
                    return gateway.Address.ToString();
                }
            }

            return null;
        }
    }
}