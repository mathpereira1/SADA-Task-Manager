using Microsoft.EntityFrameworkCore;
using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Application.Interfaces;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;
using SadaTaskManager.Infrastructure.Persistence;

namespace SadaTaskManager.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
	private readonly AppDbContext _context;

	public TaskItemRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task AddAsync(TaskItem task, CancellationToken cancellationToken)
	{
		_context.TaskItems.Add(task);

		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		TaskItem? task = await _context.TaskItems.FirstOrDefaultAsync(task => task.Id == id, cancellationToken);

		return task;
	}

	public async Task<List<TaskItem>> ListAsync(TaskItemFilter filter, CancellationToken cancellationToken)
	{
		IQueryable<TaskItem> query = _context.TaskItems.AsNoTracking();

		if (filter.Status != null)
		{
			query = query.Where(task => task.Status == filter.Status);
		}

		if (filter.DueDateFrom != null)
		{
			query = query.Where(task => task.DueDate >= filter.DueDateFrom);
		}

		if (filter.DueDateTo != null)
		{
			query = query.Where(task => task.DueDate <= filter.DueDateTo);
		}

		query = query
			.OrderBy(task => task.DueDate == null)
			.ThenBy(task => task.DueDate)
			.ThenBy(task => task.Title);

		List<TaskItem> tasks = await query.ToListAsync(cancellationToken);

		return tasks;
	}

	public async Task<int> CountWithoutDueDateAsync(TaskItemStatus? status, CancellationToken cancellationToken)
	{
		IQueryable<TaskItem> query = _context.TaskItems.Where(task => task.DueDate == null);

		if (status != null)
		{
			query = query.Where(task => task.Status == status);
		}

		int count = await query.CountAsync(cancellationToken);

		return count;
	}

	public async Task UpdateAsync(TaskItem task, CancellationToken cancellationToken)
	{
		await _context.SaveChangesAsync(cancellationToken);
	}

	public async Task DeleteAsync(TaskItem task, CancellationToken cancellationToken)
	{
		_context.TaskItems.Remove(task);

		await _context.SaveChangesAsync(cancellationToken);
	}
}
