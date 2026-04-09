using SimulationExercise.Core.Contracts.Infrastructure;
using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOs.DatabaseDTOs;
using SimulationExercise.Core.Utilities;

namespace SimulationExercise.Infrastructure.Repository
{
    public class AverageProvinceDataRepository : IAverageProvinceDataRepository
    {
        private readonly string _mainTableName = "AverageProvinceData";

        public async Task InsertAsync(AverageProvinceDataInsertDTO dto, IContext context)
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

            await context.ExecuteAsync(sql, new
            {
                dto.Province, dto.SensorTypeName, dto.Unit, 
                dto.AverageValue, dto.AverageDaysOfMeasure,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.IsExported
            });
        }

        public async Task UpdateAsync(AverageProvinceDataUpdateDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            await context.ExecuteAsync($@"UPDATE {_mainTableName} SET ISEXPORTED = @ISEXPORTED 
                                          WHERE AVERAGEPROVINCEDATAID = @AVERAGEPROVINCEDATAID;",
                                       new { dto.IsExported, dto.AverageProvinceDataId });
        }

        public async Task<IList<AverageProvinceDataGetDTO>> GetByIsExportedAsync(bool isExported, IContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var sql = $@"SELECT AVERAGEPROVINCEDATAID, PROVINCE, 
                             SENSORTYPENAME, AVERAGEVALUE, UNIT, AVERAGEDAYSOFMEASURE, 
                             ISEXPORTED FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED 
                                ORDER BY CREATIONTIME DESC;";

            return await context.QueryAsync<AverageProvinceDataGetDTO>(sql, new { isExported });
        }
    }
}
