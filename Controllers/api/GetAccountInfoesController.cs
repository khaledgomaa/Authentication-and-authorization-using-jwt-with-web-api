using jwt.Authorization;
using jwt.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace jwt.Controllers.api
{
    public class GetAccountInfoesController : ApiController
    {
        protected ApplicationDbContext dbContext;

        GetAccountInfoesController()
        {
            dbContext = new ApplicationDbContext();
        }

        [jwtAuthorize]
        public IHttpActionResult GetAccountInfoes()
        {
            return Ok(dbContext.AccountInfo.ToList());
        }
    }
}
