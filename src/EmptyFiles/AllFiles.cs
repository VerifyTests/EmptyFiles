namespace EmptyFiles;

public static class AllFiles
{
    public static FrozenDictionary<string, EmptyFile> Files => files;

    static FrozenDictionary<string, EmptyFile> files;

    public static FrozenDictionary<string, EmptyFile> Archives => archives;

    static FrozenDictionary<string, EmptyFile> archives;

    public static FrozenDictionary<string, EmptyFile> Documents => documents;

    static FrozenDictionary<string, EmptyFile> documents;

    public static FrozenDictionary<string, EmptyFile> Images => images;

    static FrozenDictionary<string, EmptyFile> images;

    public static FrozenDictionary<string, EmptyFile> Sheets => sheets;

    static FrozenDictionary<string, EmptyFile> sheets;

    public static FrozenDictionary<string, EmptyFile> Binary => binary;

    static FrozenDictionary<string, EmptyFile> binary;

    public static FrozenDictionary<string, EmptyFile> Slides => slides;

    static FrozenDictionary<string, EmptyFile> slides;

    static AllFiles()
    {
        archives = AddCategory(archiveExtensions, Category.Archive);
        documents = AddCategory(documentExtensions, Category.Document);
        images = AddCategory(imageExtensions, Category.Image);
        sheets = AddCategory(sheetExtensions, Category.Sheet);
        slides = AddCategory(slideExtensions, Category.Slide);
        binary = AddCategory(binaryExtensions, Category.Binary);
        var all = new Dictionary<string, EmptyFile>(StringComparer.OrdinalIgnoreCase);
        Append(archives);
        Append(documents);
        Append(images);
        Append(sheets);
        Append(slides);
        Append(binary);

        files = all.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        void Append(FrozenDictionary<string, EmptyFile> files)
        {
            foreach (var (key, value) in files)
            {
                all[key] = value;
            }
        }
    }

    static FrozenDictionary<string, EmptyFile> AddCategory(FrozenSet<string> extensions, Category category)
    {
        Dictionary<string, EmptyFile> items = new(StringComparer.OrdinalIgnoreCase);
        var categoryName = category
            .ToString()
            .ToLowerInvariant();
        foreach (var extension in extensions)
        {
            var resourceName = $"EmptyFiles.{categoryName}.empty{extension}";
            items[extension] = EmptyFile.Embedded(category, extension, resourceName);
        }

        return items.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    static readonly Lazy<string> extractDirectory = new(
        () =>
        {
            // AssemblyVersion and FileVersion are both pinned (1.0.0), so use the
            // informational version, which tracks the package version, to keep
            // extracted templates isolated per release. Strip any "+sha" suffix
            // appended by source control integration.
            var assembly = typeof(AllFiles).Assembly;
            var informational = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            var version = "unknown";
            if (informational != null)
            {
                var plusIndex = informational.IndexOf('+');
                version = plusIndex == -1 ? informational : informational[..plusIndex];
            }

            return Path.Combine(Path.GetTempPath(), "EmptyFiles", version);
        });

    internal static string ExtractDirectory => extractDirectory.Value;

    static void ExtractAll(string directory)
    {
        ExtractCategory(directory, "archive", archives);
        ExtractCategory(directory, "binary", binary);
        ExtractCategory(directory, "document", documents);
        ExtractCategory(directory, "image", images);
        ExtractCategory(directory, "sheet", sheets);
        ExtractCategory(directory, "slide", slides);
    }

    static void ExtractCategory(string root, string categoryName, FrozenDictionary<string, EmptyFile> items)
    {
        var categoryDirectory = Path.Combine(root, categoryName);
        Directory.CreateDirectory(categoryDirectory);
        foreach (var emptyFile in items.Values)
        {
            using var resource = emptyFile.OpenRead();
            var target = Path.Combine(categoryDirectory, $"empty{emptyFile.Extension}");
            if (File.Exists(target) &&
                new FileInfo(target).Length == resource.Length)
            {
                continue;
            }

            using var fileStream = File.Create(target);
            resource.CopyTo(fileStream);
        }
    }

    public static void WriteAllTo(string directory)
    {
        Guard.AgainstNullOrEmpty(directory);
        Directory.CreateDirectory(directory);
        ExtractAll(directory);
    }

    public static void UseFile(Category category, string file)
    {
        Guard.FileExists(file);
        var extension = Path.GetExtension(file);
        var emptyFile = new EmptyFile(file, File.GetLastWriteTime(file), category);
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
            case Category.Binary:
                Init(ref binary, ref binaryExtensions);
                break;
            default:
                throw new($"Unknown category: {category}");
        }

        var mergedFiles = new Dictionary<string, EmptyFile>(files, StringComparer.OrdinalIgnoreCase)
        {
            [extension] = emptyFile
        };
        files = mergedFiles.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);

