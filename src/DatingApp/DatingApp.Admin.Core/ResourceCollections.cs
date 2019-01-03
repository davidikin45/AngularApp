using AspNetCore.ApiBase;

namespace DatingApp.Admin.Core
{
    public class ResourceCollections
    {
        public class Tenants
        {
            public const string Name = "tenants";

            public class Operations
            {
                
            }

            public class Scopes
            {
                public const string Create = Name + "." + ResourceCollectionsCore.CRUD.Operations.Create;
                public const string Read = Name + "." + ResourceCollectionsCore.CRUD.Operations.Read;
                public const string ReadOwner = Name + "." + ResourceCollectionsCore.CRUD.Operations.ReadOwner;
                public const string Update = Name + "." + ResourceCollectionsCore.CRUD.Operations.Update;
                public const string UpdateOwner = Name + "." + ResourceCollectionsCore.CRUD.Operations.UpdateOwner;
                public const string Delete = Name + "." + ResourceCollectionsCore.CRUD.Operations.Delete;
                public const string DeleteOwner = Name + "." + ResourceCollectionsCore.CRUD.Operations.DeleteOwner;
            }
        }
    }
}
