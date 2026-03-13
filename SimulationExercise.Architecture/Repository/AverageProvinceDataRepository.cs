using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class AverageProvinceDataRepository : IAverageProvinceDataRepository
    {

        private readonly string _mainTableName = "AverageProvinceData";

        public void Insert(AverageProvinceDataInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            string sql = $@"INSERT INTO {_mainTableName} (PROVINCE, 
                                 SENSORTYPENAME, UNIT, AVERAGEVALUE, AVERAGEDAYSOFMEASURE, 
                                 CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED) 
                                    VALUES (@PROVINCE, @SENSORTYPENAME, 
                                            @UNIT, @AVERAGEVALUE, @AVERAGEDAYSOFMEASURE, 
                                            @CREATIONTIME, @LASTUPDATETIME,
                                            @LASTUPDATEUSER, @ISEXPORTED);";

            context.Execute(sql, new
            {
                dto.Province, dto.SensorTypeName, dto.Unit, 
                dto.AverageValue, dto.AverageDaysOfMeasure,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.IsExported
            });
        }

        public void Update(AverageProvinceDataUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.Execute($@"UPDATE {_mainTableName} SET ISEXPORTED = @ISEXPORTED 
                                   WHERE AVERAGEPROVINCEDATAID = @AVERAGEPROVINCEDATAID;",
                            new { dto.IsExported, dto.AverageProvinceDataId });
        }

        public IList<AverageProvinceDataGetDTO> GetByIsExported(bool isExported, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sql = $@"SELECT AVERAGEPROVINCEDATAID, PROVINCE, 
                             SENSORTYPENAME, AVERAGEVALUE, UNIT, AVERAGEDAYSOFMEASURE, 
                             ISEXPORTED FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED 
                                ORDER BY CREATIONTIME DESC;";

            return context.Query<AverageProvinceDataGetDTO>(sql, new { isExported });
        }
    }
}
