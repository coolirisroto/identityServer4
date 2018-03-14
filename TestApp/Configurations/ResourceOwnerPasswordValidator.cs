using System;
using System.Data;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using TestApp.models;
using System.Security.Claims;
using Npgsql;
using System.Diagnostics;

namespace TestApp.Configurations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {

        public static IDbConnection OpenConnection(string connStr)
        {
            var conn = new NpgsqlConnection(connStr);
            conn.Open();
            return conn;
        }

        //this is used to validate your user account with provided grant at /connect/token
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {


            User user = new User();
            var submittedPassword = context.Password;
            using (var conn = OpenConnection("Server=54.172.56.232;database=users;user id=postgres; Password=stagingusr"))
            {
                
                //var hashedPassword = BCrypt.Net.BCrypt.HashPassword(submittedPassword);

                //Debug.WriteLine(submittedPassword);

                //7Debug.WriteLine(hashedPassword);

                //Debug.WriteLine(validPassword);

                user = conn.Query<User>("SELECT * from public.user WHERE USERNAME=@UserName",
                          new { UserName = context.UserName, Pass = context.Password }).SingleOrDefault<User>();
                //user = conn.Query<User>("SELECT * from public.user WHERE USERNAME=@UserName and PASSWORD=@Pass",
                //          new { UserName = context.UserName, Pass = context.Password }).SingleOrDefault<User>();                
            }
            if (user == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Username or password incorrect");
                return Task.FromResult(0);
            }
            bool validPassword = BCrypt.Net.BCrypt.Verify(submittedPassword, user.Password);
            Debug.WriteLine(validPassword);


            context.Result = new GrantValidationResult(user.Id.ToString(), "password");
            return Task.FromResult(0);

        }

    }

}