using AspnetCore.ApiBase.Validation.Errors;
using AspNetCore.ApiBase.Mapping;
using AspNetCore.ApiBase.Users;
using AspNetCore.ApiBase.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace AspNetCore.ApiBase.ApplicationServices
{
    public abstract class ApplicationServiceBase : IApplicationService
    {
        public IMapper Mapper { get; }

        public string ServiceName { get; }
        public IAuthorizationService AuthorizationService { get; }
        public IUserService UserService { get; }
        public IValidationService ValidationService { get; }

        public ApplicationServiceBase(string serviceName, IMapper mapper, IAuthorizationService authorizationService, IUserService userService, IValidationService validationService)
        {
            ServiceName = serviceName;
            Mapper = mapper;
            AuthorizationService = authorizationService;
            UserService = userService;
            ValidationService = validationService;
        }

        public async void AuthorizeOperationAsync(string operation)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(UserService.User, ServiceName + operation);
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedErrors(new GeneralError(String.Format(Messages.UnauthorisedServiceOperation, ServiceName + operation)));
            }
        }

        public async void AuthorizeResourceOperationAsync(string operation, object resource)
        {
            var authorizationResult = await AuthorizationService.AuthorizeAsync(UserService.User, resource, new OperationAuthorizationRequirement() { Name = ServiceName + operation });
            if (!authorizationResult.Succeeded)
            {
                throw new UnauthorizedErrors(new GeneralError(String.Format(Messages.UnauthorisedServiceOperation, ServiceName + operation)));
            }
        }

        public Expression<Func<TDestination, Object>>[] GetMappedIncludes<TSource, TDestination>(Expression<Func<TSource, Object>>[] selectors)
        {
            return AutoMapperHelper.GetMappedIncludes<TSource, TDestination>(Mapper, selectors);
        }

        public Expression<Func<TDestination, TProperty>> GetMappedSelector<TSource, TDestination, TProperty>(Expression<Func<TSource, TProperty>> selector)
        {
            return AutoMapperHelper.GetMappedSelector<TSource, TDestination, TProperty>(Mapper, selector);
        }

        public Func<IQueryable<TDestination>, IOrderedQueryable<TDestination>> GetMappedOrderBy<TSource, TDestination>(Expression<Func<IQueryable<TSource>, IOrderedQueryable<TSource>>> orderBy)
        {
            //return LamdaHelper.GetMappedOrderBy<TSource, TDestination>(Mapper, orderBy);
            if (orderBy == null)
                return null;

            return AutoMapperHelper.GetMappedOrderByCompiled<TSource, TDestination>(Mapper, orderBy);
        }
    }
}
