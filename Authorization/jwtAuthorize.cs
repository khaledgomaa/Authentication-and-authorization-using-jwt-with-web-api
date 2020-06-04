using jwt.Controllers.api;
using jwt.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace jwt.Authorization
{
    public class jwtAuthorize : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext httpContext)
        {
            JwtController jwtactions = new JwtController();
            var request = httpContext.Request;
            var headers = request.Headers;
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (headers.Contains("Authorization"))
            {
                string token = headers.GetValues("Authorization").FirstOrDefault();
                if (token != null)
                {
                    string tokenUsername = jwtactions.ValidateToken(token);

                    if (tokenUsername == null)
                        throw new HttpResponseException(HttpStatusCode.Unauthorized);
                    else if (UserManager.FindByName(tokenUsername) == null)
                    {
                        throw new HttpResponseException(HttpStatusCode.Unauthorized);
                    }
                    else
                    {
                        return true;
                    }
                }
                return false;
            }
            return false;
        }
    }
}