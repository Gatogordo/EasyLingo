using Sitecore.Data;

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
        public VersionStatus Status { get; set; }

        public string Name { get; set; }

        public bool HasOrigin => !ID.IsNullOrEmpty(Origin);

        public ID Origin { get; set; }

        public string Icon { get; set; }

    }
}
