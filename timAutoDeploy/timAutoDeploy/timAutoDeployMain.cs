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

namespace timAutoDeploy {

    //class
    class timAutoDeploy {
        /// <summary>
        ///this is a fake method to prevent VS from bitching about echo, to be removed
        /// </summary>
        /// <param name="p"></param>
        private void Echo(string p) {
            Console.WriteLine(p);
        }
        //making an empty string to represent storage, dont include this in copy to PB
        string Storage = "";

        //remove this line when import to SE
        IMyGridTerminalSystem GridTerminalSystem;
        IMyGridProgramRuntimeInfo gridProgram;


        //******************************************************
        //start of code to be exported to SE
        //******************************************************


        //******************************************************
        //******************************************************
        // Script Name: TIM Auto-Deploy script
        // Author: Therian
        // Version: 0.2
        // Purpose: To automatically setup TIM on new grids, without having to do 100 lines of data entry.
        // Dependancies: Taleden's Inventory Manager [TIM]
        // Before You Start:
        // This script was designed to be ran when a ship is first built, and you want to install TIM.
        // If you just want to save yourself some time, you should be able to run this completely default and save yourself quite a bit of typing.
        // Make sure you don't have any connector or merge blocks attached if you don't want this to effect that grid as well.
        // MAKE SURE YOU/FACTION OWN ALL THE BLOCKS ON THE GRID. 
        // Usage: 
        // 1. Copy and paste from start of code to end of code tags into your programming block, OR use the blueprint which will only include these.
        // 2. Alter the below config variables to your preference.
        // 3. Click Check code, if you're Configuration is acceptable it should compile successfully.
        // 4. Click Remember and Exit
        // 5. Click Run on the Programming block
        // 6. If you assigned the assemblers, you still need to go to every assembler in the Production tab, and assign it the appropiate component, and then turn on repeat mode. (This is a space engineers limitation)
        // Planned Features: setting cargo container groups, changing that cargo container group, changing cargo groups over antenna
        //
        //
        //******************************************************
        //******************************************************
        //CHANGE LOG:
        // 0.2:
        // By request we now include oxygen generators, change assignOxygen to alter its behavior.
        //
        //
        //******************************************************
        //******************************************************

        //******************************************************
        //******************************************************
        // CONFIG ZONE, Safe to alter
        //******************************************************
        //******************************************************
        //set this to false if you don't want your assemblers to produce ammo
        public bool assignAmmo = true;
        //set this to false if you don't want to assign the assembler component array names to the assemblers
        public bool assignAssemblers = true;
        //set this to false if you don't want tim to manage your refineries and arcs
        public bool assignRefineries = true;
        //set this to false if you don't want tim to assign your reactors uranium
        public bool assignReactors = true;
        //this is how much uranium to give to each reactor, make sure to have the trailing F if you change it, since thats what makes it a float in C# land
        public float ReactorUraniumCount = 100.0F;
        // set this to false if you don't want to use stock TIM values
        public bool assignLcds = true;
        // set this to false if you don't want TIM to assign ice to your oxygen generators
        public bool assignOxygen = true;
        //set this to false if you don't want to assign docking passwords to your connectors
        public bool assignDockingPassword = true;
        // change "password" to whatever you want your shared key to be. this isn't exactly secure at all being just plain text in the name of the connector. Know that anyone who can see that grid can get the password to connect. dont use this for grid security against fellow faction members. detemined/smart griefers will figure this out.
        public string DockingPassword = "password";
        //these arrays are made up of all the components that an assembler can produce. 
        public string[] assemblerComponentArray = { "STEELPLATE", "CONSTRUCTION", "SMALLTUBE", "LARGETUBE", "COMPUTER", "MOTOR",
                                                      "METALGRID", "POWERCELL", "BULLETPROOFGLASS", "DETECTOR", "DISPLAY", "REACTOR",
                                                      "GIRDER", "SOLARCELL", "INTERIORPLATE", "MEDICAL", "THRUST", "RADIOCOMMUNICATION",
                                                      "GRAVITYGENERATOR", "EXPLOSIVES", "SUPERCONDUCTOR" };

