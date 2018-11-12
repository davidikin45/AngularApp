using System;
using System.Collections.Generic;
using System.Reflection;

namespace AspNetCore.ApiBase.Reflection
{
    public interface IAssemblyProvider
    {
        IEnumerable<Assembly> GetAssemblies(IEnumerable<string> paths = null, Func<string, Boolean> filter = null);
    }
}
