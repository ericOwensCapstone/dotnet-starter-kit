using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
public class GetAnimalTypeRequest : IRequest<AnimalTypeResponse>
{
    public Guid Id { get; set; }
    public GetAnimalTypeRequest(Guid id) => Id = id;
}


