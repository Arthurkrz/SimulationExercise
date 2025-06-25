using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Architecture.Repository
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
                            (INPUTFILEID, BYTES, CREATIONTIME, 
                            LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES (@InputFileId, @Bytes, @CreationTime, 
                                        @LastUpdateTime, @LastUpdateUser, @StatusId)";

            context.Execute(sql, new 
            { 
                dto.InputFileId, dto.Bytes,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status 
            });

            context.Commit();
        }

        public void Update(ReadingUpdateDTO dto, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @StatusId 
                               WHERE ReadingId = @ReadingId;",
                            new { dto.Status, dto.ReadingId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName} 
                                    (READINGID, CREATIONDATE, LASTUPDATEDATE, LASTUPDATEUSER) 
                                        VALUES (@ReadingId, @CreationDate, @LastUpdateDate, 
                                                @LastUpdateUser, @message);";

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

            context.Commit();
        }

        public IList<ReadingGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT READINGID, BYTES, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE STATUSID = @statusId
                                ORDER BY CreationTime DESC";

            var result = context.Query<ReadingGetDTO>(sql, new { statusId });
            return result;
        }
    }
}
