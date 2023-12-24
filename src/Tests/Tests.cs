public class Tests
{
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

        var path = AllFiles.GetPathFor("jpg");

        #endregion

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

        var path = AllFiles.GetPathFor("jpg");
        True(AllFiles.IsEmptyFile(path));
        var temp = Path.GetTempFileName();
        False(AllFiles.IsEmptyFile(temp));

        #endregion

        File.Delete(temp);
    }

    [Test]
    public void Aliases()
    {
        var path = AllFiles.GetPathFor("jpeg");
        True(AllFiles.IsEmptyFile(path));

        IsTrue(AllFiles.ImageExtensions.Contains("jpeg"));
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
    public void UseFile()
    {
        var pathToFile = ThisFile();

        #region UseFile

        AllFiles.UseFile(Category.Document, pathToFile);
        IsTrue(AllFiles.DocumentPaths.Contains(pathToFile));

        #endregion
    }

    [Test]
    public async Task WriteExtensions()
    {
        var md = Path.Combine(SolutionDirectoryFinder.Find(), "extensions.include.md");
        File.Delete(md);
        using var writer = File.CreateText(md);
        await WriteCategory(writer, "Archive", AllFiles.Archives);
        await WriteCategory(writer, "Document", AllFiles.Documents);
        await WriteCategory(writer, "Image", AllFiles.Images);
        await WriteCategory(writer, "Sheet", AllFiles.Sheets);
        await WriteCategory(writer, "Slide", AllFiles.Slides);
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
}