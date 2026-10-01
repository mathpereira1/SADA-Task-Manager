using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Domain.Enums;

namespace SadaTaskManager.IntegrationTests.Api;

public class TasksEndpointsTests : IDisposable
{
	private readonly CustomWebApplicationFactory _factory;
	private readonly HttpClient _client;
	private readonly JsonSerializerOptions _jsonOptions;

	public TasksEndpointsTests()
	{
		_factory = new CustomWebApplicationFactory();
		_client = _factory.CreateClient();
		_jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
		_jsonOptions.Converters.Add(new JsonStringEnumConverter());
	}

	public void Dispose()
	{
		_client.Dispose();
		_factory.Dispose();
	}

	private async Task<TaskItemResponse> CreateTaskAsync(CreateTaskItemRequest request)
	{
		HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tasks", request, _jsonOptions);
		response.EnsureSuccessStatusCode();

		TaskItemResponse? task = await response.Content.ReadFromJsonAsync<TaskItemResponse>(_jsonOptions);
		Assert.NotNull(task);

		return task;
	}

	[Fact]
	public async Task Create_WithValidRequest_Returns201WithLocation()
	{
		CreateTaskItemRequest request = new CreateTaskItemRequest { Title = "Estudar C#", Status = TaskItemStatus.EmProgresso };

		HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tasks", request, _jsonOptions);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.NotNull(response.Headers.Location);

		string body = await response.Content.ReadAsStringAsync();
		Assert.Contains("\"status\":\"EmProgresso\"", body);
	}

	[Fact]
	public async Task Create_WithEmptyTitle_Returns400WithMessage()
	{
		CreateTaskItemRequest request = new CreateTaskItemRequest { Title = "" };

		HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tasks", request, _jsonOptions);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

		string body = await response.Content.ReadAsStringAsync();
		Assert.Contains("O título é obrigatório.", body);
	}

	[Fact]
	public async Task Create_WithPastDueDate_Returns400FromDomain()
	{
		DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
		CreateTaskItemRequest request = new CreateTaskItemRequest { Title = "Atrasada", DueDate = yesterday };

		HttpResponseMessage response = await _client.PostAsJsonAsync("/api/tasks", request, _jsonOptions);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

		string body = await response.Content.ReadAsStringAsync();
		Assert.Contains("A data de vencimento não pode estar no passado.", body);
	}

