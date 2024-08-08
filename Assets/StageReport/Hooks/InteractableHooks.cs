using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace StageReport
{
    public static class InteractableHooks
    {
        public static void Init()
        {
            On.RoR2.PurchaseInteraction.PreStartClient += PurchaseInteraction_PreStartClient;
            On.RoR2.ShrineChanceBehavior.AddShrineStack += ShrineChanceBehavior_AddShrineStack;

            On.RoR2.MultiShopController.Start += MultiShopController_Start;
            On.RoR2.MultiShopController.OnPurchase += MultiShopController_OnPurchase;
            On.RoR2.BarrelInteraction.Start += BarrelInteraction_Start;
            On.RoR2.BarrelInteraction.CoinDrop += BarrelInteraction_CoinDrop;
            On.RoR2.ScrapperController.Start += ScrapperController_Start;

            On.RoR2.CharacterBody.Start += CharacterBody_Start;
            On.RoR2.CharacterDeathBehavior.OnDeath += CharacterDeathBehavior_OnDeath;
            On.RoR2.ShopTerminalBehavior.UpdatePickupDisplayAndAnimations += ShopTerminalBehavior_UpdatePickupDisplayAndAnimations;
        }

        private static void ShopTerminalBehavior_UpdatePickupDisplayAndAnimations(On.RoR2.ShopTerminalBehavior.orig_UpdatePickupDisplayAndAnimations orig, ShopTerminalBehavior self)
        {
            orig(self);
            if (self.pickupIndex == PickupIndex.none)
            {
                return;
            }

            uint netId = self.GetComponent<NetworkIdentity>().netId.Value;
            int? index = FindInteractableIndex(netId);

            if (index == null)
            {
                return;
            }

            TrackedInteractable trackedInteractable = InteractableTracker.instance.trackedInteractables[index.Value];
            InteractableDef interactableDef = InteractablesCollection.instance[trackedInteractable.type];
            if (!interactableDef.unstackable)
            {
                return;
            }

            PickupDef pickupDef = PickupCatalog.GetPickupDef(self.pickupIndex);
            ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
            trackedInteractable.itemIndex = itemDef.itemIndex;


            InteractableTracker.instance.trackedInteractables[index.Value] = trackedInteractable;
        }

        private static void CharacterDeathBehavior_OnDeath(On.RoR2.CharacterDeathBehavior.orig_OnDeath orig, CharacterDeathBehavior self)
        {
            orig(self);
            if (NetworkServer.active)
            {
                uint netId = self.GetComponent<NetworkIdentity>().netId.Value;
                SetCharges(netId, 0);
            }
        }

        private static void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            TryRegistering("CharacterBody_Start", self, log: false);
        }

        private static void ScrapperController_Start(On.RoR2.ScrapperController.orig_Start orig, ScrapperController self)
        {
            orig(self);
            TryRegistering("ScrapperController_Start", self);
        }

        private static void BarrelInteraction_CoinDrop(On.RoR2.BarrelInteraction.orig_CoinDrop orig, BarrelInteraction self)
        {
            orig(self);

            Log.Debug("BarrelInteraction_Start " + self.gameObject.name);
            if (NetworkServer.active)
            {
                uint netId = self.GetComponent<NetworkIdentity>().netId.Value;
                SetCharges(netId, 0);
            }
        }

        private static void BarrelInteraction_Start(On.RoR2.BarrelInteraction.orig_Start orig, BarrelInteraction self)
        {
            orig(self);
            TryRegistering("BarrelInteraction_Start", self);
        }

        private static void MultiShopController_Start(On.RoR2.MultiShopController.orig_Start orig, MultiShopController self)
        {
            orig(self);
            TryRegistering("MultiShopController_Start", self);
        }

        private static void TryRegistering(string caller, NetworkBehaviour self, bool log = true)
        {
            if (log)
            {
                Log.Debug(caller + " " + self.gameObject.name);
            }
            if (NetworkServer.active)
            {
                InteractableDef interactableDef = InteractablesCollection.instance.GetByGameObjectName(self.gameObject.name);

                if (interactableDef == null)
                {
                    if (log)
                    {
                        Log.Debug("interactableDef not found");
                    }
                    return;
                }

                Log.Debug(interactableDef.type + " found");

                var trackedInteractable = new TrackedInteractable
                {
                    netId = self.GetComponent<NetworkIdentity>().netId.Value,
                    type = interactableDef.type,
                    charges = interactableDef.charges
                };

                InteractableTracker.instance.trackedInteractables.Add(trackedInteractable);
            }
        }

        private static void MultiShopController_OnPurchase(On.RoR2.MultiShopController.orig_OnPurchase orig, MultiShopController self, Interactor interactor, PurchaseInteraction purchaseInteraction)
        {
            orig(self, interactor, purchaseInteraction);

            Log.Debug("MultiShopController_OnPurchase " + self.gameObject.name);
            if (NetworkServer.active)
            {
                int charges = 0;
                foreach (GameObject obj in self._terminalGameObjects)
                {
                    if (obj.GetComponent<PurchaseInteraction>().Networkavailable)
                    {
                        charges++;
                    }
                }

                uint netId = self.GetComponent<NetworkIdentity>().netId.Value;
                SetCharges(netId, charges);
            }
        }

        private static void ShrineChanceBehavior_AddShrineStack(On.RoR2.ShrineChanceBehavior.orig_AddShrineStack orig, ShrineChanceBehavior self, Interactor activator)
        {
            orig(self, activator);

            Log.Debug("ShrineChanceBehavior_AddShrineStack " + self.gameObject.name);
            if (NetworkServer.active)
            {
                uint netId = self.GetComponent<NetworkIdentity>().netId.Value;
                SetCharges(netId, self.maxPurchaseCount - self.successfulPurchaseCount);
            }
        }

        private static void PurchaseInteraction_PreStartClient(On.RoR2.PurchaseInteraction.orig_PreStartClient orig, PurchaseInteraction self)
        {
            orig(self);

            Log.Debug("PurchaseInteraction_PreStartClient " + self.gameObject.name);
            if (NetworkServer.active)
            {
                if (!self.gameObject.activeInHierarchy)
                {
                    Log.Debug("not active");
                    return;
                }

                InteractableDef interactableDef = InteractablesCollection.instance.GetByGameObjectName(self.gameObject.name);

                if (interactableDef == null)
                {
                    Log.Debug("interactableDef not found");
                    return;
                }

                Log.Debug(interactableDef.type + " found");

                int index = InteractableTracker.instance.trackedInteractables.Count;
                var trackedInteractable = new TrackedInteractable
                {
                    netId = self.GetComponent<NetworkIdentity>().netId.Value,
                    type = interactableDef.type,
                    charges = interactableDef.charges
                };

                InteractableTracker.instance.trackedInteractables.Add(trackedInteractable);

                if (interactableDef.charges != 1 
                    && interactableDef.type != InteractableType.BloodShrine
                    && interactableDef.type != InteractableType.WoodShrine)
                {
                    Log.Debug("charges != 1");
                    return;
                }

                self.onPurchase.AddListener(interactor =>
                {
                    trackedInteractable.charges = Mathf.Max(0, trackedInteractable.charges - 1);
                    InteractableTracker.instance.trackedInteractables[index] = trackedInteractable;
                });
            }
        }

        private static int? FindInteractableIndex(uint netId)
        {
            return InteractableTracker.instance.trackedInteractables
                .Select((x, i) => (
                    netId: x.netId,
                    index: i
                ))
                .Where(x => x.netId == netId)
                .Select(x => (int?)x.index)
                .FirstOrDefault();
        }

        private static bool SetCharges(uint netId, int charges)
        {
            int? index = FindInteractableIndex(netId);

            if (index == null)
            {
                Log.Debug("index == null. netId = " + netId);
                return false;
            }

            var trackedInteractable = InteractableTracker.instance.trackedInteractables[index.Value];
            trackedInteractable.charges = charges;
            InteractableTracker.instance.trackedInteractables[index.Value] = trackedInteractable;

            return true;
        }
    }
}
