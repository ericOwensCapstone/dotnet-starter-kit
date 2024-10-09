using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Search.v1;

public record SearchLifecycleProgramsCommand(PaginationFilter filter) : IRequest<PagedList<LifecycleProgramResponse>>;

