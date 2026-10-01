using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.Domain;

public class TaskItem
{
	public Guid Id { get; private set; }
	public string Title { get; private set; } = string.Empty;
	public string? Description { get; private set; }
	public TaskItemStatus Status { get; private set; }
	public DateOnly? DueDate { get; private set; }

	public TaskItem(string title, string? description = null, TaskItemStatus status = TaskItemStatus.Pending, DateOnly? dueDate = null)
	{
		Validate(title, description, status);
		ValidateDueDateIsNotInThePast(dueDate);

		Id = Guid.CreateVersion7();
		Title = title;
		Description = description;
		Status = status;
		DueDate = dueDate;
	}

	private TaskItem()
	{
	}

	public void Update(string title, string? description, TaskItemStatus status, DateOnly? dueDate)
	{
		Validate(title, description, status);

		if (dueDate != DueDate)
		{
			ValidateDueDateIsNotInThePast(dueDate);
		}

		Title = title;
		Description = description;
		Status = status;
		DueDate = dueDate;
	}

	private static void Validate(string title, string? description, TaskItemStatus status)
	{
		if (string.IsNullOrWhiteSpace(title))
		{
			throw new DomainException("O título é obrigatório.");
		}

		if (title.Length > TaskItemConstraints.TitleMaxLength)
		{
			throw new DomainException($"O título deve ter no máximo {TaskItemConstraints.TitleMaxLength} caracteres.");
		}

		if (description != null && description.Length > TaskItemConstraints.DescriptionMaxLength)
		{
			throw new DomainException($"A descrição deve ter no máximo {TaskItemConstraints.DescriptionMaxLength} caracteres.");
		}

		if (!Enum.IsDefined(status))
		{
			throw new DomainException("Status inválido.");
		}
	}

	private static void ValidateDueDateIsNotInThePast(DateOnly? dueDate)
	{
		if (dueDate == null)
		{
			return;
		}

		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);

		if (dueDate.Value < today)
		{
			throw new DomainException("A data de vencimento não pode estar no passado.");
		}
	}
}
