public class ContentTypesTests
{
    [Test]
    public void TryGetExtension()
    {
        True(ContentTypes.TryGetExtension("application/json", out var extension));
        AreEqual("json", extension);
        True(ContentTypes.TryGetExtension("foo/bar+json", out extension));
        AreEqual("json", extension);
        True(ContentTypes.TryGetExtension("text/html; charset=utf-8", out extension));
        AreEqual("html", extension);
        True(ContentTypes.TryGetExtension("foo/bin", out extension));
        AreEqual("bin", extension);
    }

    [Test]
    public void TryGetMediaType()
    {
        True(ContentTypes.TryGetMediaType("json", out var media));
        AreEqual("application/json", media);
        True(ContentTypes.TryGetMediaType("html", out media));
        AreEqual("text/html", media);
        True(ContentTypes.TryGetMediaType("bin", out media));
        AreEqual("application/octet-stream", media);
    }

    [Test]
    public void IsText()
    {
        True(ContentTypes.IsText("application/json", out var extension));
        AreEqual("json", extension);
        True(ContentTypes.IsText("text/html; charset=utf-8", out extension));
        AreEqual("html", extension);
        True(ContentTypes.IsText("foo/bar+json", out extension));
        AreEqual("json", extension);
        False(ContentTypes.IsText("foo/bin", out extension));

        True(ContentTypes.IsText("application/json"));
        True(ContentTypes.IsText("foo/bar+json"));
        False(ContentTypes.IsText("foo/bin"));
    }
}