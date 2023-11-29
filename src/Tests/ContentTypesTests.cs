public class ContentTypesTests
{
    [Test]
    public void TryGetExtension()
    {
        True(ContentTypes.TryGetExtension("application/json", out var extension));
        AreEqual("json", extension);
        True(ContentTypes.TryGetExtension("foo/bar+json", out extension));
        AreEqual("json", extension);
        True(ContentTypes.TryGetExtension("foo/bin", out extension));
        AreEqual("bin", extension);
    }

    [Test]
    public void IsText()
    {
        True(ContentTypes.IsText("application/json", out var extension));
        AreEqual("json", extension);
        True(ContentTypes.IsText("foo/bar+json", out extension));
        AreEqual("json", extension);
        False(ContentTypes.IsText("foo/bin", out extension));

        True(ContentTypes.IsText("application/json"));
        True(ContentTypes.IsText("foo/bar+json"));
        False(ContentTypes.IsText("foo/bin"));
    }
}