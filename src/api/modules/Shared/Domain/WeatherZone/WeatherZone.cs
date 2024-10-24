using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Events;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
public class WeatherZone : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public double YearlyAverageTemp { get; private set; }
    public double TempRange { get; private set; }
    public double DeviationPeriod { get; private set; }
    public double DeviationAmplitude { get; private set; }
    public DateTime PeakTempDate { get; private set; }


    public static WeatherZone Create
    (
        string name, 
        string? description,
        double yearlyAverageTemp,
        double tempRange,
        double deviationPeriod,
        double deviationAmplitude,
        DateTime peakTempDate
    )
    {
        var weatherZone = new WeatherZone();

        weatherZone.Name = name;
        weatherZone.Description = description;
        weatherZone.YearlyAverageTemp = yearlyAverageTemp;
        weatherZone.TempRange = tempRange;
        weatherZone.DeviationPeriod = deviationPeriod;
        weatherZone.DeviationAmplitude = deviationAmplitude;
        weatherZone.PeakTempDate = peakTempDate;

        weatherZone.QueueDomainEvent(new WeatherZoneCreated() { WeatherZone = weatherZone });

        return weatherZone;
    }

    public static WeatherZone Create
    (
        Guid id, 
        string name, 
        string? description,
        double yearlyAverageTemp,
        double tempRange,
        double deviationPeriod,
        double deviationAmplitude,
        DateTime peakTempDate
    )
    {
        var weatherZone = new WeatherZone
        {
            Id = id,
            Name = name,
            Description = description,
            YearlyAverageTemp = yearlyAverageTemp,
            TempRange = tempRange,
            DeviationPeriod = deviationPeriod,
            DeviationAmplitude = deviationAmplitude,
            PeakTempDate = peakTempDate
        };

        return weatherZone;
    }

    public WeatherZone Update
    (
        string? name, 
        string? description,
        double? yearlyAverageTemp,
        double? tempRange,
        double? deviationPeriod,
        double? deviationAmplitude,
        DateTime? peakTempDate
    )
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (yearlyAverageTemp.HasValue && YearlyAverageTemp != yearlyAverageTemp) YearlyAverageTemp = yearlyAverageTemp.Value;
        if (tempRange.HasValue && TempRange != tempRange) TempRange = tempRange.Value;
        if (deviationPeriod.HasValue && DeviationPeriod != deviationPeriod) DeviationPeriod = deviationPeriod.Value;
        if (deviationAmplitude.HasValue && DeviationAmplitude != deviationAmplitude) DeviationAmplitude = deviationAmplitude.Value;
        if (peakTempDate.HasValue && PeakTempDate != peakTempDate) PeakTempDate = peakTempDate.Value;

        this.QueueDomainEvent(new WeatherZoneUpdated() { WeatherZone = this });
        return this;
    }

    public static WeatherZone Update
    (
        Guid id, 
        string name, 
        string? description,
        double yearlyAverageTemp,
        double tempRange,
        double deviationPeriod,
        double deviationAmplitude,
        DateTime peakTempDate
    )
    {
        var weatherZone = new WeatherZone
        {
            Id = id,
            Name = name,
            Description = description,
            YearlyAverageTemp = yearlyAverageTemp,
            TempRange = tempRange,
            DeviationPeriod = deviationPeriod,
            DeviationAmplitude = deviationAmplitude,
            PeakTempDate = peakTempDate
        };

        weatherZone.QueueDomainEvent(new WeatherZoneUpdated() { WeatherZone = weatherZone });

        return weatherZone;
    }
}





