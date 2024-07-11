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

    public static FrozenDictionary<string, EmptyFile> Slides => slides;

    static FrozenDictionary<string, EmptyFile> slides;

    static AllFiles()
    {
        var directory = FindEmptyFilesDirectory();

        archives = AddCategory(archiveExtensions, Category.Archive, directory);
        documents = AddCategory(documentExtensions, Category.Document, directory);
        images = AddCategory(imageExtensions, Category.Image, directory);
        sheets = AddCategory(sheetExtensions, Category.Sheet, directory);
        slides = AddCategory(slideExtensions, Category.Slide, directory);
        var all = new Dictionary<string, EmptyFile>();
        Append(archives);
        Append(documents);
        Append(images);
        Append(sheets);
        Append(slides);

        files = all.ToFrozenDictionary();

        void Append(FrozenDictionary<string, EmptyFile> files)
        {
            foreach (var (key, value) in files)
            {
                all[key] = value;
            }
        }
    }

    static FrozenDictionary<string, EmptyFile> AddCategory(FrozenSet<string> extensions, Category category, string emptyDirectory)
    {
        Dictionary<string, EmptyFile> items = [];
        var categoryDirectory = Path.Combine(
            emptyDirectory,
            category
                .ToString()
                .ToLowerInvariant());
        foreach (var extension in extensions)
        {
            var file = Path.Combine(categoryDirectory, $"empty{extension}");
            items[extension] = EmptyFile.Build(file, category);
        }

        return items.ToFrozenDictionary();
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

        void Init(ref FrozenDictionary<string, EmptyFile> emptyFiles, ref FrozenSet<string> extensions)
        {
            var tempDictionary = new Dictionary<string, EmptyFile>();
            foreach (var (key, value) in emptyFiles)
            {
                tempDictionary[key] = value;
            }

            tempDictionary[extension] = emptyFile;
            emptyFiles = tempDictionary.ToFrozenDictionary();
            var tempSet = new HashSet<string>(extensions)
            {
                extension
            };
            extensions = tempSet.ToFrozenSet();
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
        ".tar",
        ".xz",
        ".zip"
    ]);

    public static IEnumerable<string> DocumentPaths => documents.Values.Select(_ => _.Path);

    public static FrozenSet<string> DocumentExtensions => documentExtensions;

    static FrozenSet<string> documentExtensions = FrozenSet.ToFrozenSet(
    [
        ".docx",
        ".odt",
        ".pdf",
        ".rtf"
    ]);

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
    ]);

    public static IEnumerable<string> SheetPaths => sheets.Values.Select(_ => _.Path);

    public static FrozenSet<string> SheetExtensions => sheetExtensions;

    static FrozenSet<string> sheetExtensions = FrozenSet.ToFrozenSet(
    [
        ".ods",
        ".xlsx"
    ]);

    public static IEnumerable<string> SlidePaths => slides.Values.Select(_ => _.Path);

    public static FrozenSet<string> SlideExtensions => slideExtensions;

    static FrozenSet<string> slideExtensions = FrozenSet.ToFrozenSet(
    [
        ".odp",
        ".pptx"
    ]);

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