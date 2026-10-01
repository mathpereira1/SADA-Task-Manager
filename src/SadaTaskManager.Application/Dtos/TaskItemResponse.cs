using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

public record TaskItemResponse(Guid Id, string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate);
