using EmptyFiles;

public class ExtensionsTests(ITestOutputHelper output) :
    XunitContextBase(output)
{
    [Fact]
    public void IsText()
    {
        #region IsText

        Assert.True(FileExtensions.IsText("file.txt"));
        Assert.False(FileExtensions.IsText("file.bin"));
        Assert.True(FileExtensions.IsText(".txt"));
        Assert.False(FileExtensions.IsText(".bin"));
        Assert.True(FileExtensions.IsText("txt"));
        Assert.False(FileExtensions.IsText("bin"));

        #endregion
    }

    [Fact]
    public void AddTextExtension()
    {
        #region AddTextExtension

        FileExtensions.AddTextExtension("ext1");
        FileExtensions.AddTextExtension(".ext2");
        Assert.True(FileExtensions.IsText("ext1"));
        Assert.True(FileExtensions.IsText("ext2"));

        #endregion
    }

    [Fact]
    public void RemoveTextExtension()
    {
        #region RemoveTextExtension

        FileExtensions.AddTextExtension("ext1");
        FileExtensions.AddTextExtension(".ext2");
        Assert.True(FileExtensions.IsText("ext1"));
        Assert.True(FileExtensions.IsText("ext2"));
        FileExtensions.RemoveTextExtension("ext1");
        FileExtensions.RemoveTextExtension(".ext2");
        Assert.False(FileExtensions.IsText("ext1"));
        Assert.False(FileExtensions.IsText("ext2"));

        #endregion
    }

    [Fact]
    public void Run()
    {
        Assert.Equal("txt", FileExtensions.GetExtension("file.txt"));
        Assert.Equal("txt", FileExtensions.GetExtension("c:/file.txt"));
        Assert.Equal("txt", FileExtensions.GetExtension(".txt"));
        Assert.Equal("txt", FileExtensions.GetExtension("./File.txt"));
        Assert.Equal("txt", FileExtensions.GetExtension("txt"));
    }
}