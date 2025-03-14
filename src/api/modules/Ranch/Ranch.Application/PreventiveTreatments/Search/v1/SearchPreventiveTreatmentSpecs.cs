using Ardalis.Specification;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Search.v1;
public class SearchPreventiveTreatmentSpecs : EntitiesByPaginationFilterSpec<PreventiveTreatment, PreventiveTreatmentResponse>
{
    public SearchPreventiveTreatmentSpecs(SearchPreventiveTreatmentsCommand command)
        : base(command) =>
        Query
            .OrderBy(c => c.Name, !command.HasOrderBy());
}

