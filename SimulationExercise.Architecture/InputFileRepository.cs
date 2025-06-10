using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Architecture
{
    public class InputFileRepository : IInputFileRepository
    {
        private readonly string _tableName = "InputFile";

        public void Insert(InputFileInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $"INSERT INTO {_tableName}" +
                "(Name, Extension, Bytes, StatusId, CreationTime, " +
                "LastUpdate, LastUpdateUser) VALUES (@Name, @Extension, " +
                "@Bytes, @Status, @CreationTime, @LastUpdateTime, @LastUpdateUser)";

            context.Execute(sql, new
            {
                dto.Name, dto.Extension, dto.Bytes, dto.Status, 
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(), 
                LastUpdateUser = SystemIdentity.CurrentName()
            });

            context.Commit();
        }

        public void Update(InputFileUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($"UPDATE {_tableName} SET Status = @Status, " +
                $"Messages = @Messages WHERE InputFileId = @InputFileId",
                new { dto.InputFileId, dto.Status, dto.Messages });
        }

        public IList<InputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            if (status == null) throw new ArgumentNullException(nameof(status));
            if (context == null) throw new ArgumentNullException(nameof(context));

            var inputFileId = status;

            return context.Query<InputFileGetDTO>("SELECT InputFileId, Name, " +
                 "Extension, Bytes, CreationTime, LastUpdateTime, " +
                $"LastUpdateUser, StatusId FROM {_tableName} WHERE " +
                 "InputFileId = @inputFileId", new { inputFileId });
        }
    }
}
