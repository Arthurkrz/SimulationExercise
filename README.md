## SimulationExercise
Project developed in C# using .NET 8 with the purpose of simulating a real-world business scenario in an educational context.  
The system reads data from CSV files, processes it to calculate averages, exports results to a CSV output file, and persists records in a SQL Server database using Dapper.

## Project Structure
The project is composed by the following layers:
 - **SimulationExercise.Architecture** - Contains SQL Server configuration and Dapper implementation;
 - **SimulationExercise.Console** - Contains application entrypoint and logging configuration;
 - **SimulationExercise.Core** - Contains entities and contract interfaces;
 - **SimulationExercise.IOC** - Contains startup for dependency injection;
 - **SimulationExercise.Services** - Contains bussiness logic;
 - **SimulationExercise.Tests** - Contains unit tests for service classes;
 - **SimulationExercise.Tests.Integration** - Contains integration tests for repository and service layers.

## Functionalities
 - Reading of CSV files containing environmental data;
 - Validation of CSV structure and data content;
 - Traceability by data persistance in database;
 - Automatic table creation in database;
 - Result (average of values per province) export in CSV file;
 - Error message export in output file (Errors.log);
 - Automated service and repository tests.

## Configuration & Execution
### 1. Database configuration:
  - Install SQL Server (LocalDB or Express);
  - Create the database and test database manually using SQL Server Management Studio or Object Explorer from Visual Studio, applying the following query:

	```sql
	(QUERY FOR DB CREATION)
	
	(QUERY FOR TEST DB CREATION)
	```
### 2. Project Configuration:
  - Execute the command `dotnet restore` to restore NuGet packages;
  - Execute the command `dotnet test`.
