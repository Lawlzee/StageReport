using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace StageReport
{
    [DefaultExecutionOrder(-1)]
    public class HudInstantier : MonoBehaviour
    {
        public GameObject stageReportPrefab;
        public InteractablesCollection interactablesCollection;
        public TrackedInteractable[] trackedInteractables;
        private int initalised;
        private NetworkUser networkUser;
        GameObject camera;

        void Awake()
        {
            if (Application.isPlaying)
            {
                //PlatformSystems.Init();

                //GameObject.Find("PP Volume").GetComponent<PostProcessVolume>().sharedProfile = ScriptableObject.CreateInstance<PostProcessProfile>();

                //networkUser = Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/Network/NetworkUser.prefab").WaitForCompletion()).GetComponent<NetworkUser>();
                //Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/Rewired/Rewired Input Manager.prefab").WaitForCompletion());
                //Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/MPEventSystem/MPEventSystemManager.prefab").WaitForCompletion());
                //var eventSystem = Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/MPEventSystem/MPEventSystem.prefab").WaitForCompletion()).GetComponent<MPEventSystem>();

                interactablesCollection.Init();
                ContentProvider.stageReportPanelPrefab = stageReportPrefab;
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

                if (initalised == 0)
                {

                    //Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HUDSimple.prefab").WaitForCompletion().GetComponent<MPEventSystemProvider>().eventSystem = GameObject.Find("MPEventSystem Player0").GetComponent<MPEventSystem>();
                    //Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/HUDSimple.prefab").WaitForCompletion().GetComponent<HUD>().localUserViewer = networkUser.localUser;

                    var cameraPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Core/Camera/Main Camera.prefab").WaitForCompletion();
                    cameraPrefab.GetComponent<CameraRigController>().nextTarget = gameObject;
                    //cameraPrefab.GetComponent<CameraRigController>().viewer = networkUser;
                    camera = Instantiate(cameraPrefab);
                    //camera.GetComponent<CameraRigController>().viewer = networkUser;
                    initalised = 1;
                    return;
                }

                if (initalised == 1)
                {
                    initalised = 2;
                    var hud = GameObject.Find("HUDSimple(Clone)");
                    HUD.cvHudEnable.value = true;

                    Transform mainContainer = hud.transform.Find("MainContainer");
                    Transform mainUIArea = mainContainer.Find("MainUIArea");
                    mainUIArea.gameObject.SetActive(true);

                    var springCanvas = mainUIArea.Find("SpringCanvas");
                    Transform upperRight = springCanvas.Find("UpperRightCluster");
                    //Transform bottomRight = springCanvas.Find("BottomRightCluster");
                    //bottomRight.gameObject.SetActive(false);

                    var runHudPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ClassicRun/ClassicRunInfoHudPanel.prefab").WaitForCompletion();
                    Instantiate(runHudPrefab, upperRight);

                    StageReportPanel.Show(trackedInteractables);
                    return;
                }

                //Log.Debug("hasOverride:" + camera.GetComponent<CameraRigController>().hasOverride);
                //Log.Debug("target:" + camera.GetComponent<CameraRigController>().target);
                //Log.Debug("isHudAllowed:" +camera.GetComponent<CameraRigController>().isHudAllowed);
                //Log.Debug("cvHudEnable:" + HUD.cvHudEnable.value);
            }
        }
    }
}
