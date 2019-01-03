using System;

namespace AspNetCore.ApiBase.Authorization
{
    public class ResourceCollectionAttribute : Attribute
    {
        public string CollectionId { get;}
        public ResourceCollectionAttribute(string collectionId)
        {
            CollectionId = collectionId;
        }
    }
}
