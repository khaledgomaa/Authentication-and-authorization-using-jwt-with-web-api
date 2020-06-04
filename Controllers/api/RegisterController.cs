using jwt.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace jwt.Controllers.api
{
    public class RegisterController : ApiController
    {
        protected ApplicationDbContext dbContext;
        private ApplicationDbContext db;

        public RegisterController()
        {
            dbContext = new ApplicationDbContext();
            db = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            dbContext.Dispose();
        }


        [HttpPost]
        public IHttpActionResult RegisterUser(RegisterModel userData)
        {
            //check userData validations
            if (!ModelState.IsValid)
                return BadRequest();

            AccountInfo accountInfo = new AccountInfo()
            { Email = userData.Email, PhoneNumber = userData.PhoneNumber };
            dbContext.AccountInfo.Add(accountInfo);
            dbContext.SaveChanges();
            //the id generated from this process will be
            //added tp AspNetusers clientId field as a foreign key and username too 
            var user = new ApplicationUser
            { ClientId = accountInfo.Id, UserName = userData.UserName };

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var check = userManager.Create(user, userData.Password);
            if (!check.Succeeded)
                return BadRequest();
            
            return Ok(userData);
        }
    }
}
