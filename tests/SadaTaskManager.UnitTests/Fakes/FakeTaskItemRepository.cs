using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Application.Interfaces;
using SadaTaskManager.Domain;

namespace SadaTaskManager.UnitTests.Fakes;

public class FakeTaskItemRepository : ITaskItemRepository
{
	public List<TaskItem> Tasks { get; } = new List<TaskItem>();

	public int UpdateCount { get; private set; }

	public TaskItemFilter? LastFilter { get; private set; }

	public int TasksWithoutDueDateCount { get; set; }

	public TaskItemFilter? LastCountFilter { get; private set; }

	public Task AddAsync(TaskItem task, CancellationToken cancellationToken)
	{
		Tasks.Add(task);
		return Task.CompletedTask;
	}

	public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		foreach (TaskItem task in Tasks)
		{
			if (task.Id == id)
			{
				return Task.FromResult<TaskItem?>(task);
			}
		}

		return Task.FromResult<TaskItem?>(null);
	}

	public Task<List<TaskItem>> ListAsync(TaskItemFilter filter, CancellationToken cancellationToken)
	{
		LastFilter = filter;
		return Task.FromResult(new List<TaskItem>(Tasks));
	}

	public Task<int> CountWithoutDueDateAsync(TaskItemFilter filter, CancellationToken cancellationToken)
	{
		LastCountFilter = filter;
		return Task.FromResult(TasksWithoutDueDateCount);
	}

	public Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
	{
		UpdateCount++;
		return Task.CompletedTask;
	}

	public Task DeleteAsync(TaskItem task, CancellationToken cancellationToken)
	{
		Tasks.Remove(task);
		return Task.CompletedTask;
	}
}
