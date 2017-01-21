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
        //making an empty string to represent storage, dont include this in copy to PB
        string Storage = "";

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
        public float ReactorUraniumCount = 100.0F;
        //set this to false if you don't want to assign docking passwords to your connectors
        public bool assignDockingPassword = true;
        // change "password" to whatever you want your shared key to be. this isn't exactly secure at all being just plain text in the name of the connector. Know that anyone who can see that grid can get the password to connect. dont use this for grid security against fellow faction members. detemined/smart griefers will figure this out.
        public string DockingPassword = "password";
        //these arrays are made up of all the components that an assembler can produce. 
        public string[] assemblerComponentArray = { "STEELPLATE", "CONSTRUCTION", "SMALLTUBE", "LARGETUBE", "COMPUTER", "MOTOR", "METALGRID", "POWERCELL", "BULLETPROOFGLASS", "DETECTOR", "DISPLAY", "REACTOR", "GIRDER", "SOLARCELL", "REACTOR", "INTERIORPLATE", "MEDICAL", "THRUST", "RADIOCOMMUNICATION", "GRAVITYGENERATOR", "EXPLOSIVES", "SUPERCONDUCTOR" };
        public string[] ammoArray = { "Missile200mm", "NATO_25x184mm", "NATO_5p56x45mm" };
        //set this to true if you want to have the assembler add modded items. get the modded item names by putting them into a cargo container that a running TIM script can see. Then using TIM's LCD component function, you should see the item listed on the LCD screen.
        public bool addModComponentList = true; //todo set this to false for production
        //you can adjust these arrays if you wish to have it build items from different mods. by default it includes CSD autocannon and Battlecannon 
        public string[] modComponentArray = { "Autocannon_Box","Autocannon_Box_Large","250shell","88shell","88hekc"};

        //BE CAREFUL MESSING WITH THIS VARIABLE, IF YOU SET IT TO TRUE AFTER THE FIRST RUN, IT MAY OVERWRITE ANY CHANGES YOU HAVE MADE. YOU HAVE BEEN WARNED.
        public bool firstRun = true;

        void Main(string argument){
            if (firstRun) {
                if (assignAssemblers)
                    setAssemblerNames();
                if (assignRefineries)
                    setRefineryNames();
                if (assignReactors)
                    setReactorNames();
                if (assignDockingPassword)
                    setDockingRights();
                firstRun = false;
            }
            //cargo assignment stuff goes here

           
        }
        /// <summary>
        /// set the Refineries and Arcs to have the TIM Ore tag
        /// </summary>
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
            bool ammoChecked = false;
            foreach (IMyAssembler assembler in assemblerBlocks) { // todo make sure to check the length of the array isnt shorter than our loop counter
                string name = assembler.CustomName;
                if (ammoFirst && !ammoChecked) {
                    if (ammoArray.Length >= loopCounter) {
                        name = name + "[TIM " + ammoArray[loopCounter] + "]";
                        assembler.SetCustomName(name);
                        loopCounter++;
                    } else {
                        ammoChecked = true;
                        loopCounter = 0;
                    }
                    break;
                }
                if (assemblerComponentArray.Length >= loopCounter) {
                    name = name + "[TIM " + ammoArray[loopCounter] + "]";
                    assembler.SetCustomName(name);
                    loopCounter++;
                }
            }
        }
        /// <summary>
        /// set the name of current grids connectors to include the TIM DOCK:"Password" tag inorder to talk to other grids
        /// </summary>
        void setDockingRights() {
            List<IMyTerminalBlock> connectors = getConnectors();
            foreach(IMyShipConnector connector in connectors){
                string name = connector.CustomName;
                name = name + "[TIM DOCK:\"" + DockingPassword + "\"]";
                connector.SetCustomName(name);
            }
        }
        /// <summary>
        /// insure that reactors have the set amount of Uranium in them
        /// </summary>
        void setReactorNames() {
            List<IMyTerminalBlock> reactors = getReactors();
            foreach (IMyReactor reactor in reactors) {
                string name = reactor.CustomName;
                name = name + "[TIM uranium:P1:" + ReactorUraniumCount.ToString() + "]";
                reactor.SetCustomName(name);
            }
        }

        /// <summary>
        /// get all the refineries connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>refineries</returns>
        List<IMyTerminalBlock> getRefineries(){
            List<IMyTerminalBlock> refineryBlocks;
            refineryBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineryBlocks);
            return refineryBlocks;
        }
        /// <summary>
        /// get all the assemblers connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>assemblers</returns>
        List<IMyTerminalBlock> getAssemblers(){
            List<IMyTerminalBlock> AssemblerBlocks;
            AssemblerBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(AssemblerBlocks);
            return AssemblerBlocks;
        }
        /// <summary>
        /// get all the cargo containers, small, large, or medium connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>cargo</returns>
        List<IMyTerminalBlock> getCargo(){
            List<IMyTerminalBlock> cargoBlocks;
            cargoBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargoBlocks);
            return cargoBlocks;
        }
        /// <summary>
        /// get all the connectors attached to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>connectors</returns>
        List<IMyTerminalBlock> getConnectors(){
            List<IMyTerminalBlock> connectorBlocks;
            connectorBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connectorBlocks);
            return connectorBlocks;
        }
        /// <summary>
        /// get all the reactors attached to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>reactors</returns>
        List<IMyTerminalBlock> getReactors() {
            List<IMyTerminalBlock> reactorBlocks;
            reactorBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyReactor>(reactorBlocks);
            return reactorBlocks;
        }

        //currently unused methods that are from SE, found good example by malware, hope that dude is gettin paid by keen.
        public int _value1;
        public int _value2; 
    public void Program() {
            
            if (Storage.Length > 0) {
                var parts = Storage.Split(';');
                _value1 = int.Parse(parts[0]);
                _value2 = int.Parse(parts[1]);
            }
            Echo("Constructed"); 
    }
    public void Save() {
	    Storage = _value1 + ";" + _value2;
	    Echo("Saved"); 
    }

        //******************************************************
        //end of code to be exported to SE
        //******************************************************


    }
}
