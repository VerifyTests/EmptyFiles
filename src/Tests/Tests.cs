namespace EmptyFilesTests;

[NotInParallel]
public class Tests
{
    // UseFile mutates global state with no unregister, so register once for the
    // whole fixture and keep the backing file on disk until every test (notably
    // WriteAllTo, which reads it) has run.
    static string useFileTarget = null!;

    [Before(Class)]
    public static void RegisterUseFile()
    {
        useFileTarget = Path.Combine(Path.GetTempPath(), $"EmptyFilesUseFile{Guid.NewGuid():N}.usefileext");
        File.WriteAllText(useFileTarget, "content");
        AllFiles.UseFile(Category.Document, useFileTarget);
    }

    [After(Class)]
    public static void CleanupUseFile() =>
        File.Delete(useFileTarget);

    [Test]
    public async Task UseFile_UpdatesLookups()
    {
        await Assert.That(AllFiles.DocumentPaths.Contains(useFileTarget)).IsTrue();

        // Regression: the merged Files dictionary and every lookup built on it
        // must see the registered file, not just the per-category dictionary.
        await Assert.That(AllFiles.Files.ContainsKey(".usefileext")).IsTrue();
        await Assert.That(AllFiles.GetPathFor(".usefileext")).IsEqualTo(useFileTarget);
        await Assert.That(AllFiles.TryGetPathFor("usefileext", out var path)).IsTrue();
        await Assert.That(path).IsEqualTo(useFileTarget);
        await Assert.That(AllFiles.IsEmptyFile(useFileTarget)).IsTrue();

        // Lookups are case-insensitive.
        await Assert.That(AllFiles.GetPathFor(".USEFILEEXT")).IsEqualTo(useFileTarget);
        await Assert.That(AllFiles.TryGetPathFor("USEFILEEXT", out _)).IsTrue();
    }

    [Test]
    public async Task TryCreateFile_extensionless()
    {
        await Assert.That(AllFiles.TryCreateFile("Dockerfile", useEmptyStringForTextFiles: true)).IsFalse();
        await Assert.That(AllFiles.TryCreateFile("LICENSE")).IsFalse();
    }

    [Test]
    public async Task GetPathFor_caseInsensitive()
    {
        await Assert.That(AllFiles.GetPathFor(".PNG")).IsNotNull();
        await Assert.That(AllFiles.GetPathFor("PNG")).IsNotNull();
    }

    [Test]
    public async Task TryGetPathFor_normalizesDotlessAndCase()
    {
        await Assert.That(AllFiles.TryGetPathFor("png", out var path)).IsTrue();
        await Assert.That(path).IsNotNull();
        await Assert.That(AllFiles.TryGetPathFor(".png", out _)).IsTrue();
        await Assert.That(AllFiles.TryGetPathFor("PNG", out _)).IsTrue();
        await Assert.That(AllFiles.TryGetPathFor(".PNG", out _)).IsTrue();
    }

