using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Search.v1;

public class SearchLifecycleProgramsCommand : PaginationFilter, IRequest<PagedList<LifecycleProgramResponse>>
{    
}

