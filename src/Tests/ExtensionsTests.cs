using EmptyFiles;

public class ExtensionsTests
{
    [Test]
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

    [Test]
    public void AddTextExtension()
    {
        #region AddTextExtension

        FileExtensions.AddTextExtension("ext1");
        FileExtensions.AddTextExtension(".ext2");
        Assert.True(FileExtensions.IsText("ext1"));
        Assert.True(FileExtensions.IsText("ext2"));

        #endregion
    }

    [Test]
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

    [Test]
    public void Run()
    {
        Assert.AreEqual("txt", FileExtensions.GetExtension("file.txt"));
        Assert.AreEqual("txt", FileExtensions.GetExtension("c:/file.txt"));
        Assert.AreEqual("txt", FileExtensions.GetExtension(".txt"));
        Assert.AreEqual("txt", FileExtensions.GetExtension("./File.txt"));
        Assert.AreEqual("txt", FileExtensions.GetExtension("txt"));
    }
}