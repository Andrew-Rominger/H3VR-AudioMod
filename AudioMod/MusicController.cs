using System;
using System.IO;
using System.Linq;
using FistVR;
using UnityEngine;
using Valve.Newtonsoft.Json;
using Object = UnityEngine.Object;

namespace AudioMod
{
    /// <summary>
    /// MusicController object used to modify currently playing music
    /// </summary>
    public class MusicController : FVRPhysicalObject
    {
        protected override void Awake()
        {
            base.Awake();

            //Create a primitive cube to get MeshRender from
            var prim = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var primRend = prim.GetComponent<MeshRenderer>();

            //Load object texture
            var imageAsset = File.ReadAllBytes("Mods/AudioMod/Model/Texture.jpg");

            //Set collison sound for earlier null check

            DependantRBs = new Rigidbody[0];

            //Add mesh filter component
            var meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.name = "MusicControllerMesh";

            //Load the .obj file, and scale it to a smaller max size
            var musicControllerMesh = FastObjImporter.Instance
                .ImportFile("Mods/AudioMod/Model/SonyWalkman.obj")
                .ScaleToMaxSize(.1f);

            meshFilter.mesh = musicControllerMesh;
            meshFilter.transform.localPosition = new Vector3(0, 0, 0);

            var meshRender = gameObject.AddComponent<MeshRenderer>();
            //Create texture object and load textureImage
            var texture = new Texture2D(2, 2);
            texture.LoadImage(imageAsset);

            //Get the material from the primitive cube and assign its texture to the one we created
            meshRender.material.CopyPropertiesFromMaterial(primRend.sharedMaterial);
            meshRender.material.mainTexture = texture;
            Destroy(prim);

            //Create a RigidBody component and set its parameters
            var rigidBody = gameObject.AddComponent<Rigidbody>();
            rigidBody.mass = 1f;
            rigidBody.useGravity = true;
            rigidBody.angularDrag = 0.05f;
            rigidBody.drag = 1f;
            rigidBody.detectCollisions = true;
            RootRigidbody = rigidBody;

            //Add a collider. Set its size to match the imported mesh and center to match its position

            var collider = gameObject.AddComponent<BoxCollider>();
            collider.isTrigger = false;
            
            collider.size = new Vector3(meshFilter.mesh.bounds.size.x, meshFilter.mesh.bounds.size.y, meshFilter.mesh.bounds.size.z);
            collider.center = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            var colid = m_colliders.ToList();
            colid.Add(collider);
            m_colliders = colid.ToArray();
            SetAllCollidersToLayer(false, "default");
            //H3VR properties set here
            IsSimpleInteract = false;
            UseGripRotInterp = true;
            ControlType = FVRInteractionControlType.GrabToggle; //Makes it so releasing trigger wont drop object
            SpawnLockable = false; //Make non spawnlockable
            Harnessable = true; //Make harnessable (green tinted quick belt slot w/ auto return)
            Size = FVRPhysicalObjectSize.Small; //Set size to small so it fits in small quick belt slots
            UsesGravity = true; //Make use gravity
            PositionInterpSpeed = 4f; //IDK what this does
            RotationInterpSpeed = 4f;  //IDK what this does
            enabled = true;  //IDK what this does
            DistantGrabbable = true; //Makes it so the empty hand touchpad beam can pick up this item
            IsPickUpLocked = false;

            ObjectWrapper = new FVRObject()
            {
                Category = FVRObject.ObjectCategory.Tool,
                ItemID = nameof(MusicController),
                DisplayName = "Music Controller",
                Mass = 1
            }; 
        }

        public override bool IsInteractable()
        {
            return true;
        }

        public override bool IsSelectionRestricted()
        {
            return false;
        }

        /// <summary>
        /// Called to update the objects interaction with a vive controller
        /// </summary>
        /// <param name="hand">The hand interacting</param>
        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            //Check if the touchpad is clicked
            if (hand.Input.TouchpadDown)
            {
                var touchpadAxes = hand.Input.TouchpadAxes;
                //Only do checks if the user actually ment to click
                if (touchpadAxes.magnitude > 0.2f)
                {
                    if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
                    {
                        //Up on touchpad
                        InjectionMethods.CurrentManagerWrapper.CurrentManager.GetComponent<AudioModComponent>().IncreaseMusicVolume();

                    }
                    else if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f)
                    {
                        //Down on touchpad
                        InjectionMethods.CurrentManagerWrapper.CurrentManager.GetComponent<AudioModComponent>().DecreaseMusicVolume();
                    }
                    else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f )
                    {
                        //Right on touchpad
                        //Get AudioMod component and skip song
                        if (InjectionMethods.CurrentManagerWrapper.GetState() == ManagerWrapper.State.Taking)
                            InjectionMethods.CurrentManagerWrapper.CurrentManager.GetComponent<AudioModComponent>().SkipTakeMusic();
                        else
                            InjectionMethods.CurrentManagerWrapper.CurrentManager.GetComponent<AudioModComponent>().SkipHoldMusic();
                    }
                }
            }
        }
    }
}
