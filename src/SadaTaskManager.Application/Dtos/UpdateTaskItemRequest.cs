using System.ComponentModel.DataAnnotations;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

public record UpdateTaskItemRequest
{
	[Required]
	[MaxLength(TaskItemConstraints.TitleMaxLength)]
	public string Title { get; init; } = string.Empty;

	[MaxLength(TaskItemConstraints.DescriptionMaxLength)]
	public string? Description { get; init; }

	[EnumDataType(typeof(TaskItemStatus))]
	public TaskItemStatus Status { get; init; }

	public DateOnly? DueDate { get; init; }
}
