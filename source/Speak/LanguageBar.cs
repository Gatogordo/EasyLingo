using System.Collections.Generic;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.ExperienceEditor.Speak.Ribbon;
using Sitecore.Globalization;
using Sitecore.Mvc.Presentation;
using Sitecore.Web;
using Sitecore.Web.UI.Controls;

namespace TheReference.DotNet.Sitecore.EasyLingo.Speak
{
    public class LanguageBar : RibbonComponentControlBase
    {
        public LanguageBar()
        {
            InitializeControl();
        }

        public LanguageBar(RenderingParametersResolver parametersResolver)
            : base(parametersResolver)
        {
            Assert.ArgumentNotNull(parametersResolver, "parametersResolver");
            InitializeControl();
        }

        protected virtual IList<ComponentBase> Controls { get; set; }

        protected void InitializeControl()
        {
            Class = "sc-languagebar";
            DataBind = "visible: isVisible";
            Requires.Script("easylingo", "LanguageBar.js");
            Requires.Css("easylingo", "LanguageBar.css");
            HasNestedComponents = true;
            Controls = new List<ComponentBase>();
        }

        protected override void PreRender()
        {
            base.PreRender();
            Attributes["data-sc-itemid"] = RibbonDatabase.GetItem(WebUtil.GetQueryString("itemid")).ID.ToString();
            Attributes["data-sc-dic-go"] = Translate.Text("Go");
            Attributes["data-sc-dic-edit"] = Translate.Text("Edit");
            Attributes["data-sc-dic-edit-tooltip"] = Translate.Text("Edit the current page in the language of your choice.");
            Attributes["data-sc-dic-treeview-tooltip"] = Translate.Text("View the available languages for this item.");
        }

        protected override void Render(HtmlTextWriter output)
        {
            base.Render(output);
            AddAttributes(output);
            output.AddAttribute(HtmlTextWriterAttribute.Class, Class);
            output.AddAttribute(HtmlTextWriterAttribute.Id, "languageBarContent" + Attributes["data-sc-itemid"]);
            output.RenderBeginTag("nav");
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.AddAttribute(HtmlTextWriterAttribute.Style, "display=none");
            output.RenderEndTag();
            output.RenderEndTag();
        }
    }
}