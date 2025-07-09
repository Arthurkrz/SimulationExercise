using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Architecture.Repository
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
                             LONGITUDE, CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES (@READINGID, @SENSORID, @SENSORTYPENAME, @UNIT, @VALUE, 
                                        @PROVINCE, @CITY, @ISHISTORIC, @DAYSOFMEASURE, @UTMNORD, 
                                        @UTMEST, @LATITUDE, @LONGITUDE, @CREATIONTIME, 
                                        @LASTUPDATETIME, @LASTUPDATEUSER, @STATUS);";

            context.Execute(sql, new
            {
                dto.ReadingId, dto.SensorId, dto.SensorTypeName, dto.Unit,
                dto.Value, dto.Province, dto.City, dto.IsHistoric,
                dto.DaysOfMeasure, dto.UtmNord, dto.UtmEst, 
                dto.Latitude, dto.Longitude,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status
            });
        }

        public void Update(ConsistentReadingUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @STATUS 
                                   WHERE CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                            new { dto.Status, dto.ConsistentReadingId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName}(
                                    CONSISTENTREADINGID, CREATIONDATE, 
                                    LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE) 
                                        VALUES (@CONSISTENTREADINGID, 
                                                @CREATIONDATE, @LASTUPDATEDATE, 
                                                @LASTUPDATEUSER, @MESSAGE);";

                    context.Execute(sql, new
                    {
                        dto.ConsistentReadingId,
                        CreationDate = SystemTime.Now(),
                        LastUpdateDate = SystemTime.Now(),
                        LastUpdateUser = SystemIdentity.CurrentName(),
                        message
                    });
                }
            }
        }

        public IList<ConsistentReadingGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT CONSISTENTREADINGID, READINGID, SENSORID, 
                         SENSORTYPENAME, UNIT, VALUE, PROVINCE, CITY, 
                         ISHISTORIC, DAYSOFMEASURE, UTMNORD, UTMEST, 
                         LATITUDE, LONGITUDE, STATUSID AS STATUS
                            FROM {_mainTableName} WHERE STATUSID = @STATUSID
                                ORDER BY CREATIONTIME DESC";

            var result = context.Query<ConsistentReadingGetDTO>(sql, new { statusId });
            return result;
        }
    }
}
