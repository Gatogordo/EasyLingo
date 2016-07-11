using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;
using Sitecore.Web;

namespace TheReference.DotNet.Sitecore.EasyLingo
{
    public class LanguageVersionsSection
    {
        private const string SectionName = "Languages";
        private const string IconPath = "Network/16x16/earth.png";
        private const string DefaultLanguageIconPath = "Network/16x16/earth.png";

        private Item CurrentItem { get; set; }
        private Database CurrentDatabase { get; set; }
        private string CurrentLanguage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Is checked by Assert")]
        public void Process(RenderContentEditorArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var parentControl = args.Parent;
            var editorFormatter = args.EditorFormatter;
            CurrentItem = args.Item;

            if (CurrentItem == null)
            {
                return;
            }

            CurrentDatabase = CurrentItem.Database;
            CurrentLanguage = CurrentItem.Language.Name;
            var languageVersions = GetLanguageVersions(CurrentItem);
            editorFormatter.RenderSectionBegin(parentControl, SectionName, SectionName, SectionName, IconPath, false, true);
            RenderLanguageVersions(languageVersions, editorFormatter, parentControl);
            editorFormatter.RenderSectionEnd(parentControl, true, false);
        }

        private IEnumerable<LanguageVersion> GetLanguageVersions(Item sitecoreItem)
        {
            var languageList = new List<LanguageVersion>();
            var allowedLanguages = GetAllowedLanguages().ToList();
            foreach (var language in sitecoreItem.Languages)
            {
                if (!allowedLanguages.Contains(language.Name, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                using (new LanguageSwitcher(language))
                {
                    var translatedItem = CurrentDatabase.GetItem(sitecoreItem.ID, language);
                    if (translatedItem?.Versions != null && translatedItem.Versions.Count > 0)
                    {
                        languageList.Add(new LanguageVersion {
                            Language = language,
                            Status = translatedItem.IsFallback ? VersionStatus.IsFallback : VersionStatus.Exists });
                    }
                    else
                    {
                        languageList.Add(new LanguageVersion
                        {
                            Language = language,
                            Status = VersionStatus.None
                        });
                    }
                }
            }

            languageList = languageList.OrderBy(l => l.Language.Name).ToList();
            return languageList;
        }

        private void RenderLanguageVersions(IEnumerable<LanguageVersion> languageVersions, EditorFormatter editorFormatter, Control parentControl)
        {
            var languages = languageVersions.ToList();
            using (new SecurityDisabler())
            {
                editorFormatter.AddLiteralControl(parentControl, "<table><tr>");
                editorFormatter.AddLiteralControl(parentControl, "<td style=\"padding-bottom:10px\"><span style=\"margin:0px 27px 0px 5px;display:inline-block;\">Version/<span style=\"font-style:italic\">Fallback</span>:&nbsp;</span></td><td style=\"padding-bottom:10px\">");
                foreach (var languageVersion in languages.Where(l => l.Status == VersionStatus.Exists))
                {
                    var htmlChunk = GetLanguageControl(languageVersion.Language, string.Empty);
                    if (!string.IsNullOrEmpty(htmlChunk))
                    {
                        editorFormatter.AddLiteralControl(parentControl, htmlChunk);
                    }
                }

                editorFormatter.AddLiteralControl(parentControl, "<span style=\"margin:0px 27px 0px 20px;display:inline-block;\">&nbsp;</span>");

                foreach (var languageVersion in languages.Where(l => l.Status == VersionStatus.IsFallback))
                {
                    var htmlChunk = GetLanguageControl(languageVersion.Language, "font-style:italic");
                    if (!string.IsNullOrEmpty(htmlChunk))
                    {
                        editorFormatter.AddLiteralControl(parentControl, htmlChunk);
                    }
                }

                var noVersions = languages.Where(l => l.Status == VersionStatus.None).ToList();
                if (noVersions.Any())
                {
                    editorFormatter.AddLiteralControl(parentControl, "</td></tr><tr><td style=\"padding-bottom:10px\"><span style=\"margin:0px 27px 0px 5px;display:inline-block;\">No version:&nbsp;</span></td><td style=\"padding-bottom:10px\">");
                    foreach (var languageVersion in noVersions)
                    {
                        var htmlChunk = GetLanguageControl(languageVersion.Language, string.Empty);
                        if (!string.IsNullOrEmpty(htmlChunk))
                        {
                            editorFormatter.AddLiteralControl(parentControl, htmlChunk);
                        }
                    }
                }

                editorFormatter.AddLiteralControl(parentControl, "</td></tr></table>");
            }
        }

        private string GetLanguageControl(Language language, string style)
        {
            if (language.Origin == null || ID.IsNullOrEmpty(language.Origin.ItemId))
            {
                return null;
            }

            var languageItem = CurrentDatabase.GetItem(language.Origin.ItemId);

            var languageIconPath = languageItem != null ? languageItem.Appearance.Icon : DefaultLanguageIconPath;
            var languageIconUrl = GetIconUrl(languageIconPath);

            var languageName = languageItem != null ? languageItem.Name : language.Name;
            if (languageName.Equals(CurrentLanguage, StringComparison.OrdinalIgnoreCase))
            {
                style += "font-weight:bold";
            }

            var link = $"javascript: return scForm.postEvent(this,event,'item:load(id={CurrentItem.ID.Guid.ToString("B").ToUpperInvariant()},language={language.Name},version=0)')";
            var htmlChunk = $"<span style=\"margin-right:7px;display:inline-block;cursor:pointer;{style}\" onClick=\"{link}\">";
            htmlChunk += $"<img src=\"{languageIconUrl}\" style=\"vertical-align: text-bottom;\" />&nbsp;{languageName}";
            htmlChunk += "</span>";

            return htmlChunk;
        }

        private static string GetIconUrl(string iconPath)
        {
            iconPath = iconPath.Replace("24x24", "16x16").Replace("32x32", "16x16").Replace("48x48", "16x16");
            return $"/temp/IconCache/{iconPath}";
        }

        private IEnumerable<string> GetAllowedLanguages()
        {
            var sites = GetSites(CurrentItem);
            var result = new List<string>();
            foreach (var site in sites)
            {
                var allowedLanguages = site.Properties[LanguageResolver.LanguagesProperty];
                if (!string.IsNullOrEmpty(allowedLanguages))
                {
                    result.AddRange(allowedLanguages.Split(','));
                }
            }

            return result.Any() ? 
                result.Distinct(StringComparer.OrdinalIgnoreCase) : 
                LanguageManager.GetLanguages(CurrentDatabase).Select(l => l.Name);
        }

        private static IEnumerable<SiteInfo> GetSites(Item item)
        {
            return global::Sitecore.Configuration.Factory.GetSiteInfoList().Where(s => !string.IsNullOrEmpty(s.RootPath) && item.Paths.FullPath.StartsWith(s.RootPath));
        }
    }
}
