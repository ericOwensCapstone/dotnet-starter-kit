namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
public sealed record AnimalTypeResponse
(
    Guid? Id, 
    string Name, 
    string? Description,
    double FcrMean,
    double FcrStdDev,
    double DiseaseIncidenceMean,
    double DiseaseIncidenceStdDev,
    double CarcassYieldMean,
    double CarcassYieldStdDev,
    double QualityGradeMean,
    double QualityGradeStdDev,
    double LowerCriticalTemp,
    double UpperCriticalTemp,
    double ArrivalHeadCountMean,
    double ArrivalHeadCountStdDev,
    double ArrivalWeightMean,
    double ArrivalWeightStdDev,
    double ArrivalCostPerCwtMean,
    double ArrivalCostPerCwtStdDev
);


