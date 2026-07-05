[TestFixture]
public class Tests
{
    // UseFile mutates global state with no unregister, so register once for the
    // whole fixture and keep the backing file on disk until every test (notably
    // WriteAllTo, which reads it) has run.
    static string useFileTarget = null!;

    [OneTimeSetUp]
    public void RegisterUseFile()
    {
        useFileTarget = Path.Combine(Path.GetTempPath(), $"EmptyFilesUseFile{Guid.NewGuid():N}.usefileext");
        File.WriteAllText(useFileTarget, "content");
        AllFiles.UseFile(Category.Document, useFileTarget);
    }

    [OneTimeTearDown]
    public void CleanupUseFile() =>
        File.Delete(useFileTarget);

    [Test]
    public void UseFile_UpdatesLookups()
    {
        True(AllFiles.DocumentPaths.Contains(useFileTarget));

        // Regression: the merged Files dictionary and every lookup built on it
        // must see the registered file, not just the per-category dictionary.
        True(AllFiles.Files.ContainsKey(".usefileext"));
        AreEqual(useFileTarget, AllFiles.GetPathFor(".usefileext"));
        True(AllFiles.TryGetPathFor("usefileext", out var path));
        AreEqual(useFileTarget, path);
        True(AllFiles.IsEmptyFile(useFileTarget));

        // Lookups are case-insensitive.
        AreEqual(useFileTarget, AllFiles.GetPathFor(".USEFILEEXT"));
        True(AllFiles.TryGetPathFor("USEFILEEXT", out _));
    }

    [Test]
    public void TryCreateFile_extensionless()
    {
        False(AllFiles.TryCreateFile("Dockerfile", useEmptyStringForTextFiles: true));
        False(AllFiles.TryCreateFile("LICENSE"));
    }

    [Test]
    public void GetPathFor_caseInsensitive()
    {
        NotNull(AllFiles.GetPathFor(".PNG"));
        NotNull(AllFiles.GetPathFor("PNG"));
    }

    [Test]
    public void TryGetPathFor_normalizesDotlessAndCase()
    {
        True(AllFiles.TryGetPathFor("png", out var path));
        NotNull(path);
        True(AllFiles.TryGetPathFor(".png", out _));
        True(AllFiles.TryGetPathFor("PNG", out _));
        True(AllFiles.TryGetPathFor(".PNG", out _));
    }

    [Test]
    public void IsEmptyFile_empty_throwsArgumentNull()
    {
        var exception = Throws<ArgumentNullException>(() => AllFiles.IsEmptyFile(""));
        AreEqual("path", exception!.ParamName);
    }

    [Test]
    public void ExtractDirectory_isVersionIsolated()
    {
        var path = AllFiles.GetPathFor(".png");
        var versionDirectory = new DirectoryInfo(Path.GetDirectoryName(path)!).Parent!;
        AreEqual("EmptyFiles", versionDirectory.Parent!.Name);

        // AssemblyVersion and FileVersion are both pinned to 1.0.0; extraction
        // must be keyed on the package (informational) version instead.
        var assemblyVersion = typeof(AllFiles).Assembly.GetName().Version!.ToString();
        AreNotEqual(assemblyVersion, versionDirectory.Name);
        AreNotEqual("1.0.0", versionDirectory.Name);
        AreNotEqual("unknown", versionDirectory.Name);
    }

    [Test]
    public void Extraction_leavesNoTempFiles()
    {
        var path = AllFiles.GetPathFor(".png");
        True(File.Exists(path));
        var directory = Path.GetDirectoryName(path)!;
        IsEmpty(Directory.GetFiles(directory, "*.tmp"));
    }

    [Test]
    public void EmptyFile_OpenRead_userFile()
    {
        var file = Path.Combine(Path.GetTempPath(), $"EmptyFileOpenRead{Guid.NewGuid():N}.dat");
        File.WriteAllText(file, "hello");
        try
        {
            var emptyFile = new EmptyFile(file, File.GetLastWriteTime(file), Category.Binary);
            using var stream = emptyFile.OpenRead();
            using var reader = new StreamReader(stream);
            AreEqual("hello", reader.ReadToEnd());
        }
        finally
        {
            File.Delete(file);
        }
    }

    [Test]
    public void EmptyFile_OpenRead_embedded()
    {
        using var stream = AllFiles.Images[".png"].OpenRead();
        True(stream.Length > 0);
    }

    [Test]
    public void CreateFile_overwrite_binary()
    {
        AllFiles.CreateFile("foo.bmp");
        AllFiles.CreateFile("foo.bmp");
        True(File.Exists("foo.bmp"));
    }

