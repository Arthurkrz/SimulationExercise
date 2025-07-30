using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class InputFileRepository : IInputFileRepository
    {
        private readonly string _mainTableName = "InputFile";
        private readonly string _messageTableName = "InputFileMessage";

        public void Insert(InputFileInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName}
                            (NAME, BYTES, EXTENSION, CREATIONTIME,
                            LASTUPDATETIME, LASTUPDATEUSER, STATUSID) 
                                VALUES (@NAME, @BYTES, @EXTENSION,
                                        @CREATIONTIME, @LASTUPDATETIME, 
                                        @LASTUPDATEUSER, @STATUS);";

            context.Execute(sql, new
            {
                dto.Name, dto.Extension, dto.Bytes,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(), 
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status
            });
        }

        public void Update(InputFileUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @STATUS 
                                   WHERE INPUTFILEID = @INPUTFILEID;", 
                            new { dto.Status, dto.InputFileId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName}(INPUTFILEID, 
                                    CREATIONDATE, LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE)
                                        VALUES (@INPUTFILEID, @CREATIONDATE, @LASTUPDATEDATE,
                                        @LASTUPDATEUSER, @MESSAGE);";

                    context.Execute(sql, new 
                    { 
                        dto.InputFileId, 
                        CreationDate = SystemTime.Now(), 
                        LastUpdateDate = SystemTime.Now(), 
                        LastUpdateUser = SystemIdentity.CurrentName(), 
                        message
                    });
                }
            }
        }

        public IList<InputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT INPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE STATUSID = @STATUSID 
                                ORDER BY CreationTime DESC";

            return context.Query<InputFileGetDTO>(sql, new { statusId });
        }
    }
}