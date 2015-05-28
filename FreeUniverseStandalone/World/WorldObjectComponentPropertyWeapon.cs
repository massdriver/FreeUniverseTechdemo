using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FreeUniverse.Arch;
using FreeUniverse.Common;
using FreeUniverse.Core;

namespace FreeUniverse.World
{
    public struct ProjectileLaunchInfo
    {
        public Vector3 targetPoint;
        public Vector3 launchPosition;
        public Quaternion launchRotation;
    }

    public class WorldObjectComponentPropertyWeapon : WorldObjectComponentProperty
    {
        public float refireTimer { get; set; }
        public int ammo { get; set; }
        public float heat { get; set; }
        public bool jammed { get; set; }
        public Vector2 direction { get; set; }
        public Vector2 targetDirection { get; set; }
        public ArchWorldObjectComponentPropertyWeapon arch { get; set; }
        private AudioSource audioSource { get; set; }
        private GameObject launchSoundSource { get; set; }
        private AudioClip launchSound { get; set; }

        public WorldObjectComponentPropertyWeapon(WorldObjectComponent parent, ArchWorldObjectComponentPropertyWeapon arch)
            : base(parent, arch)
        {
            this.arch = arch;
            type = ComponentPropertyType.Weapon;

            launchSound = ResourceManager.LoadAudioClip(arch.launchSound);
            launchSoundSource = new GameObject();
            launchSoundSource.layer = parentComponent.parentWorldObject.worldController.layer;
            audioSource = launchSoundSource.AddComponent<AudioSource>();
            audioSource.dopplerLevel = 0.0f;
            audioSource.loop = false;
            audioSource.volume = 0.2f;
            audioSource.bypassEffects = true;
            audioSource.maxDistance = 1000.0f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.spread = 0.0f;

            weaponID = (uint)parentComponent.parentWorldObject.indexWeapons.Insert(this);
        }

        public uint weaponID { get; set; }

        public WorldObjectComponentPropertyWeapon(
            WorldObjectComponent parent,
            ArchWorldObjectComponentPropertyWeapon arch,
            WorldObjectComponentHardpoint hp) : this(parent, arch)
        {
            
        }

        public bool Fire()
        {
            if (refireTimer > 0.0f)
                return false;

            if (!activated)
                return false;

            if ((arch.ammoPerShot > 0) && (ammo < arch.ammoPerShot))
                return false;

            if (jammed)
                return false;

            if (!parentComponent.parentWorldObject.DrawEnergy(arch.powerPerShot))
                return false;

            if (arch.ammoPerShot > 0)
                ammo -= arch.ammoPerShot;

            refireTimer = arch.refireRate;

            audioSource.PlayOneShot(launchSound);

            return true;
        }

        public ProjectileLaunchInfo GetLaunchInfo()
        {
            ProjectileLaunchInfo info = new ProjectileLaunchInfo();

            // weapon is not attached to hp, cant fire lol
            if (parentComponent.connectionHardpoint == null)
                return info;

            info.launchPosition = parentComponent.connectionHardpoint.position + arch.relativeLaunchPoint;
            info.launchRotation = parentComponent.connectionHardpoint.rotation;
            info.targetPoint = parentComponent.parentWorldObject.controlPanel.weaponAimPoint;

            return info;
        }

        public override void PostInit()
        {
            base.PostInit();

            if (parentComponent.connectionHardpoint != null)
            {
                launchSoundSource.transform.position = parentComponent.connectionHardpoint.position;
                launchSoundSource.transform.parent = parentComponent.connectionHardpoint.transform;
            }
            else
            {
                throw new Exception("Weapon property is not allowed without connection hardpoint, please make subcomponent and then attach");
            }
        }
        
        public override void Update(float time)
        {
            base.Update(time);

            // sync sound source pos to hp pos
            //_launchSoundSource.transform.parent = _parentComponent.connectionHardpoint.transform;

            if (refireTimer > 0.0f)
                refireTimer -= time;
        }

        public override void Release()
        {
            ResourceManager.ReleaseAudioClip(arch.launchSound);

            audioSource = null;
            launchSound = null;
            launchSoundSource = null;
            arch = null;

            base.Release();
        }
    }
}
