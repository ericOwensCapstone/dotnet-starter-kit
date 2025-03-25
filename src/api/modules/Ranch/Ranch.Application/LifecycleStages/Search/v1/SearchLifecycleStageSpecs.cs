using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Search.v1;
public class SearchLifecycleStageSpecs : EntitiesByPaginationFilterSpec<LifecycleStage, LifecycleStageResponse>
{
    public SearchLifecycleStageSpecs(SearchLifecycleStagesCommand command)
        : base(command) =>
        Query
            .Where(p => p.RationId == command.RationId!.Value, command.RationId.HasValue)
            .Where(p => p.GrowthTreatmentId == command.GrowthTreatmentId!.Value, command.GrowthTreatmentId.HasValue)
            .Where(p => p.PreventiveTreatmentId == command.PreventiveTreatmentId!.Value, command.PreventiveTreatmentId.HasValue)
            // Includes are implemented in context model builder
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

