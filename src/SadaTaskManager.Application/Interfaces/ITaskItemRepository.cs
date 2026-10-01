using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Domain;

namespace SadaTaskManager.Application.Interfaces;

public interface ITaskItemRepository
{
	Task AddAsync(TaskItem task, CancellationToken cancellationToken);

	Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

	Task<List<TaskItem>> ListAsync(TaskItemFilter filter, CancellationToken cancellationToken);

	Task UpdateAsync(TaskItem task, CancellationToken cancellationToken);

	Task DeleteAsync(TaskItem task, CancellationToken cancellationToken);
}
