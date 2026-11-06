using HomeApps.BuildingBlocks.Application.Messaging;

namespace Todo.Application.Households.PurgeLeftLists;

/// <summary>T5: deletes private lists (with their tasks) whose owner has been gone for 30 days. Returns how many. Sent by the clean-up job.</summary>
public sealed record PurgeLeftListsCommand : ICommand<int>;
