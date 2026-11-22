using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Todo.Infrastructure.ExternalCalendars;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: refused schemes never open a connection; the unresolvable name fails at DNS.</summary>
public sealed class SsrfSchemeAndDnsTests
{
    private sealed class Env(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "t";
        public string ContentRootPath { get; set; } = "/";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Theory]
    [InlineData("file:///etc/hosts")]
    [InlineData("ftp://example.com/a.ics")]
    [InlineData("gopher://example.com/a.ics")]
    public async Task Download_RefusesSchemesOtherThanHttp(string url)
    {
        using var client = new SsrfSafeHttpClient(new Env("Development"));

        var result = await client.DownloadAsync(url, CancellationToken.None);

        Assert.Equal("planner.ssrf_rejected", result.Errors[0].Metadata["Code"].ToString());
    }

    [Fact]
    public async Task Download_ReportsAnUnresolvableHostAsASyncFailureNotAnSsrfRefusal()
    {
        using var client = new SsrfSafeHttpClient(new Env("Development"));

        var result = await client.DownloadAsync("https://no-such-host.invalid/a.ics", CancellationToken.None);

        Assert.Equal("planner.sync_failed", result.Errors[0].Metadata["Code"].ToString());
    }

    [Fact]
    public async Task Download_RefusesFtpInProductionToo()
    {
        using var client = new SsrfSafeHttpClient(new Env("Production"));

        var result = await client.DownloadAsync("ftp://example.com/a.ics", CancellationToken.None);

        Assert.Equal("planner.ssrf_rejected", result.Errors[0].Metadata["Code"].ToString());
    }
}
