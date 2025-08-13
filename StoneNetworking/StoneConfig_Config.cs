using System;
using System.Collections.Generic;
using System.Text;
using VioletFree.Utilities;
using VioletFree.Mods.Stone;
using GunTemplateee;
using UnityEngine;

namespace VioletFree.Mods.Stone
{
    internal class StoneConfig_Config
    {
        public static string StoneVersion = "1.0.0"; // If this is outdated, please update it.
        public static string ProjectName = "Violet";

        public static void GunEvent(string Event)
        {
            GunTemplate.StartBothGuns(() =>
            {
                StoneBase.SendEvent(Event, RigManager.GetPlayerFromVRRig(GunTemplate.lockedPlayer));
            }, true);
        }

        public static void EventAll(string Event)
        {
            StoneBase.SendEvent(Event);
        }

        public static void Grab()
        {
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {

                if (rig != GorillaTagger.Instance.offlineVRRig && (Vector3.Distance(GorillaTagger.Instance.rightHandTransform.position, rig.headMesh.transform.position) < 0.9f) && ControllerInputPoller.instance.rightGrab)
                {
                    StoneBase.SendEvent("GrabR", RigManager.GetPlayerFromVRRig(rig));
                }

                if (rig != GorillaTagger.Instance.offlineVRRig && (Vector3.Distance(GorillaTagger.Instance.leftHandTransform.position, rig.headMesh.transform.position) < 0.9f) && ControllerInputPoller.instance.leftGrab)
                {
                    StoneBase.SendEvent("GrabL", RigManager.GetPlayerFromVRRig(rig));
                }
            }
        }
    }
}
