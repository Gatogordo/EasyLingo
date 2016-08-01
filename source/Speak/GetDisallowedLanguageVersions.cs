using System.Linq;
using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Globalization;

namespace TheReference.DotNet.Sitecore.EasyLingo.Speak
{
    public class GetDisallowedLanguageVersions : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            if (RequestContext.Item == null)
            {
                return new PipelineProcessorResponseValue
                           {
                               AbortMessage = Translate.Text("The target item could not be found.")
                           };
            }

            var languageVersions = LanguageContainer.GetLanguageVersions(RequestContext.Item).Where(i => i.Status == VersionStatus.Extra);
            return new PipelineProcessorResponseValue
            {
                Value = languageVersions
            };
        }
    }
}
