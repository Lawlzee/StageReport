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
        public InteractableDef[] interactables;
    }
}
