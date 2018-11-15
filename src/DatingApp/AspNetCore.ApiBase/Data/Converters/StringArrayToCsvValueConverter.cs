using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.ApiBase.Data.Converters
{
    public class StringArrayToCsvValueConverter : ValueConverter<string[], string>
    {
        public StringArrayToCsvValueConverter()
        : base(Csv, Array)
        {
        }

        private static Expression<Func<string[], string>>
            Csv = v => string.Join(',', v);

        private static Expression<Func<string, string[]>>
            Array = x => x.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Select(h => h.Trim()).ToArray();
    }
}


