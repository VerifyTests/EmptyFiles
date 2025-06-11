# <img src="/src/icon.png" height="30px"> EmptyFiles

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/4mrhpal9rwtqajws/branch/main?svg=true)](https://ci.appveyor.com/project/SimonCropp/EmptyFiles)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.svg?label=EmptyFiles)](https://www.nuget.org/packages/EmptyFiles/)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.Tool.svg?label=dotnet%20tool)](https://www.nuget.org/packages/EmptyFiles.Tool/)

A collection of minimal binary files.

**See [Milestones](../../milestones?state=closed) for release notes.**


## Sponsors

include: zzz


## Sponsored by

[![JetBrains logo.](https://resources.jetbrains.com/storage/products/company/brand/logos/jetbrains.svg)](https://jb.gg/OpenSourceSupport)


## NuGet

 * https://nuget.org/packages/EmptyFiles/
 * https://nuget.org/packages/EmptyFiles.Tool/


## Files

All files: https://github.com/VerifyTests/EmptyFiles/tree/main/files


<!-- include: extensions. path: /src/extensions.include.md -->
### Archive

  * .7z (32 bytes)
  * .7zip (32 bytes)
  * .bz2 (14 bytes)
  * .bzip2 (14 bytes)
  * .gz (29 bytes)
  * .gzip (29 bytes)
  * .tar (1.5 KB)
  * .xz (32 bytes)
  * .zip (22 bytes)

### Document

  * .docx (1.9 KB)
  * .odt (2.2 KB)
  * .pdf (280 bytes)
  * .rtf (6 bytes)

### Image

  * .avif (298 bytes)
  * .bmp (58 bytes)
  * .dds (136 bytes)
  * .dib (58 bytes)
  * .emf (620 bytes)
  * .exif (734 bytes)
  * .gif (799 bytes)
  * .heic (3.2 KB)
  * .heif (209 bytes)
  * .ico (70 bytes)
  * .j2c (270 bytes)
  * .jfif (734 bytes)
  * .jp2 (354 bytes)
  * .jpc (270 bytes)
  * .jpe (734 bytes)
  * .jpeg (734 bytes)
  * .jpg (734 bytes)
  * .jxr (300 bytes)
  * .pbm (8 bytes)
  * .pcx (131 bytes)
  * .pgm (12 bytes)
  * .png (119 bytes)
  * .ppm (14 bytes)
  * .rle (58 bytes)
  * .tga (543 bytes)
  * .tif (250 bytes)
  * .tiff (250 bytes)
  * .wdp (300 bytes)
  * .webp (228 bytes)
  * .wmp (300 bytes)

### Sheet

  * .ods (2.7 KB)
  * .xlsx (4.5 KB)

### Slide

  * .odp (7.8 KB)
  * .pptx (13.3 KB)<!-- endInclude -->


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
<a id='snippet-CreateFile'></a>
```cs
AllFiles.CreateFile(pathOfFileToCreate);
```
<sup><a href='/src/Tests/Tests.cs#L136-L140' title='Snippet source file'>snippet source</a> | <a href='#snippet-CreateFile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Throws an exception if the extension is not known. There is also a `TryCreateFile` that will return false if the extension is not known.

Use the optional `useEmptyStringForTextFiles` to create a empty text file if the extension is text. The file will be UTF8 no BOM as per https://www.unicode.org/versions/Unicode5.0.0/ch02.pdf "Use of a BOM is neither required nor recommended for UTF-8".


### GetPathFor

Gets the path to an empty file for a given extension

<!-- snippet: GetPathFor -->
<a id='snippet-GetPathFor'></a>
```cs
var path = AllFiles.GetPathFor(".jpg");
```
<sup><a href='/src/Tests/Tests.cs#L116-L120' title='Snippet source file'>snippet source</a> | <a href='#snippet-GetPathFor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Throws an exception if the extension is not known. There is also a `TryGetPathFor` that will return false if the extension is not known.


### IsEmptyFile

Returns true if the target file is an empty file.

<!-- snippet: IsEmptyFile -->
<a id='snippet-IsEmptyFile'></a>
```cs
var path = AllFiles.GetPathFor(".jpg");
True(AllFiles.IsEmptyFile(path));
var temp = Path.GetTempFileName();
False(AllFiles.IsEmptyFile(temp));
```
<sup><a href='/src/Tests/Tests.cs#L165-L172' title='Snippet source file'>snippet source</a> | <a href='#snippet-IsEmptyFile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### AllPaths

Enumerates all empty files

<!-- snippet: AllPaths -->
<a id='snippet-AllPaths'></a>
```cs
foreach (var path in AllFiles.AllPaths)
{
    Trace.WriteLine(path);
}
```
<sup><a href='/src/Tests/Tests.cs#L182-L189' title='Snippet source file'>snippet source</a> | <a href='#snippet-AllPaths' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### UseFile

Use or replace a file

<!-- snippet: UseFile -->
<a id='snippet-UseFile'></a>
```cs
AllFiles.UseFile(Category.Document, pathToFile);
IsTrue(AllFiles.DocumentPaths.Contains(pathToFile));
```
<sup><a href='/src/Tests/Tests.cs#L202-L207' title='Snippet source file'>snippet source</a> | <a href='#snippet-UseFile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Extensions helper


#### IsText

https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json

<!-- snippet: IsText -->
<a id='snippet-IsText'></a>
```cs
True(FileExtensions.IsTextFile("file.txt"));
False(FileExtensions.IsTextFile("file.bin"));
True(FileExtensions.IsTextExtension(".txt"));
False(FileExtensions.IsTextExtension(".bin"));
True(FileExtensions.IsTextExtension("txt"));
False(FileExtensions.IsTextExtension("bin"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L6-L15' title='Snippet source file'>snippet source</a> | <a href='#snippet-IsText' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### AddTextExtension

<!-- snippet: AddTextExtension -->
<a id='snippet-AddTextExtension'></a>
```cs
FileExtensions.AddTextExtension(".ext1");
True(FileExtensions.IsTextExtension(".ext1"));
True(FileExtensions.IsTextFile("file.ext1"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L53-L59' title='Snippet source file'>snippet source</a> | <a href='#snippet-AddTextExtension' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### RemoveTextExtension

<!-- snippet: RemoveTextExtension -->
<a id='snippet-RemoveTextExtension'></a>
```cs
FileExtensions.AddTextExtension(".ext1");
True(FileExtensions.IsTextExtension(".ext1"));
FileExtensions.RemoveTextExtension(".ext1");
False(FileExtensions.IsTextExtension(".ext1"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L69-L76' title='Snippet source file'>snippet source</a> | <a href='#snippet-RemoveTextExtension' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### AddTextFileConvention

`AddTextFileConvention` allows the use of a convention based text file detection via a callback.

At app startup add a convention using `FileExtensions.AddTextFileConvention`:

<!-- snippet: AddTextFileConvention -->
<a id='snippet-AddTextFileConvention'></a>
```cs
[ModuleInitializer]
public static void AddTextFileConvention() =>
    // Treat files ending with .txtViaConvention as text files
    FileExtensions.AddTextFileConvention(path => path.EndsWith(".txtViaConvention"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L27-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-AddTextFileConvention' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Then any call to `FileExtensions.IsTextFile` will, in addition to checking the known text extensions, also check if any of the added text contentions return true.

<!-- snippet: TextViaConvention -->
<a id='snippet-TextViaConvention'></a>
```cs
True(FileExtensions.IsTextFile("c:/path/file.txtViaConvention"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L20-L24' title='Snippet source file'>snippet source</a> | <a href='#snippet-TextViaConvention' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

An alternative approach to a text file convention would be to check if a file has a preamble that matches an known text encoding.


## Icon

[Hollow](https://thenounproject.com/term/hollow/51835/) designed by [Michael Senkow](https://thenounproject.com/mhsenkow/) from [The Noun Project](https://thenounproject.com).
