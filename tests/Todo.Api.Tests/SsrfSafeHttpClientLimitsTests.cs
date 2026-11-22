using System.Net;
using Todo.Infrastructure.ExternalCalendars;

namespace Todo.Api.Tests;

/// <summary>No Docker needed: a stub handler stands in for the pinned sockets, so redirects, the size cap and the timeout are checked in isolation.</summary>
public sealed class SsrfSafeHttpClientLimitsTests
{
    private sealed class Stub(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> answer) : HttpMessageHandler
    {
        public List<Uri> Seen { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Seen.Add(request.RequestUri!);
            return answer(request, cancellationToken);
        }
    }

    private static HttpResponseMessage Redirect(string to) =>
        new(HttpStatusCode.Redirect) { Headers = { Location = new Uri(to, UriKind.RelativeOrAbsolute) } };

    private static string CodeOf(FluentResults.ResultBase r) => r.Errors[0].Metadata["Code"].ToString()!;

    private static SsrfSafeHttpClient Client(Stub stub, bool httpsOnly = false, TimeSpan? timeout = null) =>
        new(httpsOnly, stub, timeout ?? TimeSpan.FromSeconds(5));

    [Fact]
    public async Task A_redirect_is_followed_and_its_body_returned()
    {
        var stub = new Stub((r, _) => Task.FromResult(r.RequestUri!.AbsolutePath == "/a" ? Redirect("/b") : new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("BEGIN:VCALENDAR") }));
        using var client = Client(stub);

        var result = await client.DownloadAsync("https://example.com/a", CancellationToken.None);

        Assert.Equal("BEGIN:VCALENDAR", result.Value);
        Assert.Equal(["/a", "/b"], stub.Seen.Select(u => u.AbsolutePath));
    }

    [Theory]
    [InlineData("ftp://example.com/x")]
    [InlineData("file:///etc/passwd")]
    public async Task A_redirect_to_another_scheme_is_refused(string target)
    {
        var stub = new Stub((_, _) => Task.FromResult(Redirect(target)));
        using var client = Client(stub);

        var result = await client.DownloadAsync("https://example.com/a", CancellationToken.None);

        Assert.Equal("planner.ssrf_rejected", CodeOf(result));
        Assert.Single(stub.Seen);
    }

    [Fact]
    public async Task A_redirect_from_https_to_http_is_refused_in_production()
    {
        var stub = new Stub((_, _) => Task.FromResult(Redirect("http://example.com/x")));
        using var client = Client(stub, httpsOnly: true);

        var result = await client.DownloadAsync("https://example.com/a", CancellationToken.None);

        Assert.Equal("planner.ssrf_rejected", CodeOf(result));
    }

    [Fact]
    public async Task Redirects_stop_after_three_hops()
    {
        var stub = new Stub((_, _) => Task.FromResult(Redirect("https://example.com/again")));
        using var client = Client(stub);

        var result = await client.DownloadAsync("https://example.com/a", CancellationToken.None);

        Assert.Equal("planner.sync_failed", CodeOf(result));
        Assert.Equal(4, stub.Seen.Count);
    }

    [Fact]
    public async Task A_redirect_without_location_fails()
    {
        using var client = Client(new Stub((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.Redirect))));

        Assert.Equal("planner.sync_failed", CodeOf(await client.DownloadAsync("https://example.com/a", CancellationToken.None)));
    }

    [Fact]
    public async Task A_body_over_two_megabytes_is_refused_even_without_content_length()
    {
        var big = new byte[SsrfSafeHttpClient.MaxBytes + 1];
        using var client = Client(new Stub((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StreamContent(new MemoryStream(big)) })));

        Assert.Equal("planner.sync_failed", CodeOf(await client.DownloadAsync("https://example.com/a", CancellationToken.None)));
    }

    [Fact]
    public async Task A_body_of_exactly_two_megabytes_is_accepted()
    {
        var body = new byte[SsrfSafeHttpClient.MaxBytes];
        Array.Fill(body, (byte)'a');
        using var client = Client(new Stub((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(body) })));

        Assert.Equal(SsrfSafeHttpClient.MaxBytes, (await client.DownloadAsync("https://example.com/a", CancellationToken.None)).Value.Length);
    }

    [Fact]
    public async Task A_declared_length_over_the_cap_is_refused_before_reading()
    {
        var content = new ByteArrayContent([1]);
        content.Headers.ContentLength = SsrfSafeHttpClient.MaxBytes + 1;
        using var client = Client(new Stub((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = content })));

        Assert.Equal("planner.sync_failed", CodeOf(await client.DownloadAsync("https://example.com/a", CancellationToken.None)));
    }

    [Fact]
    public async Task A_slow_server_hits_the_timeout()
    {
        var stub = new Stub(async (_, ct) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(30), ct);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        using var client = Client(stub, timeout: TimeSpan.FromMilliseconds(100));

        Assert.Equal("planner.sync_failed", CodeOf(await client.DownloadAsync("https://example.com/a", CancellationToken.None)));
    }

    [Fact]
    public void The_production_timeout_is_ten_seconds_and_the_cap_two_megabytes()
    {
        Assert.Equal((TimeSpan.FromSeconds(10), 2 * 1024 * 1024), (SsrfSafeHttpClient.Timeout, SsrfSafeHttpClient.MaxBytes));
    }
}
