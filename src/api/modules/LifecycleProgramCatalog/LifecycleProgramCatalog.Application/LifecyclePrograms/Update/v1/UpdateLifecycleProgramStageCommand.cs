using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
public class UpdateLifecycleProgramStageCommand
{
    public Guid LifecycleStageId { get; set; }
    public int Order { get; set; }
}

