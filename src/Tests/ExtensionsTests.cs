[NotInParallel]
public class ExtensionsTests
{
    [Test]
    public async Task IsText()
    {
        #region IsText

        await Assert.That(FileExtensions.IsTextFile("file.txt")).IsTrue();
        await Assert.That(FileExtensions.IsTextFile("file.bin")).IsFalse();
        await Assert.That(FileExtensions.IsTextExtension(".txt")).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension(".bin")).IsFalse();
        await Assert.That(FileExtensions.IsTextExtension("txt")).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension("bin")).IsFalse();

        #endregion

        await Assert.That(FileExtensions.IsTextFile(".StartingWithDot")).IsFalse();
        await Assert.That(FileExtensions.IsTextFile("NoExtension")).IsFalse();

        #region TextViaConvention

        await Assert.That(FileExtensions.IsTextFile("c:/path/file.txtViaConvention")).IsTrue();

        #endregion
    }

    #region AddTextFileConvention
    [ModuleInitializer]
    public static void AddTextFileConvention() =>
        // Treat files ending with .txtViaConvention as text files
        FileExtensions.AddTextFileConvention(path => path.EndsWith(".txtViaConvention"));

    #endregion

    [Test]
    public async Task IsTextExtension_CaseInsensitive()
    {
        await Assert.That(FileExtensions.IsTextExtension(".TXT")).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension("TXT")).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension(".Txt")).IsTrue();
        await Assert.That(FileExtensions.IsTextFile("FILE.TXT")).IsTrue();
        await Assert.That(FileExtensions.IsTextFile("README.MD")).IsTrue();
    }

    [Test]
    public async Task IsTextExtension_Span()
    {
        await Assert.That(FileExtensions.IsTextExtension(".txt".AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension("txt".AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension("TXT".AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension("bin".AsSpan())).IsFalse();
    }

    [Test]
    public async Task AddRemoveTextExtension_CaseInsensitive()
    {
        FileExtensions.AddTextExtension(".CaseExt");
        try
        {
            await Assert.That(FileExtensions.IsTextExtension(".caseext")).IsTrue();
            await Assert.That(FileExtensions.IsTextExtension("CASEEXT")).IsTrue();
            await Assert.That(FileExtensions.IsTextExtension("caseext".AsSpan())).IsTrue();
        }
        finally
        {
            FileExtensions.RemoveTextExtension(".CASEEXT");
        }

        await Assert.That(FileExtensions.IsTextExtension(".caseext")).IsFalse();
        await Assert.That(FileExtensions.IsTextExtension("caseext")).IsFalse();
    }

    [Test]
    public void IsTextExtension_Empty_Throws() =>
        Assert.ThrowsExactly<ArgumentNullException>(() => FileExtensions.IsTextExtension(""));

    [Test]
    public async Task IsTextLegacy()
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await Assert.That(FileExtensions.IsText("file.txt")).IsTrue();
        await Assert.That(FileExtensions.IsText("file.bin")).IsFalse();
        await Assert.That(FileExtensions.IsText("c:/file.txt")).IsTrue();
        await Assert.That(FileExtensions.IsText("c:/file.bin")).IsFalse();
        await Assert.That(FileExtensions.IsText(".txt")).IsTrue();
        await Assert.That(FileExtensions.IsText("txt")).IsTrue();
        await Assert.That(FileExtensions.IsText(".bin")).IsFalse();
        await Assert.That(FileExtensions.IsText("bin")).IsFalse();
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Test]
    public async Task AddTextExtension()
    {
        #region AddTextExtension

        FileExtensions.AddTextExtension(".ext1");
        await Assert.That(FileExtensions.IsTextExtension(".ext1")).IsTrue();
        await Assert.That(FileExtensions.IsTextFile("file.ext1")).IsTrue();

        #endregion

        FileExtensions.AddTextExtension("ext2");
        await Assert.That(FileExtensions.IsTextExtension("ext2")).IsTrue();
        await Assert.That(FileExtensions.IsTextFile("file.ext2")).IsTrue();
    }

    [Test]
    public async Task RemoveTextExtension()
    {
        #region RemoveTextExtension

        FileExtensions.AddTextExtension(".ext1");
        await Assert.That(FileExtensions.IsTextExtension(".ext1")).IsTrue();
        FileExtensions.RemoveTextExtension(".ext1");
        await Assert.That(FileExtensions.IsTextExtension(".ext1")).IsFalse();

        #endregion

        FileExtensions.AddTextExtension("ext1");
        await Assert.That(FileExtensions.IsTextExtension("ext1")).IsTrue();
        FileExtensions.RemoveTextExtension("ext1");
        await Assert.That(FileExtensions.IsTextExtension("ext1")).IsFalse();
    }

    [Test]
    [Arguments("bicepparam")]
    [Arguments("BICEPPARAM")]
    [Arguments("BicepParam")]
    public async Task BicepParametersAreText(string extension)
    {
        await Assert.That(FileExtensions.IsTextExtension(extension)).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension(extension.AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension($".{extension}")).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension($".{extension}".AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextFile($"main.dev.{extension}")).IsTrue();
        await Assert.That(FileExtensions.IsTextFile($"main.dev.{extension}".AsSpan())).IsTrue();
    }

    [Test]
    [Arguments("geojson")]
    [Arguments("gpx")]
    [Arguments("kml")]
    public async Task MapFormatsAreText(string extension)
    {
        await Assert.That(FileExtensions.IsTextExtension(extension)).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension(extension.AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension($".{extension}")).IsTrue();
        await Assert.That(FileExtensions.IsTextExtension($".{extension}".AsSpan())).IsTrue();
        await Assert.That(FileExtensions.IsTextFile($"route.{extension}")).IsTrue();
        await Assert.That(AllFiles.TryCreateFile($"route.{extension}", useEmptyStringForTextFiles: true)).IsTrue();
        File.Delete($"route.{extension}");
    }
}