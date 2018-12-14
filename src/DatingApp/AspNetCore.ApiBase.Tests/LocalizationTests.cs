using AspNetCore.ApiBase.Localization;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace AspNetCore.ApiBase.Tests
{
    public class LocalizationTests
    {
        [Fact]
        public void Locales()
        {
            var locales = CultureHelper.SpecificCultureList();
        }

        [Fact]
        public void NeutralLocales()
        {
            var locales = CultureHelper.NeutralCultureList();
        }

        [Fact]
        public void Currencies()
        {
            var currencies = CurrencyHelper.CurrencyList();
        }
    }
}
