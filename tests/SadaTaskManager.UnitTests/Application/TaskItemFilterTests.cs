using System.ComponentModel.DataAnnotations;
using SadaTaskManager.Application.Dtos;

namespace SadaTaskManager.UnitTests.Application;

public class TaskItemFilterTests
{
	[Fact]
	public void Validate_WithDueDateFromAfterDueDateTo_ReturnsError()
	{
		TaskItemFilter filter = new TaskItemFilter
		{
			DueDateFrom = new DateOnly(2026, 12, 10),
			DueDateTo = new DateOnly(2026, 12, 1)
		};

		IEnumerable<ValidationResult> errors = filter.Validate(new ValidationContext(filter));

		Assert.Single(errors);
	}

	[Fact]
	public void Validate_WithDueDateFromBeforeDueDateTo_ReturnsNoError()
	{
		TaskItemFilter filter = new TaskItemFilter
		{
			DueDateFrom = new DateOnly(2026, 12, 1),
			DueDateTo = new DateOnly(2026, 12, 10)
		};

		IEnumerable<ValidationResult> errors = filter.Validate(new ValidationContext(filter));

		Assert.Empty(errors);
	}
}
