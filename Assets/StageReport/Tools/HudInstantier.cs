using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace StageReport
{
    public class HudInstantier : MonoBehaviour
    {
        public GameObject stageReportPrefab;
        public InteractablesCollection interactablesCollection;
        public TrackedInteractable[] trackedInteractables;
        private bool initalised;

        void Awake()
        {
            if (Application.isPlaying)
            {
                interactablesCollection.Init();
                ContentProvider.stageReportPanelPrefab = stageReportPrefab;
                var cameraPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/Main Camera.prefab").WaitForCompletion();
                var camera = Instantiate(cameraPrefab);
            }
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.F5))
                {
                    StageReportPanel.Toggle(trackedInteractables);
                }

                if (!initalised)
                {
                    initalised = true;
                    var hud = GameObject.Find("HUDSimple(Clone)");
                    Transform mainContainer = hud.transform.Find("MainContainer");
                    Transform mainUIArea = mainContainer.Find("MainUIArea");
                    mainUIArea.gameObject.SetActive(true);

                    var springCanvas = mainUIArea.Find("SpringCanvas");
                    Transform upperRight = springCanvas.Find("UpperRightCluster");

                    var runHudPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClassicRun/ClassicRunInfoHudPanel.prefab").WaitForCompletion();
                    Instantiate(runHudPrefab, upperRight);

                    StageReportPanel.Show(trackedInteractables);
                }
            }
        }
    }
}
