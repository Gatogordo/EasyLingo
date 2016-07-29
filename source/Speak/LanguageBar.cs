using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Caches;
using Sitecore.ExperienceEditor.Speak.Ribbon;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using Sitecore.Web;
using Sitecore.Web.UI.Controls;
using System.Collections.Generic;
using System.Web.UI;

namespace TheReference.DotNet.Sitecore.EasyLingo.Speak
{
    public class LanguageBar : RibbonComponentControlBase
    {
        public virtual IList<ComponentBase> Controls { get; set; }

        public LanguageBar()
        {
            this.InitializeControl();
        }

        public LanguageBar(RenderingParametersResolver parametersResolver)
          : base(parametersResolver)
        {
            Assert.ArgumentNotNull((object)parametersResolver, "parametersResolver");
            this.InitializeControl();
        }

        protected void InitializeControl()
        {
            this.Class = "sc-languagebar";
            this.DataBind = "visible: isVisible";
            ResourcesCache.RequireJs((RibbonComponentControlBase)this, "ribbon", "LanguageBar.js");
            ResourcesCache.RequireCss((RibbonComponentControlBase)this, "ribbon", "LanguageBar.css");
            this.HasNestedComponents = true;
            this.Controls = (IList<ComponentBase>)new List<ComponentBase>();
        }

        protected override void PreRender()
        {
            base.PreRender();
            this.Attributes[(object)"data-sc-itemid"] = this.RibbonDatabase.GetItem(WebUtil.GetQueryString("itemid")).ID.ToString();
            this.Attributes[(object)"data-sc-dic-go"] = Translate.Text("Go");
            this.Attributes[(object)"data-sc-dic-edit"] = Translate.Text("Edit");
            this.Attributes[(object)"data-sc-dic-edit-tooltip"] = Translate.Text("Edit the current page in the language of your choice.");
            this.Attributes[(object)"data-sc-dic-treeview-tooltip"] = Translate.Text("View the available languages for this item.");
        }

        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);
            this.AddAttributes(output);
            output.AddAttribute(HtmlTextWriterAttribute.Class, this.Class);
            output.AddAttribute(HtmlTextWriterAttribute.Id, "languageBarContent" + this.Attributes[(object)"data-sc-itemid"]);
            output.RenderBeginTag("nav");
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.AddAttribute(HtmlTextWriterAttribute.Style, "display=none");
            output.RenderEndTag();
            output.RenderEndTag();
        }
    }
}
