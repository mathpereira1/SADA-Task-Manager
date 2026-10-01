using System.ComponentModel.DataAnnotations;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

public record TaskItemFilter : IValidatableObject
{
	/// <summary>Filtra por status: "Pendente", "EmProgresso" ou "Concluida".</summary>
	[EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Status inválido.")]
	public TaskItemStatus? Status { get; init; }

	/// <summary>Vencimento a partir desta data, inclusive (AAAA-MM-DD).</summary>
	public DateOnly? DueDateFrom { get; init; }

	/// <summary>Vencimento até esta data, inclusive (AAAA-MM-DD).</summary>
	public DateOnly? DueDateTo { get; init; }

	/// <summary>Busca no título, sem diferenciar maiúsculas de minúsculas (ex.: "estudar").</summary>
	public string? Search { get; init; }

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
