using System;
using System.Data;
using System.Data.SqlClient;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TestApp.models;
using System.Security.Claims;
using IdentityModel;

namespace TestApp.Configurations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        //this is used to validate your user account with provided grant at /connect/token
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            using (IDbConnection db = new SqlConnection("Server=localhost;database=IdentityServer;user id=postgres; Password=cooliris"))
            {
                var user = db.Query<User>("select * from users where Username=@username and Password=@Pass",
                                          new { UserName = context.UserName, Pass = context.Password }).SingleOrDefault<User>();
                if (user == null)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Username or password incorrect papus");
                    return Task.FromResult(0);
                }
                context.Result = new GrantValidationResult(user.id.ToString(), "password");
                return Task.FromResult(0);
            }
        }

    }
    //build claims array from user data
    public static Claim[] GetUserClaims(User user)
    {
        return new Claim[]
        {
            new Claim("user_id", user.id.ToString() ?? ""),
            //new Claim(JwtClaimTypes.Name, (!string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Lastname)) ? (user.Firstname + " " + user.Lastname) : ""),
            //new Claim(JwtClaimTypes.GivenName, user.Firstname  ?? ""),
            //new Claim(JwtClaimTypes.FamilyName, user.Lastname  ?? ""),
            new Claim(JwtClaimTypes.Email, user.UserName  ?? ""),
            //roles
            //new Claim(JwtClaimTypes.Role, user.Role)
        };
    }
}