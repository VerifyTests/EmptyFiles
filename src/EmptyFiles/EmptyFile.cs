namespace EmptyFiles;

public class EmptyFile
{
    public string Path { get; }
    public DateTime LastWriteTime { get; }
    public Category Category { get; }

    internal static EmptyFile Build(string file, Category category)
    {
        var lastWriteTime = File.GetLastWriteTime(file);
        return new(file, lastWriteTime, category);
    }

    public EmptyFile(string path, in DateTime lastWriteTime, in Category category)
    {
        Guard.AgainstNullOrEmpty(path);
        Path = path;
        LastWriteTime = lastWriteTime;
        Category = category;
    }
}