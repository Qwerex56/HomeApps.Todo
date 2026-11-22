using Todo.Domain.Households;

namespace Todo.Domain.Tests;

public sealed class HouseholdMemberProjectionTests
{
    private static HouseholdMemberProjection New() => new(Guid.NewGuid(), Guid.NewGuid(), "Member", A.Now);

    [Fact]
    public void LeftAndReturned_ToggleActiveAndResetJoinDate()
    {
        var m = New();
        m.Left(A.Now.AddDays(1));
        var afterLeft = m.IsActive;
        m.Returned("Admin", A.Now.AddDays(2));

        Assert.Equal((false, true, "Admin", A.Now.AddDays(2)), (afterLeft, m.IsActive, m.Role, m.JoinedAt));
    }

    [Fact]
    public void Left_KeepsTheFirstLeaveDate()
    {
        var m = New();
        m.Left(A.Now.AddDays(1));
        m.Left(A.Now.AddDays(5));

        Assert.Equal(A.Now.AddDays(1), m.LeftAt);
    }

    [Fact]
    public void Enabling_a_member_who_has_left_keeps_them_inactive()
    {
        var m = New();
        m.Disable();
        m.Left(A.Now.AddDays(1));
        m.Enable();

        Assert.False(m.IsActive);
    }

    [Fact]
    public void DisableAndEnable_ToggleActive()
    {
        var m = New();
        m.Disable();
        var disabled = m.IsActive;
        m.Enable();

        Assert.Equal((false, true), (disabled, m.IsActive));
    }

    [Fact]
    public void ArchiveAndRestore_ToggleActive()
    {
        var m = New();
        m.Archive();
        var archived = m.IsActive;
        m.Restore();

        Assert.Equal((false, true), (archived, m.IsActive));
    }
}
