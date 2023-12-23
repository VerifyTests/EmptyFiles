﻿// ReSharper disable UnusedMember.Global

namespace EmptyFiles;

public static class AllFiles
{
    static Dictionary<string, string> aliases = new()
    {
        {
            "jpeg", "jpg"
        },
        {
            "tiff", "tif"
        },
        {
            "7zip", "7z"
        },
        {
            "gzip", "gz"
        },
        {
            "bzip2", "bz2"
        }
    };

    public static IReadOnlyDictionary<string, EmptyFile> Files => files;

    static ConcurrentDictionary<string, EmptyFile> files = [];

    public static IReadOnlyDictionary<string, EmptyFile> Archives => archives;

    static ConcurrentDictionary<string, EmptyFile> archives = [];

    public static IReadOnlyDictionary<string, EmptyFile> Documents => documents;

    static ConcurrentDictionary<string, EmptyFile> documents = [];

    public static IReadOnlyDictionary<string, EmptyFile> Images => images;

    static ConcurrentDictionary<string, EmptyFile> images = [];

    public static IReadOnlyDictionary<string, EmptyFile> Sheets => sheets;

    static ConcurrentDictionary<string, EmptyFile> sheets = [];

    public static IReadOnlyDictionary<string, EmptyFile> Slides => slides;

    static ConcurrentDictionary<string, EmptyFile> slides = [];

    static AllFiles()
    {
        var emptyFiles = FindEmptyFilesDirectory();
        foreach (var file in Directory.EnumerateFiles(emptyFiles, "*.*", SearchOption.AllDirectories))
        {
            var category = GetCategory(file);
            var emptyFile = EmptyFile.Build(file, category);
            var extension = FileExtensions.GetExtension(file);
            var categoryFiles = FindDictionaryForCategory(category);

            categoryFiles[extension] = emptyFile;
            files[extension] = emptyFile;
            var alias = aliases.SingleOrDefault(_ => _.Value == extension);
            if (alias.Key != null)
            {
                categoryFiles[alias.Key] = emptyFile;
                files[alias.Key] = emptyFile;
            }
        }
    }

    static string FindEmptyFilesDirectory()
    {
        var directories = FindDirectories()
            .Select(_ => Path.Combine(_, "EmptyFiles"))
            .ToList();
        foreach (var directory in directories)
        {
            if (Directory.Exists(directory))
            {
                return directory;
            }
        }

        throw new(
            $"""
             Could not find empty files directory. Searched:
             {string.Join(Environment.NewLine, directories)}
             """);
    }

    static IEnumerable<string> FindDirectories()
    {
        yield return AppDomain.CurrentDomain.BaseDirectory;
        yield return AssemblyLocation.Directory;
        yield return Environment.CurrentDirectory;
    }

    static ConcurrentDictionary<string, EmptyFile> FindDictionaryForCategory(Category category) =>
        category switch
        {
            Category.Archive => archives,
            Category.Document => documents,
            Category.Image => images,
            Category.Sheet => sheets,
            Category.Slide => slides,
            _ => throw new($"Unknown category: {category}")
        };

    public static void UseFile(Category category, string file)
    {
        Guard.FileExists(file);
        var extension = FileExtensions.GetExtension(file);
        var emptyFile = EmptyFile.Build(file, category);
        FindDictionaryForCategory(category)
            .AddOrUpdate(extension, _ => emptyFile, (_, _) => emptyFile);
        files.AddOrUpdate(extension, _ => emptyFile, (_, _) => emptyFile);
    }

    static Category GetCategory(string file)
    {
        var directory = Directory.GetParent(file)!.Name;
        return (Category) Enum.Parse(typeof(Category), directory, true);
    }

    public static IEnumerable<string> AllPaths => files.Values.Select(_ => _.Path);

    public static IEnumerable<string> AllExtensions => files.Keys;

    public static IEnumerable<string> ArchivePaths => archives.Values.Select(_ => _.Path);

    public static IEnumerable<string> ArchiveExtensions => archives.Keys;

    public static IEnumerable<string> DocumentPaths => documents.Values.Select(_ => _.Path);

    public static IEnumerable<string> DocumentExtensions => documents.Keys;

    public static IEnumerable<string> ImagePaths => images.Values.Select(_ => _.Path);

    public static IEnumerable<string> ImageExtensions => images.Keys;

    public static IEnumerable<string> SheetPaths => sheets.Values.Select(_ => _.Path);

    public static IEnumerable<string> SheetExtensions => sheets.Keys;

    public static IEnumerable<string> SlidePaths => slides.Values.Select(_ => _.Path);

    public static IEnumerable<string> SlideExtensions => slides.Keys;

    public static bool IsEmptyFile(string path)
    {
        Guard.FileExists(path);
        var extension = FileExtensions.GetExtension(path);
        if (!files.TryGetValue(extension, out var emptyFile))
        {
            return false;
        }

        return File.GetLastWriteTime(path) == emptyFile.LastWriteTime;
    }

    public static void CreateFile(string path, bool useEmptyStringForTextFiles = false, Encoding? encoding = null)
    {
        TryCreateDirectory(path);
        var extension = FileExtensions.GetExtension(path);
        if (useEmptyStringForTextFiles &&
            FileExtensions.IsText(extension))
        {
            CreateTextFile(path, encoding);
            return;
        }

        File.Copy(GetPathFor(extension), path, true);
    }

    static void CreateTextFile(string path, Encoding? encoding)
    {
        File.Delete(path);
        encoding ??= Encoding.UTF8;
        File.WriteAllBytes(path, encoding.GetPreamble());
    }

    public static string GetPathFor(string extension)
    {
        Guard.AgainstNullOrEmpty(extension);
        extension = FileExtensions.GetExtension(extension);
        if (files.TryGetValue(extension, out var emptyFile))
        {
            return emptyFile.Path;
        }

        throw new($"Unknown extension: {extension}");
    }

    public static bool TryCreateFile(string path, bool useEmptyStringForTextFiles = false, Encoding? encoding = null)
    {
        Guard.AgainstNullOrEmpty(path);
        var extension = Path.GetExtension(path);

        if (useEmptyStringForTextFiles &&
            FileExtensions.IsText(extension))
        {
            TryCreateDirectory(path);

            CreateTextFile(path, encoding);
            return true;
        }

        if (!TryGetPathFor(extension, out var source))
        {
            return false;
        }

        TryCreateDirectory(path);

        File.Copy(source, path, true);
        return true;
    }

    static void TryCreateDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    public static bool TryGetPathFor(string extension, [NotNullWhen(true)] out string? path)
    {
        extension = FileExtensions.GetExtension(extension);
        if (files.TryGetValue(extension, out var emptyFile))
        {
            path = emptyFile.Path;
            return true;
        }

        path = null;
        return false;
    }
}