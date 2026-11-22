using System.Net;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class SsrfIpPolicyTests
{
    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("127.9.9.9")]
    [InlineData("::1")]
    [InlineData("10.1.2.3")]
    [InlineData("172.16.0.1")]
    [InlineData("172.31.255.255")]
    [InlineData("192.168.1.1")]
    [InlineData("169.254.169.254")]
    [InlineData("fe80::1")]
    [InlineData("100.64.0.1")]
    [InlineData("100.127.255.255")]
    [InlineData("224.0.0.1")]
    [InlineData("239.255.255.250")]
    [InlineData("ff02::1")]
    [InlineData("0.0.0.0")]
    [InlineData("0.1.2.3")]
    [InlineData("::")]
    [InlineData("fc00::1")]
    [InlineData("fd12:3456::1")]
    [InlineData("::ffff:127.0.0.1")]
    [InlineData("::ffff:10.0.0.5")]
    [InlineData("::ffff:169.254.169.254")]
    [InlineData("::ffff:100.64.0.1")]
    public void Refuses(string address) => Assert.True(SsrfIpPolicy.IsDisallowed(IPAddress.Parse(address)));

    [Theory]
    [InlineData("93.184.216.34")]
    [InlineData("8.8.8.8")]
    [InlineData("172.15.255.255")]
    [InlineData("172.32.0.1")]
    [InlineData("100.63.255.255")]
    [InlineData("100.128.0.1")]
    [InlineData("2606:4700:4700::1111")]
    [InlineData("::ffff:8.8.8.8")]
    public void Allows(string address) => Assert.False(SsrfIpPolicy.IsDisallowed(IPAddress.Parse(address)));
}