        public string[] ammoArray = { "Missile200mm", "NATO_25x184mm", "NATO_5p56x45mm" };
        //set this to true if you want to have the assembler add modded items. get the modded item names by putting them into a cargo container that a running TIM script can see. Then using TIM's LCD component function, you should see the item listed on the LCD screen.
        public bool addModComponentList = false;
        //you can adjust these arrays if you wish to have it build items from different mods. by default it includes CSD autocannon and Battlecannon 
        public string[] modComponentArray = { "Autocannon_Box", "Autocannon_Box_Large", "250shell", "88shell", "88hekc" };

        
        // Automatic Cargo Sorting configs
        // set this to true if you dont want this grid to do automatic cargo sorting
        public bool assignCargo = true;

        public enum cargoModes {globalOnly, customCargo, both};

        cargoModes cargoOption = cargoModes.both;

        //******************************************************
        //******************************************************
        //END OF CONFIG ZONE, careful changing below this
        //******************************************************
        //******************************************************
        //BE CAREFUL MESSING WITH THIS VARIABLE, IF YOU SET IT TO TRUE AFTER THE FIRST RUN, IT MAY OVERWRITE ANY CHANGES YOU HAVE MADE. YOU HAVE BEEN WARNED.
        public bool firstRun = true;

        void Main(string argument) {
            if (firstRun) {
                if (assignAssemblers)
                    setAssemblerNames();
                if (assignRefineries)
                    setRefineryNames();
                if (assignReactors)
                    setReactorNames();
                if (assignDockingPassword)
                    setDockingRights();
                if (assignOxygen)
                    setOxygenNames();
                if (assignLcds)
                    setLcdNames();
                firstRun = false;
            }

            if (assignCargo) {
                cargoController(argument);
            }

        }
        void cargoController(string argument) { 
        
        }
        /// <summary>
        /// set the Refineries and Arcs to have the TIM Ore tag
        /// </summary>
        /// <quirks>
        /// Since Arc furances are refineries, it will name the arcs Refinery, it didnt really bother me enough to dig into it more since i dont manually alter inventories anymore.
        /// </quirks>
        void setRefineryNames() {
            List<IMyTerminalBlock> refineryBlocks = getRefineries();
            int inc = 0;
            foreach (IMyRefinery refinery in refineryBlocks) {
                string name = "refinery " + (inc < 10 ? "0" + inc.ToString() : inc.ToString()) + " [TIM Ore]";
                refinery.SetCustomName(name);
                inc++;
            }
        }
        /// <summary>
        /// set the name of all the assemblers so TIM knows where to look to build any given component.
        /// </summary>
        /// <quirks>
        /// because of Space Engineers limitations, we cannot set assembler productions, you will still need to set these on repeat for each named assembler.
        /// </quirks>
        void setAssemblerNames() {
            List<IMyTerminalBlock> assemblerBlocks = getAssemblers();
            int loopCounter = 0;
            int iteration = 0;
            int assemblerCount = assemblerBlocks.Count;
            int componentCount = assemblerComponentArray.Length + (assignAmmo ? ammoArray.Length : 0) + (addModComponentList ? modComponentArray.Length : 0);
            bool ammoChecked = false;
            bool modsChecked = false;
            bool compChecked = false;
            bool masterAssigned = false;
            
            if (componentCount > assemblerCount) {
                Echo("You are trying to assign: " + componentCount + " Components, But you only have " + assemblerCount + " assemblers");
                Echo("Script will assign as many assemblers as it can starting with ammo first, build " + (componentCount - assemblerCount) + " more assemblers for full TIM assembler managment");
            }
            else if (componentCount == assemblerCount) {
                Echo("TIM will take all your assemblers, if you're not at your block limit, its nice to have a few slaves assemblers");
            }
            else {
                Echo("TIM will be assigned all of the assemblers except for " + (assemblerCount - componentCount) + " assemblers. These will be used to make a Master Slave chain.");
            }

            foreach (IMyAssembler assembler in assemblerBlocks) {
                string name = "assembler " + (loopCounter < 10 ? "0"+loopCounter.ToString() : loopCounter.ToString());
                if (assignAmmo && !ammoChecked) {
                    if (ammoArray.Length > iteration) {
                        name = name + " [TIM " + ammoArray[iteration] + "]";
                        assembler.SetCustomName(name);
                        iteration++;
                        loopCounter++;
                        continue;
                    }
                    else {
                        ammoChecked = true;
                        iteration = 0;
                    }
                    
                }
                 if (addModComponentList && !modsChecked) {
                    if (modComponentArray.Length > iteration) {
                        name = name + " [TIM " + modComponentArray[iteration] + "]";
                        assembler.SetCustomName(name);
                        iteration++;
                        loopCounter++;
                        continue;
                    }
                    else {
                        modsChecked = true;
                        iteration = 0;
                    }
                    
                }
                 if (!compChecked) {
                    if (assemblerComponentArray.Length > iteration) {
                        name = name + " [TIM " + assemblerComponentArray[iteration] + "]";
                        assembler.SetCustomName(name);
                        iteration++;
                        loopCounter++;
                        continue;
                    }
                    else {
                        compChecked = true;
                        iteration = 0;
                    }          
                }
                
                    if (!masterAssigned) {
                        assembler.SetCustomName("Master assembler " + loopCounter);
                        masterAssigned = true;
                        loopCounter++;
                        continue;
                    }
                    else {
                        assembler.SetCustomName("Slave assembler " + loopCounter);
                        assembler.ApplyAction("slaveMode");
                        loopCounter++;
                        continue;
                    }
                    
                


            }
        }
        /// <summary>
        /// set the name of current grids connectors to include the TIM DOCK:"Password" tag inorder to talk to other grids
        /// </summary>
        void setDockingRights() {
            List<IMyTerminalBlock> connectors = getConnectors();
            int inc = 0;
            foreach (IMyShipConnector connector in connectors) {
                string name = "connector " + (inc < 10 ? "0" + inc.ToString() : inc.ToString()) + "[TIM DOCK:\"" + DockingPassword + "\"]";
                connector.SetCustomName(name);
                inc++;
            }
        }
        /// <summary>
        /// insure that reactors have the set amount of Uranium in them
        /// </summary>
        void setReactorNames() {
            List<IMyTerminalBlock> reactors = getReactors();
            int inc = 0;
            foreach (IMyReactor reactor in reactors) {
                string name = "reactor "  +(inc < 10 ? "0" + inc.ToString() : inc.ToString())  +"[TIM uranium:P1:" + ReactorUraniumCount.ToString() + "]";
                reactor.SetCustomName(name);
                inc++;
            }
        }

