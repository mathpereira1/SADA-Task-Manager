using System.ComponentModel.DataAnnotations;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

public record TaskItemFilter : IValidatableObject
{
	[EnumDataType(typeof(TaskItemStatus))]
	public TaskItemStatus? Status { get; init; }

	public DateOnly? DueDateFrom { get; init; }

	public DateOnly? DueDateTo { get; init; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		List<ValidationResult> errors = new List<ValidationResult>();

		if (DueDateFrom != null && DueDateTo != null && DueDateFrom > DueDateTo)
		{
			errors.Add(new ValidationResult(
				"A data inicial do filtro não pode ser maior que a data final.",
				new[] { nameof(DueDateFrom), nameof(DueDateTo) }));
		}

		return errors;
	}
}
