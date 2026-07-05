namespace EmptyFiles;

public class EmptyFile
{
    public string Path
    {
        get
        {
            EnsureExtracted();
            return path!;
        }
    }

    public DateTime LastWriteTime
    {
        get
        {
            EnsureExtracted();
            return lastWriteTime;
        }
    }

    public Category Category { get; }
    public string Extension { get; }
    internal string ResourceName { get; }

    string? path;
    DateTime lastWriteTime;
    bool extracted;
    readonly bool embedded;
    readonly object sync = new();

    internal static EmptyFile Embedded(Category category, string extension, string resourceName) =>
        new(category, extension, resourceName);

    EmptyFile(Category category, string extension, string resourceName)
    {
        Category = category;
        Extension = extension;
        ResourceName = resourceName;
        embedded = true;
    }

    public EmptyFile(string path, in DateTime lastWriteTime, in Category category)
    {
        Guard.AgainstNullOrEmpty(path);
        this.path = path;
        this.lastWriteTime = lastWriteTime;
        Category = category;
        Extension = System.IO.Path.GetExtension(path);
        ResourceName = $"EmptyFiles.{category.ToString().ToLowerInvariant()}.empty{Extension}";
        extracted = true;
    }

    void EnsureExtracted()
    {
        if (extracted)
        {
            return;
        }

        lock (sync)
        {
            if (extracted)
            {
                return;
            }

            var categoryName = Category.ToString().ToLowerInvariant();
            var directory = System.IO.Path.Combine(AllFiles.ExtractDirectory, categoryName);
            Directory.CreateDirectory(directory);
            var target = System.IO.Path.Combine(directory, $"empty{Extension}");
            using var resource = OpenRead();
            if (!File.Exists(target) ||
                new FileInfo(target).Length != resource.Length)
            {
                ExtractTo(directory, target, resource);
            }

            path = target;
            lastWriteTime = File.GetLastWriteTime(target);
            extracted = true;
        }
    }

    // Extract via a unique temp file then move into place, so that parallel
    // test processes sharing the temp directory cannot observe or write a
    // partially written file. A concurrent process winning the race is treated
    // as success, since every process writes byte-identical content.
    static void ExtractTo(string directory, string target, Stream resource)
    {
        var temp = System.IO.Path.Combine(directory, $"{Guid.NewGuid():N}.tmp");
        try
        {
            using (var fileStream = File.Create(temp))
            {
                resource.CopyTo(fileStream);
            }

            try
            {
                File.Move(temp, target);
            }
            catch (IOException) when (File.Exists(target))
            {
                // Another process published the file first.
            }
        }
        finally
        {
            if (File.Exists(temp))
            {
                File.Delete(temp);
            }
        }
    }

    public Stream OpenRead()
    {
        if (!embedded)
        {
            return File.OpenRead(Path);
        }

        var assembly = typeof(EmptyFile).Assembly;
        return assembly.GetManifestResourceStream(ResourceName) ??
               throw new($"Embedded resource not found: {ResourceName}");
    }
}
