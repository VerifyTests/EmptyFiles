public class ExtensionsTests
{
    [Test]
    public void IsText()
    {
        #region IsText

        True(FileExtensions.IsText("file.txt"));
        False(FileExtensions.IsText("file.bin"));
        True(FileExtensions.IsText(".txt"));
        False(FileExtensions.IsText(".bin"));
        True(FileExtensions.IsText("txt"));
        False(FileExtensions.IsText("bin"));

        #endregion
    }

    [Test]
    public void AddTextExtension()
    {
        #region AddTextExtension

        FileExtensions.AddTextExtension("ext1");
        FileExtensions.AddTextExtension(".ext2");
        True(FileExtensions.IsText("ext1"));
        True(FileExtensions.IsText("ext2"));

        #endregion
    }

    [Test]
    public void RemoveTextExtension()
    {
        #region RemoveTextExtension

        FileExtensions.AddTextExtension("ext1");
        FileExtensions.AddTextExtension(".ext2");
        True(FileExtensions.IsText("ext1"));
        True(FileExtensions.IsText("ext2"));
        FileExtensions.RemoveTextExtension("ext1");
        FileExtensions.RemoveTextExtension(".ext2");
        False(FileExtensions.IsText("ext1"));
        False(FileExtensions.IsText("ext2"));

        #endregion
    }

    [Test]
    public void Run()
    {
        AreEqual("txt", FileExtensions.GetExtension("file.txt"));
        AreEqual("txt", FileExtensions.GetExtension("c:/file.txt"));
        AreEqual("txt", FileExtensions.GetExtension(".txt"));
        AreEqual("txt", FileExtensions.GetExtension("./File.txt"));
        AreEqual("txt", FileExtensions.GetExtension("txt"));
    }
}