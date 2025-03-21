using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Core.Contracts
{
    public interface IReadingImportService
    {
        ImportResult Import(Stream stream);
    }
}
