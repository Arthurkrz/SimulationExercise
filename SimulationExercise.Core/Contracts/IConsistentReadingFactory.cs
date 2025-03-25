﻿namespace SimulationExercise.Core.Contracts
{
    public interface IConsistentReadingFactory
    {
        Result<ConsistentReading> CreateConsistentReading(Reading reading);
    }
}
