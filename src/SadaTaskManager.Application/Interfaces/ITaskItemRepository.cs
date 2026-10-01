using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Application.Interfaces;

public interface ITaskItemRepository
{
	Task AddAsync(TaskItem task, CancellationToken cancellationToken);

	Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

	Task<List<TaskItem>> ListAsync(TaskItemFilter filter, CancellationToken cancellationToken);

	Task<int> CountWithoutDueDateAsync(TaskItemStatus? status, CancellationToken cancellationToken);

	Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);

	Task DeleteAsync(TaskItem task, CancellationToken cancellationToken);
}
