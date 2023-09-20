using EmptyFiles;

public class Tests
{
    [Test]
    public void CreateFile_overwrite_binary()
    {
        AllFiles.CreateFile("foo.bmp");
        AllFiles.CreateFile("foo.bmp");
        Assert.True(File.Exists("foo.bmp"));
    }

    [Test]
    public void CreateFile_NoDir_binary()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        AllFiles.CreateFile("myTempDir/foo.bmp");
        Assert.True(File.Exists("myTempDir/foo.bmp"));
    }

    [Test]
    public void CreateFile_preamble()
    {
        AllFiles.CreateFile("foo.txt", true);

        var preamble = Encoding.UTF8.GetPreamble();
        var bytes = File.ReadAllBytes("foo.txt");
        if (bytes.Length < preamble.Length ||
            preamble.Where((p, i) => p != bytes[i]).Any())
        {
            throw new ArgumentException("Not utf8-BOM");
        }
    }

    [Test]
    public void CreateFile_overwrite_txt()
    {
        AllFiles.CreateFile("foo.txt", true);
        AllFiles.CreateFile("foo.txt", true);
        Assert.True(File.Exists("foo.txt"));
    }

    [Test]
    public void CreateFile_NoDir_txt()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        AllFiles.CreateFile("myTempDir/foo.txt", true);
        Assert.True(File.Exists("myTempDir/foo.txt"));
    }

    [Test]
    public void TryCreateFile_overwrite_txt()
    {
        Assert.True(AllFiles.TryCreateFile("foo.txt", true));
        Assert.True(AllFiles.TryCreateFile("foo.txt", true));
        Assert.True(File.Exists("foo.txt"));
    }

    [Test]
    public void TryCreateFile_NoDir_txt()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        Assert.True(AllFiles.TryCreateFile("myTempDir/foo.txt", true));
        Assert.True(File.Exists("myTempDir/foo.txt"));
    }

    [Test]
    public void TryCreateFile_overwrite_binary()
    {
        Assert.True(AllFiles.TryCreateFile("foo.bmp"));
        Assert.True(AllFiles.TryCreateFile("foo.bmp"));
        Assert.True(File.Exists("foo.bmp"));
    }

    [Test]
    public void TryCreateFile_NoDir_binary()
    {
        if (Directory.Exists("myTempDir"))
        {
            Directory.Delete("myTempDir", true);
        }

        Assert.True(AllFiles.TryCreateFile("myTempDir/foo.bmp"));
        Assert.True(File.Exists("myTempDir/foo.bmp"));
    }

    [Test]
    public void Unknown_extension()
    {
        Assert.Throws<Exception>(() => AllFiles.GetPathFor("txt"));
        Assert.False(AllFiles.TryGetPathFor("txt", out var result));
        Assert.Null(result);
        Assert.False(AllFiles.TryGetPathFor(".txt", out result));
        Assert.Null(result);
        Assert.False(AllFiles.TryCreateFile("foo.txt"));
        Assert.Null(result);
        Assert.Throws<Exception>(() => AllFiles.GetPathFor(".txt"));
        Assert.Throws<Exception>(() => AllFiles.CreateFile("foo.txt"));
    }

    [Test]
    public void GetPathFor()
    {
        #region GetPathFor

        var path = AllFiles.GetPathFor("jpg");

        #endregion

        var path2 = AllFiles.GetPathFor(".jpg");
        Assert.NotNull(path);
        Assert.NotNull(path2);
        Assert.True(File.Exists(path));
        Assert.True(File.Exists(path2));
    }

    [Test]
    public void CreateFile()
    {
        var pathOfFileToCreate = "file.jpg";
        File.Delete(pathOfFileToCreate);

        #region CreateFile

        AllFiles.CreateFile(pathOfFileToCreate);

        #endregion

        Assert.True(File.Exists(pathOfFileToCreate));
        File.Delete(pathOfFileToCreate);

        AllFiles.CreateFile("foo.txt", true);
        Assert.True(File.Exists("foo.txt"));
        File.Delete("foo.txt");

        Assert.True(AllFiles.TryCreateFile(pathOfFileToCreate));
        Assert.True(File.Exists(pathOfFileToCreate));
        File.Delete(pathOfFileToCreate);

        Assert.False(AllFiles.TryCreateFile("foo.txt"));
        Assert.False(File.Exists("foo.txt"));
        File.Delete("foo.txt");

        Assert.True(AllFiles.TryCreateFile("foo.txt", true));
        Assert.True(File.Exists("foo.txt"));
        File.Delete("foo.txt");
    }

    [Test]
    public void IsEmptyFile()
    {
        #region IsEmptyFile

        var path = AllFiles.GetPathFor("jpg");
        Assert.True(AllFiles.IsEmptyFile(path));
        var temp = Path.GetTempFileName();
        Assert.False(AllFiles.IsEmptyFile(temp));

        #endregion

        var path2 = AllFiles.GetPathFor(".jpg");
        Assert.True(AllFiles.IsEmptyFile(path2));
        File.Delete(temp);
    }

    [Test]
    public void Aliases()
    {
        var path = AllFiles.GetPathFor("jpeg");
        Assert.True(AllFiles.IsEmptyFile(path));

        Assert.IsTrue(AllFiles.ImageExtensions.Contains("jpeg"));
    }

    [Test]
    public void AllPaths()
    {
        Assert.IsNotEmpty(AllFiles.AllPaths);

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
        Assert.IsTrue(AllFiles.DocumentPaths.Contains(pathToFile));

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