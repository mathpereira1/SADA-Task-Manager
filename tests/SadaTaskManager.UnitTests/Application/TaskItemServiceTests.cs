using Microsoft.Extensions.Logging.Abstractions;
using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Application.Services;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;
using SadaTaskManager.UnitTests.Fakes;

namespace SadaTaskManager.UnitTests.Application;

public class TaskItemServiceTests
{
	private readonly FakeTaskItemRepository _repository;
	private readonly TaskItemService _service;

	public TaskItemServiceTests()
	{
		_repository = new FakeTaskItemRepository();
		_service = new TaskItemService(_repository, NullLogger<TaskItemService>.Instance);
	}

	[Fact]
	public async Task CreateAsync_WithValidRequest_SavesTask()
	{
		CreateTaskItemRequest request = new CreateTaskItemRequest { Title = "Estudar C#" };

		TaskItemResponse response = await _service.CreateAsync(request, CancellationToken.None);

		Assert.NotEqual(Guid.Empty, response.Id);
		Assert.Single(_repository.Tasks);
	}

	[Fact]
	public async Task CreateAsync_WithEmptyTitle_DoesNotSaveTask()
	{
		CreateTaskItemRequest request = new CreateTaskItemRequest { Title = "" };

		await Assert.ThrowsAsync<DomainException>(() => _service.CreateAsync(request, CancellationToken.None));

		Assert.Empty(_repository.Tasks);
	}

	[Fact]
	public async Task GetByIdAsync_WithUnknownId_ReturnsNull()
	{
		TaskItemResponse? response = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

		Assert.Null(response);
	}

	[Fact]
	public async Task UpdateAsync_WithUnknownId_ReturnsNull()
	{
		UpdateTaskItemRequest request = new UpdateTaskItemRequest { Title = "Estudar C#" };

		TaskItemResponse? response = await _service.UpdateAsync(Guid.NewGuid(), request, CancellationToken.None);

		Assert.Null(response);
	}

	[Fact]
	public async Task UpdateAsync_WithExistingId_UpdatesTask()
	{
		TaskItem task = new TaskItem("Estudar C#");
		_repository.Tasks.Add(task);
		UpdateTaskItemRequest request = new UpdateTaskItemRequest { Title = "Estudar EF Core", Status = TaskItemStatus.Concluida };

		TaskItemResponse? response = await _service.UpdateAsync(task.Id, request, CancellationToken.None);

		Assert.NotNull(response);
		Assert.Equal("Estudar EF Core", response.Title);
		Assert.Equal(TaskItemStatus.Concluida, response.Status);
		Assert.Equal(1, _repository.UpdateCount);
	}

	[Fact]
	public async Task UpdateAsync_WithoutStatus_ThrowsDomainExceptionAndDoesNotUpdate()
	{
		TaskItem task = new TaskItem("Estudar C#", null, TaskItemStatus.Concluida);
		_repository.Tasks.Add(task);
		UpdateTaskItemRequest request = new UpdateTaskItemRequest { Title = "Sem status" };

		await Assert.ThrowsAsync<DomainException>(() => _service.UpdateAsync(task.Id, request, CancellationToken.None));

		Assert.Equal("Estudar C#", task.Title);
		Assert.Equal(TaskItemStatus.Concluida, task.Status);
		Assert.Equal(0, _repository.UpdateCount);
	}

	[Fact]
	public async Task ListAsync_PassesFilterToRepository()
	{
		_repository.Tasks.Add(new TaskItem("Estudar C#"));
		TaskItemFilter filter = new TaskItemFilter { Status = TaskItemStatus.Pendente };

		TaskItemListResponse response = await _service.ListAsync(filter, CancellationToken.None);

		Assert.Same(filter, _repository.LastFilter);
		Assert.Single(response.Items);
	}

	[Fact]
	public async Task ListAsync_WithDueDateFilter_ReturnsTasksWithoutDueDateCount()
	{
		_repository.TasksWithoutDueDateCount = 2;
		TaskItemFilter filter = new TaskItemFilter { DueDateFrom = DateOnly.FromDateTime(DateTime.UtcNow) };

		TaskItemListResponse response = await _service.ListAsync(filter, CancellationToken.None);

		Assert.Equal(2, response.TasksWithoutDueDate);
	}

	[Fact]
	public async Task ListAsync_WithDueDateFilter_PassesSameFilterToCount()
	{
		TaskItemFilter filter = new TaskItemFilter { Status = TaskItemStatus.Concluida, Search = "estudar", DueDateFrom = DateOnly.FromDateTime(DateTime.UtcNow) };

		await _service.ListAsync(filter, CancellationToken.None);

		Assert.Same(filter, _repository.LastCountFilter);
	}

	[Fact]
	public async Task ListAsync_WithoutDueDateFilter_ReturnsZeroTasksWithoutDueDate()
	{
		_repository.TasksWithoutDueDateCount = 2;
		TaskItemFilter filter = new TaskItemFilter();

		TaskItemListResponse response = await _service.ListAsync(filter, CancellationToken.None);

		Assert.Equal(0, response.TasksWithoutDueDate);
	}

	[Fact]
	public async Task DeleteAsync_WithExistingId_RemovesTask()
	{
		TaskItem task = new TaskItem("Estudar C#");
		_repository.Tasks.Add(task);

		bool deleted = await _service.DeleteAsync(task.Id, CancellationToken.None);

		Assert.True(deleted);
		Assert.Empty(_repository.Tasks);
	}

	[Fact]
	public async Task DeleteAsync_WithUnknownId_ReturnsFalse()
	{
		_repository.Tasks.Add(new TaskItem("Estudar C#"));

		bool deleted = await _service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

		Assert.False(deleted);
		Assert.Single(_repository.Tasks);
	}
}
