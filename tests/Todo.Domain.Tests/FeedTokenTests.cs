using Todo.Domain.Events;

namespace Todo.Domain.Tests;

public sealed class FeedTokenTests
{
    [Fact]
    public void Generate_Is43UrlSafeCharactersAndUnique()
    {
        var a = FeedToken.Generate();

        Assert.Equal((43, true, false), (a.Length, a.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_'), a == FeedToken.Generate()));
    }

    [Fact]
    public void Hash_IsStableLowercaseSha256Hex()
    {
        Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", FeedToken.Hash("abc"));
    }
}
