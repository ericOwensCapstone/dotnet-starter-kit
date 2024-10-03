using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Search.v1;

public record SearchLifecycleStagesCommand(PaginationFilter filter) : IRequest<PagedList<LifecycleStageResponse>>;

