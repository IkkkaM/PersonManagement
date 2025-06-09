namespace PersonDirectory.Application.Interfaces;
public interface IPersonService
{
    Task<Result<PersonResponse>> CreatePersonAsync(PersonCreateRequest request, CancellationToken cancellationToken = default);
    Task<Result<PersonResponse>> UpdatePersonAsync(int id, PersonUpdateRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeletePersonAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PersonResponse>> GetPersonByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PersonResponse>> UploadPersonImageAsync(int id, string imagePath, CancellationToken cancellationToken = default);
    Task<Result> AddPersonConnectionAsync(PersonConnectionRequest request, CancellationToken cancellationToken = default);
    Task<Result> RemovePersonConnectionAsync(int personId, int connectedPersonId, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<PersonListResponse>>> QuickSearchAsync(PersonQuickSearchRequest request, CancellationToken cancellationToken = default);
    Task<Result<PagedResponse<PersonListResponse>>> DetailedSearchAsync(PersonDetailedSearchRequest request, CancellationToken cancellationToken = default);
    Task<Result<PersonConnectionReportResponse>> GetConnectionReportAsync(CancellationToken cancellationToken = default);
}