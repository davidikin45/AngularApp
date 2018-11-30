using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ApiBase.Domain.Translation
{
    public abstract class Translation<T> where T : Translation<T>, new()
    {
        public Guid Id { get; set; }

        public string CultureName { get; set; }

        protected Translation()
        {
            Id = Guid.NewGuid();
        }

    }
}
