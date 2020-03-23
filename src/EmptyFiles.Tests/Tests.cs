using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
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
        await WriteCategory(writer, "Archive", EmptyFiles.ArchivePaths);
        await WriteCategory(writer, "Document", EmptyFiles.DocumentPaths);
        await WriteCategory(writer, "Image", EmptyFiles.ImagePaths);
        await WriteCategory(writer, "Sheet", EmptyFiles.SheetPaths);
        await WriteCategory(writer, "Slide", EmptyFiles.SlidePaths);
    }
    
	#region Empty Files Directory Tests

	/// <summary>
	/// The 'EmptyFiles' directory and assembly are copies and referenced in the same folder. 
	/// </summary>
	/// <remarks>
	/// This would typically be in the same folder as the starting assembly.
	/// </remarks>
	[Fact]
	public void EmptyFilesDirectoryStandardDeployment()
	{
		try
		{
			var result = CallGetEmptyFilesDirectory(_currentDirectory);
			Assert.Equal(Path.Combine(_currentDirectory, "EmptyFiles"), result);
		}
		catch (DirectoryNotFoundException e)
		{
			Assert.True(false, "'EmptyFiles' directory was not where it was expected to be.  " + e.Message);
		}
	}
	
	/// <summary>
	/// The 'EmptyFiles' directory is in the parent folder from where the assembly is referenced from. 
	/// </summary>
	/// <remarks>
	/// This could happen if the 'EmptyFiles' assembly is being referenced and accessed directly from
	/// a nuget package directory, and the 'EmptyFiles' directory is at the root of the package directory
	/// and the assembly is in ./lib/{target Framework}/.
	/// </remarks>
	[Fact]
	public void EmptyFilesDirectoryNugetDeployment()
	{
		try
		{
			var result = CallGetEmptyFilesDirectory(Path.Combine(_currentDirectory, ".\\tmp1\\tmp2"));
			Assert.Equal(Path.Combine(_currentDirectory, "EmptyFiles"), result);
		}
		catch (DirectoryNotFoundException e)
		{
			Assert.True(false, "'EmptyFiles' directory was not where it was expected to be.  " + e.Message);
		}
	}

	/// <summary>
	/// The 'EmptyFiles' directory and assembly are not present, ensure we throw the correct error. 
	/// </summary>
	/// <remarks>
	/// This would typically be in the same folder as the starting assembly.
	/// </remarks>
	[Fact]
	public void EmptyFilesDirectoryMissing()
	{
		Assert.Throws<DirectoryNotFoundException>(() =>
			CallGetEmptyFilesDirectory(Path.Combine(_currentDirectory, "tmp1/tmp2/tmp3/tmp4")));
	}
    
	private static string? CallGetEmptyFilesDirectory(string currentDirectory)
	{
		var methodInfo = typeof(EmptyFiles).GetMethod("GetEmptyFilesDirectory", BindingFlags.Static | BindingFlags.NonPublic);
		if (methodInfo == null)
			throw new NullReferenceException("GetEmptyFilesDirectory method was not found on EmptyFiles");

		try
		{
			return methodInfo.Invoke(null, new object[] { currentDirectory }) as string;
		}
		catch (TargetInvocationException e)
		{
			// Unwrap any TargetInvocationException.
			while (e.InnerException is TargetInvocationException exception)
			{
				e = exception;
			}

			// Rethrow the DirectoryNotFoundException.
			if (e.InnerException is DirectoryNotFoundException directoryNotFound)
			{
				throw e.InnerException;
			}

			// Rethrow the original
			throw;
		}
	}

	#endregion

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
    
    private readonly string _currentDirectory = string.Empty;

    public Tests(ITestOutputHelper output) :
        base(output)
    {
	    var assembly = typeof(Tests).Assembly;
	    if (assembly.CodeBase != null)
	    {
		    var path = assembly.CodeBase
			    .Replace("file:///", "")
			    .Replace("file://", "")
			    .Replace(@"file:\\\", "")
			    .Replace(@"file:\\", "");

		    _currentDirectory = Path.GetDirectoryName(path) ?? SourceDirectory;
	    }
    }
}