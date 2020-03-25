using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using EmptyFiles;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    XunitContextBase
{
    [Fact]
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

    void CreateFile(string pathOfFileToCreate)
    {
        #region CreateFile
        AllFiles.CreateFile(pathOfFileToCreate);
        #endregion
    }

    void CreateFile(string pathOfFileToCreate)
    {
        #region CreateFile
        AllFiles.CreateFile(pathOfFileToCreate);
        #endregion
    }

    [Fact]
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

    [Fact]
    public void AllPaths()
    {
        Assert.NotEmpty(AllFiles.AllPaths);
        #region AllPaths
        foreach (var path in AllFiles.AllPaths)
        {
            Trace.WriteLine(path);
        }
        #endregion
    }

    [Fact]
    public async Task WriteExtensions()
    {
        var md = Path.Combine(SourceDirectory, "extensions.include.md");
        File.Delete(md);
        await using var writer = File.CreateText(md);
        await WriteCategory(writer, "Archive", AllFiles.ArchivePaths);
        await WriteCategory(writer, "Document", AllFiles.DocumentPaths);
        await WriteCategory(writer, "Image", AllFiles.ImagePaths);
        await WriteCategory(writer, "Sheet", AllFiles.SheetPaths);
        await WriteCategory(writer, "Slide", AllFiles.SlidePaths);
    }

    private static async Task WriteCategory(StreamWriter writer, string category, IEnumerable<string> allPaths)
    {
        await writer.WriteLineAsync($"### {category}");
        await writer.WriteLineAsync("");
        foreach (var path in allPaths)
        {
            var size = Size.Suffix(new FileInfo(path).Length);
            var ext = Path.GetExtension(path).Substring(1);
            await writer.WriteLineAsync($"  * {ext} ({size})");
        }
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}