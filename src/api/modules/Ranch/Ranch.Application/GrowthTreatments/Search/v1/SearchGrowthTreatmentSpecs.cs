using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.GrowthTreatments;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Search.v1;
public class SearchGrowthTreatmentSpecs : EntitiesByPaginationFilterSpec<GrowthTreatment, GrowthTreatmentResponse>
{
    public SearchGrowthTreatmentSpecs(SearchGrowthTreatmentsCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

