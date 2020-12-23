using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Npgsql;
using SchemaNameOptionsExtension = Infrastructure.Data.SchemaNameOptionsExtension;

namespace Metabase.Data
{
    // Inspired by
    // [Authentication and authorization for SPAs](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.0)
    // [Customize Identity Model](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.0)
  public sealed class ApplicationDbContext
    : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        static ApplicationDbContext()
        {
            RegisterEnumerations();
        }

        private static void RegisterEnumerations()
        {
          // https://www.npgsql.org/efcore/mapping/enum.html#mapping-your-enum
          NpgsqlConnection.GlobalTypeMapper.MapEnum<ValueObjects.ComponentCategory>();
        }

        private static void CreateEnumerations(ModelBuilder builder)
        {
          // https://www.npgsql.org/efcore/mapping/enum.html#creating-your-database-enum
          // Create enumerations in public schema because that is where `NpgsqlConnection.GlobalTypeMapper.MapEnum` expects them to be by default.
          builder.HasPostgresEnum<ValueObjects.ComponentCategory>("public");
        }

        private static
          EntityTypeBuilder<TEntity>
          ConfigureEntity<TEntity>(
            EntityTypeBuilder<TEntity> builder
            )
          where TEntity : Infrastructure.Data.Entity
        {
          // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
          builder
            .Property(e => e.Id)
            .HasDefaultValueSql("gen_random_uuid()");
          // https://www.npgsql.org/efcore/modeling/concurrency.html#the-postgresql-xmin-system-column
          builder
            .UseXminAsConcurrencyToken();
          return builder;
        }

        private static void ConfigureIdentityEntities(
            ModelBuilder builder
            )
        {
            // https://stackoverflow.com/questions/19902756/asp-net-identity-dbcontext-confusion/35722688#35722688
            builder.Entity<User>().ToTable("user");
            builder.Entity<Role>().ToTable("role");
            builder.Entity<UserClaim>().ToTable("user_claim");
            builder.Entity<UserRole>().ToTable("user_role");
            builder.Entity<UserLogin>().ToTable("user_login");
            builder.Entity<RoleClaim>().ToTable("role_claim");
            builder.Entity<UserToken>().ToTable("user_token");
        }

        private readonly string _schemaName;

        // https://docs.microsoft.com/en-us/ef/core/miscellaneous/nullable-reference-types#dbcontext-and-dbset
        public DbSet<Component> Components { get; private set; } = default!;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options
            )
          : base(options)
        {
            var schemaNameOptions = options.FindExtension<SchemaNameOptionsExtension>();
            _schemaName = schemaNameOptions is null ? "metabase" : schemaNameOptions.SchemaName;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(_schemaName);
            builder.HasPostgresExtension("pgcrypto"); // https://www.npgsql.org/efcore/modeling/generated-properties.html#guiduuid-generation
            CreateEnumerations(builder);
            ConfigureIdentityEntities(builder);
            ConfigureEntity(
                builder.Entity<Component>()
                )
              .ToTable("component");
        }
    }
}
