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
    //U - Update - PUTs
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [AllowAnonymous]
    public abstract class MvcControllerEntityBase<TCreate, TRead, TUpdate, TDelete, IEntityService> : MvcControllerEntityAuthorizeBase<TCreate, TRead, TUpdate, TDelete, IEntityService>
        where TCreate : class
        where TRead : class
        where TUpdate : class
        where TDelete : class
        where IEntityService : IApplicationServiceEntity<TCreate, TRead, TUpdate, TDelete>
    {
        public MvcControllerEntityBase(Boolean admin, IEntityService service, IMapper mapper, IEmailService emailService, AppSettings appSettings, IActionEventsService actionEventsService)
        : base(admin, service, mapper, emailService, appSettings, actionEventsService)
        {
        }

    
    }
}

