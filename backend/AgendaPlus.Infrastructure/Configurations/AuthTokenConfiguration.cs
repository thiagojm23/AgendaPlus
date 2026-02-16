using AgendaPlus.Application.Interfaces.Services;
using AgendaPlus.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AgendaPlus.Infrastructure.Configurations;

public class AuthTokenConfiguration(ICurrentUserService currentUserService) : IEntityTypeConfiguration<AuthToken>
{
    public void Configure(EntityTypeBuilder<AuthToken> builder)
    {
        builder.ToTable("auth_tokens");

        builder.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        builder.Property(x => x.RefreshToken).HasColumnName("refresh_token");
        builder.Property(x => x.ExpiresAt).HasColumnName("expires_at");
        builder.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(x => x.LoginFailedAttempts).HasColumnName("failed_attempts").HasDefaultValue(0);

        builder.HasOne(t => t.User)
            .WithOne(u => u.Token)
            .HasForeignKey<AuthToken>(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(x => currentUserService.TenantsId.Contains(x.TenantId));
    }
}