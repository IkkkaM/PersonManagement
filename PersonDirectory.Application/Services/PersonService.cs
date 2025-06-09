namespace PersonDirectory.Application.Services;

public class PersonService : IPersonService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IStringLocalizer<PersonService> _localizer;

    public PersonService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<PersonService> localizer)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Result<PersonResponse>> CreatePersonAsync(PersonCreateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _unitOfWork.CityRepository.ExistsAsync(c => c.Id == request.CityId, cancellationToken))
                return Result<PersonResponse>.Failure(_localizer[ErrorMessages.CityNotFound]);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var person = CreatePersonFromRequest(request);
            var createdPerson = await _unitOfWork.PersonRepository.AddAsync(person, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await AddPhoneNumbersToPersonAsync(createdPerson, request.PhoneNumbers);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var fullPerson = await GetPersonWithDetailsAsync(createdPerson.Id, cancellationToken);
            var response = _mapper.Map<PersonResponse>(fullPerson);

            return Result<PersonResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<PersonResponse>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<PersonResponse>> UpdatePersonAsync(int id, PersonUpdateRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var person = await _unitOfWork.PersonRepository.FirstOrDefaultWithTrackingIncludedAsync(
                p => p.Id == id, p => p.PhoneNumbers);

            if (person == null)
                return Result<PersonResponse>.Failure(_localizer[ErrorMessages.PersonNotFound]);

            if (!await _unitOfWork.CityRepository.ExistsAsync(c => c.Id == request.CityId, cancellationToken))
                return Result<PersonResponse>.Failure(_localizer[ErrorMessages.CityNotFound]);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            UpdatePersonFromRequest(person, request);
            await UpdatePersonPhoneNumbersAsync(person, request.PhoneNumbers);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var updatedPerson = await GetPersonWithDetailsAsync(id, cancellationToken);
            var response = _mapper.Map<PersonResponse>(updatedPerson);

            return Result<PersonResponse>.Success(response);
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<PersonResponse>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result> DeletePersonAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var person = await _unitOfWork.PersonRepository.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (person == null)
                return Result.Failure(_localizer[ErrorMessages.PersonNotFound]);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await DeletePersonRelatedDataAsync(id, cancellationToken);
            _unitOfWork.PersonRepository.Remove(person);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<PersonResponse>> GetPersonByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var person = await GetPersonWithDetailsAsync(id, cancellationToken);
            if (person == null)
                return Result<PersonResponse>.Failure(_localizer[ErrorMessages.PersonNotFound]);

            var response = _mapper.Map<PersonResponse>(person);
            return Result<PersonResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<PersonResponse>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<PersonResponse>> UploadPersonImageAsync(int id, string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var person = await _unitOfWork.PersonRepository.FirstOrDefaultWithTrackingAsync(p => p.Id == id, cancellationToken);
            if (person == null)
                return Result<PersonResponse>.Failure(_localizer[ErrorMessages.PersonNotFound]);

            person.UpdateImagePath(imagePath);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var updatedPerson = await GetPersonWithDetailsAsync(id, cancellationToken);
            var response = _mapper.Map<PersonResponse>(updatedPerson);

            return Result<PersonResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<PersonResponse>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result> AddPersonConnectionAsync(PersonConnectionRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await PersonExistsAsync(request.PersonId, cancellationToken) ||
                !await PersonExistsAsync(request.ConnectedPersonId, cancellationToken))
                return Result.Failure(_localizer[ErrorMessages.PersonNotFound]);

            if (await _unitOfWork.PersonConnectionRepository.ConnectionExistsAsync(request.PersonId, request.ConnectedPersonId, cancellationToken))
                return Result.Failure(_localizer[ErrorMessages.ConnectionAlreadyExists]);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _unitOfWork.PersonConnectionRepository.AddBidirectionalConnectionAsync(
                request.PersonId, request.ConnectedPersonId, request.ConnectionType, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result> RemovePersonConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await _unitOfWork.PersonConnectionRepository.ConnectionExistsAsync(personId, connectedPersonId, cancellationToken))
                return Result.Failure(_localizer[ErrorMessages.PersonNotFound]);

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            await _unitOfWork.PersonConnectionRepository.DeleteBidirectionalConnectionAsync(personId, connectedPersonId, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<PagedResponse<PersonListResponse>>> QuickSearchAsync(PersonQuickSearchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedPersons = await _unitOfWork.PersonRepository.QuickSearchAsync(
                request.SearchTerm, request.PageNumber, request.PageSize, cancellationToken);

            var response = _mapper.Map<PagedResponse<PersonListResponse>>(pagedPersons);
            return Result<PagedResponse<PersonListResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<PagedResponse<PersonListResponse>>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<PagedResponse<PersonListResponse>>> DetailedSearchAsync(PersonDetailedSearchRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedPersons = await _unitOfWork.PersonRepository.DetailedSearchAsync(
                request.FirstName, request.LastName, request.PersonalNumber, request.Gender,
                request.DateOfBirthFrom, request.DateOfBirthTo, request.CityId,
                request.PageNumber, request.PageSize, cancellationToken);

            var response = _mapper.Map<PagedResponse<PersonListResponse>>(pagedPersons);
            return Result<PagedResponse<PersonListResponse>>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<PagedResponse<PersonListResponse>>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    public async Task<Result<PersonConnectionReportResponse>> GetConnectionReportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var reportData = await _unitOfWork.PersonRepository.GetAllPersonsConnectionReportAsync(cancellationToken);
            var response = new PersonConnectionReportResponse
            {
                PersonConnections = _mapper.Map<List<PersonConnectionSummary>>(reportData)
            };

            return Result<PersonConnectionReportResponse>.Success(response);
        }
        catch (Exception ex)
        {
            return Result<PersonConnectionReportResponse>.Failure($"{_localizer[ErrorMessages.DatabaseOperationFailed]}: {ex.Message}");
        }
    }

    #region Private Helper Methods

    private Person CreatePersonFromRequest(PersonCreateRequest request)
    {
        return new Person(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.PersonalNumber,
            request.DateOfBirth,
            request.CityId);
    }

    private void UpdatePersonFromRequest(Person person, PersonUpdateRequest request)
    {
        person.UpdateBasicInfo(
            request.FirstName,
            request.LastName,
            request.Gender,
            request.PersonalNumber,
            request.DateOfBirth,
            request.CityId);
    }

    private async Task AddPhoneNumbersToPersonAsync(Person person, ICollection<PhoneNumberRequest> phoneRequests)
    {
        foreach (var phoneRequest in phoneRequests)
        {
            person.AddPhoneNumber(phoneRequest.Type, phoneRequest.Number);
        }
    }

    private async Task UpdatePersonPhoneNumbersAsync(Person person, ICollection<PhoneNumberRequest> phoneRequests)
    {
        person.ClearPhoneNumbers();
        await AddPhoneNumbersToPersonAsync(person, phoneRequests);
    }

    private async Task DeletePersonRelatedDataAsync(int personId, CancellationToken cancellationToken)
    {
        await _unitOfWork.PersonConnectionRepository.DeletePersonConnectionsAsync(personId, cancellationToken);
        await _unitOfWork.PhoneNumberRepository.DeleteByPersonIdAsync(personId, cancellationToken);
    }

    private async Task<Person?> GetPersonWithDetailsAsync(int id, CancellationToken cancellationToken)
    {
        return await _unitOfWork.PersonRepository.GetPersonWithDetailsAsync(id, cancellationToken);
    }

    private async Task<bool> PersonExistsAsync(int personId, CancellationToken cancellationToken)
    {
        return await _unitOfWork.PersonRepository.ExistsAsync(p => p.Id == personId, cancellationToken);
    }

    #endregion
}