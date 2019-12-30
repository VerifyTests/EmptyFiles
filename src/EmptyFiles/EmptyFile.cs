using System;

class EmptyFile
{
    public string Path { get; }
    public DateTime LastWriteTime { get; }
    public EmptyFileCategory Category { get; }

    public EmptyFile(string path, in DateTime lastWriteTime, in EmptyFileCategory category)
    {
        Path = path;
        LastWriteTime = lastWriteTime;
        Category = category;
    }
}