using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ApiBase.Domain.Translation
{
    public class TranslationCollection<T> : Collection<T> where T : Translation<T>, new()
    {

        public T this[CultureInfo culture]
        {
            get
            {
                var translation = this.FirstOrDefault(x => x.CultureName == culture.Name);
                if (translation == null)
                {
                    translation = new T();
                    translation.CultureName = culture.Name;
                    Add(translation);
                }

                return translation;
            }
            set
            {
                var translation = this.FirstOrDefault(x => x.CultureName == culture.Name);
                if (translation != null)
                {
                    Remove(translation);
                }

                value.CultureName = culture.Name;
                Add(value);
            }
        }

        public T this[string culture]
        {
            get
            {
                var translation = this.FirstOrDefault(x => x.CultureName == culture);
                if (translation == null)
                {
                    translation = new T();
                    translation.CultureName = culture;
                    Add(translation);
                }

                return translation;
            }
            set
            {
                var translation = this.FirstOrDefault(x => x.CultureName == culture);
                if (translation != null)
                {
                    Remove(translation);
                }

                value.CultureName = culture;
                Add(value);
            }
        }

        public bool HasCulture(string culture)
        {
            return this.Any(x => x.CultureName == culture);
        }

        public bool HasCulture(CultureInfo culture)
        {
            return this.Any(x => x.CultureName == culture.Name);
        }

    }
}
