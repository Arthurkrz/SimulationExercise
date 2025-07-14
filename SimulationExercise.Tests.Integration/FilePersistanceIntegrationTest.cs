using SimulationExercise.Core.DTOS;
using SimulationExercise.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Tests.Integration
{
    public class FilePersistanceIntegrationTest
    {

    }
}

//using (var assertContext = _contextFactoryMock.Object.Create())
//{
//    var inputFiles = _inputFileRepositoryMock.Object.GetByStatus(Status.New, assertContext);

//    Assert.Equal(2, inputFiles.Count);

//    Assert.False(inputFiles.Any(x => x.Bytes.Length == 0));
//    Assert.False(inputFiles.Any(x => x.Extension != ".csv"));

//    Assert.Equal("CSVTest0.csv", inputFiles[0].Name);
//    Assert.Equal("CSVTest1.csv", inputFiles[1].Name);
//}

//var backupFiles = Directory.GetFiles(_backupDirectoryPath);
//Assert.Equal(2, backupFiles.Length);

//Assert.Equal("CSVTest0.csv", backupFiles[0]);
//Assert.Equal("CSVTest1.csv", backupFiles[1]);

//using (var assertContext = _contextFactoryMock.Object.Create())
//{
//    var errorInputFiles = _inputFileRepositoryMock.Object.GetByStatus(Status.Error, assertContext);

//    foreach (var inputFile in errorInputFiles)
//    {
//        var inputFileErrorMessages = assertContext.Query<InputFileUpdateDTO>(updateValidationSQL, );

//        Assert.Equal("")
//                }
//}

