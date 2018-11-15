using AspNetCore.ApiBase.Mapping;
using AspNetCore.ApiBase.Users;
using DatingApp.Domain;

namespace DatingApp.Core.Dtos
{
    public class RegisterDto : RegisterDtoBase, IMapTo<User>
    {
    }
}
