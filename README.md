## CSVReader
Project developed in .NET 8.0 with the purpose of processing CSV files. The system reads data from CSV files, processes it to calculate averages, exports results to a CSV output file, and persists records in a SQL Server database using Dapper.

## Project Structure
The project is composed by the following layers: 
 - **CSVReader.Architecture** - Contains repository classes for objects created in the workflow, along with configuration classes for Dapper;
 - **CSVReader.Console** - Contains application entrypoint, console menu setup and logging configuration;
 - **CSVReader.Core** - Contains entities, contracts (factories, infrastructure, repositories and services), DTOs (export and database), enumerators, validators (FluentValidation) and common log messages;
 - **CSVReader.IOC** - Contains Dependency Injection;
 - **CSVReader.Services** - Contains classes specialized in analysis, creation and validation of objects, along with handlers for menu options;
 - **CSVReader.Tests** - Contains unit tests for service classes, validators and factories;
 - **CSVReader.Tests.Integration** - Contains integration test for repository and service classes.

## Workflow
 **Check pipeline.png for workflow information.**

## Functionalities
 - Capture of values of environmental data in CSV files with FileHelpers;
 - Validation of CSV structure, data content and created objects with FluentValidation;
 - Persistance of data in database ensuring traceability, along with persistance of errors in database when processing objects;
 - Export of results (average per province) in CSV files and error logs;
 - Automatic creation of tables when initializing the system.
	
## Configuration & Execution
### 1. Database configuration:
 - Install SQL Server
 - Create the databases and tables manually using SSMS or Object Explorer from Visual Studio, applying the queries present in the file **queries.txt** (ready for copy and paste)

