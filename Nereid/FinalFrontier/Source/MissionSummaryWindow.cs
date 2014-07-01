﻿using System;
using UnityEngine;
using Toolbar;
using System.Collections.Generic;

namespace Nereid
{
   namespace FinalFrontier
   {
      public class MissionSummaryWindow : AbstractWindow
      {

         private static readonly GUIStyle STYLE_SUMMARY = new GUIStyle(HighLogic.Skin.scrollView);
         private static readonly GUIStyle STYLE_NAME = new GUIStyle(FFStyles.STYLE_LABEL);
         private static readonly GUIStyle STYLE_TEXT = new GUIStyle(HighLogic.Skin.label);
         private static readonly GUIStyle STYLE_LINE = new GUIStyle(HighLogic.Skin.label);

         static MissionSummaryWindow()
         {
            STYLE_SUMMARY.stretchHeight = false;
            STYLE_SUMMARY.fixedHeight = 200;
            STYLE_SUMMARY.stretchWidth = false;
            STYLE_SUMMARY.fixedWidth = 370;
            STYLE_SUMMARY.alignment = TextAnchor.UpperLeft;
            STYLE_TEXT.normal.textColor = HighLogic.Skin.button.normal.textColor;
            STYLE_LINE.alignment = TextAnchor.UpperLeft;
            STYLE_LINE.border = new RectOffset(0, 0, 0, 0);
            STYLE_LINE.margin = new RectOffset(22, 10, 0, 0);
            STYLE_LINE.padding = new RectOffset(0, 0, 0, 0);

         }

         private Vector2 scrollPosSummary = Vector2.zero;

         private readonly List<Summary> Summaries = new List<Summary>();

         private ProtoVessel vessel;

         private class Summary
         {
            public readonly ProtoCrewMember kerbal;
            public  readonly List<Ribbon> newRibbons = new List<Ribbon>();

            public Summary(ProtoCrewMember kerbal)
            {
               this.kerbal = kerbal;
            }
         }

         public MissionSummaryWindow()
            : base(Constants.WINDOW_ID_MISSION_SUMMARY, "Mission Summary")
         {

         }

         protected override void OnWindow(int id)
         {
            Configuration config = FinalFrontier.configuration;
            GUILayout.BeginVertical();
            GUILayout.Label(vessel!=null?vessel.vesselName:"no vessel",FFStyles.STYLE_LABEL);
            DrawSummary();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            config.SetMissionSummaryEnabled(GUILayout.Toggle(config.IsMissionSummaryEnabled(), "show summary", FFStyles.STYLE_TOGGLE));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", FFStyles.STYLE_BUTTON)) SetVisible(false);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            CenterWindow();
         }


         protected override int GetInitialHeight()
         {
            return 300;
         }         

         private void DrawSummary()
         {
            scrollPosSummary = GUILayout.BeginScrollView(scrollPosSummary, FFStyles.STYLE_SCROLLVIEW);
            if (Summaries.Count > 0)
            {
               foreach(Summary s in Summaries)
               {
                  GUILayout.Label(s.kerbal.name, STYLE_NAME);
                  if (s.newRibbons.Count == 0)
                  {
                     GUILayout.Label("   no new ribbons", STYLE_TEXT);
                  }
                  else
                  {
                     int n = 0;
                     int RIBBONS_PER_LINE = 5;
                     foreach (Ribbon ribbon in s.newRibbons)
                     {
                        if (n % RIBBONS_PER_LINE == 0)
                        {
                           GUILayout.BeginHorizontal(STYLE_LINE);
                        }
                        String tooltip = ribbon.GetName() + "\n" + ribbon.GetText();
                        GUILayout.Button(new GUIContent(ribbon.getTexture(), tooltip), FFStyles.STYLE_RIBBON);
                        n++;
                        if (n % RIBBONS_PER_LINE == 0) GUILayout.EndHorizontal();
                     }
                     if (n % RIBBONS_PER_LINE != 0)
                     {
                        GUILayout.EndHorizontal();
                     }
                  }
               }
            }
            else
            {
               GUILayout.Label("nothing happened", STYLE_TEXT);
            }
            GUILayout.FlexibleSpace();
            // filler
            GUILayout.Label(" ", STYLE_TEXT);
            GUILayout.EndScrollView();            
         }

         public void SetSummaryForVessel(ProtoVessel vessel)
         {
            Summaries.Clear();
            this.vessel = vessel;
            if (vessel == null) return;
            Log.Detail("showing mission summary for "+vessel);
            foreach(ProtoCrewMember kerbal in vessel.GetVesselCrew())
            {
               Summary summary = new Summary(kerbal);
               Summaries.Add(summary);
               foreach(Ribbon ribbon in HallOfFame.instance.GetRibbonsOfLatestMission(kerbal))
               {
                  summary.newRibbons.Add(ribbon);
               }
            }
         }
      }
   }
}
