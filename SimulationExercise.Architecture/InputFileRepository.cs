using SimulationExercise.Core.Contracts.Repository;
using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using SimulationExercise.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Architecture
{
    public class InputFileRepository : IInputFileRepository
    {
        public void Insert(InputFileInsertDTO dto, IContext context)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (context == null) throw new ArgumentNullException(nameof(context));

            const string sql = "insert into dbo.InputFile" +
                "(Name, Extension, Bytes, StatusId, CreationTime, " +
                "LastUpdate, LastUpdateUser) values (@Name, @Extension, " +
                "@Bytes, @Status, @CreationTime, @LastUpdateTime, @LastUpdateUser)";

            context.Execute(sql, new
            {
                dto.Name, dto.Extension, dto.Bytes, dto.Status, 
                CreationTime = SystemTime.Now(),
                LastUpdateTime = SystemTime.Now(), 
                LastUpdateUser = SystemIdentity.CurrentName()
            });
        }

        public void Update(InputFileUpdateDTO dto, IContext context)
        {
            throw new NotImplementedException();
        }

        public IList<InputFileGetDTO> GetByStatus(Status status, IContext context)
        {
            throw new NotImplementedException();
        }
    }
}
