using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Events;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
public class AnimalType : AuditableEntity, IAggregateRoot
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public double FcrMean { get; private set; }
    public double FcrStdDev { get; private set; }
    public double DiseaseIncidenceMean { get; private set; }
    public double DiseaseIncidenceStdDev { get; private set; }
    public double CarcassYieldMean { get; private set; }
    public double CarcassYieldStdDev { get; private set; }
    public double QualityGradeMean { get; private set; }
    public double QualityGradeStdDev { get; private set; }
    public double LowerCriticalTemp { get; private set; }
    public double UpperCriticalTemp { get; private set; }
    public double ArrivalHeadCountMean { get; private set; }
    public double ArrivalHeadCountStdDev { get; private set; }
    public double ArrivalWeightMean { get; private set; }
    public double ArrivalWeightStdDev { get; private set; }
    public double ArrivalCostPerCwtMean { get; private set; }
    public double ArrivalCostPerCwtStdDev { get; private set; }

    public static AnimalType Create
    (
        string name, 
        string? description,
        double fcrMean,
        double fcrStdDev,
        double diseaseIncidenceMean,
        double diseaseIncidenceStdDev,
        double carcassYieldMean,
        double carcassYieldStdDev,
        double qualityGradeMean,
        double qualityGradeStdDev,
        double lowerCriticalTemp,
        double upperCriticalTemp,
        double arrivalHeadCountMean,
        double arrivalHeadCountStdDev,
        double arrivalWeightMean,
        double arrivalWeightStdDev,
        double arrivalCostPerCwtMean,
        double arrivalCostPerCwtStdDev
    )
    {
        var animalType = new AnimalType();

        animalType.Name = name;
        animalType.Description = description;

        animalType.FcrMean = fcrMean;
        animalType.FcrStdDev = fcrStdDev;
        animalType.DiseaseIncidenceMean = diseaseIncidenceMean;
        animalType.DiseaseIncidenceStdDev = diseaseIncidenceStdDev;
        animalType.CarcassYieldMean = carcassYieldMean;
        animalType.CarcassYieldStdDev = carcassYieldStdDev;
        animalType.QualityGradeMean = qualityGradeMean;
        animalType.QualityGradeStdDev = qualityGradeStdDev;
        animalType.LowerCriticalTemp = lowerCriticalTemp;
        animalType.UpperCriticalTemp = upperCriticalTemp;
        animalType.ArrivalHeadCountMean = arrivalHeadCountMean;
        animalType.ArrivalHeadCountStdDev = arrivalHeadCountStdDev;
        animalType.ArrivalWeightMean = arrivalWeightMean;
        animalType.ArrivalWeightStdDev = arrivalWeightStdDev;
        animalType.ArrivalCostPerCwtMean = arrivalCostPerCwtMean;
        animalType.ArrivalCostPerCwtStdDev = arrivalCostPerCwtStdDev;

        animalType.QueueDomainEvent(new AnimalTypeCreated() { AnimalType = animalType });

        return animalType;
    }

    public static AnimalType Create
    (
        Guid id, 
        string name, 
        string? description,
        double fcrMean,
        double fcrStdDev,
        double diseaseIncidenceMean,
        double diseaseIncidenceStdDev,
        double carcassYieldMean,
        double carcassYieldStdDev,
        double qualityGradeMean,
        double qualityGradeStdDev,
        double lowerCriticalTemp,
        double upperCriticalTemp,
        double arrivalHeadCountMean,
        double arrivalHeadCountStdDev,
        double arrivalWeightMean,
        double arrivalWeightStdDev,
        double arrivalCostPerCwtMean,
        double arrivalCostPerCwtStdDev
    )
    {
        var animalType = new AnimalType
        {
            Id = id,
            Name = name,
            Description = description,
            FcrMean = fcrMean,
            FcrStdDev = fcrStdDev,
            DiseaseIncidenceMean = diseaseIncidenceMean,
            DiseaseIncidenceStdDev = diseaseIncidenceStdDev,
            CarcassYieldMean = carcassYieldMean,
            CarcassYieldStdDev = carcassYieldStdDev,
            QualityGradeMean = qualityGradeMean,
            QualityGradeStdDev = qualityGradeStdDev,
            LowerCriticalTemp = lowerCriticalTemp,
            UpperCriticalTemp = upperCriticalTemp,
            ArrivalHeadCountMean = arrivalHeadCountMean,
            ArrivalHeadCountStdDev = arrivalHeadCountStdDev,
            ArrivalWeightMean = arrivalWeightMean,
            ArrivalWeightStdDev = arrivalWeightStdDev,
            ArrivalCostPerCwtMean = arrivalCostPerCwtMean,
            ArrivalCostPerCwtStdDev = arrivalCostPerCwtStdDev
        };

        return animalType;
    }

    public AnimalType Update
    (
        string? name, 
        string? description,
        double fcrMean,
        double fcrStdDev,
        double diseaseIncidenceMean,
        double diseaseIncidenceStdDev,
        double carcassYieldMean,
        double carcassYieldStdDev,
        double qualityGradeMean,
        double qualityGradeStdDev,
        double lowerCriticalTemp,
        double upperCriticalTemp,
        double arrivalHeadCountMean,
        double arrivalHeadCountStdDev,
        double arrivalWeightMean,
        double arrivalWeightStdDev,
        double arrivalCostPerCwtMean,
        double arrivalCostPerCwtStdDev
    )
    {
        if (name is not null && Name?.Equals(name, StringComparison.OrdinalIgnoreCase) is not true) Name = name;
        if (description is not null && Description?.Equals(description, StringComparison.OrdinalIgnoreCase) is not true) Description = description;
        if (fcrMean != FcrMean) FcrMean = fcrMean;
        if (fcrStdDev != FcrStdDev) FcrStdDev = fcrStdDev;
        if (diseaseIncidenceMean != DiseaseIncidenceMean) DiseaseIncidenceMean = diseaseIncidenceMean;
        if (diseaseIncidenceStdDev != DiseaseIncidenceStdDev) DiseaseIncidenceStdDev = diseaseIncidenceStdDev;
        if (carcassYieldMean != CarcassYieldMean) CarcassYieldMean = carcassYieldMean;
        if (carcassYieldStdDev != CarcassYieldStdDev) CarcassYieldStdDev = carcassYieldStdDev;
        if (qualityGradeMean != QualityGradeMean) QualityGradeMean = qualityGradeMean;
        if (qualityGradeStdDev != QualityGradeStdDev) QualityGradeStdDev = qualityGradeStdDev;
        if (lowerCriticalTemp != LowerCriticalTemp) LowerCriticalTemp = lowerCriticalTemp;
        if (upperCriticalTemp != UpperCriticalTemp) UpperCriticalTemp = upperCriticalTemp;
        if (arrivalHeadCountMean != ArrivalHeadCountMean) ArrivalHeadCountMean = arrivalHeadCountMean;
        if (arrivalHeadCountStdDev != ArrivalHeadCountStdDev) ArrivalHeadCountStdDev = arrivalHeadCountStdDev;
        if (arrivalWeightMean != ArrivalWeightMean) ArrivalWeightMean = arrivalWeightMean;
        if (arrivalWeightStdDev != ArrivalWeightStdDev) ArrivalWeightStdDev = arrivalWeightStdDev;
        if (arrivalCostPerCwtMean != ArrivalCostPerCwtMean) ArrivalCostPerCwtMean = arrivalCostPerCwtMean;
        if (arrivalCostPerCwtStdDev != ArrivalCostPerCwtStdDev) ArrivalCostPerCwtStdDev = arrivalCostPerCwtStdDev;

        this.QueueDomainEvent(new AnimalTypeUpdated() { AnimalType = this });
        return this;
    }

    public static AnimalType Update
    (
        Guid id, 
        string name, 
        string? description,
        double fcrMean,
        double fcrStdDev,
        double diseaseIncidenceMean,
        double diseaseIncidenceStdDev,
        double carcassYieldMean,
        double carcassYieldStdDev,
        double qualityGradeMean,
        double qualityGradeStdDev,
        double lowerCriticalTemp,
        double upperCriticalTemp,
        double arrivalHeadCountMean,
        double arrivalHeadCountStdDev,
        double arrivalWeightMean,
        double arrivalWeightStdDev,
        double arrivalCostPerCwtMean,
        double arrivalCostPerCwtStdDev
    )
    {
        var animalType = new AnimalType
        {
            Id = id,
            Name = name,
            Description = description,
            FcrMean = fcrMean,
            FcrStdDev = fcrStdDev,
            DiseaseIncidenceMean = diseaseIncidenceMean,
            DiseaseIncidenceStdDev = diseaseIncidenceStdDev,
            CarcassYieldMean = carcassYieldMean,
            CarcassYieldStdDev = carcassYieldStdDev,
            QualityGradeMean = qualityGradeMean,
            QualityGradeStdDev = qualityGradeStdDev,
            LowerCriticalTemp = lowerCriticalTemp,
            UpperCriticalTemp = upperCriticalTemp,
            ArrivalHeadCountMean = arrivalHeadCountMean,
            ArrivalHeadCountStdDev = arrivalHeadCountStdDev,
            ArrivalWeightMean = arrivalWeightMean,
            ArrivalWeightStdDev = arrivalWeightStdDev,
            ArrivalCostPerCwtMean = arrivalCostPerCwtMean,
            ArrivalCostPerCwtStdDev = arrivalCostPerCwtStdDev
        };

        animalType.QueueDomainEvent(new AnimalTypeUpdated() { AnimalType = animalType });

        return animalType;
    }
}





