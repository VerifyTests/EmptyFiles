# <img src="/src/icon.png" height="30px"> EmptyFiles

[![Discussions](https://img.shields.io/badge/Verify-Discussions-yellow?svg=true&label=)](https://github.com/orgs/VerifyTests/discussions)
[![Build status](https://ci.appveyor.com/api/projects/status/4mrhpal9rwtqajws/branch/main?svg=true)](https://ci.appveyor.com/project/SimonCropp/EmptyFiles)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.svg?label=EmptyFiles)](https://www.nuget.org/packages/EmptyFiles/)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.Tool.svg?label=dotnet%20tool)](https://www.nuget.org/packages/EmptyFiles.Tool/)

A collection of minimal binary files.

**See [Milestones](../../milestones?state=closed) for release notes.**


## NuGet package

 * https://nuget.org/packages/EmptyFiles/
 * https://nuget.org/packages/EmptyFiles.Tool/


## Files

All files: https://github.com/VerifyTests/EmptyFiles/tree/main/files

<!-- include: extensions. path: /src/extensions.include.md -->
### Archive
<!-- endInclude -->


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
<sup><a href='/src/Tests/Tests.cs#L132-L136' title='Snippet source file'>snippet source</a> | <a href='#snippet-createfile' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/src/Tests/Tests.cs#L116-L120' title='Snippet source file'>snippet source</a> | <a href='#snippet-getpathfor' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Throws an exception if the extension is not known. There is also a `TryGetPathFor` that will return false if the extension is not known.


### IsEmptyFile

Returns true if the target file is an empty file.

<!-- snippet: IsEmptyFile -->
<a id='snippet-isemptyfile'></a>
```cs
var path = AllFiles.GetPathFor("jpg");
True(AllFiles.IsEmptyFile(path));
var temp = Path.GetTempFileName();
False(AllFiles.IsEmptyFile(temp));
```
<sup><a href='/src/Tests/Tests.cs#L161-L168' title='Snippet source file'>snippet source</a> | <a href='#snippet-isemptyfile' title='Start of snippet'>anchor</a></sup>
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
<sup><a href='/src/Tests/Tests.cs#L178-L185' title='Snippet source file'>snippet source</a> | <a href='#snippet-allpaths' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### UseFile

Use or replace a file

<!-- snippet: UseFile -->
<a id='snippet-usefile'></a>
```cs
AllFiles.UseFile(Category.Document, pathToFile);
IsTrue(AllFiles.DocumentPaths.Contains(pathToFile));
```
<sup><a href='/src/Tests/Tests.cs#L196-L201' title='Snippet source file'>snippet source</a> | <a href='#snippet-usefile' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


### Extensions helper


#### IsText

https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json

<!-- snippet: IsText -->
<a id='snippet-istext'></a>
```cs
True(FileExtensions.IsTextFile("file.txt"));
False(FileExtensions.IsTextFile("file.bin"));
True(FileExtensions.IsTextExtension("txt"));
False(FileExtensions.IsTextExtension("bin"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L6-L13' title='Snippet source file'>snippet source</a> | <a href='#snippet-istext' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### AddTextExtension

<!-- snippet: AddTextExtension -->
<a id='snippet-addtextextension'></a>
```cs
FileExtensions.AddTextExtension(".ext1");
True(FileExtensions.IsTextExtension(".ext1"));
True(FileExtensions.IsTextFile("file.ext1"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L19-L25' title='Snippet source file'>snippet source</a> | <a href='#snippet-addtextextension' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


#### RemoveTextExtension

<!-- snippet: RemoveTextExtension -->
<a id='snippet-removetextextension'></a>
```cs
FileExtensions.AddTextExtension(".ext1");
True(FileExtensions.IsTextExtension(".ext1"));
FileExtensions.RemoveTextExtension(".ext1");
False(FileExtensions.IsTextExtension(".ext1"));
```
<sup><a href='/src/Tests/ExtensionsTests.cs#L31-L38' title='Snippet source file'>snippet source</a> | <a href='#snippet-removetextextension' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


## Icon

[Hollow](https://thenounproject.com/term/hollow/51835/) designed by [Michael Senkow](https://thenounproject.com/mhsenkow/) from [The Noun Project](https://thenounproject.com).
