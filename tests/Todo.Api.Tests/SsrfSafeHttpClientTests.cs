using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Todo.Infrastructure.ExternalCalendars;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: every address here is refused before anything is sent.</summary>
public sealed class SsrfSafeHttpClientTests
{
    private sealed class Env(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "t";
        public string ContentRootPath { get; set; } = "/";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Theory]
    [InlineData("http://127.0.0.1/a.ics")]
    [InlineData("http://localhost:5400/a.ics")]
    [InlineData("http://[::ffff:127.0.0.1]/a.ics")]
    [InlineData("http://169.254.169.254/latest/meta-data")]
    [InlineData("http://100.64.0.1/a.ics")]
    [InlineData("http://10.0.0.1/a.ics")]
    [InlineData("http://224.0.0.1/a.ics")]
    public async Task Download_RefusesInternalAddresses(string url)
    {
        using var client = new SsrfSafeHttpClient(new Env("Development"));

        var result = await client.DownloadAsync(url, CancellationToken.None);

        Assert.True(result.IsFailed);
        Assert.Contains("ssrf_rejected", result.Errors[0].Metadata["Code"].ToString());
    }

    [Fact]
    public async Task Download_RefusesPlainHttpInProduction()
    {
        using var client = new SsrfSafeHttpClient(new Env("Production"));

        var result = await client.DownloadAsync("http://example.com/a.ics", CancellationToken.None);

        Assert.Contains("ssrf_rejected", result.Errors[0].Metadata["Code"].ToString());
    }
}
