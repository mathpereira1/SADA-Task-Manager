using Microsoft.AspNetCore.Mvc;
using SadaTaskManager.Application.Dtos;
using SadaTaskManager.Application.Interfaces;

namespace SadaTaskManager.Api.Controllers;

[ApiController]
[Route("api/tasks")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
	private readonly ITaskItemService _service;

	public TasksController(ITaskItemService service)
	{
		_service = service;
	}

	/// <summary>
	/// Cria uma nova tarefa.
	/// </summary>
	/// <param name="request">Dados da tarefa. Só o título é obrigatório; o status padrão é "Pendente".</param>
	/// <param name="cancellationToken">Cancelamento da requisição.</param>
	/// <response code="201">Tarefa criada. O header Location aponta para ela.</response>
	/// <response code="400">Dados inválidos ou regra de negócio violada (ex.: vencimento no passado).</response>
	[HttpPost]
	[ProducesResponseType<TaskItemResponse>(StatusCodes.Status201Created)]
	[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<TaskItemResponse>> Create(CreateTaskItemRequest request, CancellationToken cancellationToken)
	{
		TaskItemResponse response = await _service.CreateAsync(request, cancellationToken);

		return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
	}

	/// <summary>
	/// Lista as tarefas, com filtros opcionais por status e por intervalo de vencimento.
	/// </summary>
	/// <remarks>
	/// A lista vem ordenada pelo vencimento mais próximo; tarefas sem vencimento ficam no fim.
	/// Quando há filtro de data, tarefas sem vencimento ficam de fora e são contadas em "tasksWithoutDueDate".
	/// </remarks>
	/// <param name="filter">Filtros opcionais. As datas são inclusivas (formato AAAA-MM-DD).</param>
	/// <param name="cancellationToken">Cancelamento da requisição.</param>
	/// <response code="200">Lista de tarefas e quantidade de tarefas sem vencimento fora do filtro.</response>
	/// <response code="400">Filtro inválido (ex.: data inicial maior que a final).</response>
	[HttpGet]
	[ProducesResponseType<TaskItemListResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<TaskItemListResponse>> List([FromQuery] TaskItemFilter filter, CancellationToken cancellationToken)
	{
		TaskItemListResponse response = await _service.ListAsync(filter, cancellationToken);

		return Ok(response);
	}

	/// <summary>
	/// Busca uma tarefa pelo Id.
	/// </summary>
	/// <param name="id">Id da tarefa.</param>
	/// <param name="cancellationToken">Cancelamento da requisição.</param>
	/// <response code="200">Tarefa encontrada.</response>
	/// <response code="404">Nenhuma tarefa com esse Id.</response>
	[HttpGet("{id:guid}")]
	[ProducesResponseType<TaskItemResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<TaskItemResponse>> GetById(Guid id, CancellationToken cancellationToken)
	{
		TaskItemResponse? response = await _service.GetByIdAsync(id, cancellationToken);

		if (response == null)
		{
			return NotFound();
		}

		return Ok(response);
	}

	/// <summary>
	/// Edita uma tarefa, substituindo todos os campos (PUT).
	/// </summary>
	/// <param name="id">Id da tarefa.</param>
	/// <param name="request">Novos dados. Descrição ou vencimento nulos apagam o valor anterior.</param>
	/// <param name="cancellationToken">Cancelamento da requisição.</param>
	/// <response code="204">Tarefa atualizada.</response>
	/// <response code="400">Dados inválidos ou regra de negócio violada.</response>
	/// <response code="404">Nenhuma tarefa com esse Id.</response>
	[HttpPut("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
	[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Update(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken)
	{
		TaskItemResponse? response = await _service.UpdateAsync(id, request, cancellationToken);

		if (response == null)
		{
			return NotFound();
		}

		return NoContent();
	}

	/// <summary>
	/// Exclui uma tarefa.
	/// </summary>
	/// <param name="id">Id da tarefa.</param>
	/// <param name="cancellationToken">Cancelamento da requisição.</param>
	/// <response code="204">Tarefa excluída.</response>
	/// <response code="404">Nenhuma tarefa com esse Id.</response>
	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
	{
		bool deleted = await _service.DeleteAsync(id, cancellationToken);

		if (!deleted)
		{
			return NotFound();
		}

		return NoContent();
	}
}
