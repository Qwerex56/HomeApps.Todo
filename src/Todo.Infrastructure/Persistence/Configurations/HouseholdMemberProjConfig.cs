using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Todo.Domain.Households;

namespace Todo.Infrastructure.Persistence.Configurations;

internal sealed class HouseholdMemberProjConfig : IEntityTypeConfiguration<HouseholdMemberProjection>
{
    public void Configure(EntityTypeBuilder<HouseholdMemberProjection> builder)
    {
        builder.ToTable("HouseholdMemberProjections");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();
        builder.Property(m => m.Role).HasMaxLength(32);
        builder.Ignore(m => m.IsActive);

        builder.HasIndex(m => new { m.HouseholdId, m.UserId }).IsUnique().HasDatabaseName("ux_household_member_proj_household_user");
    }
}
