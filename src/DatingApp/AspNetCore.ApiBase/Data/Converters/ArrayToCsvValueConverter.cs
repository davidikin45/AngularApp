using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq.Expressions;

namespace AspNetCore.ApiBase.Data.Converters
{
    public class ArrayToCsvValueConverter : ValueConverter<string[], string>
    {
        public ArrayToCsvValueConverter()
        : base(Csv, ArrayObject)
        {
        }

        private static Expression<Func<string[], string>>
            Csv = v => string.Join(',', v);

        private static Expression<Func<string, string[]>>
            ArrayObject = x => x.Split(',', StringSplitOptions.RemoveEmptyEntries);
    }
}
