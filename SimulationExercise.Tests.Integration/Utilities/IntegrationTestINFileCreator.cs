namespace SimulationExercise.Tests.Integration.Utilities
{
    public class IntegrationTestINFileCreator
    {
        public void CreateINFiles(string inDirectoryPath, int numberOfFilesToBeCreated, Stream inputFileStream)
        {
            if (!Directory.Exists(inDirectoryPath))
                Directory.CreateDirectory(inDirectoryPath);

            for (int objectNumber = 0; objectNumber < numberOfFilesToBeCreated; objectNumber++)
            {
                string importFilePath = Path.Combine(inDirectoryPath, 
                    $"INTestFile{objectNumber}.csv");

                using var fileStream = new FileStream(importFilePath, FileMode.Create, FileAccess.Write);
                inputFileStream.CopyTo(fileStream);
                inputFileStream.Position = 0;
            }
        }
    }
}