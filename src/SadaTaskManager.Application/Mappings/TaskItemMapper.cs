using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Domain;

namespace SadaTaskManager.Application.Mappings;

public static class TaskItemMapper
{
	public static TaskItemResponse ToResponse(TaskItem task)
	{
		return new TaskItemResponse(task.Id, task.Title, task.Description, task.Status, task.DueDate);
	}
}
