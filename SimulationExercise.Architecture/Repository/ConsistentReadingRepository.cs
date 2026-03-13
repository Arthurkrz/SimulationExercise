using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class ConsistentReadingRepository : IConsistentReadingRepository
    {
        private readonly string _mainTableName = "ConsistentReading";
        private readonly string _messageTableName = "ConsistentReadingMessage";

        public void Insert(ConsistentReadingInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName}
                            (READINGID, SENSORID, SENSORTYPENAME, UNIT, VALUE, PROVINCE, 
                             CITY, ISHISTORIC, DAYSOFMEASURE, UTMNORD, UTMEST, LATITUDE, 
                             LONGITUDE, CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED)
                                VALUES (@READINGID, @SENSORID, @SENSORTYPENAME, @UNIT, @VALUE, 
                                        @PROVINCE, @CITY, @ISHISTORIC, @DAYSOFMEASURE, @UTMNORD, 
                                        @UTMEST, @LATITUDE, @LONGITUDE, @CREATIONTIME, 
                                        @LASTUPDATETIME, @LASTUPDATEUSER, @ISEXPORTED);";

            context.Execute(sql, new
            {
                dto.ReadingId, dto.SensorId, dto.SensorTypeName, dto.Unit,
                dto.Value, dto.Province, dto.City, dto.IsHistoric,
                dto.DaysOfMeasure, dto.UtmNord, dto.UtmEst, 
                dto.Latitude, dto.Longitude,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.IsExported,
            });
        }

        public void Update(ConsistentReadingUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_mainTableName} SET ISEXPORTED = @ISEXPORTED 
                                   WHERE CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                            new { dto.IsExported, dto.ConsistentReadingId });
        }

        public IList<ConsistentReadingGetDTO> GetByIsExported(bool isExported, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sql = $@"SELECT CONSISTENTREADINGID, READINGID, SENSORID, 
                         SENSORTYPENAME, UNIT, VALUE, PROVINCE, CITY, 
                         ISHISTORIC, DAYSOFMEASURE, UTMNORD, UTMEST, 
                         LATITUDE, LONGITUDE, ISEXPORTED
                            FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<ConsistentReadingGetDTO>(sql, new { isExported });
        }
    }
}
