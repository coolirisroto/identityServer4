﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IdentityServer4.Postgresql.Extensions;
using Marten;
using IdentityServer4.Models;
using IdentityServer4.Postgresql.Mappers;
using IdentityServer4.Postgresql.Entities ;

using Foo = IdentityServer4.Postgresql.Entities;
using IdentityServer4.Validation;
using IdentityServer4.Services;

namespace TestApp
{
    public class Startup
    {
        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        //public IConfiguration Configuration { get; }

        private const string connection = "host=54.165.58.27;database=identity_server;user id=postgres; Password=";
           
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            //services.AddIdentityServer()
            //.AddInMemoryClients(Config.GetClients())
            //        .AddInMemoryIdentityResources(Config.GetIdentityResources())
            //        .AddInMemoryApiResources(Config.GetApiResources())
            //        .AddTestUsers(Config.GetTestUsers())
            //.AddDeveloperSigningCredential();
            services.AddIdentityServer()
                    .AddConfigurationStore(connection)
                    .AddOperationalStore()
                    //.AddTestUsers(Config.GetTestUsers())
                    .AddDeveloperSigningCredential();
            services.AddTransient<IResourceOwnerPasswordValidator, Configurations.ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, Configurations.ProfileService>();


        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitData(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvcWithDefaultRoute();
        }

        private void InitData(IApplicationBuilder app)
        {
            var store = DocumentStore.For(connection);
            store.Advanced.Clean.CompletelyRemoveAll();
            using (var session = store.LightweightSession())
            {
                if (!session.Query<Foo.ApiResource>().Any())
                {
                    var resources = new List<Foo.ApiResource> {
                        new Foo.ApiResource{ Name = "resorts" , Description = "Api Resorts" , DisplayName ="Api Resorts" , Scopes = new List<ApiScope> { new ApiScope { Name = "resorts" , DisplayName ="Api Resorts"  } } },
                        new Foo.ApiResource{ Name = "residences" , Description = "Api Residences" , DisplayName ="Api Residences" , Scopes = new List<ApiScope> { new ApiScope { Name = "residences" , DisplayName ="Api Residences"  } } }

                    };
                    session.StoreObjects(resources);
                }

                if (!session.Query<IdentityServer4.Postgresql.Entities.IdentityResource>().Any())
                {
                    var resources = new List<IdentityServer4.Postgresql.Entities.IdentityResource> {
                        new IdentityResources.OpenId().ToEntity(),
                        new IdentityResources.Profile().ToEntity(),
                        new IdentityResources.Email().ToEntity(),
                        new IdentityResources.Phone().ToEntity()
                    };
                    session.StoreObjects(resources);
                }
                if (!session.Query<IdentityServer4.Postgresql.Entities.Client>().Any())
                {
                    var clients = new List<IdentityServer4.Postgresql.Entities.Client>
                    {
                          new Foo.Client
                            {
                              AllowOfflineAccess = true,
                                Id = "ro.client",
                                ClientId ="ro.client",
                                ClientName = "mvc",
                            AllowedGrantTypes =  new List<ClientGrantType> { new ClientGrantType { GrantType = GrantType.ResourceOwnerPassword } },
                                //AllowedCorsOrigins =  new List<ClientCorsOrigin>  {new ClientCorsOrigin { Origin = "http://localhost" } },
                                RequireClientSecret = true,
                                ClientSecrets = new List<ClientSecret> { new ClientSecret { Value = "secret".Sha256() }  },
                                RequireConsent = false,
                                AllowedScopes = new List<ClientScope>{
                                     new ClientScope { Scope = IdentityServer4.IdentityServerConstants.StandardScopes.OpenId },
                                     new ClientScope { Scope = IdentityServer4.IdentityServerConstants.StandardScopes.Profile },
                                     new ClientScope { Scope ="api1" }
                                },
                                RedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri ="http://localhost:5003/signin-oidc" }}
                            },
                        new Foo.Client
                            {
                              AllowOfflineAccess = true,
                                Id = "addonis",
                                ClientId ="addonis",
                                ClientName = "Addonis",
                            AllowedGrantTypes =  new List<ClientGrantType> { new ClientGrantType { GrantType = GrantType.ClientCredentials } },
                                //AllowedCorsOrigins =  new List<ClientCorsOrigin>  {new ClientCorsOrigin { Origin = "http://localhost" } },
                                RequireClientSecret = true,
                                ClientSecrets = new List<ClientSecret> { new ClientSecret { Value = "secret".Sha256() }  },
                                RequireConsent = false,
                                AllowedScopes = new List<ClientScope>{
                                     new ClientScope { Scope = IdentityServer4.IdentityServerConstants.StandardScopes.OpenId },
                                     new ClientScope { Scope = IdentityServer4.IdentityServerConstants.StandardScopes.Profile },
                                     new ClientScope { Scope ="residences" },
                                     new ClientScope { Scope ="resorts" }
                                },
                                RedirectUris = new List<ClientRedirectUri> { new ClientRedirectUri { RedirectUri ="http://localhost:5003/signin-oidc" }}
                            }

                        };
                    session.StoreObjects(clients);
                }
                session.SaveChanges();
            }
        }
    }
}
