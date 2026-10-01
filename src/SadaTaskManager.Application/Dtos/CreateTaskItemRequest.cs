using System.ComponentModel.DataAnnotations;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

public record CreateTaskItemRequest
{
	/// <summary>Título da tarefa (obrigatório, até 100 caracteres).</summary>
	[Required(ErrorMessage = "O título é obrigatório.")]
	[MaxLength(TaskItemConstraints.TitleMaxLength, ErrorMessage = "O título deve ter no máximo {1} caracteres.")]
	public string Title { get; init; } = string.Empty;

	/// <summary>Descrição opcional (até 1000 caracteres).</summary>
	[MaxLength(TaskItemConstraints.DescriptionMaxLength, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
	public string? Description { get; init; }

	/// <summary>Status inicial: "Pendente" (padrão), "EmProgresso" ou "Concluida".</summary>
	[EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Status inválido.")]
	public TaskItemStatus Status { get; init; } = TaskItemStatus.Pendente;

	/// <summary>Data de vencimento opcional (AAAA-MM-DD). Não pode estar no passado.</summary>
	public DateOnly? DueDate { get; init; }
}
