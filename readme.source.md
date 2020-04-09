# <img src="/src/icon.png" height="30px"> EmptyFiles

[![Build status](https://ci.appveyor.com/api/projects/status/4mrhpal9rwtqajws/branch/master?svg=true)](https://ci.appveyor.com/project/SimonCropp/EmptyFiles)
[![NuGet Status](https://img.shields.io/nuget/v/EmptyFiles.svg?label=EmptyFiles)](https://www.nuget.org/packages/EmptyFiles/)

A collection of minimal binary files.

Support is available via a [Tidelift Subscription](https://tidelift.com/subscription/pkg/nuget-emptyfiles?utm_source=nuget-emptyfiles&utm_medium=referral&utm_campaign=enterprise).

toc


## NuGet package

 * https://nuget.org/packages/EmptyFiles/


## Files

All files: https://github.com/SimonCropp/EmptyFiles/tree/master/files

include: extensions


## Usage


### CreateFile

Creates a new empty file

snippet: CreateFile

Throws an exception if the extension is not known. There is also a `TryCreateFile` that will return false if the extension is not known.


### GetPathFor

Gets the path to an empty file for a given extension

snippet: GetPathFor

Throws an exception if the extension is not known. There is also a `TryGetPathFor` that will return false if the extension is not known.


### IsEmptyFile

Returns true if the target file is an empty file.

snippet: IsEmptyFile


### AllPaths

Enumerates all empty files

snippet: AllPaths


### UseFile

Use or replace a file

snippet: UseFile


### Extensions helper


#### IsText

https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json

snippet: IsText


#### AddTextExtension

snippet: AddTextExtension


#### RemoveTextExtension

snippet: RemoveTextExtension


## Security contact information

To report a security vulnerability, use the [Tidelift security contact](https://tidelift.com/security). Tidelift will coordinate the fix and disclosure.


## Icon

[Hollow](https://thenounproject.com/term/hollow/51835/) designed by [Michael Senkow](https://thenounproject.com/mhsenkow/) from [The Noun Project](https://thenounproject.com).
