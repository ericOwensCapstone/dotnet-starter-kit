using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Search.v1;

public record SearchRationsCommand(PaginationFilter filter) : IRequest<PagedList<RationResponse>>;

