using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Search.v1;

public class SearchLifecycleStagesCommand : PaginationFilter, IRequest<PagedList<LifecycleStageResponse>>
{    
    public Guid? RationId { get; set; }
    public Guid? GrowthTreatmentId { get; set; }
    public Guid? PreventiveTreatmentId { get; set; }
}

