using RoR2.UI;
using RoR2.UI.SkinControllers;
using System;
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

        public int interactableIconCount;

        public void Awake()
        {
        }

        public void Render(IList<TrackedInteractable> trackedInteractables)
        {
            GameObject labelPrefab = Addressables.LoadAssetAsync<GameObject>(labelPrefabKey).WaitForCompletion();
            GameObject interactableIconPrefab = Addressables.LoadAssetAsync<GameObject>(interactableIconPrefabKey).WaitForCompletion();

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

            var interactableGroups = trackedInteractables
                .GroupBy(x => x.type)
                .OrderBy(x => x.Key)
                .Select(kvp => (
                    type: kvp.Key,
                    charges: kvp.Select(x => x.charges).Sum(),
                    count: kvp.Count()))
                .ToList();

            foreach (var interactableGroup in interactableGroups)
            {
                var interactableDef = InteractablesCollection.instance[interactableGroup.type];

                var interactableIcon = Instantiate(interactableIconPrefab, interactablePanel.transform);

                RawImage rawImage = interactableIcon.GetComponent<RawImage>();
                rawImage.texture = interactableDef.texture;

                var stackText = interactableIcon.transform.GetChild(0);
                HGTextMeshProUGUI stackLabel = stackText.GetComponent<HGTextMeshProUGUI>();
                stackLabel.text = $"{interactableGroup.count - interactableGroup.charges / (float)interactableDef.charges:0.##}/{interactableGroup.count}";
            }
        }
    }
}
