using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Application.Mappings;
using SadaTaskManager.Domain;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.UnitTests.Application;

public class TaskItemMapperTests
{
	[Fact]
	public void ToResponse_CopiesAllFields()
	{
		DateOnly dueDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(3);
		TaskItem task = new TaskItem("Estudar C#", "Capítulo 3", TaskItemStatus.EmProgresso, dueDate);

		TaskItemResponse response = TaskItemMapper.ToResponse(task);

		Assert.Equal(task.Id, response.Id);
		Assert.Equal("Estudar C#", response.Title);
		Assert.Equal("Capítulo 3", response.Description);
		Assert.Equal(TaskItemStatus.EmProgresso, response.Status);
		Assert.Equal(dueDate, response.DueDate);
	}
}
