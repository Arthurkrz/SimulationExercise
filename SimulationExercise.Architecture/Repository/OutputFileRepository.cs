using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class OutputFileRepository : IOutputFileRepository
    {
        private readonly string _mainTableName = "OutputFile";

        public async Task InsertAsync(OutputFileInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName}
                            (NAME, BYTES, EXTENSION, OBJECTTYPE, CREATIONTIME, 
                             LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED) 
                                 VALUES (@NAME, @BYTES, @EXTENSION, @OBJECTTYPE, 
                                         @CREATIONTIME, @LASTUPDATETIME, 
                                         @LASTUPDATEUSER, @ISEXPORTED);";

            await context.ExecuteAsync(sql, new
            {
                dto.Name, dto.Bytes, dto.Extension,
                dto.ObjectType, 
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.IsExported
            });
        }

        public async Task<IList<OutputFileGetDTO>> GetByIsExportedAsync(bool isExported, string objectType, IContext context)
        {
            ArgumentNullException.ThrowIfNull(nameof(context));
            ArgumentException.ThrowIfNullOrWhiteSpace(objectType);

            var sql = $@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, 
                         OBJECTTYPE, ISEXPORTED FROM {_mainTableName} 
                            WHERE ISEXPORTED = @ISEXPORTED 
                            AND OBJECTTYPE = @OBJECTTYPE 
                                ORDER BY CREATIONTIME DESC;";

            return await context.QueryAsync<OutputFileGetDTO>(sql, new { isExported, objectType });
        }
    }
}