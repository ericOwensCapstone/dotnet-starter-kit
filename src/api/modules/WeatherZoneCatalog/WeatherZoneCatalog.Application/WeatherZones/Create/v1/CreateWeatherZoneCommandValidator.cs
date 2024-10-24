using FluentValidation;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Create.v1;
public class CreateWeatherZoneCommandValidator : AbstractValidator<CreateWeatherZoneCommand>
{
    public CreateWeatherZoneCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}


