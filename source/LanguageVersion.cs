namespace TheReference.DotNet.Sitecore.EasyLingo
{
    public enum VersionStatus
    {
        Exists,
        IsFallback,
        None,
        Extra
    }

    public class LanguageVersion
    {
        public VersionStatus Status { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }
    }
}
