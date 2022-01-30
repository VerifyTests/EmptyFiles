using EmptyFiles;

public class ExtensionsTests :
    XunitContextBase
{
    [Fact]
    public void IsText()
    {
        #region IsText

        Assert.True(Extensions.IsText("file.txt"));
        Assert.False(Extensions.IsText("file.bin"));
        Assert.True(Extensions.IsText(".txt"));
        Assert.False(Extensions.IsText(".bin"));
        Assert.True(Extensions.IsText("txt"));
        Assert.False(Extensions.IsText("bin"));

        #endregion
    }

    [Fact]
    public void AddTextExtension()
    {
        #region AddTextExtension

        Extensions.AddTextExtension("ext1");
        Extensions.AddTextExtension(".ext2");
        Assert.True(Extensions.IsText("ext1"));
        Assert.True(Extensions.IsText("ext2"));

        #endregion
    }

    [Fact]
    public void RemoveTextExtension()
    {
        #region RemoveTextExtension

        Extensions.AddTextExtension("ext1");
        Extensions.AddTextExtension(".ext2");
        Assert.True(Extensions.IsText("ext1"));
        Assert.True(Extensions.IsText("ext2"));
        Extensions.RemoveTextExtension("ext1");
        Extensions.RemoveTextExtension(".ext2");
        Assert.False(Extensions.IsText("ext1"));
        Assert.False(Extensions.IsText("ext2"));

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