    [Test]
    public async Task IsEmptyFile_empty_throwsArgumentNull()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() => AllFiles.IsEmptyFile(""));
        await Assert.That(exception!.ParamName).IsEqualTo("path");
    }

    [Test]
    public async Task ExtractDirectory_isVersionIsolated()
    {
        var path = AllFiles.GetPathFor(".png");
        var versionDirectory = new DirectoryInfo(Path.GetDirectoryName(path)!).Parent!;
        await Assert.That(versionDirectory.Parent!.Name).IsEqualTo("EmptyFiles");

        // AssemblyVersion and FileVersion are both pinned to 1.0.0; extraction
        // must be keyed on the package (informational) version instead.
        var assemblyVersion = typeof(AllFiles).Assembly.GetName().Version!.ToString();
        await Assert.That(versionDirectory.Name).IsNotEqualTo(assemblyVersion);
        await Assert.That(versionDirectory.Name).IsNotEqualTo("1.0.0");
        await Assert.That(versionDirectory.Name).IsNotEqualTo("unknown");
    }

    [Test]
    public async Task Extraction_leavesNoTempFiles()
    {
        var path = AllFiles.GetPathFor(".png");
        await Assert.That(File.Exists(path)).IsTrue();
        var directory = Path.GetDirectoryName(path)!;
        await Assert.That(Directory.GetFiles(directory, "*.tmp")).IsEmpty();
    }

    [Test]
    public async Task EmptyFile_OpenRead_userFile()
    {
        var file = Path.Combine(Path.GetTempPath(), $"EmptyFileOpenRead{Guid.NewGuid():N}.dat");
        File.WriteAllText(file, "hello");
        try
        {
            var emptyFile = new EmptyFile(file, File.GetLastWriteTime(file), Category.Binary);
            using var stream = emptyFile.OpenRead();
            using var reader = new StreamReader(stream);
            await Assert.That(reader.ReadToEnd()).IsEqualTo("hello");
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Test]
    public async Task EmptyFile_OpenRead_embedded()
    {
        using var stream = AllFiles.Images[".png"].OpenRead();
        await Assert.That(stream.Length > 0).IsTrue();
    }

    [Test]
    public async Task CreateFile_overwrite_binary()
    {
        AllFiles.CreateFile("foo.bmp");
        AllFiles.CreateFile("foo.bmp");
        await Assert.That(File.Exists("foo.bmp")).IsTrue();
    }

    [Test]
    public async Task CreateFile_NoDir_binary()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        AllFiles.CreateFile("myTempDir/foo.bmp");
        await Assert.That(File.Exists("myTempDir/foo.bmp")).IsTrue();
    }

    [Test]
    public void CreateFile_preamble()
    {
        AllFiles.CreateFile("foo.txt", true);

        var preamble = Encoding.UTF8.GetPreamble();
        var bytes = File.ReadAllBytes("foo.txt");
        if (bytes.Length < preamble.Length ||
            preamble
                .Where((p, i) => p != bytes[i])
                .Any())
        {
            throw new ArgumentException("Not utf8-BOM");
        }
    }

    [Test]
    public async Task CreateFile_overwrite_txt()
    {
        AllFiles.CreateFile("foo.txt", true);
        AllFiles.CreateFile("foo.txt", true);
        await Assert.That(File.Exists("foo.txt")).IsTrue();
    }

    [Test]
    public async Task CreateFile_NoDir_txt()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        AllFiles.CreateFile("myTempDir/foo.txt", true);
        await Assert.That(File.Exists("myTempDir/foo.txt")).IsTrue();
    }

    [Test]
    public async Task TryCreateFile_overwrite_txt()
    {
        await Assert.That(AllFiles.TryCreateFile("foo.txt", true)).IsTrue();
        await Assert.That(AllFiles.TryCreateFile("foo.txt", true)).IsTrue();
        await Assert.That(File.Exists("foo.txt")).IsTrue();
    }

    [Test]
    public async Task TryCreateFile_NoDir_txt()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        await Assert.That(AllFiles.TryCreateFile("myTempDir/foo.txt", true)).IsTrue();
        await Assert.That(File.Exists("myTempDir/foo.txt")).IsTrue();
    }

    [Test]
    public async Task TryCreateFile_overwrite_binary()
    {
        await Assert.That(AllFiles.TryCreateFile("foo.bmp")).IsTrue();
        await Assert.That(AllFiles.TryCreateFile("foo.bmp")).IsTrue();
        await Assert.That(File.Exists("foo.bmp")).IsTrue();
    }

    [Test]
    public async Task TryCreateFile_NoDir_binary()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        await Assert.That(AllFiles.TryCreateFile("myTempDir/foo.bmp")).IsTrue();
        await Assert.That(File.Exists("myTempDir/foo.bmp")).IsTrue();
    }

    [Test]
    public async Task Unknown_extension()
    {
        Assert.ThrowsExactly<Exception>(() => AllFiles.GetPathFor("txt"));
        await Assert.That(AllFiles.TryGetPathFor("txt", out var result)).IsFalse();
        await Assert.That(result).IsNull();
        await Assert.That(AllFiles.TryGetPathFor(".txt", out result)).IsFalse();
        await Assert.That(result).IsNull();
        await Assert.That(AllFiles.TryCreateFile("foo.txt")).IsFalse();
        await Assert.That(result).IsNull();
        Assert.ThrowsExactly<Exception>(() => AllFiles.GetPathFor(".txt"));
        Assert.ThrowsExactly<Exception>(() => AllFiles.CreateFile("foo.txt"));
    }

    [Test]
    public async Task GetPathFor()
    {
        #region GetPathFor

        var path = AllFiles.GetPathFor(".jpg");

        #endregion

        await Assert.That(path).IsNotNull();
        await Assert.That(File.Exists(path)).IsTrue();

        path = AllFiles.GetPathFor("jpg");
        await Assert.That(path).IsNotNull();
        await Assert.That(File.Exists(path)).IsTrue();
    }

    [Test]
    public async Task CreateFile()
    {
        var pathOfFileToCreate = "file.jpg";
        File.Delete(pathOfFileToCreate);

        #region CreateFile

        AllFiles.CreateFile(pathOfFileToCreate);

        #endregion

        await Assert.That(File.Exists(pathOfFileToCreate)).IsTrue();
        File.Delete(pathOfFileToCreate);

        AllFiles.CreateFile("foo.txt", true);
        await Assert.That(File.Exists("foo.txt")).IsTrue();
        File.Delete("foo.txt");

        await Assert.That(AllFiles.TryCreateFile(pathOfFileToCreate)).IsTrue();
        await Assert.That(File.Exists(pathOfFileToCreate)).IsTrue();
        File.Delete(pathOfFileToCreate);

        await Assert.That(AllFiles.TryCreateFile("foo.txt")).IsFalse();
        await Assert.That(File.Exists("foo.txt")).IsFalse();
        File.Delete("foo.txt");

        await Assert.That(AllFiles.TryCreateFile("foo.txt", true)).IsTrue();
        await Assert.That(File.Exists("foo.txt")).IsTrue();
        File.Delete("foo.txt");
    }

    [Test]
    public async Task IsEmptyFile()
    {
        #region IsEmptyFile

        var path = AllFiles.GetPathFor(".jpg");
        await Assert.That(AllFiles.IsEmptyFile(path)).IsTrue();
        var temp = Path.GetTempFileName();
        await Assert.That(AllFiles.IsEmptyFile(temp)).IsFalse();

        #endregion

        File.Delete(temp);
    }

    [Test]
    public async Task WriteAllTo()
    {
        using var directory = new TempDirectory();

        #region WriteAllTo

        AllFiles.WriteAllTo(directory);

        #endregion

        foreach (var category in Enum.GetValues<Category>())
        {
            var categoryDir = Path.Combine(directory, category.ToString().ToLowerInvariant());
            await Assert.That(Directory.Exists(categoryDir)).IsTrue();
        }

        foreach (var file in AllFiles.Files.Values)
        {
            var expected = Path.Combine(
                directory,
                file.Category.ToString().ToLowerInvariant(),
                $"empty{file.Extension}");
            await Assert.That(File.Exists(expected)).IsTrue().Because(expected);
            await Assert.That(new FileInfo(expected).Length).IsEqualTo(new FileInfo(file.Path).Length);
        }

        Directory.Delete(directory, true);
    }

    [Test]
    public async Task AllPaths()
    {
        await Assert.That(AllFiles.AllPaths).IsNotEmpty();

        #region AllPaths

        foreach (var path in AllFiles.AllPaths)
        {
            Trace.WriteLine(path);
        }

        #endregion
    }

    static string ThisFile([CallerFilePath] string testFile = "") =>
        testFile;

    //[Test]
