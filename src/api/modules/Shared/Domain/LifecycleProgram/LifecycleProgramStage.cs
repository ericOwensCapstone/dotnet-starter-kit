using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
public class LifecycleProgramStage
{
    public Guid LifecycleProgramId { get; set; }
    public LifecycleProgram LifecycleProgram { get; set; } = default!;

    public Guid LifecycleStageId { get; set; }
    public LifecycleStage LifecycleStage { get; set; } = default!;

    public int Order { get; set; }
}
