public class ContentTypesTests
{
    [Test]
    public async Task TryGetExtension()
    {
        await Assert.That(ContentTypes.TryGetExtension("application/json", out var extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("json");
        await Assert.That(ContentTypes.TryGetExtension("foo/bar+json", out extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("json");
        await Assert.That(ContentTypes.TryGetExtension("text/html; charset=utf-8", out extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("html");
        await Assert.That(ContentTypes.TryGetExtension("foo/bin", out extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("bin");
    }

    [Test]
    public async Task TryGetMediaType()
    {
        await Assert.That(ContentTypes.TryGetMediaType("json", out var media)).IsTrue();
        await Assert.That(media).IsEqualTo("application/json");
        await Assert.That(ContentTypes.TryGetMediaType("html", out media)).IsTrue();
        await Assert.That(media).IsEqualTo("text/html");
        await Assert.That(ContentTypes.TryGetMediaType("bin", out media)).IsTrue();
        await Assert.That(media).IsEqualTo("application/octet-stream");
    }

    [Test]
    public async Task Heic()
    {
        // The extension must not carry a leading dot, unlike every other mapping.
        await Assert.That(ContentTypes.TryGetExtension("image/heic", out var extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("heic");
        await Assert.That(ContentTypes.TryGetMediaType("heic", out var media)).IsTrue();
        await Assert.That(media).IsEqualTo("image/heic");
        await Assert.That(ContentTypes.TryGetMediaType(".heic", out media)).IsTrue();
        await Assert.That(media).IsEqualTo("image/heic");
    }

    [Test]
    public async Task IsText()
    {
        await Assert.That(ContentTypes.IsText("application/json", out var extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("json");
        await Assert.That(ContentTypes.IsText("text/html; charset=utf-8", out extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("html");
        await Assert.That(ContentTypes.IsText("foo/bar+json", out extension)).IsTrue();
        await Assert.That(extension).IsEqualTo("json");
        await Assert.That(ContentTypes.IsText("foo/bin", out extension)).IsFalse();

        await Assert.That(ContentTypes.IsText("application/json")).IsTrue();
        await Assert.That(ContentTypes.IsText("foo/bar+json")).IsTrue();
        await Assert.That(ContentTypes.IsText("foo/bin")).IsFalse();
    }
}