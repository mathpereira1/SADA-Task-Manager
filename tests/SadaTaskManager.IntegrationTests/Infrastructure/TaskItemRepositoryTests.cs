using Microsoft.EntityFrameworkCore;
using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;
using SadaTaskManager.Infrastructure.Persistence;
using SadaTaskManager.Infrastructure.Repositories;

namespace SadaTaskManager.IntegrationTests.Infrastructure;

public class TaskItemRepositoryTests
{
	private readonly string _databaseName = Guid.NewGuid().ToString();

	private AppDbContext CreateContext()
	{
		DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(_databaseName)
			.Options;

		return new AppDbContext(options);
	}

	private async Task AddTasksAsync(List<TaskItem> tasks)
	{
		using AppDbContext context = CreateContext();
		TaskItemRepository repository = new TaskItemRepository(context);

		foreach (TaskItem task in tasks)
		{
			await repository.AddAsync(task, CancellationToken.None);
		}
	}

	[Fact]
	public async Task AddAsync_ThenGetByIdAsync_ReturnsSavedTask()
	{
		TaskItem task = new TaskItem("Estudar C#", "Capítulo 3");
		await AddTasksAsync(new List<TaskItem> { task });

		using AppDbContext context = CreateContext();
		TaskItemRepository repository = new TaskItemRepository(context);
		TaskItem? savedTask = await repository.GetByIdAsync(task.Id, CancellationToken.None);

		Assert.NotNull(savedTask);
		Assert.Equal("Estudar C#", savedTask.Title);
		Assert.Equal("Capítulo 3", savedTask.Description);
	}

	[Fact]
	public async Task UpdateAsync_SavesChanges()
	{
		TaskItem task = new TaskItem("Estudar C#");
		await AddTasksAsync(new List<TaskItem> { task });

		using (AppDbContext context = CreateContext())
		{
			TaskItemRepository repository = new TaskItemRepository(context);
			TaskItem? taskToUpdate = await repository.GetByIdAsync(task.Id, CancellationToken.None);
			Assert.NotNull(taskToUpdate);
			taskToUpdate.Update("Estudar EF Core", null, TaskItemStatus.Completed, null);
			await repository.UpdateAsync(taskToUpdate, CancellationToken.None);
		}

		using AppDbContext newContext = CreateContext();
		TaskItemRepository newRepository = new TaskItemRepository(newContext);
		TaskItem? updatedTask = await newRepository.GetByIdAsync(task.Id, CancellationToken.None);

		Assert.NotNull(updatedTask);
		Assert.Equal("Estudar EF Core", updatedTask.Title);
		Assert.Equal(TaskItemStatus.Completed, updatedTask.Status);
	}

	[Fact]
	public async Task DeleteAsync_RemovesTask()
	{
		TaskItem task = new TaskItem("Estudar C#");
		await AddTasksAsync(new List<TaskItem> { task });

		using (AppDbContext context = CreateContext())
		{
			TaskItemRepository repository = new TaskItemRepository(context);
			TaskItem? taskToDelete = await repository.GetByIdAsync(task.Id, CancellationToken.None);
			Assert.NotNull(taskToDelete);
			await repository.DeleteAsync(taskToDelete, CancellationToken.None);
		}

		using AppDbContext newContext = CreateContext();
		TaskItemRepository newRepository = new TaskItemRepository(newContext);
		TaskItem? deletedTask = await newRepository.GetByIdAsync(task.Id, CancellationToken.None);

		Assert.Null(deletedTask);
	}

	[Fact]
	public async Task ListAsync_WithStatusFilter_ReturnsOnlyThatStatus()
	{
		await AddTasksAsync(new List<TaskItem>
		{
			new TaskItem("Pendente", null, TaskItemStatus.Pending),
			new TaskItem("Concluída", null, TaskItemStatus.Completed)
		});

		using AppDbContext context = CreateContext();
		TaskItemRepository repository = new TaskItemRepository(context);
		List<TaskItem> tasks = await repository.ListAsync(new TaskItemFilter { Status = TaskItemStatus.Completed }, CancellationToken.None);

		Assert.Single(tasks);
		Assert.Equal("Concluída", tasks[0].Title);
	}

	[Fact]
	public async Task ListAsync_WithDueDateRange_IncludesLimitsAndExcludesTasksWithoutDueDate()
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		await AddTasksAsync(new List<TaskItem>
		{
			new TaskItem("No início", null, TaskItemStatus.Pending, today.AddDays(1)),
			new TaskItem("No fim", null, TaskItemStatus.Pending, today.AddDays(5)),
			new TaskItem("Depois do fim", null, TaskItemStatus.Pending, today.AddDays(6)),
			new TaskItem("Sem data")
		});

		using AppDbContext context = CreateContext();
		TaskItemRepository repository = new TaskItemRepository(context);
		TaskItemFilter filter = new TaskItemFilter { DueDateFrom = today.AddDays(1), DueDateTo = today.AddDays(5) };
		List<TaskItem> tasks = await repository.ListAsync(filter, CancellationToken.None);

		Assert.Equal(2, tasks.Count);
		Assert.Equal("No início", tasks[0].Title);
		Assert.Equal("No fim", tasks[1].Title);
	}

	[Fact]
	public async Task ListAsync_OrdersByDueDateWithTasksWithoutDueDateLast()
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		await AddTasksAsync(new List<TaskItem>
		{
			new TaskItem("Sem data"),
			new TaskItem("Vence depois", null, TaskItemStatus.Pending, today.AddDays(10)),
			new TaskItem("Vence antes", null, TaskItemStatus.Pending, today.AddDays(2))
		});

		using AppDbContext context = CreateContext();
		TaskItemRepository repository = new TaskItemRepository(context);
		List<TaskItem> tasks = await repository.ListAsync(new TaskItemFilter(), CancellationToken.None);

		Assert.Equal("Vence antes", tasks[0].Title);
		Assert.Equal("Vence depois", tasks[1].Title);
		Assert.Equal("Sem data", tasks[2].Title);
	}

	[Fact]
	public async Task CountWithoutDueDateAsync_CountsOnlyTasksWithoutDueDate()
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		await AddTasksAsync(new List<TaskItem>
		{
			new TaskItem("Sem data 1"),
			new TaskItem("Sem data 2"),
			new TaskItem("Com data", null, TaskItemStatus.Pending, today.AddDays(3))
		});

		using AppDbContext context = CreateContext();
		TaskItemRepository repository = new TaskItemRepository(context);
		int count = await repository.CountWithoutDueDateAsync(null, CancellationToken.None);

		Assert.Equal(2, count);
	}
}
