using Sandbox.ModAPI.Ingame;
using System;
using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

////////////////////////////////////////////////////////////////////////////////////////////////

        private const int MAX_JUMP_LENGTH_FRAMES = 40;

        private List<IMyThrust> JumpJets = new List<IMyThrust>();

        private int CurrentJumpFrame = 0;

        public Program()
        {
            IdentifyJumpJets();
        }

        public void Save()
        {
        
        }

        public void Main(string argument, UpdateType updateSource)
        {
            this.Runtime.UpdateFrequency = UpdateFrequency.Update10;

            if (argument != null && argument.ToLower() == "jump") {
                Jump();
            }

            TickTimer();
        }

        private void IdentifyJumpJets() {
            List<IMyThrust> Thrusters = new List<IMyThrust>();
            GridTerminalSystem.GetBlocksOfType<IMyThrust>(Thrusters);
            if (Thrusters == null || Thrusters.Count == 0) {
                throw new InvalidOperationException("There are no thrusters in the grid");
            }

            foreach(IMyThrust thruster in Thrusters) {
                if (thruster.Orientation.Forward == Base6Directions.Direction.Down) {
                    JumpJets.Add(thruster);
                }
            }
            if (JumpJets.Count == 0) {
                throw new InvalidOperationException("There are no thrusters pointing downwards in the grid");
            }
        }

        private void SetJumpJetsState(bool state) {
            foreach(IMyThrust JumpJet in JumpJets) {
                JumpJet.Enabled = state;
                JumpJet.ThrustOverride = state ? JumpJet.MaxThrust : 0;
            }
        }

        private void Jump() {
            SetJumpJetsState(true);
            CurrentJumpFrame = 0;
        }

        private void TickTimer() {
            
            if (CurrentJumpFrame < MAX_JUMP_LENGTH_FRAMES) {
                CurrentJumpFrame++;
                if (CurrentJumpFrame == MAX_JUMP_LENGTH_FRAMES) {
                    SetJumpJetsState(false);
                }
            }
        }


////////////////////////////////////////////////////////////////////////////////////////////////


    }
}
