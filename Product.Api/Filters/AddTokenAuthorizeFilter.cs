﻿using System;
using Identity.Api.Infrastructure.Consts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Product.Infrastructure.Exceptions;
using Product.Infrastructure.Models;
using Product.Infrastructure.Utils;

namespace Product.Api.Filters;

public class AppAuthorizeFilter : IAuthorizationFilter 
{
    private readonly IServiceProvider services;
    private readonly IMemoryCache cache;
    
    public AppAuthorizeFilter(
        IServiceProvider services,
        IMemoryCache cache
        )
    {
        this.services = services;
        this.cache = cache;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var authorization = context.HttpContext.Request.Headers.Authorization.ToString();
        if (string.IsNullOrWhiteSpace(authorization))
        {
            return;
        }
        var referenceToken = authorization.Split(" ")[1];
        var token = cache.Get<AuthModel.GetTokenModel>(CacheKeys.Token + referenceToken);
        if (token is null)
            throw new MyApplicationException.Unauthorized();
         
        context.HttpContext.Request.Headers.Authorization = token.AccessToken;
        SetCurrentUser(context.HttpContext);
        
    }
    
    private void SetCurrentUser(HttpContext context)
    {
        using var scope = services.CreateScope();
        var currentUser = scope.ServiceProvider.GetRequiredService<ICurrentUser>();
        currentUser.Set(context.User);
    }
}
