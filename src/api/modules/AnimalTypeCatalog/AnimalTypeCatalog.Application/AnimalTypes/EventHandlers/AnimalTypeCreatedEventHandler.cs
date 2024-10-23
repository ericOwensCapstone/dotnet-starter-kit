using FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.EventHandlers;

public class AnimalTypeCreatedEventHandler(ILogger<AnimalTypeCreatedEventHandler> logger) : INotificationHandler<AnimalTypeCreated>
{
    public async Task Handle(AnimalTypeCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling animalType created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling animalType created domain event..");
    }
}



