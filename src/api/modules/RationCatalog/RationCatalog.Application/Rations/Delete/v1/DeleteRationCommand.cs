using MediatR;

namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Delete.v1;
public sealed record DeleteRationCommand(
    Guid Id) : IRequest;

