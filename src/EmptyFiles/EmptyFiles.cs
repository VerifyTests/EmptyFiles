using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public static class EmptyFiles
{
    public static Dictionary<string, EmptyFile> Files = new Dictionary<string, EmptyFile>();
    public static Dictionary<string, EmptyFile> Archives = new Dictionary<string, EmptyFile>();
    public static Dictionary<string, EmptyFile> Documents = new Dictionary<string, EmptyFile>();
    public static Dictionary<string, EmptyFile> Images = new Dictionary<string, EmptyFile>();
    public static Dictionary<string, EmptyFile> Sheets = new Dictionary<string, EmptyFile>();
    public static Dictionary<string, EmptyFile> Slides = new Dictionary<string, EmptyFile>();

    static EmptyFiles()
    {
        var emptyFiles = Path.Combine(CodeBaseLocation.CurrentDirectory, "EmptyFiles");
        foreach (var file in Directory.EnumerateFiles(emptyFiles, "*.*", SearchOption.AllDirectories))
        {
            var lastWriteTime = File.GetLastWriteTime(file);
            var category = GetCategory(file);
            var emptyFile = new EmptyFile(file, lastWriteTime, category);
            var extension = FileHelpers.Extension(file);
            Files[extension] = emptyFile;
            switch (category)
            {
                case EmptyFileCategory.Archive:
                    Archives[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Document:
                    Documents[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Image:
                    Images[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Sheet:
                    Sheets[extension] = emptyFile;
                    break;
                case EmptyFileCategory.Slide:
                    Slides[extension] = emptyFile;
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
        get { return Files.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> ArchivePaths
    {
        get { return Archives.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> DocumentPaths
    {
        get { return Documents.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> ImagePaths
    {
        get { return Images.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> SheetPaths
    {
        get { return Sheets.Values.Select(x => x.Path); }
    }

    public static IEnumerable<string> SlidePaths
    {
        get { return Slides.Values.Select(x => x.Path); }
    }

    public static bool IsEmptyFile(string path)
    {
        Guard.FileExists(path, nameof(path));
        var extension = FileHelpers.Extension(path);
        if (!Files.TryGetValue(extension, out var emptyFile))
        {
            return false;
        }

        return File.GetLastWriteTime(path) == emptyFile.LastWriteTime;
    }

    public static string GetPathFor(string extension)
    {
        Guard.AgainstNullOrEmpty(extension, nameof(extension));
        if (Files.TryGetValue(extension, out var emptyFile))
        {
            return emptyFile.Path;
        }

        throw new Exception($"Unknown extension: {extension}");
    }

    public static bool TryGetPathFor(string extension, [NotNullWhen(true)] out string? path)
    {
        Guard.AgainstNullOrEmpty(extension, nameof(extension));
        if (Files.TryGetValue(extension, out var emptyFile))
        {
            path = emptyFile.Path;
            return true;
        }

        path = null;
        return false;
    }
}