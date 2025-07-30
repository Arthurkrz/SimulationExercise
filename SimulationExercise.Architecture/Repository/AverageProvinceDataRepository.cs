using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class AverageProvinceDataRepository : IAverageProvinceDataRepository
    {
        private readonly string _mainTableName = "AverageProvinceData";
        private readonly string _messageTableName = "AverageProvinceDataMessage";

        public void Insert(AverageProvinceDataInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName}
                                (OUTPUTFILEID, PROVINCE, SENSORTYPENAME, UNIT, 
                                 AVERAGEVALUE, AVERAGEDAYSOFMEASURE, CREATIONTIME, 
                                 LASTUPDATETIME, LASTUPDATEUSER, STATUSID) 
                                    VALUES (@OUTPUTFILEID, @PROVINCE, @SENSORTYPENAME, 
                                            @UNIT, @AVERAGEVALUE, @AVERAGEDAYSOFMEASURE, 
                                            @CREATIONTIME, @LASTUPDATETIME,
                                            @LASTUPDATEUSER, @STATUS);";

            context.Execute(sql, new
            {
                dto.OutputFileId, dto.Province, dto.SensorTypeName, dto.Unit, 
                dto.AverageValue, dto.AverageDaysOfMeasure,
                CreationTime = SystemTime.Now(),
                LastUpdateDate = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status
            });
        }

        public void Update(AverageProvinceDataUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @STATUS 
                                   WHERE AVERAGEPROVINCEDATAID = @AVERAGEPROVINCEDATAID", 
                            new { dto.Status, dto.AverageProvinceDataId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName}(
                                    AVERAGEPROVINCEDATAID, CREATIONDATE, 
                                    LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE) 
                                        VALUES (@AVERAGEPROVINCEDATAID, 
                                                @CREATIONDATE, @LASTUPDATEDATE, 
                                                @LASTUPDATEUSER, @MESSAGE);";

                    context.Execute(sql, new
                    {
                        dto.AverageProvinceDataId,
                        CreationTime = SystemTime.Now(),
                        LastUpdateDate = SystemTime.Now(),
                        LastUpdateUser = SystemIdentity.CurrentName(),
                        message
                    });
                }
            }
        }

        public IList<AverageProvinceDataGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT AVERAGEPROVINCEDATAID, OUTPUTFILEID, 
                         PROVINCE, SENSORTYPENAME, AVERAGEVALUE, UNIT, 
                         AVERAGEDAYSOFMEASURE, STATUSID AS STATUS) 
                            FROM {_mainTableName} WHERE STATUSID = @STATUS 
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<AverageProvinceDataGetDTO>(sql, new { statusId });
        }
    }
}
