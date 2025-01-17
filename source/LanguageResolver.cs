using System;
using System.Linq;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Web;

namespace TheReference.DotNet.Sitecore.EasyLingo
{
    public class LanguageResolver : HttpRequestProcessor
    {
        internal const string LanguagesProperty = "allowedLanguages";

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Is checked by Assert")]
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (!Context.PageMode.IsNormal)
            {
                return;
            }

            if (Context.Language == null)
            {
                return;
            }

            var url = WebUtil.GetRawUrl();
            if (IsSitecoreUrl(url))
            {
                return;
            }

            var allowedLanguages = Context.Site.Properties[LanguagesProperty];
            if (string.IsNullOrEmpty(allowedLanguages))
            {
                return;
            }

            var configuredAllowedLanguages = allowedLanguages.Split(',').ToList();
            if (!configuredAllowedLanguages.Any())
            {
                return;
            }

            if (!configuredAllowedLanguages.Contains(Context.Language.Name))
            {
                args.HttpContext.Response.RedirectPermanent("/" + configuredAllowedLanguages.First().ToLowerInvariant());
            }
        }

        private static bool IsSitecoreUrl(string url)
        {
            if (url.StartsWith("/" + Settings.Media.MediaLinkPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (url.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("/clientevent", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("/layouts/system", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("/temp/", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (url.EndsWith(".js", StringComparison.OrdinalIgnoreCase) || url.EndsWith(".css", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }
    }
}
