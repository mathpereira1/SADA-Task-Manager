using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Dtos;

/// <summary>Tarefa devolvida pela API.</summary>
/// <param name="Id">Código único da tarefa.</param>
/// <param name="Title">Título.</param>
/// <param name="Description">Descrição, se houver.</param>
/// <param name="Status">"Pendente", "EmProgresso" ou "Concluida".</param>
/// <param name="DueDate">Data de vencimento, se houver.</param>
public record TaskItemResponse(Guid Id, string Title, string? Description, TaskItemStatus Status, DateOnly? DueDate);
