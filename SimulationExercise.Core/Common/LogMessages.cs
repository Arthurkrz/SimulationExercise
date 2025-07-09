namespace SimulationExercise.Core.Common
{
    public static class LogMessages
    {
        public const string INVALIDHEADER = "Invalid header values!";
        public const string EMPTYFILE = "File is empty!";
        public const string NOREADINGSFOUND = "No readings found in file!";
        public const string NONEWOBJECTSFOUND = "No new {object}s have been found!";

        public const string CONTINUETONEXTFILE = "Continuing to next file (if exists)...\n";

        public const string EXPORTAVERAGEPROVINCEDATA = "Exporting average province data...";
        public const string IMPORTREADING = "Importing readings...";
        public const string CREATEOBJECT = "Creating {object}s...";

        public const string OBJECTCREATIONSUCCESS = "{count} {object} created successfully!";

        public const string NOREADINGIMPORTED = "No readings have been imported from file {0}!";
        public const string NOOBJECTCREATED = "No {object}s have been created!";
        public const string NOCSVFILESFOUND = "No CSV files found in the 'IN' directory!";
        public const string ERRORSFOUND = "Errors found! ({errorGroupNumber})";
    }
}