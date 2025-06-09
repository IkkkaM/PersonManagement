namespace PersonDirectory.Api.Controllers;

public class ReportsController : BaseApiController
{
    private readonly IPersonService _personService;

    public ReportsController(IPersonService personService, IStringLocalizer<ReportsController> localizer) : base(localizer)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    /// <summary>
    /// Get person connections report
    /// რეპორტი თუ რამდენი დაკავშირებული პირი ჰყავს თითოეულ ფიზიკურ პირს, კავშირის ტიპის მიხედვით
    /// </summary>
    [HttpGet("person-connections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPersonConnectionsReport(CancellationToken cancellationToken = default)
    {
        var result = await _personService.GetConnectionReportAsync(cancellationToken);
        return HandleResult(result);
    }
}