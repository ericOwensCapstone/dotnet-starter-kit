using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
namespace FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;

        public class LifecycleProgramLifecycleStage
        {
            public Guid LifecycleProgramId { get; set; }
            public Guid LifecycleStageId { get; set; }
            public LifecycleStage LifecycleStage { get; set; } = default!;

            public int Order { get; set; }
        }
        
