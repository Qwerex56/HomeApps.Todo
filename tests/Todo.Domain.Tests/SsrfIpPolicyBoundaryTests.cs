using System.Net;
using Todo.Domain.Events;

namespace Todo.Domain.Tests;

/// <summary>ADR 3.6 range edges: the first and last address of each forbidden block, and their neighbours.</summary>
public sealed class SsrfIpPolicyBoundaryTests
{
    [Theory]
    [InlineData("0.255.255.255")]
    [InlineData("10.255.255.255")]
    [InlineData("127.255.255.255")]
    [InlineData("169.254.0.0")]
    [InlineData("169.254.255.255")]
    [InlineData("172.16.0.0")]
    [InlineData("192.168.255.255")]
    [InlineData("100.64.0.0")]
    [InlineData("224.0.0.0")]
    [InlineData("255.255.255.255")]
    [InlineData("fe80::")]
    [InlineData("febf::ffff")]
    [InlineData("fc00::")]
    [InlineData("fdff::1")]
    [InlineData("ff00::")]
    [InlineData("::ffff:192.168.1.1")]
    [InlineData("::ffff:0.0.0.0")]
    [InlineData("::ffff:224.0.0.1")]
    public void Refuses_the_edges_of_forbidden_blocks(string address) =>
        Assert.True(SsrfIpPolicy.IsDisallowed(IPAddress.Parse(address)));

    [Theory]
    [InlineData("1.0.0.0")]
    [InlineData("9.255.255.255")]
    [InlineData("11.0.0.0")]
    [InlineData("126.255.255.255")]
    [InlineData("128.0.0.0")]
    [InlineData("169.253.255.255")]
    [InlineData("169.255.0.1")]
    [InlineData("172.15.0.0")]
    [InlineData("172.32.0.0")]
    [InlineData("192.167.255.255")]
    [InlineData("192.169.0.1")]
    [InlineData("100.63.255.255")]
    [InlineData("100.128.0.0")]
    [InlineData("223.255.255.255")]
    [InlineData("::ffff:223.255.255.255")]
    [InlineData("fbff::1")]
    [InlineData("2001:db8::1")]
    public void Allows_the_neighbours_of_forbidden_blocks(string address) =>
        Assert.False(SsrfIpPolicy.IsDisallowed(IPAddress.Parse(address)));
}
