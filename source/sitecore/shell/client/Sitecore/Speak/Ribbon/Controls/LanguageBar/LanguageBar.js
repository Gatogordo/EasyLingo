define(
  [
    "sitecore",
    "/-/speak/v1/ExperienceEditor/RibbonPageCode.js",
    "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"
  ],
function (Sitecore, RibbonPageCode, ExperienceEditor) {
    Sitecore.Factories.createBaseComponent({
        name: "RibbonLanguageBar",
        base: "ControlBase",
        selector: ".sc-languagebar",
        attributes: [
        ],

        allowedLanguageVersions: [],
        disallowedLanguageVersions: [],

        initialize: function () {
            document.languageBarContext = this;
            //ExperienceEditor.getPageEditingWindow().document.languageBarContext = this;
            window.parent.document.breadcrumbContext = this;
            var mode = ExperienceEditor.Web.getUrlQueryStringValue("mode");
            this.model.on("change:isVisible", this.renderLanguageBar, this);
            ExperienceEditor.Common.registerDocumentStyles(["/-/speak/v1/ribbon/LanguageBar.css"], window.parent.document);
        },

        //Render the entire language bar contents
        renderLanguageBar: function (itemId) {
            if (!itemId
              || typeof (itemId) == "object") {
                itemId = this.$el[0].attributes["data-sc-itemid"].value;
            }

            //Retrieve language versions
            this.requestAllowedLanguageVersions(itemId, this);
            this.requestDisallowedLanguageVersions(itemId, this);

            //Build HTML
            var htmlSource = "<div class=\"sc-languagebar\">";

            //ALLOWED LANGUAGES
            htmlSource += "<div class=\"sc-languagebar-section\">";

            //Available versions
            htmlSource += "<div class=\"sc-languagebar-title\">Version/<span class=\"italic\">Fallback</span>:</div>";
            var versioned = this.allowedLanguageVersions.filter(function (x) { return x.Status == 0; });
            htmlSource += "<div class=\"sc-languagebar-items\">";
            for (var i = 0; i < versioned.length; i++) {
                htmlSource += this.generateLanguageVersionHtml(versioned[i]);
            }
            htmlSource += "</div>";

            //Fallback versions
            var fallbacks = this.allowedLanguageVersions.filter(function (x) { return x.Status == 1; });
            if (fallbacks.length > 0) {
                htmlSource += "<div class=\"margin\">&nbsp;</div>";
                htmlSource += "<div class=\"sc-languagebar-items italic\">";
                for (var i = 0; i < fallbacks.length; i++) {
                    htmlSource += this.generateLanguageVersionHtml(fallbacks[i]);
                }
                htmlSource += "</div>";
            }
            htmlSource += "</div>";

            //No versions
            var unversioned = this.allowedLanguageVersions.filter(function (x) { return x.Status == 2; });
            if (unversioned.length > 0) {
                htmlSource += "<div class=\"sc-languagebar-section\">";
                htmlSource += "<div class=\"sc-languagebar-title\">No version:</div>";
                htmlSource += "<div class=\"sc-languagebar-items\">";
                for (var i = 0; i < unversioned.length; i++) {
                    htmlSource += this.generateLanguageVersionHtml(unversioned[i]);
                }
                htmlSource += "</div>";
                htmlSource += "</div>";
            }

            //DISALLOWED LANGUAGES
            htmlSource += "<div class=\"sc-languagebar-section\">";
            //Available versions
            var disallowedversions = this.disallowedLanguageVersions.filter(function (x) { return x.Status == 0; });
            if (disallowedversions.length > 0) {
                htmlSource += "<div class=\"sc-languagebar-title\">Other available languages:</div>";
                htmlSource += "<div class=\"sc-languagebar-items\">";
                for (var i = 0; i < disallowedversions.length; i++) {
                    htmlSource += this.generateLanguageVersionHtml(disallowedversions[i]);
                }
                htmlSource += "</div>";
            }
            htmlSource += "</div>";
            htmlSource += "</div>";

            //Assign HTML to bar div
            var languageBarContent = ExperienceEditor.ribbonDocument().getElementById("languageBarContent" + this.$el[0].attributes["data-sc-itemid"].value);
            languageBarContent.innerHTML = htmlSource;
        },

        //Request and retrieve the languageVersions
        requestAllowedLanguageVersions: function (itemId, appContext) {
            var context = ExperienceEditor.generateDefaultContext();
            context.currentContext.itemId = itemId;
            ExperienceEditor.PipelinesUtil.generateRequestProcessor("EasyLingo.LanguageBar.GetAllowedLanguageVersions", function (response) {
                appContext.allowedLanguageVersions = response.responseValue.value;
            }).execute(context);
        },

        //Request and retrieve the languageVersions
        requestDisallowedLanguageVersions: function (itemId, appContext) {
            var context = ExperienceEditor.generateDefaultContext();
            context.currentContext.itemId = itemId;
            ExperienceEditor.PipelinesUtil.generateRequestProcessor("EasyLingo.LanguageBar.GetDisallowedLanguageVersions", function (response) {
                appContext.disallowedLanguageVersions = response.responseValue.value;
            }).execute(context);
        },

        //Generate HTML per language version entry
        generateLanguageVersionHtml: function (item) {
            var context = ExperienceEditor.generateDefaultContext();
            var onclickFunction = "javascript:document.languageBarContext.changeLanguage('" + item.Name + "');";
            var htmlSource = "<a class=\"sc-languagebar-item\" href=\"" + onclickFunction + "\" >";
            if (context.currentContext.language == item.Name) {
                htmlSource += "<strong>";
            }
            htmlSource += "<img src=\"" + item.Icon + "\" style=\"vertical-align: text-bottom;\" />&nbsp;" + item.Name;
            if (context.currentContext.language == item.Name) {
                htmlSource += "</strong>";
            }
            htmlSource += "</a>";
            return htmlSource;
        },

        //Language change call
        changeLanguage: function (language) {
            var context = ExperienceEditor.generateDefaultContext();
            context.currentContext.value = encodeURIComponent(language) + "|" + context.currentContext.ribbonUrl;
            ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.Language.ChangeLanguage", function (response) {
                var newLanguageUrl = ExperienceEditor.Web.removeQueryStringParameter(response.responseValue.value, "sc_version");
                window.parent.document.location = newLanguageUrl + "?sc_mode=edit";
                //Sitecore 8.2 and onwards: ExperienceEditor.getPageEditingWindow().location = newLanguageUrl + "?sc_mode=edit";
            }).execute(context);
        },
    });
});