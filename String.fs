namespace Albelli

    module String = 
        let escapeDoubleQuoteForBash (s : string) : string = 
            s.Replace("\"", "\\\"");