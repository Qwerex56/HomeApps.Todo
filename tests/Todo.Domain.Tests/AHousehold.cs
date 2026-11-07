using Todo.Domain.Common;
using Todo.Domain.Lists;

namespace Todo.Domain.Tests;

/// <summary>A household with an admin, two members, its default shared list and a private list of the first member.</summary>
internal sealed class AHousehold
{
    public AHousehold(FixedTime time)
    {
        Time = time;
        Shared = TodoList.CreateDefault(Id, Admin.UserId, time);
        Private = TodoList.Create(Id, Member, "Moje", ListVisibility.Private, time).Value;
    }

    public FixedTime Time { get; }

    public Guid Id { get; } = Guid.NewGuid();

    public Actor Admin { get; } = Actor.Manager(Guid.NewGuid());

    public Actor Member { get; } = Actor.Member(Guid.NewGuid());

    public Actor Other { get; } = Actor.Member(Guid.NewGuid());

    public TodoList Shared { get; }

    public TodoList Private { get; }

    public Actor Of(string who) => who switch
    {
        "admin" => Admin,
        "member" => Member,
        "other" => Other,
        _ => throw new ArgumentOutOfRangeException(nameof(who)),
    };
}
