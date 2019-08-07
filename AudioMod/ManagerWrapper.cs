using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace AudioMod
{
    public class ManagerWrapper
    {
        public enum State
        {
            Taking,
            Holding
        }
        public MonoBehaviour CurrentManager { get; set; }

        public State GetState()
        {
            if(CurrentManager == null)
                throw new Exception($"Manager is null");
            if (CurrentManager is TNH_Manager tnhManager)
            {
                return tnhManager.Phase == TNH_Phase.Take ? State.Taking : State.Holding;
            }

            if (CurrentManager is TAH_Manager tahManager)
            {
                return tahManager.State == TAH_Manager.TAHGameState.Taking ? State.Taking : State.Holding;
            }
            throw new Exception($"Cant get state of {CurrentManager.GetType()}");
        }


    }
}
