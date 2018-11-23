using AspNetCore.ApiBase.Mapping;
using AspNetCore.ApiBase.Users;
using DatingApp.Tenant.Domain;

namespace DatingApp.Tenant.Core.Dtos
{
    public class RegisterDto : RegisterDtoBase, IMapTo<User>
    {
    }
}
