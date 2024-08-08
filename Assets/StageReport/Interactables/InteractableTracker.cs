using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace StageReport
{
    [Serializable]
    public struct TrackedInteractable : IEquatable<TrackedInteractable>
    {
        public InteractableType type;
        public int charges;
        public uint netId;
        public ItemIndex itemIndex;

        public bool Equals(TrackedInteractable other)
        {
            return type == other.type
                && charges == other.charges
                && netId == other.netId
                && itemIndex == other.itemIndex;
        }
    }

    public class SyncListTrackedInteractable : SyncListStruct<TrackedInteractable> { }

    public class InteractableTracker : NetworkBehaviour
    {
        public static InteractableTracker instance;

        public SyncListTrackedInteractable trackedInteractables;

        public void Awake()
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        public void OnDestroy()
        {
            instance = null;
        }
    }
}
