using AspNetCore.ApiBase;

namespace DatingApp.Admin.Core
{
    public class ResourceOperations
    {
        public class Values
        {
            public const string Name = "values";

            public class Operations
            {
                
            }

            public class Scopes
            {
                public const string Create = Name + "." + ResourceOperationsCore.CRUD.Operations.Create;
                public const string Read = Name + "." + ResourceOperationsCore.CRUD.Operations.Read;
                public const string ReadOwner = Name + "." + ResourceOperationsCore.CRUD.Operations.ReadOwner;
                public const string Update = Name + "." + ResourceOperationsCore.CRUD.Operations.Update;
                public const string UpdateOwner = Name + "." + ResourceOperationsCore.CRUD.Operations.UpdateOwner;
                public const string Delete = Name + "." + ResourceOperationsCore.CRUD.Operations.Delete;
                public const string DeleteOwner = Name + "." + ResourceOperationsCore.CRUD.Operations.DeleteOwner;
            }
        }
    }
}
