using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace StageReport
{
    public static class InteractableHooks
    {
        public static void Init()
        {
            On.RoR2.ChestBehavior.Start += ChestBehavior_Start;
            On.RoR2.ChestBehavior.Open += ChestBehavior_Open;
        }

        private static void ChestBehavior_Start(On.RoR2.ChestBehavior.orig_Start orig, RoR2.ChestBehavior self)
        {
            Log.Debug("ChestBehavior_Start " + self.gameObject.name);

            if (NetworkServer.active)
            {
                InteractableDef interactableDef = InteractablesCollection.instance.GetByGameObjectName(self.gameObject.name);

                if (interactableDef == null)
                {
                    return;
                }

                Log.Debug(interactableDef.type + " found");

                InteractableTracker.instance.trackedInteractables.Add(new TrackedInteractable
                {
                    netId = self.GetComponent<NetworkIdentity>().netId.Value,
                    type = interactableDef.type,
                    charges = interactableDef.charges
                });
            }

            orig(self);
        }

        private static void ChestBehavior_Open(On.RoR2.ChestBehavior.orig_Open orig, RoR2.ChestBehavior self)
        {
            orig(self);

            Log.Debug("ChestBehavior_Open " + self.gameObject.name);

            uint netId = self.GetComponent<NetworkIdentity>().netId.Value;
            int? index = InteractableTracker.instance.trackedInteractables
                .Select((x, i) => (
                    netId: x.netId,
                    index: i
                ))
                .Where(x => x.netId == netId)
                .Select(x => (int?)x.index)
                .FirstOrDefault();

            if (index == null)
            {
                return;
            }

            Log.Debug("ChestBehavior_Open " + netId);

            var trackedInteractable = InteractableTracker.instance.trackedInteractables[index.Value];
            trackedInteractable.charges--;

            InteractableTracker.instance.trackedInteractables[index.Value] = trackedInteractable;
        }
    }
}
