﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nereid
{
   namespace FinalFrontier
   {
      class ActionPool : Pool<Action>
      {

         private readonly List<Action> actions = new List<Action>();
         private readonly Dictionary<String, Action> codeMap = new Dictionary<String, Action>();

         public static readonly Action ACTION_LAUNCH   = new LaunchAction();
         public static readonly Action ACTION_DOCKING = new DockingAction();
         public static readonly Action ACTION_RECOVER = new RecoverAction();
         public static readonly Action ACTION_BOARDING = new BoardingAction();
         public static readonly EvaAction ACTION_EVA_NOATM = new EvaNoAtmosphereAction();
         public static readonly EvaAction ACTION_EVA_OXYGEN = new EvaWithOxygen();
         public static readonly EvaAction ACTION_EVA_INATM = new EvaInAtmosphereAction();

         public static readonly ActionPool instance = new ActionPool();

         private ActionPool()
         {
            Add(ACTION_LAUNCH);
            Add(ACTION_DOCKING);
            Add(ACTION_RECOVER);
            Add(ACTION_BOARDING);
            Add(ACTION_EVA_NOATM);
            Add(ACTION_EVA_OXYGEN);
            Add(ACTION_EVA_INATM);
         }


         protected override string CodeOf(Action x)
         {
            return x.GetCode();
         }



         public Action GetActionForCode(String code)
         {
            return base.GetElementForCode(code);
         }
      }
   }
}