	[Fact]
	public async Task GetById_WithUnknownId_Returns404()
	{
		HttpResponseMessage response = await _client.GetAsync($"/api/tasks/{Guid.NewGuid()}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task List_WithStatusFilter_ReturnsOnlyThatStatus()
	{
		await CreateTaskAsync(new CreateTaskItemRequest { Title = "Pendente" });
		await CreateTaskAsync(new CreateTaskItemRequest { Title = "Em progresso", Status = TaskItemStatus.EmProgresso });

		TaskItemListResponse? response = await _client.GetFromJsonAsync<TaskItemListResponse>("/api/tasks?status=EmProgresso", _jsonOptions);

		Assert.NotNull(response);
		Assert.Single(response.Items);
		Assert.Equal("Em progresso", response.Items[0].Title);
	}

	[Fact]
	public async Task List_WithDueDateFilter_ReturnsTasksWithoutDueDateCount()
	{
		DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
		await CreateTaskAsync(new CreateTaskItemRequest { Title = "Com data", DueDate = today.AddDays(3) });
		await CreateTaskAsync(new CreateTaskItemRequest { Title = "Sem data" });

		TaskItemListResponse? response = await _client.GetFromJsonAsync<TaskItemListResponse>($"/api/tasks?dueDateFrom={today:yyyy-MM-dd}", _jsonOptions);

		Assert.NotNull(response);
		Assert.Single(response.Items);
		Assert.Equal(1, response.TasksWithoutDueDate);
	}

	[Fact]
	public async Task List_WithDueDateFromAfterDueDateTo_Returns400WithMessage()
	{
		HttpResponseMessage response = await _client.GetAsync("/api/tasks?dueDateFrom=2030-12-10&dueDateTo=2030-12-01");

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

		string body = await response.Content.ReadAsStringAsync();
		Assert.Contains("A data inicial do filtro não pode ser maior que a data final.", body);
	}

	[Fact]
	public async Task Update_ThenGetById_ReturnsUpdatedTask()
	{
		TaskItemResponse task = await CreateTaskAsync(new CreateTaskItemRequest { Title = "Estudar C#" });
		UpdateTaskItemRequest request = new UpdateTaskItemRequest { Title = "Estudar EF Core", Status = TaskItemStatus.Concluida };

		HttpResponseMessage updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{task.Id}", request, _jsonOptions);
		TaskItemResponse? updatedTask = await _client.GetFromJsonAsync<TaskItemResponse>($"/api/tasks/{task.Id}", _jsonOptions);

		Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
		Assert.NotNull(updatedTask);
		Assert.Equal("Estudar EF Core", updatedTask.Title);
		Assert.Equal(TaskItemStatus.Concluida, updatedTask.Status);
	}

	[Fact]
	public async Task Update_WithoutStatus_Returns400AndKeepsTask()
	{
		TaskItemResponse task = await CreateTaskAsync(new CreateTaskItemRequest { Title = "Estudar C#", Status = TaskItemStatus.Concluida });
		object request = new { title = "Sem status" };

		HttpResponseMessage updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{task.Id}", request, _jsonOptions);
		TaskItemResponse? unchangedTask = await _client.GetFromJsonAsync<TaskItemResponse>($"/api/tasks/{task.Id}", _jsonOptions);

		Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

		ValidationProblemDetails? problem = await updateResponse.Content.ReadFromJsonAsync<ValidationProblemDetails>(_jsonOptions);
		Assert.NotNull(problem);
		Assert.True(problem.Errors.ContainsKey("Status"));
		Assert.Contains("O status é obrigatório.", problem.Errors["Status"]);

		Assert.NotNull(unchangedTask);
		Assert.Equal("Estudar C#", unchangedTask.Title);
		Assert.Equal(TaskItemStatus.Concluida, unchangedTask.Status);
	}

	[Fact]
	public async Task Update_WithPastDueDate_Returns400AndKeepsTask()
	{
		TaskItemResponse task = await CreateTaskAsync(new CreateTaskItemRequest { Title = "Estudar C#" });
		DateOnly yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
		UpdateTaskItemRequest request = new UpdateTaskItemRequest { Title = "Atrasada", Status = TaskItemStatus.Pendente, DueDate = yesterday };

		HttpResponseMessage updateResponse = await _client.PutAsJsonAsync($"/api/tasks/{task.Id}", request, _jsonOptions);
		TaskItemResponse? unchangedTask = await _client.GetFromJsonAsync<TaskItemResponse>($"/api/tasks/{task.Id}", _jsonOptions);

		Assert.Equal(HttpStatusCode.BadRequest, updateResponse.StatusCode);

		string body = await updateResponse.Content.ReadAsStringAsync();
		Assert.Contains("A data de vencimento não pode estar no passado.", body);

		Assert.NotNull(unchangedTask);
		Assert.Equal("Estudar C#", unchangedTask.Title);
		Assert.Null(unchangedTask.DueDate);
	}

	[Fact]
	public async Task Update_WithUnknownId_Returns404()
	{
		UpdateTaskItemRequest request = new UpdateTaskItemRequest { Title = "Estudar C#", Status = TaskItemStatus.Pendente };

		HttpResponseMessage response = await _client.PutAsJsonAsync($"/api/tasks/{Guid.NewGuid()}", request, _jsonOptions);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Delete_ThenGetById_Returns404()
	{
		TaskItemResponse task = await CreateTaskAsync(new CreateTaskItemRequest { Title = "Estudar C#" });

		HttpResponseMessage deleteResponse = await _client.DeleteAsync($"/api/tasks/{task.Id}");
		HttpResponseMessage getResponse = await _client.GetAsync($"/api/tasks/{task.Id}");

		Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
		Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
	}

	[Fact]
	public async Task Delete_WithUnknownId_Returns404()
	{
		HttpResponseMessage response = await _client.DeleteAsync($"/api/tasks/{Guid.NewGuid()}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task OpenApiDocument_ShowsStatusAsText()
	{
		string json = await _client.GetStringAsync("/openapi/v1.json");

		using JsonDocument document = JsonDocument.Parse(json);
		JsonElement statusSchema = document.RootElement
			.GetProperty("components")
			.GetProperty("schemas")
			.GetProperty("TaskItemStatus");

		bool hasEnumValues = statusSchema.TryGetProperty("enum", out JsonElement enumValues);

		Assert.True(hasEnumValues);
		Assert.Contains("EmProgresso", enumValues.ToString());
	}
}
