using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Search.v1;

public class SearchGrowthTreatmentsCommand : PaginationFilter, IRequest<PagedList<GrowthTreatmentResponse>>
{

}

