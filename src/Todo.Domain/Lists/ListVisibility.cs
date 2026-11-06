namespace Todo.Domain.Lists;

public enum ListVisibility
{
    /// <summary>Seen by everyone in the household; tasks are added and ticked off by members, the list itself is managed by admins.</summary>
    Shared = 0,

    /// <summary>Seen only by its owner, admins included (D2a: members' data is private).</summary>
    Private = 1,
}
