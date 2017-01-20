//windows includes
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

//name space

namespace timAutoDeploy
{

    //class
    class timAutoDeploy
    {
        /// <summary>
        ///this is a fake method to prevent VS from bitching about, to be removed
        /// </summary>
        /// <param name="p"></param>
        private void Echo(string p)
        {
            throw new NotImplementedException();
        }

        //remove this line when import to SE
        IMyGridTerminalSystem GridTerminalSystem;



        //start of real code

        void Main(string argument){

            List<IMyTerminalBlock> antennaBlocks;
            antennaBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRadioAntenna>(antennaBlocks);
            /*
            foreach (IMyRadioAntenna antenna in antennaBlocks){
                if (antenna.IsFunctional) {
                    Vector3D antpos = antenna.GetPosition();
                    Echo("antenna named " + antpos.ToString() + "is reporting functional");       
                }
            }
             */
        }

        List<IMyTerminalBlock> getRefineries(){

            List<IMyTerminalBlock> refineryBlocks;
            refineryBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineryBlocks);
            return refineryBlocks;
        }


    }
}
