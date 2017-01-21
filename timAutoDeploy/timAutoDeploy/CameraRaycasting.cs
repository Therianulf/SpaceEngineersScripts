using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//space engineers includes
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Engine;
using Sandbox.Game;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using VRageMath;



namespace timAutoDeploy {
    class CameraRaycasting {

        //remove this line when import to SE
        IMyGridTerminalSystem GridTerminalSystem;
        private void Echo(string p) {
            Console.WriteLine(p);
        }

        public MyDetectedEntityInfo info;
        public void Main(string argument) {
            List<IMyTerminalBlock> CameraBlocks;
            CameraBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName("camera", CameraBlocks);
            foreach (IMyCameraBlock camera in CameraBlocks) {
                camera.EnableRaycast = true;
                if (camera.CanScan(50000)) {
                    Echo("we can scan");
                }
                else {
                    Echo("no scan");
                }
                info = camera.Raycast(10000, 0, 0);
                Echo(info.Name);
                Echo(info.Type.ToString());
                Echo(info.Relationship.ToString());
                Echo(info.Position.ToString("0.000"));
                Echo(info.BoundingBox.Size.ToString("0.000"));
            }
        }
    }
}
