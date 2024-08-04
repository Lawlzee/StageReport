using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace StageReport
{
    [CreateAssetMenu(fileName = "InteractablesCollection", menuName = "StageReport/InteractablesCollection", order = 2)]
    public class InteractablesCollection : ScriptableObject
    {
        public static InteractablesCollection instance;

        private Dictionary<InteractableType, InteractableDef> _interactableByType;
        private Dictionary<InteractableType, int> _interactableOrder;
        private Dictionary<string, InteractableDef> _interactableByGameObjectName;
        public InteractableDef[] interactables;

        public void Init()
        {
            instance = this;

            _interactableByType = interactables
                .ToDictionary(x => x.type);

            _interactableOrder = interactables
                .Select((value, index) => (value, index))
                .ToDictionary(x => x.value.type, x => x.index);

            _interactableByGameObjectName = interactables
                .SelectMany(interactable => interactable.gameObjectNames
                    .Select(name => (interactable, name)))
                .ToDictionary(x => x.name, x => x.interactable);
        }

        public InteractableDef GetByGameObjectName(string name)
        {
            return _interactableByGameObjectName.TryGetValue(name, out  var interactable) 
                ? interactable 
                : null;
        }

        public InteractableDef this[InteractableType type] => _interactableByType[type];

        public int GetOrder(InteractableType type) => _interactableOrder[type];
    }
}
