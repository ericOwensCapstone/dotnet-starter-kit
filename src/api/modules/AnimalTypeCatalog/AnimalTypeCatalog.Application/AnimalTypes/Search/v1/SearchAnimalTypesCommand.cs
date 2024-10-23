using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Search.v1;

public record SearchAnimalTypesCommand(PaginationFilter filter) : IRequest<PagedList<AnimalTypeResponse>>;


