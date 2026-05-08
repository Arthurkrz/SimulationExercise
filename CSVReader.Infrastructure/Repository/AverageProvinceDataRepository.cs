using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Utilities;

namespace CSVReader.Infrastructure.Repository
{
    public class AverageProvinceDataRepository : IAverageProvinceDataRepository
    {
        private readonly string _mainTableName = "AverageProvinceData";

        public async Task InsertAsync(AverageProvinceDataInsertDTO dto, IContext context)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(context);

            string sql = $@"INSERT INTO {_mainTableName} (PROVINCE, SENSORTYPENAME, 
                                AVERAGEVALUE, UNIT, AVERAGEDAYSOFMEASURE, 
                                CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED) 
                                    VALUES (@PROVINCE, @SENSORTYPENAME, @AVERAGEVALUE, 
                                            @UNIT, @AVERAGEDAYSOFMEASURE, 
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
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(context);

            await context.ExecuteAsync($@"UPDATE {_mainTableName} SET ISEXPORTED = @ISEXPORTED 
                                          WHERE AVERAGEPROVINCEDATAID = @AVERAGEPROVINCEDATAID;",
                                       new { dto.IsExported, dto.AverageProvinceDataId });
        }

        public async Task<IList<AverageProvinceDataGetDTO>> GetByIsExportedAsync(bool isExported, IContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var sql = $@"SELECT AVERAGEPROVINCEDATAID, PROVINCE, 
                             SENSORTYPENAME, AVERAGEVALUE, UNIT, AVERAGEDAYSOFMEASURE, 
                             ISEXPORTED FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED 
                                ORDER BY CREATIONTIME DESC;";

            return await context.QueryAsync<AverageProvinceDataGetDTO>(sql, new { isExported });
        }
    }
}
