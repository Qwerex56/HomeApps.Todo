namespace Todo.Domain.Common;

/// <summary>
/// Who acts, as the Application layer saw them: their user id and whether their household role reaches the
/// <c>tasks.manage</c> permission from the manifest. Membership and the basic read/write permissions are already
/// checked before the domain runs (<c>HouseholdPermissionBehavior</c>); the domain only adds rules about owners.
/// </summary>
public sealed record Actor(Guid UserId, bool Manages)
{
    public static Actor Member(Guid userId) => new(userId, false);

    public static Actor Manager(Guid userId) => new(userId, true);
}
