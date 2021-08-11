using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void ArmWarheads(List<IMyWarhead> warheads, bool setTo)
        {
            foreach (var bomb in warheads)
            {
                bomb.IsArmed = setTo;
            }
        }

        public float getMagnitude(Vector3 A)
        {
            float m = (float)Math.Sqrt((A.X * A.X) + (A.Y * A.Y) + (A.Z * A.Z));
            return m;
        }

        public class Real_Timer
        {
            readonly DateTime T;
            bool init = false;
            public Real_Timer(float Seconds) { T = DateTime.Now.AddSeconds(Seconds); }
            public bool active { get { return DateTime.Now < T; } }
            public bool initialized
            {
                get { return init; }
                set { init = value; }
            }
        }


        public Real_Timer Start_real_timer(float i, ref Real_Timer timer)
        {
            Real_Timer t;
            if (!timer.initialized)
            {
                Echo($"Real Timer started for {i} seconds");
                t = new Real_Timer(i)
                {
                    initialized = true
                };
                return t;
            }
            else
                Echo("Timer initialzied");
            return timer;
        }

        public bool Timer_done(Real_Timer timer)
        {
            bool rtn = false;
            if (!timer.active)
            {
                timer.initialized = false;
                rtn = true;
            }
            else
                Echo("Timer is active");
            return rtn;
        }


        public void StockPile(List<IMyGasTank> tanks, bool setTo)
        {
            foreach (var tank in tanks)
            {
                tank.Stockpile = setTo;
            }
        }

        public Real_Timer timer = new Real_Timer(0);

        public void Main(string argument, UpdateType updateSource)
        {

            List<IMyWarhead> warheads = new List<IMyWarhead>();
            List<IMyGasTank> tanks = new List<IMyGasTank>();

            GridTerminalSystem.GetBlocksOfType<IMyWarhead>(warheads);
            GridTerminalSystem.GetBlocksOfType<IMyGasTank>(tanks);
            GridTerminalSystem.GetBlockGroupWithName("ALERT");
            IMyLargeTurretBase turret = (IMyLargeTurretBase)GridTerminalSystem.GetBlockWithName("R_DESIGNATOR");
            IMySoundBlock alert = (IMySoundBlock)GridTerminalSystem.GetBlockWithName("ALERT");
            IMyLightingBlock pLight = (IMyLightingBlock)GridTerminalSystem.GetBlockWithName("P Light");
            IMyProgrammableBlock MissleControl = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Missle Control"); ;
            
            var target = turret.GetTargetedEntity();
            Echo("Start of logic");
            if (!target.IsEmpty())
            {
                Echo("target found");
                if (1 <= getMagnitude(target.Velocity))
                {
                    Echo("is a threat");
                    if (Timer_done(timer))
                    {
                        Echo("Attcking Enemy");

                        Echo(timer.active.ToString());
                        timer = Start_real_timer(3, ref timer);


                        StockPile(tanks, false);
                        ArmWarheads(warheads, true);
                        alert.Play();
                        Echo("Firing");
                        bool success = MissleControl.TryRun("");
                        if (!success)
                        {
                            pLight.Enabled = true;
                        }
                    }
                }
                else 
                { 
                    Echo("not a threat");
                }
            }
            else
            {
                Echo("enemy not found");
            }

            target = new MyDetectedEntityInfo();
            ArmWarheads(warheads, false);
            StockPile(tanks, true);
            Echo("End of logic");

        }
    }
}
/*
            IMyLargeTurretBase turret = null;
            IMySoundBlock alert = null;
            IMyLightingBlock pLight = null;
            IMyProgrammableBlock MissleControl = null;
            /*
            try
            {
                MissleControl = (IMyProgrammableBlock)GridTerminalSystem.GetBlockWithName("Missle Control");
                Echo("Missle Control init Success\n");
            }
            catch
            {
                Echo("Missle Control init fail\n");
            }
            try { 
                turret = (IMyLargeTurretBase)GridTerminalSystem.GetBlockWithName("R_DESIGNATOR");
                Echo("Turret init Success\n");
            }
            catch 
            {
                Echo("Turret init fail\n");
            }
            try
            {
                alert = (IMySoundBlock)GridTerminalSystem.GetBlockWithName("ALERT");
                pLight = (IMyLightingBlock)GridTerminalSystem.GetBlockWithName("P Light");
                Echo("AUX init Success\n");
            }
            catch 
            {
                Echo("AUX init fail\n");
            }

            var target = turret.GetTargetedEntity();
            /*
            if (!Timer_done(timer))
            {
                target = new MyDetectedEntityInfo();
                //StockPile(tanks, false);
                ArmWarheads(warheads, false);
                return;
            }
            else if (!(target.IsEmpty()) && 1 <= getMagnitude(target.Velocity))
            {
                try {
                    Echo(timer.active.ToString());
                    timer = Start_real_timer(3, ref timer);
                }
                catch
                {
                    Echo("timer start fail");
                }
                //StockPile(tanks, false);
                ArmWarheads(warheads, true);
                alert.Play();
                Echo("Firing");
                bool success = MissleControl.TryRun("");
                if (!success) {
                    pLight.Enabled = true;
                }
            
            }
            else if (1 >= getMagnitude(target.Velocity))
            {
                
                ArmWarheads(warheads, false);
                //StockPile(tanks, true);
            }
            */