namespace EmptyFiles;

public static class ContentTypes
{
    public static bool IsText(string? mediaType) =>
        IsText(mediaType, out _);

    public static bool IsText(string? mediaType, [NotNullWhen(true)] out string? extension)
    {
        if (TryGetExtension(mediaType, out extension))
        {
            if (FileExtensions.IsTextExtension(extension))
            {
                return true;
            }
        }

        extension = null;
        return false;
    }

    public static bool TryGetExtension(string? mediaType, [NotNullWhen(true)] out string? extension)
    {
        if (mediaType == null)
        {
            extension = null;
            return false;
        }

        if (mappings.TryGetValue(mediaType, out extension))
        {
            return true;
        }

        var mediaTypeSpan = mediaType.AsSpan();
        var plusIndex = mediaTypeSpan.LastIndexOf('+');

        if (plusIndex > -1)
        {
            extension = mediaTypeSpan[(plusIndex + 1)..]
                .ToString();
            return true;
        }

        var slashIndex = mediaTypeSpan.LastIndexOf('/');

        if (slashIndex > -1)
        {
            extension = mediaTypeSpan[(slashIndex + 1)..]
                .ToString();
            return true;
        }

        return false;
    }

    static Dictionary<string, string> mappings = new(StringComparer.OrdinalIgnoreCase)
    {
        //extra
        {
            "application/graphql", "gql"
        },
        {
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "docx"
        },
        {
            "application/vnd.openxmlformats-officedocument.wordprocessingml.template", "dotx"
        },
        {
            "application/vnd.ms-word.document.macroEnabled.12", "docm"
        },
        {
            "application/vnd.ms-word.template.macroEnabled.12", "dotm"
        },
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "xlsx"
        },
        {
            "application/vnd.openxmlformats-officedocument.spreadsheetml.template", "xltx"
        },
        {
            "application/vnd.ms-excel.sheet.macroEnabled.12", "xlsm"
        },
        {
            "application/vnd.ms-excel.template.macroEnabled.12", "xltm"
        },
        {
            "application/vnd.ms-excel.addin.macroEnabled.12", "xlam"
        },
        {
            "application/vnd.ms-excel.sheet.binary.macroEnabled.12", "xlsb"
        },
        {
            "application/vnd.openxmlformats-officedocument.presentationml.presentation", "pptx"
        },
        {
            "application/vnd.openxmlformats-officedocument.presentationml.template", "potx"
        },
        {
            "application/vnd.openxmlformats-officedocument.presentationml.slideshow", "ppsx"
        },
        {
            "application/vnd.ms-powerpoint.addin.macroEnabled.12", "ppam"
        },
        {
            "application/vnd.ms-powerpoint.presentation.macroEnabled.12", "pptm"
        },
        {
            "application/vnd.ms-powerpoint.template.macroEnabled.12", "potm"
        },
        {
            "application/vnd.ms-powerpoint.slideshow.macroEnabled.12", "ppsm"
        },

        {
            "application/fsharp-script", "fsx"
        },
        {
            "application/msaccess", "adp"
        },
        {
            "application/msword", "doc"
        },
        {
            "application/octet-stream", "bin"
        },
        {
            "application/onenote", "one"
        },
        {
            "application/postscript", "eps"
        },
        {
            "application/soap+xml", "xml"
        },
        {
            "application/step", "step"
        },
        {
            "application/vnd.ms-excel", "xls"
        },
        {
            "application/vnd.ms-powerpoint", "ppt"
        },
        {
            "application/vnd.ms-works", "wks"
        },
        {
            "application/vnd.visio", "vsd"
        },
        {
            "application/x-director", "dir"
        },
        {
            "application/x-msdos-program", "exe"
        },
        {
            "application/x-shockwave-flash", "swf"
        },
        {
            "application/x-x509-ca-cert", "cer"
        },
        {
            "application/x-zip-compressed", "zip"
        },
        {
            "application/xhtml+xml", "xhtml"
        },
        {
            "application/xrd+xml", "xml"
        },
        {
            "application/xml", "xml"
        },
        {
            "audio/aac", "aac"
        },
        {
            "audio/aiff", "aiff"
        },
        {
            "audio/basic", "snd"
        },
        {
            "audio/mid", "midi"
        },
        {
            "audio/mp4", "m4a"
        },
        {
            "audio/wav", "wav"
        },
        {
            "audio/x-m4a", "m4a"
        },
        {
            "audio/x-mpegurl", "m3u"
        },
        {
            "audio/x-pn-realaudio", "ra"
        },
        {
            "audio/x-smd", "smd"
        },
        {
            "image/bmp", "bmp"
        },
        {
            "image/heic", ".heic"
        },
        {
            "image/heic-sequence", "heics"
        },
        {
            "image/jpeg", "jpg"
        },
        {
            "image/gif", "gif"
        },
        {
            "image/pict", "pic"
        },
        {
            "image/png", "png"
        },
        {
            "image/x-png", "png"
        },
        {
            "image/svg+xml", "svg"
        },
        {
            "image/tiff", "tiff"
        },
        {
            "image/x-macpaint", "mac"
        },
        {
            "image/x-quicktime", "qti"
        },
        {
            "message/rfc822", "eml"
        },
        {
            "text/calendar", "ics"
        },
        {
            "text/html", "html"
        },
        {
            "text/plain", "txt"
        },
        {
            "text/scriptlet", "wsc"
        },
        {
            "text/xml", "xml"
        },
        {
            "text/csv", "csv"
        },
        {
            "video/3gpp", "3gp"
        },
        {
            "video/3gpp2", "3gp2"
        },
        {
            "video/mp4", "mp4"
        },
        {
            "video/mpeg", "mpg"
        },
        {
            "video/quicktime", "mov"
        },
        {
            "video/vnd.dlna.mpeg-tts", "m2t"
        },
        {
            "video/x-dv", "dv"
        },
        {
            "video/x-la-asf", "lsf"
        },
        {
            "video/x-ms-asf", "asf"
        },
        {
            "x-world/x-vrml", "xof"
        }
    };
}