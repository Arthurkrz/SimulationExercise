using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class InputFileRepository : IInputFileRepository
    {
        private readonly string _mainTableName = "InputFile";
        private readonly string _messageTableName = "InputFileMessage";

        public async Task InsertAsync(InputFileInsertDTO dto, IContext context)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(context);

            string sql = $@"INSERT INTO {_mainTableName}
                            (NAME, BYTES, EXTENSION, CREATIONTIME,
                            LASTUPDATETIME, LASTUPDATEUSER, STATUSID) 
                                VALUES (@NAME, @BYTES, @EXTENSION,
                                        @CREATIONTIME, @LASTUPDATETIME, 
                                        @LASTUPDATEUSER, @STATUS);";

            await context.ExecuteAsync(sql, new
            {
                dto.Name, dto.Extension, dto.Bytes,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(), 
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status
            });
        }

        public async Task UpdateAsync(InputFileUpdateDTO dto, IContext context)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(context);

            await context.ExecuteAsync($@"UPDATE {_mainTableName} SET STATUSID = @STATUS 
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

                    await context.ExecuteAsync(sql, new 
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

        public async Task<IList<InputFileGetDTO>> GetByStatusAsync(Status status, IContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            int statusId = (int)status;
            var sql = $@"SELECT INPUTFILEID, NAME, BYTES, EXTENSION, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE STATUSID = @STATUSID 
                                ORDER BY CreationTime DESC";

            return await context.QueryAsync<InputFileGetDTO>(sql, new { statusId });
        }
    }
}