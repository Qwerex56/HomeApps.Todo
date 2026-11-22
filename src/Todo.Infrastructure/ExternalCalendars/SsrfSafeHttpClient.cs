using System.Net;
using System.Net.Sockets;
using FluentResults;
using Microsoft.Extensions.Hosting;
using Todo.Application.Abstractions;
using Todo.Domain.Common;
using Todo.Domain.Events;

namespace Todo.Infrastructure.ExternalCalendars;

/// <summary>
/// ADR 3.6: downloads an external calendar. Every connection is made by <c>ConnectCallback</c> to an address that was
/// resolved and checked there (so a DNS rebind between check and connect is impossible, and TLS still verifies the
/// original host name). Redirects are followed by hand, at most <see cref="MaxRedirects"/>, each one checked again.
/// Ten seconds in all; at most 2 MB.
/// </summary>
internal sealed class SsrfSafeHttpClient : ISsrfSafeClient, IDisposable
{
    public const int MaxBytes = 2 * 1024 * 1024;
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    private const int MaxRedirects = 3;

    private readonly HttpClient _http;
    private readonly bool _httpsOnly;

    public SsrfSafeHttpClient(IHostEnvironment environment)
        : this(environment.IsProduction(), CreatePinnedHandler(), Timeout)
    {
    }

    /// <summary>For tests: a stub handler instead of the pinned sockets, and a shorter timeout.</summary>
    internal SsrfSafeHttpClient(bool httpsOnly, HttpMessageHandler handler, TimeSpan timeout)
    {
        _httpsOnly = httpsOnly;
        _http = new HttpClient(handler, disposeHandler: true) { Timeout = timeout, MaxResponseContentBufferSize = MaxBytes };
    }

    private static SocketsHttpHandler CreatePinnedHandler() => new()
    {
        AllowAutoRedirect = false,
        ConnectTimeout = Timeout,
        PooledConnectionLifetime = TimeSpan.FromMinutes(1),
        UseProxy = false,
        ConnectCallback = async (context, ct) =>
        {
            var target = await ResolveAndValidateAsync(context.DnsEndPoint.Host, ct);
            var socket = new Socket(target.AddressFamily, SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
            try
            {
                await socket.ConnectAsync(new IPEndPoint(target, context.DnsEndPoint.Port), ct);
                return new NetworkStream(socket, ownsSocket: true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        },
    };

    public async Task<Result<string>> DownloadAsync(string url, CancellationToken cancellationToken)
    {
        try
        {
            var uri = new Uri(url, UriKind.Absolute);
            for (var hop = 0; hop <= MaxRedirects; hop++)
            {
                if (!IsAllowedScheme(uri))
                {
                    return Result.Fail(PlannerErrors.SsrfRejected);
                }

                using var request = new HttpRequestMessage(HttpMethod.Get, uri);
                request.Headers.Accept.ParseAdd("text/calendar, text/plain;q=0.5, */*;q=0.1");
                using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if ((int)response.StatusCode is >= 300 and < 400)
                {
                    if (response.Headers.Location is not { } location)
                    {
                        return Result.Fail(PlannerErrors.SyncFailed);
                    }

                    // The next hop is checked like the first: scheme here, address in ConnectCallback.
                    uri = location.IsAbsoluteUri ? location : new Uri(uri, location);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Fail(PlannerErrors.SyncFailed);
                }

                return await ReadLimitedAsync(response, cancellationToken);
            }

            return Result.Fail(PlannerErrors.SyncFailed);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            return Result.Fail(Unwrap(exception) is SsrfSecurityException ? PlannerErrors.SsrfRejected : PlannerErrors.SyncFailed);
        }
    }

    public void Dispose() => _http.Dispose();

    private static Exception Unwrap(Exception exception) => exception.GetBaseException();

    private bool IsAllowedScheme(Uri uri) =>
        uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeHttp && !_httpsOnly);

    private static async Task<Result<string>> ReadLimitedAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.Content.Headers.ContentLength > MaxBytes)
        {
            return Result.Fail(PlannerErrors.SyncFailed);
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var buffer = new MemoryStream();
        var chunk = new byte[16 * 1024];
        int read;
        while ((read = await stream.ReadAsync(chunk, cancellationToken)) > 0)
        {
            if (buffer.Length + read > MaxBytes)
            {
                return Result.Fail(PlannerErrors.SyncFailed);
            }

            buffer.Write(chunk, 0, read);
        }

        return System.Text.Encoding.UTF8.GetString(buffer.GetBuffer(), 0, (int)buffer.Length);
    }

    /// <summary>Resolves the host, refuses it when any of its addresses is forbidden, and returns the one to connect to.</summary>
    private static async Task<IPAddress> ResolveAndValidateAsync(string host, CancellationToken cancellationToken)
    {
        var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
        if (addresses.Length == 0)
        {
            throw new SsrfSecurityException("DNS resolution returned no addresses.");
        }

        foreach (var address in addresses)
        {
            if (SsrfIpPolicy.IsDisallowed(address))
            {
                throw new SsrfSecurityException("Forbidden IP destination.");
            }
        }

        return addresses[0].IsIPv4MappedToIPv6 ? addresses[0].MapToIPv4() : addresses[0];
    }
}
