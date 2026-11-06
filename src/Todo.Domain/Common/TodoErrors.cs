using FluentResults;
using HomeApps.BuildingBlocks.Domain.Errors;
using Todo.Domain.Lists;
using Todo.Domain.Tasks;

namespace Todo.Domain.Common;

public static class TodoErrors
{
    public static Error ListNameInvalid => new AppError("list.name_invalid",
        $"The list name must have 1–{ListName.MaxLength} characters and no control characters.", ErrorKind.Validation);

    /// <summary>Also for someone else's private list: its existence is private too.</summary>
    public static Error ListNotFound => new AppError("list.not_found", "The list does not exist.", ErrorKind.NotFound);

    public static Error ListForbidden => new AppError("list.forbidden",
        "Only household admins change shared lists.", ErrorKind.Forbidden);

    public static Error ListArchived => new AppError("list.archived",
        "The list is archived; restore it before changing it.", ErrorKind.Conflict);

    public static Error ListNotArchived => new AppError("list.not_archived", "The list is not archived.", ErrorKind.Conflict);

    public static Error TaskTitleInvalid => new AppError("task.title_invalid",
        $"The task title must have 1–{TaskTitle.MaxLength} characters and no control characters.", ErrorKind.Validation);

    public static Error TaskNotesInvalid => new AppError("task.notes_invalid",
        $"Notes may have at most {TaskNotes.MaxLength} characters and no control characters except new lines.",
        ErrorKind.Validation);

    public static Error TaskNotFound => new AppError("task.not_found", "The task does not exist.", ErrorKind.NotFound);

    public static Error TaskForbidden => new AppError("task.forbidden",
        "Only the task's author or a household admin deletes a task on a shared list.", ErrorKind.Forbidden);

    public static Error AssigneeInvalid => new AppError("task.assignee_invalid",
        "A task on a private list can only be assigned to the list's owner.", ErrorKind.Validation);
}
