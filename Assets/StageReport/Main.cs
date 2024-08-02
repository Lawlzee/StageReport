﻿using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace StageReport
{
    [BepInDependency("com.rune580.riskofoptions")]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "Lawlzee.StageReport";
        public const string PluginAuthor = "Lawlzee";
        public const string PluginName = "StageReport";
        public const string PluginVersion = "1.0.0";

        public void Awake()
        {
            Log.Init(Logger);

            //ModConfig.Init(Config);

            var texture = LoadTexture("icon.png");
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
            ModSettingsManager.SetModIcon(sprite);
            //ModSettingsManager.SetModDescription("Procedural Stages replaces conventional static terrains with procedurally generated environments, offering a fresh and varied experience with each stage while striving to maintain the familiar feel of vanilla stages.");

            /*
            PauseMenu.Init();

            BazaarHooks.Init();
            ConfigHooks.Init();
            StageHooks.Init();
            */

            ContentManager.collectContentPackProviders += GiveToRoR2OurContentPackProviders;
        }

        private Texture2D LoadTexture(string name)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(File.ReadAllBytes(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), name)));
            return texture;
        }

        private void GiveToRoR2OurContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new ContentProvider());
        }
    }
}