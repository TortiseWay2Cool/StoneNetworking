using ExitGames.Client.Photon;
using Fusion;
using GorillaTagScripts;
using HarmonyLib;
using Photon.Pun;
using Photon.Realtime;
using POpusCodec.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Valve.VR.InteractionSystem;
using VioletPaid.Menu;
using VioletPaid.Utilities;
using VioletPaid.Mods.Settings;
using static Mono.Security.X509.X520;
using Object = UnityEngine.Object;
using Violet_Paid.Utilities;
using Violet_Paid.Menu;

namespace VioletPaid.Mods.Stone
{
    internal class StoneBase : MonoBehaviour
    {
        private void Start()
        {
            PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
        }

        public static void NetworkedTag()
        {
            CustomAnnouncment();
            foreach (VRRig rig in GorillaParent.instance.vrrigs)
            {
                if (rig == null || rig.Creator == null || rig == GorillaTagger.Instance.offlineVRRig) continue;
                Photon.Realtime.Player p = RigManager.GetPlayerFromVRRig(rig); if (p == null) continue;

                string id = rig.Creator.UserId, label = ""; float offset = 0.7f;

                if (Euserid.Contains(id)) label = "Elysor Owner";
                else if (p.CustomProperties.TryGetValue("ElysorPaid", out object ep) && (bool)ep) label = "Elysor Paid";

                else if (userid.Contains(id)) label = "Mist Owner";
                else if (ADuserid.Contains(id)) label = "Nova (co-owner of mist!!! me so cool!!!)";
                else if (Coreuserid.Contains(id)) label = "Im Core Im Mist Admin";
                else if (p.CustomProperties.TryGetValue("MistUser", out object mu) && (bool)mu) label = "Mist Free";
                else if (p.CustomProperties.TryGetValue("MistLegal", out object ml) && (bool)ml) label = "Mist Legal";
                else if (IIAdminuserid.Contains(id)) label = "Console Admin";

                else if (Vuserid.Contains(id)) { label = "Violet Owner"; offset = 0.9f; }
                else if (p.CustomProperties.TryGetValue("VioletFreeUser", out object vf) && (bool)vf) { label = "Violet Free"; offset = 0.9f; }

                else if (Huserid.Contains(id)) { label = "Hidden Owner"; offset = 0.8f; }
                else if (p.CustomProperties.TryGetValue("HiddenMenu", out object hm) && (bool)hm) { label = "Hidden Menu"; offset = 0.8f; }

                else if (EVICTEDuserid.Contains(id)) { label = "Evicted Owner"; offset = 0.85f; }
                else if (p.CustomProperties.TryGetValue("EvictedUser", out object ed) && (bool)ed) { label = "Evicted User"; offset = 0.85f; }

                if (label != "")
                {
                    GameObject go = new GameObject("NetworkedNametagLabel");
                    var tmp = go.AddComponent<TextMeshPro>();
                    tmp.text = label;
                    tmp.font = Resources.Load<TMP_FontAsset>("LiberationSans SDF");
                    tmp.fontSize = 4f;
                    tmp.alignment = TextAlignmentOptions.Center;
                    tmp.color = new Color32(140, 194, 150, 255);
                    go.transform.position = rig.transform.position + new Vector3(0f, offset, 0f);
                    go.transform.rotation = Quaternion.LookRotation(go.transform.position - GorillaTagger.Instance.headCollider.transform.position);
                    Destroy(go, Time.deltaTime);
                }
            }
        }

        public static void Access()
        {
            if (userid.Contains(PhotonNetwork.LocalPlayer.UserId))
            {
                ButtonHandler.ChangePage(Category.StoneNetworking);
            }
            else
            {
                NotificationLib.SendNotification("<color=red>STONE/color> : You are not an Admin.");
            }
        }


        public static void SendEvent(string Action, Photon.Realtime.Player plr)
        {
            if (Time.time > Variables.Delay)
            {
                Variables.Delay = Time.time + 0.01f;
                PhotonNetwork.NetworkingClient.OpRaiseEvent(4, new ExitGames.Client.Photon.Hashtable
                {
                    { 0, Action },
                    { 1, plr.ActorNumber }
                }, new RaiseEventOptions { TargetActors = new int[] { plr.ActorNumber } }, SendOptions.SendReliable);
            }
        }


        public static void SendEvent(string Action)
        {
            var eventData = new ExitGames.Client.Photon.Hashtable
            {
                { 0, Action }
            };

            PhotonNetwork.NetworkingClient.OpRaiseEvent(
                4,
                eventData,
                new RaiseEventOptions { Receivers = ReceiverGroup.Others },
                SendOptions.SendReliable
            );
        }

        public static void CustomAnnouncment()
        {
            if (Time.time > Variables.Delay)
            {
                Variables.Delay = Time.time + 40;
                string Check = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/TortiseWay2Cool/Kill_Switch/refs/heads/main/Valid%20User%20ID").GetAwaiter().GetResult();
                string Message = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/TortiseWay2Cool/CustomAnnouncment/refs/heads/main/CustomAnc").GetAwaiter().GetResult();
                if (Check.Contains("2"))
                {
                    NotificationLib.SendNotification(Message);
                }

            }
        }


