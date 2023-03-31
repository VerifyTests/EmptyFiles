using EmptyFiles;

public class ContentTypesTests :
    XunitContextBase
{
    [Fact]
    public void TryGetExtension()
    {
        Assert.True(ContentTypes.TryGetExtension("application/json", out var extension));
        Assert.Equal("json", extension);
        Assert.True(ContentTypes.TryGetExtension("foo/bar+json", out extension));
        Assert.Equal("json", extension);
        Assert.True(ContentTypes.TryGetExtension("foo/bin", out extension));
        Assert.Equal("bin", extension);
    }

    [Fact]
    public void IsText()
    {
        Assert.True(ContentTypes.IsText("application/json", out var extension));
        Assert.Equal("json", extension);
        Assert.True(ContentTypes.IsText("foo/bar+json", out extension));
        Assert.Equal("json", extension);
        Assert.False(ContentTypes.IsText("foo/bin", out extension));
    }

    public ContentTypesTests(ITestOutputHelper output) :
        base(output)
    {
    }
}