using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Tests.Integration.Repository
{
    public class RepositoryTestFixture
    {
        public RepositoryTestFixture() 
        {
            var initializer = new RepositoryTestInitializer();
            initializer.Initialize();
        }
    }
}
