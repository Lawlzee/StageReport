using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StageReport
{
    [CreateAssetMenu(fileName = "InteractableDef", menuName = "StageReport/InteractableDef", order = 1)]
    public class InteractableDef : ScriptableObject
    {
        public InteractableType type;
        public string nameToken;
        public Texture2D texture;
        public string textureKey;
        public int charges = 1;
        public int defaultScoreWeight;
        public string[] gameObjectNames;
    }
}
