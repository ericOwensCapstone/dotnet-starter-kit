using MediatR;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Delete.v1;
public sealed record DeletePreventativeTreatmentCommand(
    Guid Id) : IRequest;