#pragma warning disable CA1822
    internal async Task UseFile()
#pragma warning restore CA1822
    {
        var pathToFile = ThisFile();

        #region UseFile

        AllFiles.UseFile(Category.Document, pathToFile);
        await Assert.That(AllFiles.DocumentPaths.Contains(pathToFile)).IsTrue();

        #endregion
    }

#if NET9_0

    [Test]
    public async Task WriteExtensions()
    {
        var md = Path.Combine(ProjectFiles.SolutionDirectory, "extensions.include.md");
        File.Delete(md);
        await using var writer = File.CreateText(md);
        await WriteCategory(writer, "Archive", AllFiles.Archives);
        await WriteCategory(writer, "Document", AllFiles.Documents);
        await WriteCategory(writer, "Image", AllFiles.Images);
        await WriteCategory(writer, "Map", AllFiles.Maps);
        await WriteCategory(writer, "Sheet", AllFiles.Sheets);
        await WriteCategory(writer, "Slide", AllFiles.Slides);
        await WriteCategory(writer, "Binary", AllFiles.Binary);
    }

    static async Task WriteCategory(StreamWriter writer, string category, IReadOnlyDictionary<string, EmptyFile> files)
    {
        await writer.WriteLineAsync("");
        await writer.WriteLineAsync($"### {category}");
        await writer.WriteLineAsync("");
        foreach (var file in files.OrderBy(_ => _.Key))
        {
            var size = Size.Suffix(new FileInfo(file.Value.Path).Length);
            await writer.WriteLineAsync($"  * {file.Key} ({size})");
        }
    }

#endif
}