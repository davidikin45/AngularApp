﻿using AspNetCore.ApiBase.ApplicationServices;
using AspNetCore.ApiBase.DomainEvents;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Settings;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System;

namespace AspNetCore.ApiBase.Controllers.Mvc
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [AllowAnonymous]
    public abstract class MvcControllerEntityReadOnlyBase<TDto, IEntityService> : MvcControllerEntityReadOnlyAuthorizeBase<TDto, IEntityService>
        where TDto : class
        where IEntityService : IApplicationServiceEntityReadOnly<TDto>
    {

        public MvcControllerEntityReadOnlyBase(Boolean admin, IEntityService service, IMapper mapper, IEmailService emailService, AppSettings appSettings)
        : base(admin, service, mapper, emailService, appSettings)
        {

        }

    }
}

