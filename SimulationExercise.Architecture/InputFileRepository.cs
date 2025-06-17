using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Architecture
{
    public class InputFileRepository : IInputFileRepository
    {
        public void Insert(InputFileInsertDTO dto, IContext context)
        {
            var tablesNames = GetDatabaseTableNames(context);
            string mainTableName = tablesNames.FirstOrDefault(name => name.Contains("InputFile", StringComparison.OrdinalIgnoreCase) 
                                                                   && !name.Contains("Message", StringComparison.OrdinalIgnoreCase));

            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $"INSERT INTO {mainTableName}" +
                "(Name, Bytes, Extension, CreationTime, " +
                "LastUpdateTime, LastUpdateUser, StatusId) VALUES (@Name, @Bytes, @Extension, " +
                "@CreationTime, @LastUpdateTime, @LastUpdateUser, @Status)";

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

        public void Update(InputFileUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var tablesNames = GetDatabaseTableNames(context);
            string messageTableName = tablesNames.FirstOrDefault(name => name.Contains("InputFileMessage", 
                                                                   StringComparison.OrdinalIgnoreCase));

            context.Execute($"UPDATE {messageTableName} SET Message = @Messages " +
                             "WHERE InputFileId = @InputFileId",
                            new { Messages = dto.Messages.First(), InputFileId = 1 });

            context.Commit();
        }

        public IList<InputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var tablesNames = GetDatabaseTableNames(context);
            string mainTableName = tablesNames.FirstOrDefault(name => name.Contains("InputFile", StringComparison.OrdinalIgnoreCase)
                                                                   && !name.Contains("Message", StringComparison.OrdinalIgnoreCase));

            var sql = "SELECT InputFileId, Name, Bytes, Extension, " +
                $"StatusId FROM {mainTableName} WHERE StatusId = @status";

            var result = context.Query<(int InputFileId, string Name, byte[] Bytes, 
                                        string Extension, int StatusId)>
                                        (sql, new { status = (int)status });

            return result.Select(r => new InputFileGetDTO(r.InputFileId, r.Name, 
                                                          r.Bytes, r.Extension, 
                                                          (Status)r.StatusId)).ToList();
        }

        private IList<string> GetDatabaseTableNames(IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            return context.Query<string>("SELECT TABLE_NAME FROM " +
                "INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'");
        }
    }
}