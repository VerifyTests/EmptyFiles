using System;

public class EmptyFile
{
    public string Path { get; }
    public DateTime LastWriteTime { get; }
    public EmptyFileCategory Category { get; }

    public EmptyFile(string path, in DateTime lastWriteTime, in EmptyFileCategory category)
    {
        Guard.AgainstNullOrEmpty(path, nameof(path));
        Path = path;
        LastWriteTime = lastWriteTime;
        Category = category;
    }
}