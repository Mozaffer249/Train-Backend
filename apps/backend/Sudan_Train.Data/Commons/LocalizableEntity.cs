using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sudan_Train.Data.Commons
{
    public class LocalizableEntity
    {
        public string NameAr { get; set; } = default!;
        public string NameEn { get; set; } = default!;

        public string GetLocalized()
        {
            CultureInfo culture = Thread.CurrentThread.CurrentCulture;
            if (culture.TwoLetterISOLanguageName.ToLower().Equals("ar"))
                return NameAr;
            return NameEn;
        }
    }
}