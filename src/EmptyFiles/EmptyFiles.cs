using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public static class EmptyFiles
{
    static Dictionary<string, EmptyFile> files = new Dictionary<string, EmptyFile>();
    static Dictionary<string, EmptyFile> archives = new Dictionary<string, EmptyFile>();
    static Dictionary<string, EmptyFile> documents = new Dictionary<string, EmptyFile>();
    static Dictionary<string, EmptyFile> images = new Dictionary<string, EmptyFile>();
    static Dictionary<string, EmptyFile> sheets = new Dictionary<string, EmptyFile>();
    static Dictionary<string, EmptyFile> slides = new Dictionary<string, EmptyFile>();

    static EmptyFiles()
    {
        var emptyFiles = Path.Combine(CodeBaseLocation.CurrentDirectory, "EmptyFiles");
        foreach (var file in Directory.EnumerateFiles(emptyFiles, "*.*", SearchOption.AllDirectories))
        {
            var lastWriteTime = File.GetLastWriteTime(file);
            var category = GetCategory(file);
            var emptyFile = new EmptyFile(file, lastWriteTime, category);
            var extension = FileHelpers.Extension(file);
            files[extension] = emptyFile;
            switch (category)
            {
                case EmptyFileCategory.Archive:
                    archives[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Document:
                    documents[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Image:
                    images[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Sheet:
                    sheets[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Slide:
                    slides[extension] = emptyFile;
                    break;
            }
        }
    }

    static EmptyFileCategory GetCategory(string file)
    {
        var directory = Directory.GetParent(file).Name;
        return (EmptyFileCategory) Enum.Parse(typeof(EmptyFileCategory), directory, true);
    }

    public static IEnumerable<string> AllPaths
    {
        get { return files.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> ArchivePaths
    {
        get { return archives.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> DocumentPaths
    {
        get { return documents.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> ImagePaths
    {
        get { return images.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> SheetPaths
    {
        get { return sheets.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> SlidePaths
    {
        get { return slides.Values.Select(x => x.Path); }
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