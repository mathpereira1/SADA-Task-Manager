using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SadaTaskManager.Domain;

namespace SadaTaskManager.Infrastructure.Persistence.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
	public void Configure(EntityTypeBuilder<TaskItem> builder)
	{
		builder.HasKey(task => task.Id);

		builder.Property(task => task.Id)
			.ValueGeneratedNever();

		builder.Property(task => task.Title)
			.IsRequired()
			.HasMaxLength(TaskItemConstraints.TitleMaxLength);

		builder.Property(task => task.Description)
			.HasMaxLength(TaskItemConstraints.DescriptionMaxLength);
	}
}
