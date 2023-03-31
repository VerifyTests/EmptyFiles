# <img src="/src/icon.png" height="30px"> EmptyFiles

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/4mrhpal9rwtqajws/branch/main?svg=true)](https://ci.appveyor.com/project/SimonCropp/EmptyFiles)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.svg?label=EmptyFiles)](https://www.nuget.org/packages/EmptyFiles/)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.Tool.svg?label=dotnet%20tool)](https://www.nuget.org/packages/EmptyFiles.Tool/)

A collection of minimal binary files.


## NuGet package

 * https://nuget.org/packages/EmptyFiles/
 * https://nuget.org/packages/EmptyFiles.Tool/


## Files

All files: https://github.com/VerifyTests/EmptyFiles/tree/main/files

 <!-- include: extensions. path: /src/EmptyFiles.Tests/extensions.include.md -->
### Archive

  * 7z (32 bytes)
  * 7zip (32 bytes)
  * bz2 (14 bytes)
  * bzip2 (14 bytes)
  * gz (29 bytes)
  * gzip (29 bytes)
  * tar (1.5 KB)
  * xz (32 bytes)
  * zip (22 bytes)

### Document

  * docx (1.9 KB)
  * odt (2.2 KB)
  * pdf (291 bytes)
  * rtf (6 bytes)

### Image

  * avif (298 bytes)
  * bmp (58 bytes)
  * dds (136 bytes)
  * dib (58 bytes)
  * emf (620 bytes)
  * exif (734 bytes)
  * gif (799 bytes)
  * heic (3.2 KB)
  * heif (209 bytes)
  * ico (70 bytes)
  * j2c (270 bytes)
  * jfif (734 bytes)
  * jp2 (357 bytes)
  * jpc (270 bytes)
  * jpe (734 bytes)
  * jpeg (734 bytes)
  * jpg (734 bytes)
  * jxr (300 bytes)
  * pbm (10 bytes)
  * pcx (131 bytes)
  * pgm (12 bytes)
  * png (119 bytes)
  * ppm (14 bytes)
  * rle (58 bytes)
  * tga (543 bytes)
  * tif (250 bytes)
  * tiff (250 bytes)
  * wdp (300 bytes)
  * webp (228 bytes)
  * wmp (300 bytes)

### Sheet

  * ods (2.7 KB)
  * xlsx (4.5 KB)

### Slide

  * odp (7.8 KB)
  * pptx (13.3 KB) <!-- endInclude -->


## Consuming files as a web resource

Files can be consumed as a web resource using the following url:

```
https://github.com/VerifyTests/EmptyFiles/raw/main/index/empty.{extension}
```

So for example to consume a jpg use

```
https://github.com/VerifyTests/EmptyFiles/raw/main/index/empty.jpg
```

A 404 will result for non-existent files.


## Tool Usage


### Installation

Ensure [dotnet CLI is installed](https://docs.microsoft.com/en-us/dotnet/core/tools/).

Install [EmptyFiles.Tool](https://nuget.org/packages/EmptyFiles.Tool/)

```ps
dotnet tool install -g EmptyFiles.Tool
```


### Extension only Usage

```
emptyfile bmp
```

Creates `{CurrentDirectory}/empty.bmp`


### File Usage

```
emptyfile myfile.bmp
```

Creates `{CurrentDirectory}/myfile.bmp`


### Path Usage

```
emptyfile path/myfile.bmp
```

Creates `path/myfile.bmp`


## Library Usage


### CreateFile

Creates a new empty file

<!-- snippet: CreateFile -->
<a id='snippet-createfile'></a>
```cs
AllFiles.CreateFile(pathOfFileToCreate);
```
<sup><a href='/src/EmptyFiles.Tests/Tests.cs#L136-L140' title='Snippet source file'>snippet source</a> | <a href='#snippet-createfile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Throws an exception if the extension is not known. There is also a `TryCreateFile` that will return false if the extension is not known.

Use the optional `useEmptyStringForTextFiles` to create a empty text file if the extension is text. The file will be UTF8 no BOM as per https://www.unicode.org/versions/Unicode5.0.0/ch02.pdf "Use of a BOM is neither required nor recommended for UTF-8".


### GetPathFor

Gets the path to an empty file for a given extension

<!-- snippet: GetPathFor -->
<a id='snippet-getpathfor'></a>
```cs
var path = AllFiles.GetPathFor("jpg");
```
<sup><a href='/src/EmptyFiles.Tests/Tests.cs#L117-L121' title='Snippet source file'>snippet source</a> | <a href='#snippet-getpathfor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Throws an exception if the extension is not known. There is also a `TryGetPathFor` that will return false if the extension is not known.


### IsEmptyFile

Returns true if the target file is an empty file.

<!-- snippet: IsEmptyFile -->
<a id='snippet-isemptyfile'></a>
```cs
var path = AllFiles.GetPathFor("jpg");
Assert.True(AllFiles.IsEmptyFile(path));
var temp = Path.GetTempFileName();
Assert.False(AllFiles.IsEmptyFile(temp));
```
<sup><a href='/src/EmptyFiles.Tests/Tests.cs#L165-L172' title='Snippet source file'>snippet source</a> | <a href='#snippet-isemptyfile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### AllPaths

Enumerates all empty files

<!-- snippet: AllPaths -->
<a id='snippet-allpaths'></a>
```cs
foreach (var path in AllFiles.AllPaths)
{
    Trace.WriteLine(path);
}
```
<sup><a href='/src/EmptyFiles.Tests/Tests.cs#L193-L200' title='Snippet source file'>snippet source</a> | <a href='#snippet-allpaths' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### UseFile

Use or replace a file

<!-- snippet: UseFile -->
<a id='snippet-usefile'></a>
```cs
AllFiles.UseFile(Category.Document, pathToFile);
Assert.Contains(pathToFile, AllFiles.DocumentPaths);
```
<sup><a href='/src/EmptyFiles.Tests/Tests.cs#L208-L213' title='Snippet source file'>snippet source</a> | <a href='#snippet-usefile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Extensions helper


#### IsText

https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json

<!-- snippet: IsText -->
<a id='snippet-istext'></a>
```cs
Assert.True(FileExtensions.IsText("file.txt"));
Assert.False(FileExtensions.IsText("file.bin"));
Assert.True(FileExtensions.IsText(".txt"));
Assert.False(FileExtensions.IsText(".bin"));
Assert.True(FileExtensions.IsText("txt"));
Assert.False(FileExtensions.IsText("bin"));
```
<sup><a href='/src/EmptyFiles.Tests/ExtensionsTests.cs#L9-L18' title='Snippet source file'>snippet source</a> | <a href='#snippet-istext' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### AddTextExtension

<!-- snippet: AddTextExtension -->
<a id='snippet-addtextextension'></a>
```cs
FileExtensions.AddTextExtension("ext1");
FileExtensions.AddTextExtension(".ext2");
Assert.True(FileExtensions.IsText("ext1"));
Assert.True(FileExtensions.IsText("ext2"));
```
<sup><a href='/src/EmptyFiles.Tests/ExtensionsTests.cs#L24-L31' title='Snippet source file'>snippet source</a> | <a href='#snippet-addtextextension' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### RemoveTextExtension

<!-- snippet: RemoveTextExtension -->
<a id='snippet-removetextextension'></a>
```cs
FileExtensions.AddTextExtension("ext1");
FileExtensions.AddTextExtension(".ext2");
Assert.True(FileExtensions.IsText("ext1"));
Assert.True(FileExtensions.IsText("ext2"));
FileExtensions.RemoveTextExtension("ext1");
FileExtensions.RemoveTextExtension(".ext2");
Assert.False(FileExtensions.IsText("ext1"));
Assert.False(FileExtensions.IsText("ext2"));
```
<sup><a href='/src/EmptyFiles.Tests/ExtensionsTests.cs#L37-L48' title='Snippet source file'>snippet source</a> | <a href='#snippet-removetextextension' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Hollow](https://thenounproject.com/term/hollow/51835/) designed by [Michael Senkow](https://thenounproject.com/mhsenkow/) from [The Noun Project](https://thenounproject.com).
