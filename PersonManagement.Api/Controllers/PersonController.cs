namespace PersonDirectory.Api.Controllers;

public class PersonController : BaseApiController
{
    private readonly IPersonService _personService;
    private readonly IFileService _fileService;
    private readonly IConfiguration _configuration;

    public PersonController(
        IPersonService personService,
        IFileService fileService,
        IConfiguration configuration,
        IStringLocalizer<PersonController> localizer) : base(localizer)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        _configuration = configuration;
    }

    /// <summary>
    /// Get person by ID with full details
    /// ფიზიკური პირის შესახებ სრული ინფორმაციის მიღება იდენტიფიკატორის მეშვეობით
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPerson(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid person ID" });

        var result = await _personService.GetPersonByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(_localizer[ErrorMessages.PersonNotFound]);

        return HandleResult(result);
    }

    /// <summary>
    /// Create new person
    /// ფიზიკური პირის დამატება
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePerson([FromBody] PersonCreateRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _personService.CreatePersonAsync(request, cancellationToken);

        if (!result.IsSuccess)
            return HandleResult(result);

        return CreatedAtAction(
            nameof(GetPerson),
            new { id = result.Data!.Id },
            new ApiResponse<object>
            {
                Success = true,
                Data = result.Data,
                Message = "Person created successfully"
            });
    }

    /// <summary>
    /// Update person basic information
    /// ფიზიკური პირის ძირითადი ინფორმაციის ცვლილება
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonUpdateRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid person ID" });

        var result = await _personService.UpdatePersonAsync(id, request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete person
    /// ფიზიკური პირის წაშლა
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeletePerson(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid person ID" });

        var result = await _personService.DeletePersonAsync(id, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Upload or update person image
    /// ფიზიკური პირის სურათის ატვირთვა/ცვლილება
    /// </summary>
    [HttpPost("{id:int}/image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadPersonImage(int id, IFormFile image, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid person ID" });

        if (image == null || image.Length == 0)
            return BadRequest(new ApiResponse { Success = false, Message = _localizer[ErrorMessages.FileUploadFailed] });

        var allowedExtensions = _configuration.GetSection("FileStorage:AllowedExtensions").Get<string[]>();
        var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (allowedExtensions == null || !allowedExtensions.Contains(fileExtension))
            return BadRequest(new ApiResponse { Success = false, Message = _localizer[ErrorMessages.InvalidFileFormat] });

        var maxFileSizeMB = _configuration.GetValue<int>("FileStorage:MaxFileSizeInMB");
        var maxFileSize = maxFileSizeMB * 1024 * 1024;

        if (image.Length > maxFileSize)
            return BadRequest(new ApiResponse { Success = false, Message = $"File size exceeds {maxFileSizeMB}MB limit" });

        try
        {
            using var stream = image.OpenReadStream();
            var fileResult = await _fileService.SaveImageAsync(stream, image.FileName, cancellationToken);

            if (!fileResult.IsSuccess)
                return HandleResult(fileResult);

            var result = await _personService.UploadPersonImageAsync(id, fileResult.Data!, cancellationToken);
            return HandleResult(result);
        }
        catch (Exception)
        {
            return BadRequest(new ApiResponse { Success = false, Message = _localizer[ErrorMessages.FileUploadFailed] });
        }
    }

    /// <summary>
    /// Quick search persons
    /// სწრაფი ძებნა (სახელი, გვარი, პირადი ნომრის მიხედვით)
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> QuickSearch([FromQuery] PersonQuickSearchRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _personService.QuickSearchAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Detailed search persons
    /// დეტალური ძებნა (ყველა ველის მიხედვით)
    /// </summary>
    [HttpGet("search/detailed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DetailedSearch([FromQuery] PersonDetailedSearchRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _personService.DetailedSearchAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Add person connection
    /// დაკავშირებული ფიზიკური პირის დამატება
    /// </summary>
    [HttpPost("{id:int}/connections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddConnection(int id, [FromBody] PersonConnectionRequest request, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid person ID" });

        // Ensure the person ID in the route matches the request
        if (request.PersonId != id)
            return BadRequest(new ApiResponse { Success = false, Message = "Person ID mismatch" });

        var result = await _personService.AddPersonConnectionAsync(request, cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Remove person connection
    /// დაკავშირებული ფიზიკური პირის წაშლა
    /// </summary>
    [HttpDelete("{id:int}/connections/{connectedId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveConnection(int id, int connectedId, CancellationToken cancellationToken = default)
    {
        if (id <= 0 || connectedId <= 0)
            return BadRequest(new ApiResponse { Success = false, Message = "Invalid person IDs" });

        var result = await _personService.RemovePersonConnectionAsync(id, connectedId, cancellationToken);
        return HandleResult(result);
    }
}