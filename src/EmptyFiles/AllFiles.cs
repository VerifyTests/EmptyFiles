#if NET6_0_OR_GREATER
using ReadOnlySet = System.Collections.Generic.IReadOnlySet<string>;
#else
using ReadOnlySet = System.Collections.Generic.IReadOnlyCollection<string>;
#endif
namespace EmptyFiles;

public static class AllFiles
{
    public static IReadOnlyDictionary<string, EmptyFile> Files => files;

    static Dictionary<string, EmptyFile> files = [];

    public static IReadOnlyDictionary<string, EmptyFile> Archives => archives;

    static IReadOnlyDictionary<string, EmptyFile> archives;

    public static IReadOnlyDictionary<string, EmptyFile> Documents => documents;

    static IReadOnlyDictionary<string, EmptyFile> documents;

    public static IReadOnlyDictionary<string, EmptyFile> Images => images;

    static IReadOnlyDictionary<string, EmptyFile> images;

    public static IReadOnlyDictionary<string, EmptyFile> Sheets => sheets;

    static IReadOnlyDictionary<string, EmptyFile> sheets;

    public static IReadOnlyDictionary<string, EmptyFile> Slides => slides;

    static IReadOnlyDictionary<string, EmptyFile> slides;

    static AllFiles()
    {
        var directory = FindEmptyFilesDirectory();

        archives = AddCategory(archiveExtensions, Category.Archive, directory);
        documents = AddCategory(documentExtensions, Category.Document, directory);
        images = AddCategory(imageExtensions, Category.Image, directory);
        sheets = AddCategory(sheetExtensions, Category.Sheet, directory);
        slides = AddCategory(slideExtensions, Category.Slide, directory);
    }

    static IReadOnlyDictionary<string, EmptyFile> AddCategory(ReadOnlySet extensions, Category category, string emptyDirectory)
    {
        Dictionary<string, EmptyFile> items = [];
        var categoryDirectory = Path.Combine(emptyDirectory, category.ToString().ToLowerInvariant());
        foreach (var extension in extensions)
        {
            var file = Path.Combine(categoryDirectory, $"empty{extension}");
            var emptyFile = EmptyFile.Build(file, category);
            items[extension] = emptyFile;
            files[extension] = emptyFile;
        }

#if NET8_0_OR_GREATER
        return items.ToFrozenDictionary();
#else
        return items;
#endif
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

    public static void UseFile(Category category, string file)
    {
        Guard.FileExists(file);
        var extension = Path.GetExtension(file);
        var emptyFile = EmptyFile.Build(file, category);
        switch (category)
        {
            case Category.Archive:
                Init(ref archives, ref archiveExtensions);
                break;
            case Category.Document:
                Init(ref documents, ref documentExtensions);
                break;
            case Category.Image:
                Init(ref images, ref imageExtensions);
                break;
            case Category.Sheet:
                Init(ref sheets, ref sheetExtensions);
                break;
            case Category.Slide:
                Init(ref slides, ref slideExtensions);
                break;
            default:
                throw new($"Unknown category: {category}");
        }

        void Init(ref IReadOnlyDictionary<string, EmptyFile> emptyFiles, ref ReadOnlySet extensions)
        {
            var tempDictionary = new Dictionary<string, EmptyFile>();
            foreach (var (key, value) in emptyFiles)
            {
                tempDictionary[key] = value;
            }
            tempDictionary[extension] = emptyFile;
#if NET8_0_OR_GREATER
            emptyFiles = tempDictionary.ToFrozenDictionary();
#else
            emptyFiles = tempDictionary;
#endif
            var tempSet = new HashSet<string>(extensions)
            {
                extension
            };
#if NET8_0_OR_GREATER
            extensions = tempSet.ToFrozenSet();
#else
            extensions = tempSet;
#endif
        }
    }

    public static IEnumerable<string> AllPaths => files.Values.Select(_ => _.Path);

    // ReSharper disable once UseSymbolAlias
    public static IReadOnlyCollection<string> AllExtensions => files.Keys;

    public static IEnumerable<string> ArchivePaths => archives.Values.Select(_ => _.Path);

    public static ReadOnlySet ArchiveExtensions => archiveExtensions;

    static ReadOnlySet archiveExtensions =
        new HashSet<string>
        {
            ".7z",
            ".7zip",
            ".bz2",
            ".bzip2",
            ".gz",
            ".gzip",
            ".tar",
            ".xz",
            ".zip"
        }
#if NET8_0_OR_GREATER
        .ToFrozenSet()
#endif
        ;

    public static IEnumerable<string> DocumentPaths => documents.Values.Select(_ => _.Path);

    public static ReadOnlySet DocumentExtensions => documentExtensions;

    static ReadOnlySet documentExtensions =
        new HashSet<string>
        {
            ".docx",
            ".odt",
            ".pdf",
            ".rtf"
        }
#if NET8_0_OR_GREATER
        .ToFrozenSet()
#endif
        ;

    public static IEnumerable<string> ImagePaths => images.Values.Select(_ => _.Path);

    public static ReadOnlySet ImageExtensions => imageExtensions;

    static ReadOnlySet imageExtensions =
        new HashSet<string>
        {
            ".avif",
            ".bmp",
            ".dds",
            ".dib",
            ".emf",
            ".exif",
            ".gif",
            ".heic",
            ".heif",
            ".ico",
            ".j2c",
            ".jfif",
            ".jp2",
            ".jpc",
            ".jpe",
            ".jpeg",
            ".jpg",
            ".jxr",
            ".pbm",
            ".pcx",
            ".pgm",
            ".png",
            ".ppm",
            ".rle",
            ".tga",
            ".tif",
            ".tiff",
            ".wdp",
            ".webp",
            ".wmp"
        }
#if NET8_0_OR_GREATER
        .ToFrozenSet()
#endif
        ;

    public static IEnumerable<string> SheetPaths => sheets.Values.Select(_ => _.Path);

    public static ReadOnlySet SheetExtensions => sheetExtensions;

    static ReadOnlySet sheetExtensions =
        new HashSet<string>
            {
            ".ods",
            ".xlsx"
        }
#if NET8_0_OR_GREATER
        .ToFrozenSet()
#endif
        ;

    public static IEnumerable<string> SlidePaths => slides.Values.Select(_ => _.Path);

    public static ReadOnlySet SlideExtensions => slideExtensions;

    static ReadOnlySet slideExtensions =
        new HashSet<string>
        {
            ".odp",
            ".pptx"
        }
#if NET8_0_OR_GREATER
        .ToFrozenSet()
#endif
        ;

    public static bool IsEmptyFile(string path)
    {
        Guard.FileExists(path);
        var extension = Path.GetExtension(path);
        if (!files.TryGetValue(extension, out var emptyFile))
        {
            return false;
        }

        return File.GetLastWriteTime(path) == emptyFile.LastWriteTime;
    }

    public static void CreateFile(string path, bool useEmptyStringForTextFiles = false, Encoding? encoding = null)
    {
        TryCreateDirectory(path);
        var extension = Path.GetExtension(path);
        if (useEmptyStringForTextFiles &&
            FileExtensions.IsTextExtension(extension))
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
        extension = Guard.ValidExtension(extension);
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
            FileExtensions.IsTextExtension(extension))
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
        if (files.TryGetValue(extension, out var emptyFile))
        {
            path = emptyFile.Path;
            return true;
        }

        path = null;
        return false;
    }
}