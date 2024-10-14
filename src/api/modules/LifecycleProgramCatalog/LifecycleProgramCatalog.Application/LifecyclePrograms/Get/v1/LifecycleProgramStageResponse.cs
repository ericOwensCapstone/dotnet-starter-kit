using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
public sealed record LifecycleProgramStageResponse
{
    public Guid LifecycleProgramId { get; set; }
    public Guid LifecycleStageId { get; set; }
    public LifecycleStage LifecycleStage { get; set; }
    public int Order { get; set; }
}
