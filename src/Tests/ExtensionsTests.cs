public class ExtensionsTests
{
    [Test]
    public void IsText()
    {
        #region IsText

        True(FileExtensions.IsTextFile("file.txt"));
        False(FileExtensions.IsTextFile("file.bin"));
        True(FileExtensions.IsTextExtension(".txt"));
        False(FileExtensions.IsTextExtension(".bin"));

        #endregion
    }

    [Test]
    public void AddTextExtension()
    {
        #region AddTextExtension

        FileExtensions.AddTextExtension(".ext1");
        True(FileExtensions.IsTextExtension(".ext1"));
        True(FileExtensions.IsTextFile("file.ext1"));

        #endregion
    }

    [Test]
    public void RemoveTextExtension()
    {
        #region RemoveTextExtension

        FileExtensions.AddTextExtension(".ext1");
        True(FileExtensions.IsTextExtension(".ext1"));
        FileExtensions.RemoveTextExtension(".ext1");
        False(FileExtensions.IsTextExtension(".ext1"));

        #endregion
    }
}