        void setOxygenNames() {
            List<IMyTerminalBlock> oxygenGens = getOxygen();
            int loopCounter = 0;
            foreach (IMyOxygenGenerator oxygenGen in oxygenGens) {
                oxygenGen.SetCustomName("oxygen generator " + (loopCounter < 10 ? "0" + loopCounter.ToString() : loopCounter.ToString()) + " [TIM ice:p1]");
                loopCounter++;
            }
        }

        void setLcdNames() {
            List<IMyTerminalBlock> lcdPanels = getLcds();
            bool compo = false;
            bool ingot = false;
            bool ore = false;
            bool ammo = false;
            foreach (IMyTerminalBlock lcdPanel in lcdPanels) { 
                if (!compo){
                    lcdPanel.SetCustomName("LCD 1 [TIM component]");
                    compo = true;
                    continue;
                }
                if (!ingot){
                    lcdPanel.SetCustomName("LCD 2 [TIM ingot]");
                    ingot = true;
                    continue;
                }
                if (!ore){
                    lcdPanel.SetCustomName("LCD 3 [TIM Ore]");
                    ore = true;
                    continue;
                }
                if (!ammo) {
                    lcdPanel.SetCustomName("LCD 4 [TIM Ammo]");
                    ammo = true;
                    continue;
                }
                if (ammo && ore && ingot && compo) {
                    break;
                }

            }
        }

        /// <summary>
        /// get all the refineries connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>refineries</returns>
        List<IMyTerminalBlock> getRefineries() {
            List<IMyTerminalBlock> refineryBlocks;
            refineryBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyRefinery>(refineryBlocks);
            return refineryBlocks;
        }
        /// <summary>
        /// get all the assemblers connected to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>assemblers</returns>
        List<IMyTerminalBlock> getAssemblers() {
            List<IMyTerminalBlock> AssemblerBlocks;
            AssemblerBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(AssemblerBlocks);
            return AssemblerBlocks;
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
        /// get all the connectors attached to our current grid.
        /// </summary>
        /// <returns>List<IMyTerminalBlock>connectors</returns>
        List<IMyTerminalBlock> getConnectors() {
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

        List<IMyTerminalBlock> getLcds() {
            List<IMyTerminalBlock> lcdBlocks;
            lcdBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(lcdBlocks);
            return lcdBlocks;
        }

        List<IMyTerminalBlock> getOxygen() {
            List<IMyTerminalBlock> oxygenBlocks;
            oxygenBlocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlocksOfType<IMyOxygenGenerator>(oxygenBlocks);
            return oxygenBlocks;
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
