﻿namespace EmptyFiles;

public static class FileExtensions
{
    public static string GetExtension(string path) =>
        GetExtension(path.AsSpan())
            .ToString();

    public static CharSpan GetExtension(CharSpan path)
    {
        Guard.AgainstEmpty(path);
        var indexOfPeriod = path.LastIndexOf('.');
        if (indexOfPeriod == -1)
        {
            throw new("Must contain a period");
        }

        var extension = path[(indexOfPeriod+1)..];
        if (extension.Length == 0)
        {
            throw new("No extension");
        }
        return extension;
    }

    public static bool IsTextFile(string path)
    {
        var extension = GetExtension(path);
        return IsTextExtension(extension);
    }

    public static bool IsTextExtension(string extension)
    {
        Guard.AgainstPeriod(extension);
        return textExtensions.Contains(extension);
    }

    public static bool IsTextFile(CharSpan path)
    {
        var extension = GetExtension(path);
        return IsTextExtension(extension);
    }

    static bool IsTextExtension(CharSpan extension) =>
        IsTextExtension(extension.ToString());

    public static void RemoveTextExtension(string extension)
    {
        Guard.AgainstNullOrEmpty(extension);
        Guard.AgainstPeriod(extension);
        textExtensions.Remove(extension);
    }

    public static void RemoveTextExtensions(params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            RemoveTextExtension(extension);
        }
    }

    public static void RemoveTextExtensions(IEnumerable<string> extensions)
    {
        foreach (var extension in extensions)
        {
            RemoveTextExtension(extension);
        }
    }

    public static void AddTextExtension(string extension)
    {
        Guard.AgainstNullOrEmpty(extension);
        Guard.AgainstPeriod(extension);
        textExtensions.Add(extension);
    }

    public static void AddTextExtensions(params string[] extensions)
    {
        foreach (var extension in extensions)
        {
            AddTextExtension(extension);
        }
    }

    public static void AddTextExtensions(IEnumerable<string> extensions)
    {
        foreach (var extension in extensions)
        {
            AddTextExtension(extension);
        }
    }

    //From https://github.com/sindresorhus/text-extensions/blob/master/text-extensions.json
    static HashSet<string> textExtensions =
    [
        "ada",
        "adb",
        "ads",
        "applescript",
        "as",
        "asc",
        "ascii",
        "ascx",
        "asm",
        "asmx",
        "asp",
        "aspx",
        "atom",
        "au3",
        "awk",
        "bas",
        "bash",
        "bashrc",
        "bat",
        "bbcolors",
        "bcp",
        "bdsgroup",
        "bdsproj",
        "bib",
        "bowerrc",
        "c",
        "cbl",
        "cc",
        "cfc",
        "cfg",
        "cfm",
        "cfml",
        "cgi",
        "cjs",
        "clj",
        "cljs",
        "cls",
        "cmake",
        "cmd",
        "cnf",
        "cob",
        "code-snippets",
        "coffee",
        "coffeekup",
        "conf",
        "config",
        "cp",
        "cpp",
        "cpt",
        "cpy",
        "crt",
        "cs",
        "csh",
        "cson",
        "csproj",
        "csr",
        "css",
        "csslintrc",
        "csv",
        "ctl",
        "curlrc",
        "cxx",
        "d",
        "dart",
        "dfm",
        "diff",
        "dof",
        "dpk",
        "dpr",
        "dproj",
        "dtd",
        "eco",
        "editorconfig",
        "ejs",
        "el",
        "elm",
        "emacs",
        "eml",
        "ent",
        "erb",
        "erl",
        "eslintignore",
        "eslintrc",
        "ex",
        "exs",
        "f",
        "f03",
        "f77",
        "f90",
        "f95",
        "fish",
        "for",
        "fpp",
        "frm",
        "fs",
        "fsproj",
        "fsx",
        "ftn",
        "gemrc",
        "gemspec",
        "gitattributes",
        "gitconfig",
        "gitignore",
        "gitkeep",
        "gitmodules",
        "go",
        "gpp",
        "gradle",
        "graphql",
        "groovy",
        "groupproj",
        "grunit",
        "gtmpl",
        "gvimrc",
        "h",
        "haml",
        "hbs",
        "hgignore",
        "hh",
        "hpp",
        "hrl",
        "hs",
        "hta",
        "htaccess",
        "htc",
        "htm",
        "html",
        "htpasswd",
        "hxx",
        "ics",
        "iced",
        "il",
        "iml",
        "inc",
        "inf",
        "info",
        "ini",
        "ino",
        "int",
        "irbrc",
        "itcl",
        "itermcolors",
        "itk",
        "jade",
        "java",
        "jhtm",
        "jhtml",
        "js",
        "jscsrc",
        "jshintignore",
        "jshintrc",
        "json",
        "json5",
        "jsonld",
        "jsp",
        "jspx",
        "jsx",
        "ksh",
        "less",
        "lhs",
        "lisp",
        "log",
        "ls",
        "lsp",
        "lua",
        "m",
        "m4",
        "mak",
        "map",
        "markdown",
        "master",
        "md",
        "mdown",
        "mdwn",
        "mdx",
        "metadata",
        "mht",
        "mhtml",
        "mjs",
        "mk",
        "mkd",
        "mkdn",
        "mkdown",
        "ml",
        "mli",
        "mm",
        "mxml",
        "nfm",
        "nfo",
        "noon",
        "npmignore",
        "npmrc",
        "nuspec",
        "nvmrc",
        "ops",
        "pas",
        "pasm",
        "patch",
        "pbxproj",
        "pch",
        "pem",
        "pg",
        "php",
        "php3",
        "php4",
        "php5",
        "phpt",
        "phtml",
        "pir",
        "pl",
        "pm",
        "pmc",
        "pod",
        "pot",
        "prettierrc",
        "properties",
        "props",
        "pt",
        "pug",
        "purs",
        "py",
        "pyx",
        "r",
        "rake",
        "rb",
        "rbw",
        "rc",
        "rdoc",
        "rdoc_options",
        "resx",
        "rexx",
        "rhtml",
        "rjs",
        "rlib",
        "ron",
        "rs",
        "rss",
        "rst",
        "rtf",
        "rvmrc",
        "rxml",
        "s",
        "sass",
        "scala",
        "scm",
        "scss",
        "seestyle",
        "sh",
        "shtml",
        "sln",
        "sls",
        "spec",
        "sql",
        "sqlite",
        "sqlproj",
        "srt",
        "ss",
        "sss",
        "st",
        "strings",
        "sty",
        "styl",
        "stylus",
        "sub",
        "sublime-build",
        "sublime-commands",
        "sublime-completions",
        "sublime-keymap",
        "sublime-macro",
        "sublime-menu",
        "sublime-project",
        "sublime-settings",
        "sublime-workspace",
        "sv",
        "svc",
        "svg",
        "swift",
        "t",
        "tcl",
        "tcsh",
        "terminal",
        "tex",
        "text",
        "textile",
        "tg",
        "tk",
        "tmLanguage",
        "tmpl",
        "tmTheme",
        "tpl",
        "ts",
        "tsv",
        "tsx",
        "tt",
        "tt2",
        "ttml",
        "twig",
        "txt",
        "v",
        "vb",
        "vbproj",
        "vbs",
        "vcproj",
        "vcxproj",
        "vh",
        "vhd",
        "vhdl",
        "vim",
        "viminfo",
        "vimrc",
        "vm",
        "vue",
        "webapp",
        "webmanifest",
        "wsc",
        "x-php",
        "xaml",
        "xht",
        "xhtml",
        "xml",
        "xs",
        "xsd",
        "xsl",
        "xslt",
        "y",
        "yaml",
        "yml",
        "zsh",
        "zshrc"
    ];

    public static IReadOnlyCollection<string> TextExtensions => textExtensions;
}