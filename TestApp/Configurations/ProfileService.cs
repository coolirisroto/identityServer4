using System;
using IdentityServer4.Services;
using System.Threading.Tasks;
using IdentityServer4.Models;
namespace TestApp.Configurations
{
    public class ProfileService: IProfileService
    {
        public Task GetProfileDataAsync(ProfileDataRequestContext context){
            context.IssuedClaims = context.Subject.Claims.ToList();
                getuser
            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }
    }
}