```sql

CREATE DATABASE [CSVReaderDatabase]
CREATE DATABASE [CSVReaderTestDatabase]

USE CSVReaderDatabase

IF OBJECT_ID('InputFile', 'U') IS NULL 
CREATE TABLE InputFile (
InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, 
Name NVARCHAR(100) NOT NULL, 
Extension VARCHAR(10) NOT NULL, 
Bytes VARBINARY(MAX) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
StatusId INT NOT NULL);

IF OBJECT_ID('InputFileMessage', 'U') IS NULL 
CREATE TABLE InputFileMessage(
InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, 
InputFileId BIGINT NOT NULL REFERENCES InputFile(InputFileId), 
CreationDate DATETIME NOT NULL, 
LastUpdateDate DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
Message NVARCHAR(MAX) NOT NULL);

IF OBJECT_ID('Reading', 'U') IS NULL 
CREATE TABLE Reading(
ReadingId BIGINT IDENTITY(1,1) PRIMARY KEY, 
InputFileId BIGINT NOT NULL REFERENCES dbo.InputFile(InputFileId), 
SensorId BIGINT NOT NULL, 
SensorTypeName NVARCHAR(100) NOT NULL, 
Unit NVARCHAR(10) NOT NULL, 
StationId BIGINT NOT NULL, 
StationName NVARCHAR(100) NOT NULL, 
Value INT NOT NULL, 
Province NVARCHAR(100) NOT NULL, 
City NVARCHAR(100) NOT NULL, 
IsHistoric BIT NOT NULL, 
StartDate DATETIME NOT NULL, 
StopDate DATETIME, 
UtmNord INT NOT NULL, 
UtmEst INT NOT NULL, 
Latitude NVARCHAR(100) NOT NULL, 
Longitude NVARCHAR(100) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
StatusId INT NOT NULL);

IF OBJECT_ID('ReadingMessage', 'U') IS NULL 
CREATE TABLE ReadingMessage(
ReadingMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, 
ReadingId BIGINT NOT NULL REFERENCES dbo.Reading(ReadingId), 
CreationDate DATETIME NOT NULL, 
LastUpdateDate DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
Message NVARCHAR(MAX) NOT NULL);

IF OBJECT_ID('ConsistentReading', 'U') IS NULL 
CREATE TABLE ConsistentReading(
ConsistentReadingId BIGINT IDENTITY(1,1) PRIMARY KEY, 
ReadingId BIGINT NOT NULL REFERENCES dbo.Reading(ReadingId), 
SensorId BIGINT NOT NULL, 
SensorTypeName NVARCHAR(100) NOT NULL, 
Unit INT NOT NULL, 
Value INT NOT NULL, 
Province NVARCHAR(100) NOT NULL, 
City NVARCHAR(100) NOT NULL, 
IsHistoric BIT NOT NULL, 
DaysOfMeasure INT NOT NULL, 
UtmNord INT NOT NULL, 
UtmEst INT NOT NULL, 
Latitude NVARCHAR(100) NOT NULL, 
Longitude NVARCHAR(100) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
IsExported BIT NOT NULL);

IF OBJECT_ID('OutputFile', 'U') IS NULL 
CREATE TABLE OutputFile(
OutputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, 
Name NVARCHAR(100) NOT NULL, 
Bytes VARBINARY(MAX) NOT NULL, 
Extension VARCHAR(10) NOT NULL, 
ObjectType NVARCHAR(50) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
IsExported BIT NOT NULL);

IF OBJECT_ID('AverageProvinceData', 'U') IS NULL 
CREATE TABLE AverageProvinceData(
AverageProvinceDataId BIGINT IDENTITY(1,1) PRIMARY KEY, 
Province NVARCHAR(100) NOT NULL, 
SensorTypeName NVARCHAR(100) NOT NULL, 
AverageValue FLOAT NOT NULL, 
Unit NVARCHAR(10) NOT NULL, 
AverageDaysOfMeasure INT NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
IsExported BIT NOT NULL);

GO

USE CSVReaderTestDatabase

IF OBJECT_ID('InputFile', 'U') IS NULL 
CREATE TABLE InputFile (
InputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, 
Name NVARCHAR(100) NOT NULL, 
Extension VARCHAR(10) NOT NULL, 
Bytes VARBINARY(MAX) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
StatusId INT NOT NULL);

IF OBJECT_ID('InputFileMessage', 'U') IS NULL 
CREATE TABLE InputFileMessage(
InputFileMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, 
InputFileId BIGINT NOT NULL REFERENCES InputFile(InputFileId), 
CreationDate DATETIME NOT NULL, 
LastUpdateDate DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
Message NVARCHAR(MAX) NOT NULL);

IF OBJECT_ID('Reading', 'U') IS NULL 
CREATE TABLE Reading(
ReadingId BIGINT IDENTITY(1,1) PRIMARY KEY, 
InputFileId BIGINT NOT NULL REFERENCES dbo.InputFile(InputFileId), 
SensorId BIGINT NOT NULL, 
SensorTypeName NVARCHAR(100) NOT NULL, 
Unit NVARCHAR(10) NOT NULL, 
StationId BIGINT NOT NULL, 
StationName NVARCHAR(100) NOT NULL, 
Value INT NOT NULL, 
Province NVARCHAR(100) NOT NULL, 
City NVARCHAR(100) NOT NULL, 
IsHistoric BIT NOT NULL, 
StartDate DATETIME NOT NULL, 
StopDate DATETIME, 
UtmNord INT NOT NULL, 
UtmEst INT NOT NULL, 
Latitude NVARCHAR(100) NOT NULL, 
Longitude NVARCHAR(100) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
StatusId INT NOT NULL);

IF OBJECT_ID('ReadingMessage', 'U') IS NULL 
CREATE TABLE ReadingMessage(
ReadingMessageId BIGINT IDENTITY(1,1) PRIMARY KEY, 
ReadingId BIGINT NOT NULL REFERENCES dbo.Reading(ReadingId), 
CreationDate DATETIME NOT NULL, 
LastUpdateDate DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
Message NVARCHAR(MAX) NOT NULL);

IF OBJECT_ID('ConsistentReading', 'U') IS NULL 
CREATE TABLE ConsistentReading(
ConsistentReadingId BIGINT IDENTITY(1,1) PRIMARY KEY, 
ReadingId BIGINT NOT NULL REFERENCES dbo.Reading(ReadingId), 
SensorId BIGINT NOT NULL, 
SensorTypeName NVARCHAR(100) NOT NULL, 
Unit INT NOT NULL, 
Value INT NOT NULL, 
Province NVARCHAR(100) NOT NULL, 
City NVARCHAR(100) NOT NULL, 
IsHistoric BIT NOT NULL, 
DaysOfMeasure INT NOT NULL, 
UtmNord INT NOT NULL, 
UtmEst INT NOT NULL, 
Latitude NVARCHAR(100) NOT NULL, 
Longitude NVARCHAR(100) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
IsExported BIT NOT NULL);

IF OBJECT_ID('OutputFile', 'U') IS NULL 
CREATE TABLE OutputFile(
OutputFileId BIGINT IDENTITY(1,1) PRIMARY KEY, 
Name NVARCHAR(100) NOT NULL, 
Bytes VARBINARY(MAX) NOT NULL, 
Extension VARCHAR(10) NOT NULL, 
ObjectType NVARCHAR(50) NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
IsExported BIT NOT NULL);

IF OBJECT_ID('AverageProvinceData', 'U') IS NULL 
CREATE TABLE AverageProvinceData(
AverageProvinceDataId BIGINT IDENTITY(1,1) PRIMARY KEY, 
Province NVARCHAR(100) NOT NULL, 
SensorTypeName NVARCHAR(100) NOT NULL, 
AverageValue FLOAT NOT NULL, 
Unit NVARCHAR(10) NOT NULL, 
AverageDaysOfMeasure INT NOT NULL, 
CreationTime DATETIME NOT NULL, 
LastUpdateTime DATETIME NOT NULL, 
LastUpdateUser NVARCHAR(100) NOT NULL, 
IsExported BIT NOT NULL);

GO

```

### 2. Project Configuration:
 - Execute the command `dotnet restore` to restore NuGet packages;
 - Execute the command `dotnet test`