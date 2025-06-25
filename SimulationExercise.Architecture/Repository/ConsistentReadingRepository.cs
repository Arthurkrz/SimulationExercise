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
                            (READINGID, BYTES, CREATIONTIME, LASTUPDATETIME, 
                             LASTUPDATEUSER, STATUSID)
                                VALUES (@ReadingId, @Bytes, @CreationTime
                                        @LastUpdateTime, @LastUpdateUser, @Status);";

            context.Execute(sql, new
            {
                dto.ReadingId, dto.Bytes,
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

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @Status 
                                   WHERE CONSISTENTREADINGID = @ConsistentReadingId;",
                            new { dto.Status, dto.ConsistentReadingId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName}(
                                    CONSISTENTREADINGID, CREATIONDATE, 
                                    LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE) 
                                        VALUES (@ConsistentReadingId, 
                                                @CreationDate, @LastUpdateDate,
                                                @LastUpdateUser, @Message);";

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

            context.Commit();
        }

        public IList<ConsistentReadingGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT CONSISTENTREADINGID, BYTES, STATUSID AS STATUS
                            FROM {_mainTableName} WHERE StatusId = @statusId
                                ORDER BY CreationTime DESC";

            var result = context.Query<ConsistentReadingGetDTO>(sql, new { statusId });
            return result;
        }
    }
}
