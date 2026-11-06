using FluentResults;
using FluentValidation;
using HomeApps.BuildingBlocks.Application.Abstractions;
using HomeApps.BuildingBlocks.Application.Messaging;
using HomeApps.Plugins;
using Todo.Application.Common;
using Todo.Domain.Lists;

namespace Todo.Application.Lists;

/// <summary>Shared lists and the caller's own private lists; private lists of others never show, not even to admins.</summary>
public sealed record GetListsQuery(Guid HouseholdId) : IQuery<IReadOnlyList<ListView>>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Read;
}

internal sealed class GetListsHandler(ITodoReader reader, ICurrentUser user) : IQueryHandler<GetListsQuery, IReadOnlyList<ListView>>
{
    public async Task<Result<IReadOnlyList<ListView>>> Handle(GetListsQuery query, CancellationToken cancellationToken) =>
        Result.Ok(await reader.ListsAsync(query.HouseholdId, user.RequiredUserId, cancellationToken));
}

/// <summary>
/// Any member keeps private lists; a shared list needs <see cref="TodoPlugin.Manage"/> (the domain says
/// <c>list.forbidden</c>), so the request itself only asks for <see cref="TodoPlugin.Write"/>.
/// </summary>
public sealed record CreateListCommand(Guid HouseholdId, string Name, ListVisibility Visibility) : ICommand<Guid>, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class CreateListValidator : AbstractValidator<CreateListCommand>
{
    public CreateListValidator()
    {
        RuleFor(c => c.Name).NotNull();
        RuleFor(c => c.Visibility).IsInEnum();
    }
}

internal sealed class CreateListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<CreateListCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = TodoList.Create(command.HouseholdId, actor, command.Name, command.Visibility, time);
        if (list.IsFailed)
        {
            return list.ToResult<Guid>();
        }

        lists.Add(list.Value);
        return list.Value.Id;
    }
}

/// <summary>Shared lists by admins, private ones by their owner (the domain decides).</summary>
public sealed record RenameListCommand(Guid HouseholdId, Guid ListId, string Name) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class RenameListValidator : AbstractValidator<RenameListCommand>
{
    public RenameListValidator() => RuleFor(c => c.Name).NotNull();
}

internal sealed class RenameListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<RenameListCommand>
{
    public async Task<Result> Handle(RenameListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        return list.IsFailed ? list.ToResult() : list.Value.Rename(actor, command.Name, time);
    }
}

public sealed record ArchiveListCommand(Guid HouseholdId, Guid ListId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class ArchiveListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<ArchiveListCommand>
{
    public async Task<Result> Handle(ArchiveListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        return list.IsFailed ? list.ToResult() : list.Value.Archive(actor, time);
    }
}

public sealed record RestoreListCommand(Guid HouseholdId, Guid ListId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class RestoreListHandler(ITodoListRepository lists, Actors actors, TimeProvider time) : ICommandHandler<RestoreListCommand>
{
    public async Task<Result> Handle(RestoreListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        return list.IsFailed ? list.ToResult() : list.Value.Restore(actor, time);
    }
}

/// <summary>A hard delete with all its tasks (they are not account data).</summary>
public sealed record DeleteListCommand(Guid HouseholdId, Guid ListId) : ICommand, IHouseholdRequest
{
    public string Permission => TodoPlugin.Write;
}

internal sealed class DeleteListHandler(ITodoListRepository lists, Actors actors) : ICommandHandler<DeleteListCommand>
{
    public async Task<Result> Handle(DeleteListCommand command, CancellationToken cancellationToken)
    {
        var actor = await actors.GetAsync(command.HouseholdId, cancellationToken);
        var list = await lists.FindVisibleAsync(command.HouseholdId, command.ListId, actor, cancellationToken);
        if (list.IsFailed)
        {
            return list.ToResult();
        }

        var allowed = list.Value.CanDelete(actor);
        if (allowed.IsSuccess)
        {
            lists.Remove(list.Value);
        }

        return allowed;
    }
}
