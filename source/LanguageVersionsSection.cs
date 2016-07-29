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

        private Item CurrentItem { get; set; }
        
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

            CurrentLanguage = CurrentItem.Language.Name;
            var languageVersions = LanguageContainer.GetAllowedLanguageVersions(CurrentItem);
            editorFormatter.RenderSectionBegin(parentControl, SectionName, SectionName, SectionName, IconPath, false, true);
            RenderLanguageVersions(languageVersions, editorFormatter, parentControl);
            editorFormatter.RenderSectionEnd(parentControl, true, false);
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
                    var htmlChunk = GetLanguageControl(languageVersion, string.Empty);
                    if (!string.IsNullOrEmpty(htmlChunk))
                    {
                        editorFormatter.AddLiteralControl(parentControl, htmlChunk);
                    }
                }

                editorFormatter.AddLiteralControl(parentControl, "<span style=\"margin:0px 27px 0px 20px;display:inline-block;\">&nbsp;</span>");

                foreach (var languageVersion in languages.Where(l => l.Status == VersionStatus.IsFallback))
                {
                    var htmlChunk = GetLanguageControl(languageVersion, "font-style:italic");
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
                        var htmlChunk = GetLanguageControl(languageVersion, string.Empty);
                        if (!string.IsNullOrEmpty(htmlChunk))
                        {
                            editorFormatter.AddLiteralControl(parentControl, htmlChunk);
                        }
                    }
                }

                editorFormatter.AddLiteralControl(parentControl, "</td></tr></table>");
            }
        }

        private string GetLanguageControl(LanguageVersion languageVersion, string style)
        {
            if (!languageVersion.HasOrigin)
            {
                return null;
            }

            if (languageVersion.Name.Equals(CurrentLanguage, StringComparison.OrdinalIgnoreCase))
            {
                style += "font-weight:bold";
            }

            var link = $"javascript: return scForm.postEvent(this,event,'item:load(id={CurrentItem.ID.Guid.ToString("B").ToUpperInvariant()},language={languageVersion.Name},version=0)')";
            var htmlChunk = $"<span style=\"margin-right:7px;display:inline-block;cursor:pointer;{style}\" onClick=\"{link}\">";
            htmlChunk += $"<img src=\"{languageVersion.Icon}\" style=\"vertical-align: text-bottom;\" />&nbsp;{languageVersion.Name}";
            htmlChunk += "</span>";

            return htmlChunk;
        }



    }
}
