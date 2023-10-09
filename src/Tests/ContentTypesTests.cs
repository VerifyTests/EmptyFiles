public class ContentTypesTests
{
    [Test]
    public void TryGetExtension()
    {
        Assert.True(ContentTypes.TryGetExtension("application/json", out var extension));
        Assert.AreEqual("json", extension);
        Assert.True(ContentTypes.TryGetExtension("foo/bar+json", out extension));
        Assert.AreEqual("json", extension);
        Assert.True(ContentTypes.TryGetExtension("foo/bin", out extension));
        Assert.AreEqual("bin", extension);
    }

    [Test]
    public void IsText()
    {
        Assert.True(ContentTypes.IsText("application/json", out var extension));
        Assert.AreEqual("json", extension);
        Assert.True(ContentTypes.IsText("foo/bar+json", out extension));
        Assert.AreEqual("json", extension);
        Assert.False(ContentTypes.IsText("foo/bin", out extension));

        Assert.True(ContentTypes.IsText("application/json"));
        Assert.True(ContentTypes.IsText("foo/bar+json"));
        Assert.False(ContentTypes.IsText("foo/bin"));
    }
}