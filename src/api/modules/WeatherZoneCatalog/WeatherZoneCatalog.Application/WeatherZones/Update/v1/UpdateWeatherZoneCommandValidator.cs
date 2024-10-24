using FluentValidation;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Update.v1;
public class UpdateWeatherZoneCommandValidator : AbstractValidator<UpdateWeatherZoneCommand>
{
    public UpdateWeatherZoneCommandValidator()
    {
        RuleFor(p => p.Name).NotEmpty().MinimumLength(2).MaximumLength(75);
    }
}


