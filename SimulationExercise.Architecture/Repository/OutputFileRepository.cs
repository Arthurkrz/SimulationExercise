using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class OutputFileRepository : IOutputFileRepository
    {
        private readonly string _mainTableName = "OutputFile";

        public void Insert(OutputFileInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName}
                            (NAME, BYTES, EXTENSION, OBJECTTYPE, CREATIONTIME, 
                             LASTUPDATETIME, LASTUPDATEUSER) 
                                 VALUES (@NAME, @BYTES, @EXTENSION, @OBJECTTYPE
                                         @CREATIONTIME, @LASTUPDATETIME, 
                                         @LASTUPDATEUSER);";

            context.Execute(sql, new
            {
                dto.Name, dto.Bytes, dto.Extension,
                dto.ObjectType, 
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
            });
        }

        public IList<OutputFileGetDTO> GetByObjectType(Type objectType, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sql = $@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, OBJECTTYPE, 
                            FROM {_mainTableName} WHERE OBJECTTYPE = @OBJECTTYPE 
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<OutputFileGetDTO>(sql, new { objectType.Name });
        }

        public IList<OutputFileGetDTO> GetByIsExported(bool isExported, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sql = $@"SELECT OUTPUTFILEID, NAME, BYTES, EXTENSION, OBJECTTYPE, 
                            FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED 
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<OutputFileGetDTO>(sql, new { isExported });
        }
    }
}