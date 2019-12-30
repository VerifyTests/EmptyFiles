using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

public class Tests :
    XunitContextBase
{
    [Fact]
    public void GetPathFor()
    {
        #region GetPathFor
        var path = EmptyFiles.GetPathFor("jpg");
        #endregion
        Assert.NotNull(path);
        Assert.True(File.Exists(path));
    }

    [Fact]
    public void IsEmptyFile()
    {
        #region IsEmptyFile
        var path = EmptyFiles.GetPathFor("jpg");
        Assert.True(EmptyFiles.IsEmptyFile(path));
        var temp = Path.GetTempFileName();
        Assert.False(EmptyFiles.IsEmptyFile(temp));
        #endregion
        File.Delete(temp);
    }

    [Fact]
    public void AllPaths()
    {
        Assert.NotEmpty(EmptyFiles.AllPaths);
        #region AllPaths
        foreach (var path in EmptyFiles.AllPaths)
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
        foreach (var path in EmptyFiles.AllPaths)
        {
            var size = SizeSuffix(new FileInfo(path).Length);
            var ext = Path.GetExtension(path).Substring(1);
            await writer.WriteLineAsync($"  * {ext} ({size})");
        }
    }

    static string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    static string SizeSuffix(long value)
    {
        var mag = (int) Math.Log(value, 1024);

        var adjustedSize = (decimal) value / (1L << (mag * 10));

        if (Math.Round(adjustedSize, 1) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return $"{adjustedSize:0.#} {SizeSuffixes[mag]}";
    }

    public Tests(ITestOutputHelper output) :
        base(output)
    {
    }
}