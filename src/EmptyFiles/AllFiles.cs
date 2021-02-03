using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace EmptyFiles
{
    public static class AllFiles
    {
        static Dictionary<string, string> aliases = new()
        {
            {"jpeg", "jpg"},
            {"tiff", "tif"},
            {"7zip", "7z"},
            {"gzip", "gz"},
            {"bzip2", "bz2"}
        };

        public static IReadOnlyDictionary<string, EmptyFile> Files
        {
            get => files;
        }

        static ConcurrentDictionary<string, EmptyFile> files = new();

        public static IReadOnlyDictionary<string, EmptyFile> Archives
        {
            get => archives;
        }

        static ConcurrentDictionary<string, EmptyFile> archives = new();

        public static IReadOnlyDictionary<string, EmptyFile> Documents
        {
            get => documents;
        }

        static ConcurrentDictionary<string, EmptyFile> documents = new();

        public static IReadOnlyDictionary<string, EmptyFile> Images
        {
            get => images;
        }

        static ConcurrentDictionary<string, EmptyFile> images = new();

        public static IReadOnlyDictionary<string, EmptyFile> Sheets
        {
            get => sheets;
        }

        static ConcurrentDictionary<string, EmptyFile> sheets = new();

        public static IReadOnlyDictionary<string, EmptyFile> Slides
        {
            get => slides;
        }

        static ConcurrentDictionary<string, EmptyFile> slides = new();

        static AllFiles()
        {
            var emptyFiles = FindEmptyFilesDirectory();
            foreach (var file in Directory.EnumerateFiles(emptyFiles, "*.*", SearchOption.AllDirectories))
            {
                var category = GetCategory(file);
                var emptyFile = EmptyFile.Build(file, category);
                var extension = Extensions.GetExtension(file);
                var categoryFiles = FindDictionaryForCategory(category);

                categoryFiles[extension] = emptyFile;
                files[extension] = emptyFile;
                var alias = aliases.SingleOrDefault(x => x.Value == extension);
                if (alias.Key != null)
                {
                    categoryFiles![alias.Key] = emptyFile;
                    files[alias.Key] = emptyFile;
                }
            }
        }

        static string FindEmptyFilesDirectory()
        {
            var currentDomainEmptyFiles = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmptyFiles");
            if (Directory.Exists(currentDomainEmptyFiles))
            {
                return currentDomainEmptyFiles;
            }

            var codebaseEmptyFiles = Path.Combine(AssemblyLocation.CurrentDirectory, "EmptyFiles");
            if (Directory.Exists(codebaseEmptyFiles))
            {
                return codebaseEmptyFiles;
            }

            throw new Exception($@"Could not find empty files directory. Searched:
 * {currentDomainEmptyFiles}
 * {codebaseEmptyFiles}
");
        }

        static ConcurrentDictionary<string, EmptyFile> FindDictionaryForCategory(Category category)
        {
            return category switch
            {
                Category.Archive => archives,
                Category.Document => documents,
                Category.Image => images,
                Category.Sheet => sheets,
                Category.Slide => slides,
                _ => throw new Exception($"Unknown category: {category}")
            };
        }

        public static void UseFile(Category category, string file)
        {
            Guard.FileExists(file, nameof(file));
            var extension = Extensions.GetExtension(file);
            var emptyFile = EmptyFile.Build(file, category);
            FindDictionaryForCategory(category).AddOrUpdate(extension, _ => emptyFile, (_, _) => emptyFile);
            files.AddOrUpdate(extension, _ => emptyFile, (_, _) => emptyFile);
        }

        static Category GetCategory(string file)
        {
            var directory = Directory.GetParent(file).Name;
            return (Category) Enum.Parse(typeof(Category), directory, true);
        }

        public static IEnumerable<string> AllPaths
        {
            get { return files.Values.Select(x => x.Path); }
        }

        public static IEnumerable<string> AllExtensions
        {
            get { return files.Keys; }
        }

        public static IEnumerable<string> ArchivePaths
        {
            get { return archives.Values.Select(x => x.Path); }
        }

        public static IEnumerable<string> ArchiveExtensions
        {
            get { return archives.Keys; }
        }

        public static IEnumerable<string> DocumentPaths
        {
            get { return documents.Values.Select(x => x.Path); }
        }

        public static IEnumerable<string> DocumentExtensions
        {
            get { return documents.Keys; }
        }

        public static IEnumerable<string> ImagePaths
        {
            get { return images.Values.Select(x => x.Path); }
        }

        public static IEnumerable<string> ImageExtensions
        {
            get { return images.Keys; }
        }

        public static IEnumerable<string> SheetPaths
        {
            get { return sheets.Values.Select(x => x.Path); }
        }

        public static IEnumerable<string> SheetExtensions
        {
            get { return sheets.Keys; }
        }

        public static IEnumerable<string> SlidePaths
        {
            get { return slides.Values.Select(x => x.Path); }
        }

        public static IEnumerable<string> SlideExtensions
        {
            get { return slides.Keys; }
        }

        public static bool IsEmptyFile(string path)
        {
            Guard.FileExists(path, nameof(path));
            var extension = Extensions.GetExtension(path);
            if (!files.TryGetValue(extension, out var emptyFile))
            {
                return false;
            }

            return File.GetLastWriteTime(path) == emptyFile.LastWriteTime;
        }

        public static void CreateFile(string path, bool useEmptyStringForTextFiles = false)
        {
            TryCreateDirectory(path);
            var extension = Extensions.GetExtension(path);
            if (useEmptyStringForTextFiles &&
                Extensions.IsText(extension))
            {
                File.Delete(path);
                //File.CreateText will be UTF8 no bom
                File.CreateText(path).Dispose();
                return;
            }

            File.Copy(GetPathFor(extension), path, true);
        }

        public static string GetPathFor(string extension)
        {
            Guard.AgainstNullOrEmpty(extension, nameof(extension));
            extension = Extensions.GetExtension(extension);
            if (files.TryGetValue(extension, out var emptyFile))
            {
                return emptyFile.Path;
            }

            throw new Exception($"Unknown extension: {extension}");
        }

        public static bool TryCreateFile(string path, bool useEmptyStringForTextFiles = false)
        {
            Guard.AgainstNullOrEmpty(path, nameof(path));
            var extension = Path.GetExtension(path);

            if (useEmptyStringForTextFiles &&
                Extensions.IsText(extension))
            {
                TryCreateDirectory(path);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                File.CreateText(path).Dispose();
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
            extension = Extensions.GetExtension(extension);
            if (files.TryGetValue(extension, out var emptyFile))
            {
                path = emptyFile.Path;
                return true;
            }

            path = null;
            return false;
        }
    }
}