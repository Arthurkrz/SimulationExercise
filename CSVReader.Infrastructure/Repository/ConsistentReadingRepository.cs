using CSVReader.Core.Contracts.Infrastructure;
using CSVReader.Core.Contracts.Repository;
using CSVReader.Core.DTOs.DatabaseDTOs;
using CSVReader.Core.Utilities;

namespace CSVReader.Infrastructure.Repository
{
    public class ConsistentReadingRepository : IConsistentReadingRepository
    {
        private readonly string _mainTableName = "ConsistentReading";

        public async Task InsertAsync(ConsistentReadingInsertDTO dto, IContext context)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(context);

            string sql = $@"INSERT INTO {_mainTableName}
                            (READINGID, SENSORID, SENSORTYPENAME, UNIT, VALUE, PROVINCE, 
                             CITY, ISHISTORIC, DAYSOFMEASURE, UTMNORD, UTMEST, LATITUDE, 
                             LONGITUDE, CREATIONTIME, LASTUPDATETIME, LASTUPDATEUSER, ISEXPORTED)
                                VALUES (@READINGID, @SENSORID, @SENSORTYPENAME, @UNIT, @VALUE, 
                                        @PROVINCE, @CITY, @ISHISTORIC, @DAYSOFMEASURE, @UTMNORD, 
                                        @UTMEST, @LATITUDE, @LONGITUDE, @CREATIONTIME, 
                                        @LASTUPDATETIME, @LASTUPDATEUSER, @ISEXPORTED);";

            await context.ExecuteAsync(sql, new
            {
                dto.ReadingId, dto.SensorId, dto.SensorTypeName, dto.Unit,
                dto.Value, dto.Province, dto.City, dto.IsHistoric,
                dto.DaysOfMeasure, dto.UtmNord, dto.UtmEst, 
                dto.Latitude, dto.Longitude,
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(),
                LastUpdateUser = SystemIdentity.CurrentName(),
                dto.IsExported
            });
        }

        public async Task UpdateAsync(ConsistentReadingUpdateDTO dto, IContext context)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNull(context);

            await context.ExecuteAsync($@"UPDATE {_mainTableName} SET ISEXPORTED = @ISEXPORTED 
                                          WHERE CONSISTENTREADINGID = @CONSISTENTREADINGID;",
                                       new { dto.IsExported, dto.ConsistentReadingId });
        }

        public async Task<IList<ConsistentReadingGetDTO>> GetByIsExportedAsync(bool isExported, IContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var sql = $@"SELECT CONSISTENTREADINGID, READINGID, SENSORID, 
                         SENSORTYPENAME, UNIT, VALUE, PROVINCE, CITY, 
                         ISHISTORIC, DAYSOFMEASURE, UTMNORD, UTMEST, 
                         LATITUDE, LONGITUDE, ISEXPORTED 
                            FROM {_mainTableName} WHERE ISEXPORTED = @ISEXPORTED
                                ORDER BY CREATIONTIME DESC;";

            return await context.QueryAsync<ConsistentReadingGetDTO>(sql, new { isExported });
        }
    }
}
