using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Delete.v1;
public sealed record DeletePreventiveTreatmentCommand(
    Guid Id) : IRequest;

