using SadaTaskManager.Application.Dtos;

namespace SadaTaskManager.Application.Interfaces;

public interface ITaskItemService
{
	Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request, CancellationToken cancellationToken);

	Task<TaskItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

	Task<List<TaskItemResponse>> ListAsync(TaskItemFilter filter, CancellationToken cancellationToken);

	Task<TaskItemResponse?> UpdateAsync(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken);

	Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}
