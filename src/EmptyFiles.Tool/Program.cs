using EmptyFiles;

if (args.Length == 0)
{
    Console.Error.Write("No input");
    return 1;
}

var arg = args[0];

string path;
if (arg.Any(_ => _ is
        '.' or
        '/' or
        '\\'))
{
    path = arg;
}
else
{
    path = $"empty.{arg}";
}

if (AllFiles.TryCreateFile(
        path,
        useEmptyStringForTextFiles: true))
{
    return 0;
}

Console.Error.Write("Unknown extension");
return 1;