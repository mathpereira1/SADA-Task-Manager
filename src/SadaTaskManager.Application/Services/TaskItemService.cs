using Microsoft.Extensions.Logging;
using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Application.Interfaces;
using SadaTaskManager.Application.Mappings;
using SadaTaskManager.Domain;

namespace SadaTaskManager.Application.Services;

public class TaskItemService : ITaskItemService
{
	private readonly ITaskItemRepository _repository;
	private readonly ILogger<TaskItemService> _logger;

	public TaskItemService(ITaskItemRepository repository, ILogger<TaskItemService> logger)
	{
		_repository = repository;
		_logger = logger;
	}

	public async Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request, CancellationToken cancellationToken)
	{
		TaskItem task = new TaskItem(request.Title, request.Description, request.Status, request.DueDate);

		await _repository.AddAsync(task, cancellationToken);

		_logger.LogInformation("Tarefa {TaskId} criada", task.Id);

		return TaskItemMapper.ToResponse(task);
	}

	public async Task<TaskItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		TaskItem? task = await _repository.GetByIdAsync(id, cancellationToken);

		if (task == null)
		{
			return null;
		}

		return TaskItemMapper.ToResponse(task);
	}

	public async Task<TaskItemListResponse> ListAsync(TaskItemFilter filter, CancellationToken cancellationToken)
	{
		List<TaskItem> tasks = await _repository.ListAsync(filter, cancellationToken);

		List<TaskItemResponse> items = tasks.Select(task => TaskItemMapper.ToResponse(task)).ToList();

		int tasksWithoutDueDate = 0;

		if (filter.DueDateFrom != null || filter.DueDateTo != null)
		{
			tasksWithoutDueDate = await _repository.CountWithoutDueDateAsync(filter, cancellationToken);
		}

		return new TaskItemListResponse(items, tasksWithoutDueDate);
	}

	public async Task<TaskItemResponse?> UpdateAsync(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken)
	{
		TaskItem? task = await _repository.GetByIdAsync(id, cancellationToken);

		if (task == null)
		{
			_logger.LogWarning("Tarefa {TaskId} não encontrada para atualização", id);
			return null;
		}

		if (request.Status == null)
		{
			throw new DomainException("O status é obrigatório.");
		}

		task.Update(request.Title, request.Description, request.Status.Value, request.DueDate);

		await _repository.UpdateAsync(task, cancellationToken);

		_logger.LogInformation("Tarefa {TaskId} atualizada", task.Id);

		return TaskItemMapper.ToResponse(task);
	}

	public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
	{
		TaskItem? task = await _repository.GetByIdAsync(id, cancellationToken);

		if (task == null)
		{
			_logger.LogWarning("Tarefa {TaskId} não encontrada para exclusão", id);
			return false;
		}

		await _repository.DeleteAsync(task, cancellationToken);

		_logger.LogInformation("Tarefa {TaskId} excluída", task.Id);

		return true;
	}
}
