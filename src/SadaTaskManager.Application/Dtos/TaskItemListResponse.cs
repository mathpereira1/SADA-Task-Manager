namespace SadaTaskManager.Application.Dtos;

/// <summary>Resultado da listagem de tarefas.</summary>
/// <param name="Items">Tarefas encontradas, da que vence primeiro para a última; sem vencimento no fim.</param>
/// <param name="TasksWithoutDueDate">Quantas tarefas sem vencimento ficaram de fora por causa do filtro de data (0 quando não há filtro de data).</param>
public record TaskItemListResponse(List<TaskItemResponse> Items, int TasksWithoutDueDate);
