﻿using SimulationExercise.Core.Entities;

namespace SimulationExercise.Core.Contracts.Services
{
    public interface IProvinceDataListFactory
    {
        IList<ProvinceData> CreateProvinceDataList(IList<ConsistentReading> consistentReadings);
    }
}