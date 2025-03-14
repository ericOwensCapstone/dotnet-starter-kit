using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Search.v1;

public class SearchPreventiveTreatmentsCommand : PaginationFilter, IRequest<PagedList<PreventiveTreatmentResponse>>
{

}

