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
        // Version: 0.3
        // UPDATE 2018-10-26
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
        // 0.3:
        // UPDATE 2018-10-26:
        // updated so no longer deprecated. 
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
                firstRun = false; 
            } 
 
 
            //cargo assignment stuff goes here 
 
 
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
                refinery.CustomName = name.ToString(); 
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
            bool masterAssigned = false;
            int assemblerCount = assemblerBlocks.Count; 
            foreach (IMyAssembler assembler in assemblerBlocks) { 
                string name = "assembler " + (loopCounter < 10 ? "0"+loopCounter.ToString() : loopCounter.ToString()); 
                 
                    if (!masterAssigned) { 
                        assembler.CustomName = "Master assembler " + loopCounter; 
                        masterAssigned = true; 
                        loopCounter++; 
                        continue; 
                    } 
                    else { 
                        assembler.CustomName = "Slave assembler " + loopCounter; 
                        assembler.CooperativeMode = true;
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
                connector.CustomName = name.ToString();
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
                reactor.CustomName = name.ToString();
                inc++; 
            } 
        } 
 
        void setOxygenNames() { 
            List<IMyTerminalBlock> oxygenGens = getOxygen(); 
            int loopCounter = 0; 
            foreach (IMyGasGenerator oxygenGen in oxygenGens) { 
                oxygenGen.CustomName = "oxygen generator " + (loopCounter < 10 ? "0" + loopCounter.ToString() : loopCounter.ToString()) + " [TIM ice:p1]"; 
                loopCounter++; 
            } 
        } 
 
        void setLcdNames() {  
         
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
            GridTerminalSystem.GetBlocksOfType<IMyGasGenerator>(oxygenBlocks); 
            return oxygenBlocks; 
        }

    }
}
