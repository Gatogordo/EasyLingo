define(["sitecore", "/-/speak/v1/ExperienceEditor/ExperienceEditor.js"], function (Sitecore, ExperienceEditor) {
  Sitecore.Commands.ShowLanguageBar =
  {
    canExecute: function (context) {
      context.app.RibbonLanguageBar.set("isVisible", context.button.get("isChecked") == "1");
      context.app.setHeight();
      return true;
    },
    execute: function (context) {
      ExperienceEditor.PipelinesUtil.generateRequestProcessor("ExperienceEditor.ToggleRegistryKey.Toggle", function (response) {
        response.context.button.set("isChecked", response.responseValue.value ? "1" : "0");
        response.context.app.RibbonLanguageBar.set("isVisible", response.responseValue.value);
        response.context.app.setHeight();
      }, { value: context.button.get("registryKey") }).execute(context);
    }
  };
});