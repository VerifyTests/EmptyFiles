using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public static class EmptyFiles
{
    static Dictionary<string, EmptyFile> files = new Dictionary<string, EmptyFile>();

    static EmptyFiles()
    {
        var emptyFiles = Path.Combine(CodeBaseLocation.CurrentDirectory, "EmptyFiles");
        foreach (var path in Directory.EnumerateFiles(emptyFiles, "*.*"))
        {
            var lastWriteTime = File.GetLastWriteTime(path);
            files[FileHelpers.Extension(path)] = new EmptyFile(path, lastWriteTime);
        }
    }

    public static IEnumerable<string> AllPaths
    {
        get { return files.Values.Select(x => x.Path); }
    }

    public static bool IsEmptyFile(string path)
    {
        Guard.FileExists(path, nameof(path));
        var extension = FileHelpers.Extension(path);
        if (!files.TryGetValue(extension, out var emptyFile))
        {
            return false;
        }

        return File.GetLastWriteTime(path) == emptyFile.LastWriteTime;
    }

    public static string GetPathFor(string extension)
    {
        Guard.AgainstNullOrEmpty(extension, nameof(extension));
        if (files.TryGetValue(extension, out var emptyFile))
        {
            return emptyFile.Path;
        }

        throw new Exception($"Unknown extension: {extension}");
    }

    public static bool TryGetPathFor(string extension, [NotNullWhen(true)] out string? path)
    {
        Guard.AgainstNullOrEmpty(extension, nameof(extension));
        if (files.TryGetValue(extension, out var emptyFile))
        {
            path = emptyFile.Path;
            return true;
        }

        path = null;
        return false;
    }
}