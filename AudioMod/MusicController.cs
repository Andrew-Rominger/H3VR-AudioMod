using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using FistVR;
using UnityEngine;

namespace AudioMod
{
    public class MusicController : FVRPhysicalObject
    {
        public void Remove()
        {
            Destroy(this);
        }
        protected override void Awake()
        {
            if (CollisionSound == null)
            {
                var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var primMesh = prim.GetComponent<MeshFilter>();
                var primRend = prim.GetComponent<MeshRenderer>();
                var imageAsset = File.ReadAllBytes("Mods/AudioMod/Model/Texture.jpg");


                CollisionSound = new FVRPhysicalSoundParams()
                {
                    Clips = new AudioClip[0]
                };
                DependantRBs = new Rigidbody[0];

                if (gameObject.GetComponent<MeshFilter>() == null)
                {
                    var meshFilter = gameObject.AddComponent<MeshFilter>();
                    meshFilter.name = "VapeMesh";
                    var vapeMesh = FastObjImporter.Instance.ImportFile("Mods/AudioMod/Model/SonyWalkman.obj").ScaleToMaxSize(.1f);

                    meshFilter.mesh = vapeMesh;

                    meshFilter.transform.localPosition = new Vector3(0, 0, 0);
                    var meshRender = gameObject.AddComponent<MeshRenderer>();
                    var texture = new Texture2D(2, 2);
                    texture.LoadImage(imageAsset);
                    meshRender.material = primRend.sharedMaterial;
                    meshRender.material.mainTexture = texture;

                    var rigidbody = gameObject.AddComponent<Rigidbody>();
                    rigidbody.mass = 1f;
                    rigidbody.useGravity = true;
                    rigidbody.angularDrag = 0.05f;
                    rigidbody.drag = 1f;

                    var collider = gameObject.AddComponent<BoxCollider>();
                    collider.size = new Vector3(meshFilter.mesh.bounds.size.x, meshFilter.mesh.bounds.size.y, meshFilter.mesh.bounds.size.z);
                    collider.center = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                }
                //GrabPointTransform.rotation = Quaternion.Euler(90,0,0);
                UseGripRotInterp = true;
                ControlType = FVRInteractionControlType.GrabToggle;
                SpawnLockable = false;
                Harnessable = true;
                Size = FVRPhysicalObjectSize.Small;
                UsesGravity = true;
                PositionInterpSpeed = 4f;
                RotationInterpSpeed = 4f;
                enabled = true;
                DistantGrabbable = true;
                
            }
            base.Awake();
        }

        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            if (hand.Input.TouchpadDown)
            {

                Logger.Log("Touchpad down - Updating Music controller interaction");
                var touchpadAxes = hand.Input.TouchpadAxes;
                if (touchpadAxes.magnitude > 0.2f)
                {
                    if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
                    {
                        Logger.Log("Touchpad up - Volume up");
                        //right
                        GM.TAHMaster.GetComponent<AudioMod.AudioModComponent>().IncreaseMusicVolume();

                    }
                    else if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f)
                    {
                        Logger.Log("Touchpad down - volume down");
                        //down
                        GM.TAHMaster.GetComponent<AudioMod.AudioModComponent>().DecreaseMusicVolume();
                    }
                    else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f )
                    {
                        Logger.Log("Touchpad right - Skipping Song");
                        //up
                        if (GM.TAHMaster.State == TAH_Manager.TAHGameState.Taking)
                            GM.TAHMaster.GetComponent<AudioMod.AudioModComponent>().SkipTakeMusic();
                        else
                            GM.TAHMaster.GetComponent<AudioMod.AudioModComponent>().SkipHoldMusic();
                        
                    }
                }
            }
        }
    }
}
