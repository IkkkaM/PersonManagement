namespace PersonDirectory.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Person, PersonResponse>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()));

        CreateMap<Person, PersonListResponse>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()))
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name));

        CreateMap<PhoneNumber, PhoneNumberResponse>();

        CreateMap<City, CityResponse>();

        CreateMap<PersonConnection, PersonConnectionResponse>()
            .ForMember(dest => dest.ConnectedPersonFirstName, opt => opt.MapFrom(src => src.ConnectedPerson.FirstName))
            .ForMember(dest => dest.ConnectedPersonLastName, opt => opt.MapFrom(src => src.ConnectedPerson.LastName));

        CreateMap(typeof(PagedEntities<>), typeof(PagedResponse<>));

        CreateMap<PersonConnectionReportItem, PersonConnectionSummary>()
            .ForMember(dest => dest.TotalConnections, opt => opt.MapFrom(src => src.ConnectionCounts.Values.Sum()));
    }
}