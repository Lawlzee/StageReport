using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StageReport
{
    public static class ModConfig
    {
        public static ConfigEntry<bool> modEnabled;
        public static ConfigEntry<bool> revealInteractableOnStageEnd;
        public static Dictionary<InteractableType, ConfigEntry<bool>> visibleInteractables = new Dictionary<InteractableType, ConfigEntry<bool>>();
        public static Dictionary<InteractableType, ConfigEntry<int>> interactablesScore = new Dictionary<InteractableType, ConfigEntry<int>>();

        public static void Init(ConfigFile config)
        {
            modEnabled = config.Bind("Configuration", "Mod enabled", true, "Mod enabled");
            ModSettingsManager.AddOption(new CheckBoxOption(modEnabled));

            revealInteractableOnStageEnd = config.Bind("Configuration", "Reveal interactables on stage end", true, "When the stage recap is shown at the end of the stage, reveal all interactables on the stage, like the Radar Scanner.");
            ModSettingsManager.AddOption(new CheckBoxOption(revealInteractableOnStageEnd));

            var interactableDefs = InteractablesCollection.instance.interactables;

            for (int i = 0; i < interactableDefs.Length; i++)
            {
                var interactableDef = interactableDefs[i];
                string name = string.Join(
                    " ",
                    interactableDef.nameToken.Split(' ')
                        .Select(Language.GetString));
                string normalisedName = NormaliseName(name);

                var visibleConfig = config.Bind("Visible", normalisedName, true, $"Is <style=cIsHealing>{name}</style> shown in the stage recap?");
                ModSettingsManager.AddOption(new CheckBoxOption(visibleConfig));
                visibleInteractables[interactableDef.type] = visibleConfig;

                var scoreConfig = config.Bind("Score", normalisedName, interactableDef.defaultScoreWeight, $"How many points does <style=cIsHealing>{name}</style> add  to the stage completion percentage?");
                ModSettingsManager.AddOption(new IntSliderOption(scoreConfig, new IntSliderConfig { min = 0, max = 100 }));
                interactablesScore[interactableDef.type] = scoreConfig;
            }

            string NormaliseName(string name)
            {
                //'=', '\n', '\t', '\\', '"', '\'', '[', ']'
                return Regex.Replace(name, @"[=\n\t\\""'\[\]]", "").Trim();
            }
        }
    }
}
