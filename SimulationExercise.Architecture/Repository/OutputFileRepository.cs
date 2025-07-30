using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DatabaseDTOs;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
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
                            (NAME, BYTES, EXTENSION, OBJECTTYPE, 
                             ISAVERAGEPROVINCEDATAEXPORTED, CREATIONTIME, 
                             LASTUPDATETIME, LASTUPDATEUSER, STATUSID) 
                                 VALUES (@NAME, @BYTES, @EXTENSION, 
                                         @CREATIONTIME, @LASTUPDATETIME, 
                                         @LASTUPDATEUSER, @STATUS);";

            context.Execute(sql, new
            {
                dto.Name, dto.Bytes, dto.Extension,
                dto.ObjectType, dto.IsAverageProvinceDataExported, 
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.Status
            });
        }

        public void Update(OutputFileUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_mainTableName} SET STATUSID = @STATUS 
                                   WHERE OUTPUTFILEID = @OUTPUTFILEID;",
                            new { dto.Status, dto.OutputFileId });

            if (dto.Messages.Any() && dto.Status == Status.Error)
            {
                foreach (var message in dto.Messages)
                {
                    string sql = $@"INSERT INTO {_messageTableName}(OUTPUTFILEID, 
                                    CREATIONDATE, LASTUPDATEDATE, LASTUPDATEUSER, MESSAGE) 
                                        VALUES (@OUTPUTFILEID, @CREATIONDATE, @LASTUPDATEDATE, 
                                                @LASTUPDATEUSER, @MESSAGE);";

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
        }

        public IList<OutputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            int statusId = (int)status;
            var sql = $@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, OBJECTTYPE, 
                         ISAVERAGEPROVINCEDATAEXPORTED, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE STATUSID = @STATUSID 
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<OutputFileGetDTO>(sql, new { statusId });
        }

        public IList<OutputFileGetDTO> GetByIsAverageProvinceDataExported(bool isExported, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sql = $@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, OBJECTTYPE,
                         ISAVERAGEPROVINCEDATAEXPORTED, STATUSID AS STATUS 
                            FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED 
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<OutputFileGetDTO>(sql, new { isExported });
        }
    }
}
