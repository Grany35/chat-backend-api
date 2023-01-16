using System;
using Core.Utilities.Hashing;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Concrete.DataExtensions
{
	public static class DataCollectionExtensions
	{
        public static IServiceCollection AddInitialUser(this IServiceCollection services)
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<SimpleContextDb>();

                if (!dataContext.OperationClaims.Any())
                {
                    OperationClaim operationClaim = new OperationClaim
                    {
                        Name = "Admin"
                    };
                    if (!dataContext.Users.Any())
                    {
                        byte[] passwordHash, paswordSalt;
                        HashingHelper.CreatePassword("Asd159123!", out passwordHash, out paswordSalt);

                        User user = new User()
                        {
                            Email = "ogun.ergin35@gmail.com",
                            Name = "Admin",
                            PasswordHash = passwordHash,
                            PasswordSalt = paswordSalt,
                        };

                        var userOperationClaim = new UserOperationClaim
                        {
                            OperationClaim = operationClaim,
                            User = user,
                        };

                        dataContext.UserOperationClaims.Add(userOperationClaim);
                        dataContext.SaveChanges();
                    }
                }
            }
            return services;
        }
    }
}

