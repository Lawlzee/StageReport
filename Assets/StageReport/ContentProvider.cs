using R2API;
using RoR2;
using RoR2.ContentManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace ProceduralStages
{
    public class ContentProvider : IContentPackProvider
    {
        public string identifier => Main.PluginGUID + "." + nameof(ContentProvider);

        public static string assetDirectory;

        public static ContentPack ContentPack = new ContentPack();

        public ContentProvider()
        {
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            ContentPack.identifier = identifier;

            var assetsFolderFullPath = System.IO.Path.GetDirectoryName(typeof(ContentProvider).Assembly.Location);
            assetDirectory = assetsFolderFullPath;

            AssetBundle stageReportBundle = null;
            yield return LoadAssetBundle(
                System.IO.Path.Combine(assetsFolderFullPath, "stageReport"),
                args.progressReceiver,
                (assetBundle) => stageReportBundle = assetBundle);

            //yield return LoadAllAssetsAsync(assetsBundle, args.progressReceiver, (Action<Texture[]>)((assets) =>
            //{
            //    texOpenCavePreview = assets.First(a => a.name == "texOpenCavePreview");
            //    texTunnelCavesPreview = assets.First(a => a.name == "texTunnelCavesPreview");
            //    texIslandPreview = assets.First(a => a.name == "texIslandPreview");
            //    texCanyonsPreview = assets.First(a => a.name == "texCanyonsPreview");
            //    texBasaltPreview = assets.First(a => a.name == "texBasaltPreview");
            //    texBlockMazePreview = assets.First(a => a.name == "texBlockMazePreview");
            //    texTemplePreview = assets.First(a => a.name == "texTemplePreview");
            //}));
            //
            //yield return LoadAllAssetsAsync(assetsBundle, args.progressReceiver, (Action<GameObject[]>)((assets) =>
            //{
            //    runConfigPrefab = assets.First(a => a.name == "Run Config");
            //    rampPrefab = assets.First(a => a.name == "Ramp");
            //
            //    foreach (var asset in assets)
            //    {
            //        ClientScene.RegisterPrefab(asset);
            //    }
            //}));
        }

        private IEnumerator LoadAssetBundle(string assetBundleFullPath, IProgress<float> progress, Action<AssetBundle> onAssetBundleLoaded)
        {
            var assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(assetBundleFullPath);
            while (!assetBundleCreateRequest.isDone)
            {
                progress.Report(assetBundleCreateRequest.progress);
                yield return null;
            }

            onAssetBundleLoaded(assetBundleCreateRequest.assetBundle);

            yield break;
        }

        private static IEnumerator LoadAllAssetsAsync<T>(AssetBundle assetBundle, IProgress<float> progress, Action<T[]> onAssetsLoaded) where T : UnityEngine.Object
        {
            var sceneDefsRequest = assetBundle.LoadAllAssetsAsync<T>();
            while (!sceneDefsRequest.isDone)
            {
                progress.Report(sceneDefsRequest.progress);
                yield return null;
            }

            onAssetsLoaded(sceneDefsRequest.allAssets.Cast<T>().ToArray());

            yield break;
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(ContentPack, args.output);

            args.ReportProgress(1f);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            args.ReportProgress(1f);
            yield break;
        }
    }
}
