﻿using KSPAPIExtensions;
using KSPAPIExtensions.PartMessage;
using System;
using UnityEngine;

namespace ProceduralParts
{

    /// <summary>
    /// Module to allow tweaking of decouplers in the VAB.
    /// 
    /// This fairly flexible module allows tech dependent tweaking of the type and size of modules.
    /// There are options for it to target just one specific module, or all the modules on a part.
    /// </summary>
    public class DecouplerTweaker : PartModule
    {
        public override void OnAwake()
        {
            base.OnAwake();
            PartMessageService.Register(this);
        }

        /// <summary>
        /// In career mode, if this tech is not available then the option to have separators is not present
        /// </summary>
        [KSPField]
        public string separatorTechRequired;

        /// <summary>
        /// Maximum ejection impulse available. 
        /// </summary>
        [KSPField]
        public float maxImpulse = float.PositiveInfinity;

        /// <summary>
        /// Listen for a specific texture message, again for use with proceedural parts. If set, then listens for ChangeAttachNodeSize message.
        /// </summary>
        [KSPField]
        public string textureMessageName;

        /// <summary>
        /// Density of the decoupler. This is for use with Procedural Parts. Listens for ChangeVolume message
        /// </summary>
        [KSPField]
        public float density = 0.0f;

        /// <summary>
        /// If specified, this will set a maximum impulse based on the diameter of the node.
        /// </summary>
        [KSPField]
        public float maxImpulseDiameterRatio = 0.0f;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Style:"),
         UI_Toggle(disabledText = "Decoupler", enabledText = "Separator")]
        public bool isOmniDecoupler = false;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiActive = false, guiName = "Impulse", guiUnits = "kNs", guiFormat = "F1"),
         UI_FloatEdit(scene = UI_Scene.Editor, minValue = 0.1f, maxValue = float.PositiveInfinity, incrementLarge = 10f, incrementSmall=0, incrementSlide = 0.1f)]
        public float ejectionImpulse;

        [KSPField(isPersistant = true, guiActiveEditor = true, guiName="Mass", guiUnits="T", guiFormat="S3")]
        public float mass;

        private ModuleDecouple decouple;

        internal const float ImpulsePerForceUnit = 0.02f;

        public override void OnStart(StartState state)
        {
            if (!FindDecoupler())
            {
                Debug.LogError("Unable to find any decoupler modules");
                isEnabled = enabled = false;
                return;
            }

            if (HighLogic.LoadedSceneIsEditor)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (mass != 0)
                    UpdateMass(mass);

                Fields["isOmniDecoupler"].guiActiveEditor =
                    string.IsNullOrEmpty(separatorTechRequired) || ResearchAndDevelopment.GetTechnologyState(separatorTechRequired) == RDTech.State.Available;

                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (ejectionImpulse == 0)
                    ejectionImpulse = (float)Math.Round(decouple.ejectionForce * ImpulsePerForceUnit, 1);

                UI_FloatEdit ejectionImpulseEdit = (UI_FloatEdit)Fields["ejectionImpulse"].uiControlEditor;
                ejectionImpulseEdit.maxValue = maxImpulse;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                decouple.isOmniDecoupler = isOmniDecoupler;
                part.mass = mass;
            }
        }

        private bool FindDecoupler()
        {
            if(decouple == null)
                decouple = part.Modules["ModuleDecouple"] as ModuleDecouple;
            return decouple != null;
        }

        public override void OnUpdate()
        {
            if (FindDecoupler())
                decouple.ejectionForce = ejectionImpulse / TimeWarp.fixedDeltaTime;
        }

        // Plugs into procedural parts.
        [PartMessageListener(typeof(PartAttachNodeSizeChanged), scenes:GameSceneFilter.AnyEditor)]
        public void ChangeAttachNodeSize(AttachNode node, float minDia, float area)
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (node.id != textureMessageName || maxImpulseDiameterRatio == 0)
                return;

            UI_FloatEdit ejectionImpulseEdit = (UI_FloatEdit)Fields["ejectionImpulse"].uiControlEditor;
            float oldRatio = ejectionImpulse / ejectionImpulseEdit.maxValue;

            maxImpulse = Mathf.Round(maxImpulseDiameterRatio * minDia);
            ejectionImpulseEdit.maxValue = maxImpulse;

            ejectionImpulse = Mathf.Round(maxImpulse * oldRatio / 0.1f) * 0.1f;
        }

        [PartMessageListener(typeof(PartVolumeChanged), scenes: GameSceneFilter.AnyEditor)]
        public void ChangeVolume(string volumeName, float volume)
        {
            if (density > 0)
                UpdateMass(density * volume);
        }

        private void UpdateMass(float updateMass)
        {
            part.mass = mass = updateMass;
            BaseField fld = Fields["mass"];
            if (updateMass < 0.1)
            {
                fld.guiUnits = "g";
                fld.guiFormat = "S3+6";
            }
            else
            {
                fld.guiUnits = "T";
                fld.guiFormat = "S3";
            }
        }
    }

}