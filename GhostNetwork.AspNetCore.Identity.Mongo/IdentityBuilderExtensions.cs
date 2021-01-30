using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace GhostNetwork.AspNetCore.Identity.Mongo
{
    public static class IdentityBuilderExtensions
    {
        public static IdentityBuilder AddMongoDbStores<TContext>(this IdentityBuilder identityBuilder, MongoOptions options)
            where TContext : IdentityDbContext<string>
        {
            identityBuilder.Services
                .AddScoped(provider => new MongoClient(options.ClientSettings).GetDatabase(options.DbName));

            identityBuilder.Services
                .AddScoped(typeof(IdentityDbContext<string>), typeof(TContext));
            //.AddScoped<IdentityDbContext<string>>();

            // custom model store
            var userStoreType = typeof(UserStore<,,,,,,>)
                .MakeGenericType(identityBuilder.UserType, typeof(IdentityRole<string>), typeof(string),
                    typeof(IdentityUserRole<string>), typeof(IdentityUserClaim<string>), typeof(IdentityUserLogin<string>), typeof(IdentityUserToken<string>));

            typeof(IdentityBuilder)
                .GetMethod("AddUserStore")!
                .MakeGenericMethod(userStoreType)
                .Invoke(identityBuilder, new object[] { });

            var roleStoreType = typeof(RoleStore<,,,>)
                .MakeGenericType(identityBuilder.RoleType, typeof(string),
                    typeof(IdentityUserRole<string>), typeof(IdentityRoleClaim<string>));

            typeof(IdentityBuilder)
                .GetMethod("AddRoleStore")!
                .MakeGenericMethod(roleStoreType)
                .Invoke(identityBuilder, new object[] { });

            return identityBuilder;
        }
    }
}