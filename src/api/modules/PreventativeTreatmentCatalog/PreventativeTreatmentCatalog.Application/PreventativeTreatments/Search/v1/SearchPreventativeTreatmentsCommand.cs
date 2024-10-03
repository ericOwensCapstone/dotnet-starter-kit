using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Search.v1;

public record SearchPreventativeTreatmentsCommand(PaginationFilter filter) : IRequest<PagedList<PreventativeTreatmentResponse>>;

