using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Search.v1;

public record SearchGrowthTreatmentsCommand(PaginationFilter filter) : IRequest<PagedList<GrowthTreatmentResponse>>;