    [Test]
    public void CreateFile_NoDir_binary()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        AllFiles.CreateFile("myTempDir/foo.bmp");
        True(File.Exists("myTempDir/foo.bmp"));
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
    public void CreateFile_overwrite_txt()
    {
        AllFiles.CreateFile("foo.txt", true);
        AllFiles.CreateFile("foo.txt", true);
        True(File.Exists("foo.txt"));
    }

    [Test]
    public void CreateFile_NoDir_txt()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        AllFiles.CreateFile("myTempDir/foo.txt", true);
        True(File.Exists("myTempDir/foo.txt"));
    }

    [Test]
    public void TryCreateFile_overwrite_txt()
    {
        True(AllFiles.TryCreateFile("foo.txt", true));
        True(AllFiles.TryCreateFile("foo.txt", true));
        True(File.Exists("foo.txt"));
    }

    [Test]
    public void TryCreateFile_NoDir_txt()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        True(AllFiles.TryCreateFile("myTempDir/foo.txt", true));
        True(File.Exists("myTempDir/foo.txt"));
    }

    [Test]
    public void TryCreateFile_overwrite_binary()
    {
        True(AllFiles.TryCreateFile("foo.bmp"));
        True(AllFiles.TryCreateFile("foo.bmp"));
        True(File.Exists("foo.bmp"));
    }

    [Test]
    public void TryCreateFile_NoDir_binary()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        True(AllFiles.TryCreateFile("myTempDir/foo.bmp"));
        True(File.Exists("myTempDir/foo.bmp"));
    }

    [Test]
    public void Unknown_extension()
    {
        Throws<Exception>(() => AllFiles.GetPathFor("txt"));
        False(AllFiles.TryGetPathFor("txt", out var result));
        Null(result);
        False(AllFiles.TryGetPathFor(".txt", out result));
        Null(result);
        False(AllFiles.TryCreateFile("foo.txt"));
        Null(result);
        Throws<Exception>(() => AllFiles.GetPathFor(".txt"));
        Throws<Exception>(() => AllFiles.CreateFile("foo.txt"));
    }

    [Test]
    public void GetPathFor()
    {
        #region GetPathFor

        var path = AllFiles.GetPathFor(".jpg");

        #endregion

        NotNull(path);
        True(File.Exists(path));

        path = AllFiles.GetPathFor("jpg");
        NotNull(path);
        True(File.Exists(path));
    }

    [Test]
    public void CreateFile()
    {
        var pathOfFileToCreate = "file.jpg";
        File.Delete(pathOfFileToCreate);

        #region CreateFile

        AllFiles.CreateFile(pathOfFileToCreate);

        #endregion

        True(File.Exists(pathOfFileToCreate));
        File.Delete(pathOfFileToCreate);

        AllFiles.CreateFile("foo.txt", true);
        True(File.Exists("foo.txt"));
        File.Delete("foo.txt");

        True(AllFiles.TryCreateFile(pathOfFileToCreate));
        True(File.Exists(pathOfFileToCreate));
        File.Delete(pathOfFileToCreate);

        False(AllFiles.TryCreateFile("foo.txt"));
        False(File.Exists("foo.txt"));
        File.Delete("foo.txt");

        True(AllFiles.TryCreateFile("foo.txt", true));
        True(File.Exists("foo.txt"));
        File.Delete("foo.txt");
    }

    [Test]
    public void IsEmptyFile()
    {
        #region IsEmptyFile

        var path = AllFiles.GetPathFor(".jpg");
        True(AllFiles.IsEmptyFile(path));
        var temp = Path.GetTempFileName();
        False(AllFiles.IsEmptyFile(temp));

        #endregion

        File.Delete(temp);
    }

    [Test]
    public void WriteAllTo()
    {
        using var directory = new TempDirectory();

        #region WriteAllTo

        AllFiles.WriteAllTo(directory);

        #endregion

        foreach (var category in Enum.GetValues<Category>())
        {
            var categoryDir = Path.Combine(directory, category.ToString().ToLowerInvariant());
            True(Directory.Exists(categoryDir));
        }

        foreach (var file in AllFiles.Files.Values)
        {
            var expected = Path.Combine(
                directory,
                file.Category.ToString().ToLowerInvariant(),
                $"empty{file.Extension}");
            True(File.Exists(expected), expected);
            That(new FileInfo(expected).Length, Is.EqualTo(new FileInfo(file.Path).Length));
        }

        Directory.Delete(directory, true);
    }

    [Test]
    public void AllPaths()
    {
        IsNotEmpty(AllFiles.AllPaths);

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
    public void UseFile()
#pragma warning restore CA1822
    {
        var pathToFile = ThisFile();

        #region UseFile

        AllFiles.UseFile(Category.Document, pathToFile);
        IsTrue(AllFiles.DocumentPaths.Contains(pathToFile));

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