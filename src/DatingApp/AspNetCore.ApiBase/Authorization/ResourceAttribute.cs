using System;

namespace AspNetCore.ApiBase.Authorization
{
    public class ResourceAttribute : Attribute
    {
        public string Name { get;}
        public ResourceAttribute(string name)
        {
            Name = name;
        }
    }
}
