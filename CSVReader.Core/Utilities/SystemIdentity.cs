using System.Security.Principal;

namespace CSVReader.Core.Utilities
{
    public static class SystemIdentity
    {
        public static Func<string?> CurrentName = () => WindowsIdentity.GetCurrent()?.Name;
    }
}
