using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.Delete.v1;
public sealed record DeleteRationCommand(
    Guid Id) : IRequest;

