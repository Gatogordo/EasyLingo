using Sitecore.Globalization;

namespace TheReference.DotNet.Sitecore.EasyLingo
{
    public enum VersionStatus
    {
        Exists,
        IsFallback,
        None
    }

    public class LanguageVersion
    {
        public Language Language { get; set; }

        public VersionStatus Status { get; set; }
    }
}
