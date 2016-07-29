using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;
using Sitecore.Globalization;
using System.Collections.Generic;

namespace TheReference.DotNet.Sitecore.EasyLingo.Speak
{
    public class GetDisallowedLanguageVersions : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            if (this.RequestContext.Item == null)
                return new PipelineProcessorResponseValue()
                {
                    AbortMessage = Translate.Text("The target item could not be found.")
                };
            
            IEnumerable<LanguageVersion> languageVersions = LanguageContainer.GetDisallowedLanguageVersions(this.RequestContext.Item);
            return new PipelineProcessorResponseValue()
            {
                Value = (object)languageVersions
            };
        }
    }
}
