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
        ///this is a fake method to prevent VS from bitching about echo, to be removed
        /// </summary>
        /// <param name="p"></param>
        private void Echo(string p)
        {
            throw new NotImplementedException();
        }

        //remove this line when import to SE
        IMyGridTerminalSystem GridTerminalSystem;


        //start of code to be exported to SE
        void Main(string argument){
           List<IMyTerminalBlock> refineryBlocks = getRefineries();
            foreach(IMyRefinery refinery in refineryBlocks){
                string nameNow = refinery.CustomName;
                nameNow = nameNow + " [TIM Ore]";
                refinery.SetCustomName(nameNow);
            }
        }

        List<IMyTerminalBlock> getRefineries(){
            List<IMyTerminalBlock> refineryBlocks;
            refineryBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineryBlocks);
            return refineryBlocks;
        }


        //currently unused methods that are from SE

public void Program() {
    // The constructor, called only once every session and
    // always before any other method is called. Use it to
    // initialize your script. 
    //     
    // The constructor is optional and can be removed if not
    // needed.
}
public void Save() {
    // Called when the program needs to save its state. Use
    // this method to save your state to the Storage field
    // or some other means. 
    // 
    // This method is optional and can be removed if not
    // needed.
}

        //end of code to be exported to SE



    }
}
