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
        //******************************************************
        //start of code to be exported to SE
        //******************************************************

        //set ammoFirst to true if you wish for assemblers to assign ammo manufacturing before they assign plates.
        // good for when you have limited assemblers in hostile territory and need constant ammo production.
        public bool ammoFirst = false;
        //set this to false if you don't want to assign the assembler component array names to the assemblers
        public bool assignAssemblers = true;
        //set this to false if you don't want tim to manage your refineries and arcs
        public bool assignRefineries = true;
        //set this to false if you don't want tim to assign your reactors uranium
        public bool assignReactors = true;
        //this is how much uranium to give to each reactor, make sure to have the trailing F if you change it, since thats what makes it a float in C# land
        public float ReactorUraniumCount = 10.0F;
        //set this to true if you want to have the assembler add modded items. get the modded item names by putting them into a cargo container that a running TIM script can see. Then using TIM's LCD component function, you should see the item listed on the LCD screen.
        public bool addModComponentList = true; //todo set this to false for production
        public string[] assemblerComponentArray = { "SteelPlate", "CONSTRUCTION", "SMALLTUBE", "LARGETUBE", "COMPUTER", "MOTOR", "METALGRID", "POWERCELL", "BULLETPROOFGLASS", "DETECTOR", "DISPLAY", "REACTOR", "GIRDER", "SOLARCELL", "REACTOR", "INTERIORPLATE", "MEDICAL", "THRUST", "RADIOCOMMUNICATION", "GRAVITYGENERATOR", "EXPLOSIVES", "SUPERCONDUCTOR" };
        public string[] ammoArray = { "Missile200mm", "NATO_25x184mm", "NATO_5p56x45mm" };
        //you can adjust these arrays if you wish to have it build items from different mods. by default it includes CSD autocannon and Battlecannon 
        public string[] modComponentArray = { "Autocannon_Box","Autocannon_Box_Large","250shell","88shell","88hekc"};
        void Main(string argument){
           
        }

        void setRefineryNames() {
            List<IMyTerminalBlock> refineryBlocks = getRefineries();
            foreach (IMyRefinery refinery in refineryBlocks){
                string name = refinery.CustomName;
                name = name + " [TIM Ore]";
                refinery.SetCustomName(name);
            }
        }
        void setAssemblerNames() {
            List<IMyTerminalBlock> assemblerBlocks = getAssemblers();
            int loopCounter = 0;
            foreach (IMyAssembler assembler in assemblerBlocks) {
                string name = assembler.CustomName;
                name = name + "[TIM " + assemblerComponentArray[loopCounter] + "]";
                assembler.SetCustomName(name);
                loopCounter++;
            }
        }


        List<IMyTerminalBlock> getRefineries(){
            List<IMyTerminalBlock> refineryBlocks;
            refineryBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineryBlocks);
            return refineryBlocks;
        }

        List<IMyTerminalBlock> getAssemblers(){
            List<IMyTerminalBlock> AssemblerBlocks;
            AssemblerBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(AssemblerBlocks);
            return AssemblerBlocks;
        }

        List<IMyTerminalBlock> getCargo(){
            List<IMyTerminalBlock> cargoBlocks;
            cargoBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoBlocks);
            return cargoBlocks;
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

        //******************************************************
        //end of code to be exported to SE
        //******************************************************


    }
}
