# Authentication-and-authorization-using-jwt-with-web-api

this project include only web apis so you can test using many tools like postman

prerequisites : 

1- install the following packages :
  Microsoft.Owin.Security.Jwt 
  Microsoft.AspNet.WebApi.Owin
  Microsoft.Owin.Host.SystemWeb
Note : last version from NuGet Packages

2- Go to App_Start folder and open Startup.Auth.cs and paste the following code and make your own configuration 
  to make sure the request is trusted or leave it until you know how to write your own

public void Configurationjwt(IAppBuilder app)  
        {  
            app.UseJwtBearerAuthentication(  
                new JwtBearerAuthenticationOptions  
                {  
                    AuthenticationMode = AuthenticationMode.Active,  
                    TokenValidationParameters = new TokenValidationParameters()  
                    {  
                        ValidateIssuer = true,  
                        ValidateAudience = true,  
                        ValidateIssuerSigningKey = true,  
                        ValidIssuer = "http://mysite.com", //some string, normally web url,  
                        ValidAudience = "http://mysite.com",  
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("my_secret_key_12345"))  
                    }  
                });  
        }  
        
3 - Go to Startup.cs in the project then call the function there like : 
      public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            Configurationjwt(app);
        }
        
4- Create a class in Model folder called AccountInfo and paste the following code there :
    
    public class AccountInfo
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }

    public class RegisterModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public int PhoneNumber { get; set; }

        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
    
5- Now we need to create table called AccountInfo so go to Model folder then IdentityModels.cs go to 
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<AccountInfo> AccountInfo { get; set; }
    }
  add public DbSet<AccountInfo> AccountInfo { get; set; } there to create a new table with AccountInfo properties as columns
  Note :- I'm using code first approach so to add table to database first we need to enable migration 
  steps to enable migrations :- open package manager console and write enable-migrations 
  next we need to add a migration so keep package manager console and write add-migration writewhatyouneedwithoutspaces
  now last step is to write update-database to save changes 
  
6- Go to IdentityModel.cs then add the following :- 
   public class ApplicationUser : IdentityUser
    {
        [ForeignKey("AccountInfo")]
        public int ClientId { get; set; }
        public AccountInfo AccountInfo { get; set; }
    }
we need to create a ForeignKey for AccountInfo in AspNetUsers table which is created by default in our database you can read about the 
identity in asp.net to understand more details. 

now to update database we need to first add-migration AddForignKeyAccountInfoInAspNetUsers then update-database

7- So now we are ready to create a web api for authentication (registering a new user) we first need to add user detials like
  Email , phonenumber in AccountInfo table then we store the username and password in aspnetusers table for more security
  
8- go to controllers folder and create new folder called api then create a controller web api called Register and paste the follwing code 
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
    
    Now we can Reginster a new user =D let go and see how to authorize this user
    
    
9- We need to create a web api controller called Jwt to manager all the functionality like generate token , get claims and so on 
paste the follwing code in jwt web api controller :- 

    public class JwtController : ApiController
    {
        private string Secret = "ERMN05OPLoDvbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHlSXKP+oi2+bXr6CUYTR==";

        [AllowAnonymous]
        [HttpPost]
        public IHttpActionResult GenerateToken(LoginModel model)
        {
            LoginModel checkUserFoundInDb = Authenticate(model);
            if (checkUserFoundInDb == null)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);

            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, model.UserName)}),
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            return Ok(handler.WriteToken(token));
        }

        private LoginModel Authenticate(LoginModel model)
        {
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            if (!ModelState.IsValid)
                throw new HttpResponseException(HttpStatusCode.Unauthorized);
            ApplicationUser checkUser = UserManager.Find(model.UserName, model.Password);
            if (checkUser != null)
                return model;
            return null;
        }
        public ClaimsPrincipal GetClaims(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string ValidateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetClaims(token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }
    }
    
10 - Now a user login we need to keep the generated token to send it back with any request so the logic like this 
     user login with username and password if he is a valid user so we need to generate a token to send it back as 
     validation for his request so lets create a custom validation attribute.
     Crate a new folder called Authorization then create a new class called JwtAuthorize and paste the follwing code there
     
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
    
    this code is cheking if the incoming request contains header called "Authorization" or not if yes so we need to get the value
    of this header which is should be the token generated before then we need to get the username of this token using getClaims function 
    and this claims can be changed as you want in the generateToken function in web api jwtcontroller so we need to check is a username
    is valid or not in aspnetuser table if yes so its authorized and if not so he can't access the resouces 
    
11- Now to see how it work lets create an api to get users info from AccountInfo so go to api folder and create web api controller called GetAccountInfoes and paste the following code there
    
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
    
 look at [jwtAuthorize] it means when you try to access this method you need to be Authorized.
 
 I'm here for any questions
 
 
 
    