        void Init(ref FrozenDictionary<string, EmptyFile> emptyFiles, ref FrozenSet<string> extensions)
        {
            var tempDictionary = new Dictionary<string, EmptyFile>(StringComparer.OrdinalIgnoreCase);
            foreach (var (key, value) in emptyFiles)
            {
                tempDictionary[key] = value;
            }

            tempDictionary[extension] = emptyFile;
            emptyFiles = tempDictionary.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
            var tempSet = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase)
            {
                extension
            };
            extensions = tempSet.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    public static IEnumerable<string> AllPaths => files.Values.Select(_ => _.Path);

    // ReSharper disable once UseSymbolAlias
    public static IReadOnlyCollection<string> AllExtensions => files.Keys;

    public static IEnumerable<string> ArchivePaths => archives.Values.Select(_ => _.Path);

    public static FrozenSet<string> ArchiveExtensions => archiveExtensions;

    static FrozenSet<string> archiveExtensions = FrozenSet.ToFrozenSet(
    [
        ".7z",
        ".7zip",
        ".bz2",
        ".bzip2",
        ".gz",
        ".gzip",
        ".kmz",
        ".nupkg",
        ".tar",
        ".xz",
        ".zip"
    ],
    StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<string> DocumentPaths => documents.Values.Select(_ => _.Path);

    public static FrozenSet<string> DocumentExtensions => documentExtensions;

    static FrozenSet<string> documentExtensions = FrozenSet.ToFrozenSet(
    [
        ".docx",
        ".odt",
        ".pdf",
        ".rtf"
    ],
    StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<string> ImagePaths => images.Values.Select(_ => _.Path);

    public static FrozenSet<string> ImageExtensions => imageExtensions;

    static FrozenSet<string> imageExtensions = FrozenSet.ToFrozenSet(
    [
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
    ],
    StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<string> SheetPaths => sheets.Values.Select(_ => _.Path);

    public static FrozenSet<string> SheetExtensions => sheetExtensions;

    static FrozenSet<string> sheetExtensions = FrozenSet.ToFrozenSet(
    [
        ".ods",
        ".xlsx"
    ],
    StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<string> SlidePaths => slides.Values.Select(_ => _.Path);

    public static FrozenSet<string> SlideExtensions => slideExtensions;

    static FrozenSet<string> slideExtensions = FrozenSet.ToFrozenSet(
    [
        ".odp",
        ".pptx"
    ],
    StringComparer.OrdinalIgnoreCase);

    public static IEnumerable<string> BinaryPaths => binary.Values.Select(_ => _.Path);

    public static FrozenSet<string> BinaryExtensions => binaryExtensions;

    static FrozenSet<string> binaryExtensions = FrozenSet.ToFrozenSet(
    [
        ".bin"
    ],
    StringComparer.OrdinalIgnoreCase);

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

        if (extension.Length == 0)
        {
            return false;
        }

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
        if (extension.Length > 0 &&
            extension[0] != '.')
        {
            extension = $".{extension}";
        }

        if (files.TryGetValue(extension, out var emptyFile))
        {
            path = emptyFile.Path;
            return true;
        }

        path = null;
        return false;
    }
}
