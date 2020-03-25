using System;

namespace EmptyFiles
{
    public class EmptyFile
    {
        public string Path { get; }
        public DateTime LastWriteTime { get; }
        public Category Category { get; }

        public EmptyFile(string path, in DateTime lastWriteTime, in Category category)
        {
            Guard.AgainstNullOrEmpty(path, nameof(path));
            Path = path;
            LastWriteTime = lastWriteTime;
            Category = category;
        }
    }
}