        public static void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == 4)
            {
                if (PhotonNetwork.InRoom)
                {
                    if (photonEvent.CustomData is ExitGames.Client.Photon.Hashtable data)
                    {
                        string Action = (string)data[0];
                        int TargetActorNumber = (int)data[1];
                        Photon.Realtime.Player player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
                        VRRig vrrig = RigManager.GetVRRigFromPlayer(player);
                        if (userid.Contains(player.UserId))
                        {
                            if (Action.Contains("Kick"))
                            {
                                PhotonNetwork.Disconnect();
                            }
                            else if (Action.Contains("Mute"))
                            {
                                GorillaTagger.Instance.myRecorder.SourceType = Photon.Voice.Unity.Recorder.InputSourceType.AudioClip;
                            }
                            else if (Action.Contains("UnMute"))
                            {
                                GorillaTagger.Instance.myRecorder.SourceType = Photon.Voice.Unity.Recorder.InputSourceType.Microphone;
                            }
                            else if (Action.Contains("Deafen"))
                            {
                                foreach (AudioSource audioSource in UnityEngine.Object.FindObjectsOfType<AudioSource>())
                                {
                                    audioSource.mute = true;
                                }
                            }
                            else if (Action.Contains("UnDeafen"))
                            {
                                foreach (AudioSource audioSource in UnityEngine.Object.FindObjectsOfType<AudioSource>())
                                {
                                    audioSource.mute = false;
                                }
                            }
                            else if (Action.Contains("Slow"))
                            {
                                GorillaTagger.Instance.ApplyStatusEffect(GorillaTagger.StatusEffect.Frozen, 5f);
                            }
                            else if (Action.Contains("Vibrate"))
                            {
                                GorillaTagger.Instance.StartVibration(false, 1,1);
                                GorillaTagger.Instance.StartVibration(true, 1, 1);
                            }
                            else if (Action.Contains("Blind"))
                            {
                                GameObject blindEffect = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                blindEffect.transform.position = GorillaTagger.Instance.headCollider.transform.position;
                                blindEffect.transform.rotation = GorillaTagger.Instance.headCollider.transform.rotation;
                                Destroy(blindEffect.GetComponent<Collider>());
                                blindEffect.GetComponent<MeshRenderer>().material.color = Color.black;
                                blindEffect.transform.localScale = new Vector3(10f, 10f, 10f);
                                Destroy(blindEffect, 1f);
                            }
                            else if (Action.Contains("Dead"))
                            {
                                GRPlayer playerInstance = GorillaTagger.Instance.GetComponent<GRPlayer>();
                                playerInstance.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, GhostReactorManager.instance);
                            }
                            else if (Action.Contains("Alive"))
                            {
                                GRPlayer playerInstance = GorillaTagger.Instance.GetComponent<GRPlayer>();
                                playerInstance.ChangePlayerState(GRPlayer.GRPlayerState.Ghost, GhostReactorManager.instance);
                            }

                            else if (Action.Contains("ScaleDown"))
                            {
                                Size -= 0.01f;
                                GorillaTagger.Instance.offlineVRRig.transform.localScale = new Vector3(Size, Size, Size);
                            }
                            else if (Action.Contains("ScaleUp"))
                            {
                                Size += 0.01f;
                                GorillaTagger.Instance.offlineVRRig.transform.localScale = new Vector3(Size, Size, Size);
                            }
                            else if (Action.Contains("ScaleReset"))
                            {
                                GorillaTagger.Instance.offlineVRRig.transform.localScale = new Vector3(1f, 1f, 1f);
                            }
                        }
                    }
                }
            }
        }
        public static float Size = 1f;

        public static string userid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/TortiseWay2Cool/Kill_Switch/refs/heads/main/Valid%20User%20ID").GetAwaiter().GetResult();
        public static string webhookUrl = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/TortiseWay2Cool/Kill_Switch/refs/heads/main/Webhook").GetAwaiter().GetResult();
        public static string ADuserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/Cha554/mist-ext/refs/heads/main/ADUserID's").GetAwaiter().GetResult();
        public static string Vuserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/TortiseWay2Cool/Kill_Switch/refs/heads/main/Valid%20User%20ID").GetAwaiter().GetResult();
        public static string Euserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/xclipse13295-commits/id-s/refs/heads/main/ValidID's").GetAwaiter().GetResult();
        public static string Huserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/menker-cs/Hidden/refs/heads/main/Player-ID.txt").GetAwaiter().GetResult();
        public static string Coreuserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/Cha554/mist-ext/refs/heads/main/Core'sUSID").GetAwaiter().GetResult();
        public static string ag = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/Cha554/mist-ext/refs/heads/main/CustomAnnouncement").GetAwaiter().GetResult();
        public static string IIAdminuserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/Cha554/mist-ext/refs/heads/main/Reusid").GetAwaiter().GetResult();
        public static string EVICTEDuserid = new HttpClient().GetStringAsync("https://raw.githubusercontent.com/JeffreyEpstein1953/EvictedID/refs/heads/main/id").GetAwaiter().GetResult();
    }
}
