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
    class CargoReporting {
        //remove this line when import to SE
        IMyGridTerminalSystem GridTerminalSystem;
        IMyGridProgramRuntimeInfo gridProgram;


        void Main (){
        
        
        }



        /// <summary>
        /// get all the cargo containers, small, large, or medium connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>cargo</returns>
        List<IMyTerminalBlock> getCargo() {
            List<IMyTerminalBlock> cargoBlocks;
            cargoBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoBlocks);
            return cargoBlocks;
        }
        /// <summary>
        /// get all the cargo containers, small, large, or medium connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>cargo</returns>
        List<IMyCargoContainer> getCargoByName(string name) { // this isnt working yet. 
            List<IMyCargoContainer> cargoBlocks;
            cargoBlocks = new List<IMyCargoContainer>();
            GridTerminalSystem.GetBlockWithName(name);
            return cargoBlocks;
            
        }
    }
}
