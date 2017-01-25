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

        public struct cargoResults{
            public string cargoName;
            public float cargoCurrentUsed;
            public float cargoMax;
            public float cargoPercentUsed;
            public void getPercentageUsed () {
                cargoPercentUsed = 100.0f * cargoCurrentUsed / cargoMax;
            }
        }

        List <cargoResults> getCargoOfBlocksByName(string name){
            List<cargoResults> cargoBlockResults;
            cargoBlockResults = new List <cargoResults>();
            List<IMyTerminalBlock> cargoBlocks;
            cargoBlocks = getCargoByName(name);
            foreach (IMyCargoContainer cargo in cargoBlocks) {
                cargoResults cargoStruct = new cargoResults();
                cargoStruct.cargoName = cargo.CustomName;
                cargoStruct.cargoCurrentUsed = (float)cargo.GetInventory(0).CurrentVolume;
                cargoStruct.cargoMax = (float)cargo.GetInventory(0).MaxVolume;
                cargoStruct.getPercentageUsed();
                cargoBlockResults.Add(cargoStruct);
            }
            return cargoBlockResults;
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
        List<IMyTerminalBlock> getCargoByName(string name) { // this isnt working yet. 
            List<IMyTerminalBlock> cargoBlocks;
            cargoBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.SearchBlocksOfName(name, cargoBlocks);
            return cargoBlocks;
        }
    }
}
