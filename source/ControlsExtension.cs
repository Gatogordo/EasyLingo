using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;
using System.Web;
using Sitecore.Mvc;
using TheReference.DotNet.Sitecore.EasyLingo.Speak;

namespace TheReference.DotNet.Sitecore.EasyLingo
{
    public static class ControlsExtension
    {
        public static HtmlString LanguageBar(this Controls controls, Rendering rendering)
        {
            Assert.ArgumentNotNull((object)controls, "controls");
            Assert.ArgumentNotNull((object)rendering, "rendering");
            return new HtmlString(new LanguageBar(controls.GetParametersResolver(rendering)).Render());
        }
    }
}
