using System.Collections.Generic;
using System.Linq;
using WebApplication.Validator.Transliterator.Models;

namespace WebApplication.Validator.Transliterator
{
    public class TransliterationService
    {
        private List<TranslitSymbol> TranslitSymbols { get; set; }

        public TransliterationService()
        {
            TranslitSymbols = new List<TranslitSymbol>();
            var gost =
                "а:a,б:b,в:v,г:g,д:d,е:e,ё:jo,ж:zh,з:z,и:i,й:jj,к:k,л:l,м:m,н:n,о:o,п:p,р:r,с:s,т:t,у:u,ф:f,х:kh,ц:c,ч:ch,ш:sh,щ:shh,ъ:\",ы:y,ь:',э:eh,ю:ju,я:ja";
            var iso =
                "а:a,б:b,в:v,г:g,д:d,е:e,ё:yo,ж:zh,з:z,и:i,й:j,к:k,л:l,м:m,н:n,о:o,п:p,р:r,с:s,т:t,у:u,ф:f,х:h,ц:c,ч:ch,ш:sh,щ:shh,ъ:\",ы:y,ь:',э:e,ю:yu,я:ya";

            // Заполняем сопоставления по ГОСТ
            foreach (var item in gost.Split(","))
            {
                var symbols = item.Split(":");
                TranslitSymbols.Add(new TranslitSymbol
                {
                    TransliterationType = TransliterationType.Gost,
                    SymbolRus = symbols[0].ToLower(),
                    SymbolEng = symbols[1].ToLower()
                });
                TranslitSymbols.Add(new TranslitSymbol
                {
                    TransliterationType = TransliterationType.Gost,
                    SymbolRus = symbols[0].ToUpper(),
                    SymbolEng = symbols[1].ToUpper()
                });
            }

            // Заполняем сопоставления по ISO
            foreach (var item in iso.Split(","))
            {
                var symbols = item.Split(":");
                TranslitSymbols.Add(new TranslitSymbol
                {
                    TransliterationType = TransliterationType.Iso,
                    SymbolRus = symbols[0].ToLower(),
                    SymbolEng = symbols[1].ToLower()
                });
                TranslitSymbols.Add(new TranslitSymbol
                {
                    TransliterationType = TransliterationType.Iso,
                    SymbolRus = symbols[0].ToUpper(),
                    SymbolEng = symbols[1].ToUpper()
                });
            }
        }

        public string Transliterate(string source, TransliterationType type)
        {
            var result = string.Empty;

            foreach (var t in source)
            {
                result += TranslitSymbols.FirstOrDefault(x =>
                    x.SymbolRus == t.ToString() && x.TransliterationType == type) == null
                    ? t.ToString()
                    : TranslitSymbols.First(x => x.SymbolRus == t.ToString() && x.TransliterationType == type)
                        .SymbolEng;
            }

            return result;
        }
    }
}