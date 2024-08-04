using RoR2.UI;
using RoR2.UI.SkinControllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace StageReport
{
    //[DefaultExecutionOrder(-100)]
    public class StageReportPanel : MonoBehaviour
    {
        public string labelPrefabKey;
        public string interactableIconPrefabKey;

        public GameObject interactablePanel;

        public InteractablesCollection interactablesCollection;

        public string stageLabelText;
        public Vector3 stageLabelScale;
        public Vector2 stageLabelPivot;
        public Color stageLabelColor;
        public int stageLabelFontSize;

        public float interactableShowInitialDelay;
        public float interactableShowDelay;

        public void Awake()
        {
        }

        public static void Show(IList<TrackedInteractable> trackedInteractables)
        {
            var container = GameObject.Find("HUDSimple(Clone)").transform
                .Find("MainContainer")
                .Find("MainUIArea")
                .Find("SpringCanvas");

            StageReportPanel stageReportPanel = Instantiate(ContentProvider.stageReportPanelPrefab, container).GetComponent<StageReportPanel>();
            stageReportPanel.Render(trackedInteractables);
        }

        public static void Toggle(IList<TrackedInteractable> trackedInteractables)
        {
            GameObject currentPanel = GameObject.Find("HUDSimple(Clone)").transform
                .Find("MainContainer")
                .Find("MainUIArea")
                .Find("SpringCanvas")
                .Find("StageReportPanel(Clone)")
                ?.gameObject;

            if (currentPanel != null)
            {
                Destroy(currentPanel);
            }
            else
            {
                Show(trackedInteractables);
            }
        }

        public void Render(IList<TrackedInteractable> trackedInteractables)
        {
            GameObject labelPrefab = Addressables.LoadAssetAsync<GameObject>(labelPrefabKey).WaitForCompletion();
            
            var stageReportLabel = Instantiate(labelPrefab, transform);

            RectTransform labelRect = (RectTransform)stageReportLabel.transform;
            labelRect.localScale = stageLabelScale;
            labelRect.pivot = stageLabelPivot;
            labelRect.SetAsFirstSibling();

            HGTextMeshProUGUI textMesh = stageReportLabel.GetComponent<HGTextMeshProUGUI>();
            textMesh.text = stageLabelText;
            textMesh.color = stageLabelColor;
            textMesh.fontSize = stageLabelFontSize;

            //int i = 0;
            //foreach (var interactableDef in InteractablesCollection.instance.interactables)
            //{
            //    var interactableIcon = Instantiate(interactableIconPrefab, interactablePanel.transform);
            //
            //    RawImage rawImage = interactableIcon.GetComponent<RawImage>();
            //    rawImage.texture = interactableDef.texture;
            //
            //    var stackText = interactableIcon.transform.GetChild(0);
            //    HGTextMeshProUGUI stackLabel = stackText.GetComponent<HGTextMeshProUGUI>();
            //    stackLabel.text = $"{i}/{i + 2}";
            //
            //    i++;
            //    i = i % 8;
            //}

            StartCoroutine(RenderInteractables(textMesh, trackedInteractables));
        }

        private IEnumerator RenderInteractables(HGTextMeshProUGUI textMesh, IList<TrackedInteractable> trackedInteractables)
        {
            yield return new WaitForSeconds(interactableShowInitialDelay);

            GameObject interactableIconPrefab = Addressables.LoadAssetAsync<GameObject>(interactableIconPrefabKey).WaitForCompletion();

            var interactableGroups = trackedInteractables
                .GroupBy(x => x.type)
                .Select(kvp => (
                    type: kvp.Key,
                    charges: kvp.Select(x => x.charges).Sum(),
                    count: kvp.Count(),
                    def: InteractablesCollection.instance[kvp.Key]
                ))
                .OrderBy(x => InteractablesCollection.instance.GetOrder(x.type))
                .ToList();

            float currentScore = 0;

            float total = interactableGroups
                .Where(x => x.def.charges > 0)
                .Select(x => x.def.defaultScoreWeight * x.count)
                .Sum();

            foreach (var interactableGroup in interactableGroups)
            {
                var interactableIcon = Instantiate(interactableIconPrefab, interactablePanel.transform);

                RawImage rawImage = interactableIcon.GetComponent<RawImage>();
                rawImage.texture = interactableGroup.def.Texture;

                var stackText = interactableIcon.transform.GetChild(0);
                HGTextMeshProUGUI stackLabel = stackText.GetComponent<HGTextMeshProUGUI>();
                if (interactableGroup.def.charges == 0)
                {
                    stackLabel.text = interactableGroup.count.ToString();
                }
                else
                {
                    currentScore += interactableGroup.def.defaultScoreWeight * (1 - (interactableGroup.charges / (float)(interactableGroup.def.charges * interactableGroup.count)));
                    stackLabel.text = $"{interactableGroup.count - interactableGroup.charges / (float)interactableGroup.def.charges:0.##}/{interactableGroup.count}";
                }

                TooltipProvider tooltipProvider = interactableIcon.GetComponent<TooltipProvider>();
                tooltipProvider.titleToken = interactableGroup.def.nameToken;

                float percent = total > 0
                    ? 100 * Mathf.Clamp01(currentScore / total)
                    : 100;
                textMesh.text = $"{stageLabelText} - {Mathf.CeilToInt(percent)}%";
                yield return new WaitForSeconds(interactableShowDelay);
            }
        }
    }
}
