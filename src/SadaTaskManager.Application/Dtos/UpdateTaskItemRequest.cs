using System.ComponentModel.DataAnnotations;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

public record UpdateTaskItemRequest
{
	/// <summary>Título da tarefa (obrigatório, até 100 caracteres).</summary>
	[Required(ErrorMessage = "O título é obrigatório.")]
	[MaxLength(TaskItemConstraints.TitleMaxLength, ErrorMessage = "O título deve ter no máximo {1} caracteres.")]
	public string Title { get; init; } = string.Empty;

	/// <summary>Descrição (até 1000 caracteres). Enviar nulo apaga a descrição atual.</summary>
	[MaxLength(TaskItemConstraints.DescriptionMaxLength, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
	public string? Description { get; init; }

	/// <summary>Status (obrigatório): "Pendente", "EmProgresso" ou "Concluida".</summary>
	[Required(ErrorMessage = "O status é obrigatório.")]
	[EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Status inválido.")]
	public TaskItemStatus? Status { get; init; }

	/// <summary>Data de vencimento (AAAA-MM-DD). Enviar nulo apaga a data; uma data nova não pode estar no passado.</summary>
	public DateOnly? DueDate { get; init; }
}
