using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly string _mainTableName = "Reading";
        private readonly string _messageTableName = "ReadingMessage";

        public void Insert(ReadingInsertDTO dto, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            string sql = $@"INSERT INTO {_mainTableName}
                            (INPUTFILEID, SENSORID, SENSORTYPENAME, UNIT, STATIONID, 
                            STATIONNAME, VALUE, PROVINCE, CITY, ISHISTORIC, STARTDATE, 
                            STOPDATE, UTMNORD, UTMEST, LATITUDE, LONGITUDE, CREATIONTIME, 
                            LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES (@INPUTFILEID, @SENSORID, @SENSORTYPENAME, @UNIT, 
                                        @STATIONID, @STATIONNAME, @VALUE, @PROVINCE, @CITY, 
                                        @ISHISTORIC, @STARTDATE, @STOPDATE, @UTMNORD, @UTMEST, 
                                        @LATITUDE, @LONGITUDE, @CREATIONTIME, @LASTUPDATETIME, 
                                        @LASTUPDATEUSER, @STATUSID)";

            context.Execute(sql, new 
            { 
                dto.InputFileId, dto.SensorId, dto.SensorTypeName, dto.Unit, 
                dto.StationId, dto.StationName, dto.Value, dto.Province, 
                dto.City, dto.IsHistoric, dto.StartDate, dto.StopDate, 
                dto.UtmNord, dto.UtmEst, dto.Latitude, dto.Longitude,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                StatusId = dto.Status 
            });
        }

        public void Update(ReadingUpdateDTO dto, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @STATUSID 
                               WHERE READINGID = @READINGID;",
                            new { StatusId = dto.Status, ReadingId = dto.ReadingId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName} 
                                    (READINGID, CREATIONDATE, LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE) 
                                        VALUES (@READINGID, @CREATIONDATE, @LASTUPDATEDATE, 
                                                @LASTUPDATEUSER, @MESSAGE);";

                    context.Execute(sql, new 
                    { 
                        dto.ReadingId,
                        CreationDate = SystemTime.Now(),
                        LastUpdateDate = SystemTime.Now(),
                        LastUpdateUser = SystemIdentity.CurrentName(),
                        message
                    });
                }
            }
        }

        public IList<ReadingGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT READINGID, INPUTFILEID, SENSORID, SENSORTYPENAME, 
                         UNIT, STATIONID, STATIONNAME, VALUE, PROVINCE, CITY, 
                         ISHISTORIC, STARTDATE, STOPDATE, UTMNORD, UTMEST, 
                         LATITUDE, LONGITUDE, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE STATUSID = @STATUSID
                                ORDER BY CREATIONTIME DESC";

            return context.Query<ReadingGetDTO>(sql, new { StatusId = statusId });
        }
    }
}
