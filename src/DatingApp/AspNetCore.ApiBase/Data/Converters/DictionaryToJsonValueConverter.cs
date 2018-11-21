using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AspNetCore.ApiBase.Data.Converters
{
    public class DictionaryToJsonValueConverter : ValueConverter<Dictionary<string,string>, string>
    {
        public DictionaryToJsonValueConverter()
        : base(Json, Dictionary)
        {
        }

        private static Expression<Func<Dictionary<string, string>, string>>
            Json = v => JsonConvert.SerializeObject(v);

        private static Expression<Func<string, Dictionary<string, string>>>
            Dictionary = x => JsonConvert.DeserializeObject<Dictionary<string, string>>(x);
    }
}


