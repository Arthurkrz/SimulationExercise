namespace SimulationExercise.Core.Entities
{
    public class ProvinceData
    {
        public ProvinceData(string province, 
                             string sensorTypeName,
                             IList<ConsistentReading> 
                                  consistentReadings) 
        {
            Province = province;
            SensorTypeName = sensorTypeName;
            ConsistentReadings = consistentReadings;
        }

        public string Province { get; }
        public string SensorTypeName { get; }
        public IList<ConsistentReading> ConsistentReadings { get; }
    }
}
