using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.UnitTests.Domain;

public class TaskItemTests
{
	[Fact]
	public void Constructor_WithValidData_SetsAllProperties()
	{
		DateOnly dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5);

		TaskItem task = new TaskItem("Estudar C#", "Capítulo 3", TaskItemStatus.Concluida, dueDate);

		Assert.NotEqual(Guid.Empty, task.Id);
		Assert.Equal("Estudar C#", task.Title);
		Assert.Equal("Capítulo 3", task.Description);
		Assert.Equal(TaskItemStatus.Concluida, task.Status);
		Assert.Equal(dueDate, task.DueDate);
	}

	[Fact]
	public void Constructor_WithOnlyTitle_UsesDefaultValues()
	{
		TaskItem task = new TaskItem("Estudar C#");

		Assert.Null(task.Description);
		Assert.Null(task.DueDate);
		Assert.Equal(TaskItemStatus.Pendente, task.Status);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Constructor_WithEmptyTitle_ThrowsDomainException(string title)
	{
		Assert.Throws<DomainException>(() => new TaskItem(title));
	}

	[Fact]
	public void Constructor_WithTitleAtMaxLength_CreatesTask()
	{
		string title = new string('a', TaskItemConstraints.TitleMaxLength);

		TaskItem task = new TaskItem(title);

		Assert.Equal(title, task.Title);
	}

	[Fact]
	public void Constructor_WithTitleAboveMaxLength_ThrowsDomainException()
	{
		string title = new string('a', TaskItemConstraints.TitleMaxLength + 1);

		Assert.Throws<DomainException>(() => new TaskItem(title));
	}

	[Fact]
	public void Constructor_WithDescriptionAboveMaxLength_ThrowsDomainException()
	{
		string description = new string('a', TaskItemConstraints.DescriptionMaxLength + 1);

		Assert.Throws<DomainException>(() => new TaskItem("Estudar C#", description));
	}

	[Fact]
	public void Constructor_WithInvalidStatus_ThrowsDomainException()
	{
		TaskItemStatus invalidStatus = (TaskItemStatus)99;

		Assert.Throws<DomainException>(() => new TaskItem("Estudar C#", null, invalidStatus));
	}

	[Fact]
	public void Constructor_WithPastDueDate_ThrowsDomainException()
	{
		DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);

		Assert.Throws<DomainException>(() => new TaskItem("Estudar C#", null, TaskItemStatus.Pendente, yesterday));
	}

	[Fact]
	public void Constructor_WithTodayDueDate_CreatesTask()
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

		TaskItem task = new TaskItem("Estudar C#", null, TaskItemStatus.Pendente, today);

		Assert.Equal(today, task.DueDate);
	}

	[Fact]
	public void Update_WithValidData_UpdatesAllFieldsAndKeepsId()
	{
		TaskItem task = new TaskItem("Estudar C#");
		Guid originalId = task.Id;
		DateOnly newDueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10);

		task.Update("Estudar EF Core", "Capítulo 5", TaskItemStatus.EmProgresso, newDueDate);

		Assert.Equal(originalId, task.Id);
		Assert.Equal("Estudar EF Core", task.Title);
		Assert.Equal("Capítulo 5", task.Description);
		Assert.Equal(TaskItemStatus.EmProgresso, task.Status);
		Assert.Equal(newDueDate, task.DueDate);
	}

	[Fact]
	public void Update_WithNullDescriptionAndDueDate_ClearsValues()
	{
		DateOnly dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5);
		TaskItem task = new TaskItem("Estudar C#", "Capítulo 3", TaskItemStatus.Pendente, dueDate);

		task.Update("Estudar C#", null, TaskItemStatus.Pendente, null);

		Assert.Null(task.Description);
		Assert.Null(task.DueDate);
	}

	[Theory]
	[InlineData("")]
	[InlineData("   ")]
	public void Update_WithEmptyTitle_ThrowsDomainException(string title)
	{
		TaskItem task = new TaskItem("Estudar C#");

		Assert.Throws<DomainException>(() => task.Update(title, null, TaskItemStatus.Pendente, null));
	}

	[Fact]
	public void Update_WithInvalidData_DoesNotChangeTask()
	{
		DateOnly dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5);
		DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
		TaskItem task = new TaskItem("Estudar C#", "Capítulo 3", TaskItemStatus.Pendente, dueDate);

		Assert.Throws<DomainException>(() => task.Update("Estudar EF Core", "Capítulo 5", TaskItemStatus.Concluida, yesterday));

		Assert.Equal("Estudar C#", task.Title);
		Assert.Equal("Capítulo 3", task.Description);
		Assert.Equal(TaskItemStatus.Pendente, task.Status);
		Assert.Equal(dueDate, task.DueDate);
	}
}
