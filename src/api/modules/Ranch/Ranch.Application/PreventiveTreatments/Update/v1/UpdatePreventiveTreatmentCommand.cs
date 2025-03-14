using MediatR;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Update.v1;
public sealed record UpdatePreventiveTreatmentCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal DollarsPerHead
) : IRequest<UpdatePreventiveTreatmentResponse>;

