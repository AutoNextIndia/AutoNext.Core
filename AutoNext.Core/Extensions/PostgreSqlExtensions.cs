using AutoNext.Core.Abstractions;
using AutoNext.Core.Services.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AutoNext.Core.Extensions
{
    public static class PostgreSqlExtensions
    {
        public static IServiceCollection AddPostgreSqlDbContext<TContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionStringName = "DefaultConnection")
            where TContext : DbContext
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException($"Connection string '{connectionStringName}' not found");
            }

            services.AddDbContext<TContext>((serviceProvider, options) =>
            {
                options.UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(TContext).Assembly.FullName);
                    npgsqlOptions.EnableRetryOnFailure(5);
                    npgsqlOptions.CommandTimeout(30);
                });

                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
            });

            services.AddHealthChecks()
                .AddDbContextCheck<TContext>("postgresql", HealthStatus.Degraded);

            return services;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
            services.AddScoped(typeof(IReadOnlyRepository<,>), typeof(EfReadOnlyRepository<,>));
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            return services;
        }
    }
}
