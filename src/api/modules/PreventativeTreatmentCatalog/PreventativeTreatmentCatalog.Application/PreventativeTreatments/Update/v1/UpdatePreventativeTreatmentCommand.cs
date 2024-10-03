using MediatR;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Update.v1;
public sealed record UpdatePreventativeTreatmentCommand(
    Guid Id,
    string? Name,
    decimal DollarsPerHead,
    string? Description = null) : IRequest<UpdatePreventativeTreatmentResponse>;

