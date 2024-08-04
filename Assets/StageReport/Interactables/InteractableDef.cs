using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

        private Texture2D _texture;
        public Texture2D Texture => _texture == null
            ? _texture = (texture != null ? texture : Addressables.LoadAssetAsync<Texture2D>(textureKey).WaitForCompletion())
            : _texture;
    }
}
