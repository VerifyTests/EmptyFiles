using EmptyFiles;
using Xunit;
using Xunit.Abstractions;

public class ExtensionsTests :
    XunitContextBase
{
    [Fact]
    public void IsTextFile()
    {
        #region IsTextFile
        Assert.True(Extensions.IsTextFile("file.txt"));
        Assert.False(Extensions.IsTextFile("file.bin"));
        #endregion
    }

    [Fact]
    public void Run()
    {
        Assert.Equal("txt", Extensions.GetExtension("file.txt"));
        Assert.Equal("txt", Extensions.GetExtension("c:/file.txt"));
        Assert.Equal("txt", Extensions.GetExtension(".txt"));
        Assert.Equal("txt", Extensions.GetExtension("./File.txt"));
        Assert.Equal("txt", Extensions.GetExtension("txt"));
    }

    public ExtensionsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}