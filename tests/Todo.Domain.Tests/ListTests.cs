using Todo.Domain.Common;
using Todo.Domain.Lists;

namespace Todo.Domain.Tests;

public sealed class ListTests
{
    private readonly FixedTime _time = new(A.Now);

    [Fact]
    public void DefaultList_IsShared_AndCalledDom()
    {
        var householdId = Guid.NewGuid();
        var by = Guid.NewGuid();

        var list = TodoList.CreateDefault(householdId, by, _time);

        Assert.Equal((householdId, "Dom", ListVisibility.Shared, by, A.Now), (list.HouseholdId, list.Name.Value, list.Visibility, list.OwnerId, list.CreatedAt));
        Assert.False(list.IsArchived);
    }

    [Theory]
    [InlineData(true, ListVisibility.Shared, true)]
    [InlineData(false, ListVisibility.Shared, false)]
    [InlineData(false, ListVisibility.Private, true)]
    [InlineData(true, ListVisibility.Private, true)]
    public void Create_SharedNeedsManage_PrivateAnyMember(bool manages, ListVisibility visibility, bool allowed)
    {
        var actor = new Actor(Guid.NewGuid(), manages);

        var result = TodoList.Create(Guid.NewGuid(), actor, " Zakupy ", visibility, _time);

        Assert.Equal(allowed, result.IsSuccess);
        if (allowed)
        {
            Assert.Equal(("Zakupy", actor.UserId), (result.Value.Name.Value, result.Value.OwnerId));
        }
        else
        {
            Assert.Equal("list.forbidden", result.Code());
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("a\u0007b")]
    [InlineData("12345678901234567890123456789012345678901")]
    public void Create_RejectsBadNames(string? name)
    {
        var result = TodoList.Create(Guid.NewGuid(), Actor.Member(Guid.NewGuid()), name, ListVisibility.Private, _time);

        Assert.Equal("list.name_invalid", result.Code());
    }

    [Theory]
    [InlineData("admin", true, true)]
    [InlineData("member", true, true)]
    [InlineData("other", true, true)]
    [InlineData("admin", false, false)]
    [InlineData("member", false, true)]
    [InlineData("other", false, false)]
    public void CanSee_SharedForAll_PrivateOnlyForOwner_EvenNotForAdmin(string who, bool shared, bool sees)
    {
        var h = new AHousehold(_time);

        Assert.Equal(sees, (shared ? h.Shared : h.Private).CanSee(h.Of(who)));
    }

    [Theory]
    [InlineData("admin", true, null)]
    [InlineData("member", true, "list.forbidden")]
    [InlineData("member", false, null)]
    [InlineData("admin", false, "list.not_found")]
    public void Rename_SharedByAdmins_PrivateByOwner(string who, bool shared, string? error)
    {
        var h = new AHousehold(_time);
        var list = shared ? h.Shared : h.Private;
        _time.Advance(TimeSpan.FromMinutes(5));

        var result = list.Rename(h.Of(who), "Nowa", _time);

        if (error is null)
        {
            Assert.True(result.IsSuccess);
            Assert.Equal(("Nowa", _time.Now), (list.Name.Value, list.UpdatedAt));
        }
        else
        {
            Assert.Equal(error, result.Code());
            Assert.NotEqual("Nowa", list.Name.Value);
        }
    }

    [Fact]
    public void Archive_MakesListReadOnly_RestoreUndoes()
    {
        var h = new AHousehold(_time);

        Assert.True(h.Shared.Archive(h.Admin, _time).IsSuccess);
        Assert.Equal(A.Now, h.Shared.ArchivedAt);
        Assert.Equal("list.archived", h.Shared.Archive(h.Admin, _time).Code());
        Assert.Equal("list.archived", h.Shared.Rename(h.Admin, "X", _time).Code());
        Assert.Equal("list.archived", h.Shared.CanChangeTasks(h.Member).Code());
        Assert.True(h.Shared.CanSee(h.Member));

        Assert.Equal("list.forbidden", h.Shared.Restore(h.Member, _time).Code());
        Assert.True(h.Shared.Restore(h.Admin, _time).IsSuccess);
        Assert.False(h.Shared.IsArchived);
        Assert.Equal("list.not_archived", h.Shared.Restore(h.Admin, _time).Code());
    }

    [Fact]
    public void Delete_SameRulesAsManaging()
    {
        var h = new AHousehold(_time);

        Assert.True(h.Shared.CanDelete(h.Admin).IsSuccess);
        Assert.Equal("list.forbidden", h.Shared.CanDelete(h.Member).Code());
        Assert.True(h.Private.CanDelete(h.Member).IsSuccess);
        Assert.Equal("list.not_found", h.Private.CanDelete(h.Admin).Code());
    }

    [Fact]
    public void T5_PrivateListOfLeaver_IsHidden_ComesBack_OrIsPurgedAfter30Days()
    {
        var h = new AHousehold(_time);

        h.Private.OwnerLeft(_time);
        h.Shared.OwnerLeft(_time);

        Assert.Equal(A.Now, h.Private.OwnerLeftAt);
        Assert.Null(h.Shared.OwnerLeftAt);
        Assert.False(h.Private.CanSee(h.Member));
        Assert.Equal("list.not_found", h.Private.CanChangeTasks(h.Member).Code());
        Assert.False(h.Private.IsDueForPurge(A.Now.AddDays(29)));
        Assert.True(h.Private.IsDueForPurge(A.Now.AddDays(30)));
        Assert.False(h.Shared.IsDueForPurge(A.Now.AddDays(365)));

        h.Private.OwnerReturned(_time);

        Assert.Null(h.Private.OwnerLeftAt);
        Assert.True(h.Private.CanSee(h.Member));
        Assert.False(h.Private.IsDueForPurge(A.Now.AddDays(30)));
    }
}
