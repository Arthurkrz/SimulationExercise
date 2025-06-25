using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Architecture.Repository
{
    public class OutputFileRepository : IOutputFileRepository
    {
        private readonly string _mainTableName = "OutputFile";
        private readonly string _messageTableName = "OutputFileMessage";

        public void Insert(OutputFileInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName}
                            (NAME, BYTES, EXTENSION, CREATIONTIME, 
                            LASTUPDATETIME, LASTUPDATEUSER, STATUSID)
                                VALUES (@Name, @Bytes, @Extension, 
                                        @CreationTime, @LastUpdateTime, 
                                        @LastUpdateUser, @Status);";

            context.Execute(sql, new
            {
                dto.Name, dto.Extension, dto.Bytes,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status
            });

            context.Commit();
        }

        public void Update(OutputFileUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_messageTableName} SET STATUSID = @Status 
                                   WHERE OUTPUTFILEID = @OutputFileId;",
                            new { dto.Status, dto.OutputFileId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName}(OUTPUTFILEID, 
                                    CREATIONDATE, LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE) 
                                        VALUES (@OutputFileId, @CreationDate, @LastUpdateDate, 
                                                @LastUpdateUser, @Message);";

                    context.Execute(sql, new
                    {
                        dto.OutputFileId,
                        CreationDate = SystemTime.Now(),
                        LastUpdateDate = SystemTime.Now(),
                        LastUpdateUser = SystemIdentity.CurrentName(),
                        message
                    });
                }
            }

            context.Commit();
        }

        public IList<OutputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE STATUSID = @statusId
                                ORDER BY CreationTime DESC";

            var result = context.Query<OutputFileGetDTO>(sql, new { statusId });
            return result;
        }
    }
}
