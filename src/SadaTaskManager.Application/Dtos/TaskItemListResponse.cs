namespace SadaTaskManager.Application.Dtos;

public record TaskItemListResponse(List<TaskItemResponse> Items, int TasksWithoutDueDate);
