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

    public ExtensionsTests(ITestOutputHelper output) :
        base(output)
    {
    }
}