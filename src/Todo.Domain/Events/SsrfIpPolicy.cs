using System.Net;
using System.Net.Sockets;

namespace Todo.Domain.Events;

/// <summary>ADR 3.6: which addresses an imported calendar URL must never resolve to.</summary>
public static class SsrfIpPolicy
{
    public static bool IsDisallowed(IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);
        var ip = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;
        if (IPAddress.IsLoopback(ip) || IPAddress.Any.Equals(ip) || IPAddress.IPv6Any.Equals(ip))
        {
            return true;
        }

        var b = ip.GetAddressBytes();
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            return b[0] is 0 or 10 or 127
                || (b[0] == 100 && b[1] is >= 64 and <= 127)
                || (b[0] == 169 && b[1] == 254)
                || (b[0] == 172 && b[1] is >= 16 and <= 31)
                || (b[0] == 192 && b[1] == 168)
                || b[0] >= 224;
        }

        return ip.AddressFamily != AddressFamily.InterNetworkV6
            || ip.IsIPv6LinkLocal || ip.IsIPv6SiteLocal || ip.IsIPv6Multicast || (b[0] & 0xFE) == 0xFC;
    }
}
