using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Arch
{
    public struct ArchEngineExhaustEffect : ICloneable
    {
        ulong effect;
        ulong hardpoint;

        public ArchEngineExhaustEffect(ulong effect, ulong hardpoint)
        {
            this.effect = effect;
            this.hardpoint = hardpoint;
        }

        public ArchEngineExhaustEffect(ArchEngineExhaustEffect e)
        {
            this.effect = e.effect;
            this.hardpoint = e.hardpoint;
        }

        public object Clone()
        {
            return new ArchEngineExhaustEffect(this);
        }
    }

    public class ArchWorldObjectComponentPropertyEngine : ArchWorldObjectComponentProperty
    {
        public ArchWorldObjectComponentPropertyEngine()
        {

        }

        public string sound { get; set; }

        float _forceThrustForwardBackward;
        float _forceThrustUpDown;
        float _forceThrustLeftRight;
        float _torqueYaw;
        float _torquePitch;
        float _torqueRoll;
        float _heatGenerationThrust;
        float _heatGenerationTorque;
        float _maxHeatCapacity;
        float _overheatMalfunctionChance;
        float _constPowerUsage;
        float _coolantEfficiency;
        float _empResistance;
        float _oneshotForwardThrust;
        float _oneshotForwardThrustHeat;
        float _frictionTorque;
        float _frictionVelocity;
        List<ArchEngineExhaustEffect> _engineExhaustEffects = new List<ArchEngineExhaustEffect>();

        public List<ArchEngineExhaustEffect> engineExhaustEffects { get { return _engineExhaustEffects; } }
        public float frictionTorque { get { return _frictionTorque; } set { _frictionTorque = value; } }
        public float frictionVelocity { get { return _frictionVelocity; } set { _frictionVelocity = value; } }
        public float forceThrustForwardBackward { get { return _forceThrustForwardBackward; } set { _forceThrustForwardBackward = value; } }
        public float forceThrustUpDown { get { return _forceThrustUpDown; } set { _forceThrustUpDown = value; } }
        public float forceThrustLeftRight { get { return _forceThrustLeftRight; } set { _forceThrustLeftRight = value; } }
        public float torqueYaw { get { return _torqueYaw; } set { _torqueYaw = value; } }
        public float torquePitch { get { return _torquePitch; } set { _torquePitch = value; } }
        public float torqueRoll { get { return _torqueRoll; } set { _torqueRoll = value; } }
        public float heatGenerationThrust { get { return _heatGenerationThrust; } set { _heatGenerationThrust = value; } }
        public float heatGenerationTorque { get { return _heatGenerationTorque; } set { _heatGenerationTorque = value; } }
        public float maxHeatCapacity { get { return _maxHeatCapacity; } set { _maxHeatCapacity = value; } }
        public float overheatMalfunctionChance { get { return _overheatMalfunctionChance; } set { _overheatMalfunctionChance = value; } }
        public float constPowerUsage { get { return _constPowerUsage; } set { _constPowerUsage = value; } }
        public float coolantEfficiency { get { return _coolantEfficiency; } set { _coolantEfficiency = value; } }
        public float empResistance { get { return _empResistance; } set { _empResistance = value; } }
        public float oneshotForwardThrust { get { return _oneshotForwardThrust; } set { _oneshotForwardThrust = value; } }
        public float oneshotForwardThrustHeat { get { return _oneshotForwardThrustHeat; } set { _oneshotForwardThrustHeat = value; } }

        public ArchWorldObjectComponentPropertyEngine(ArchWorldObjectComponentPropertyEngine arch)
        {
            _engineExhaustEffects = Helpers.DeepCopyCloneableList<ArchEngineExhaustEffect>(arch._engineExhaustEffects);
            _forceThrustForwardBackward = arch._forceThrustForwardBackward;
            _forceThrustUpDown = arch._forceThrustUpDown;
            _forceThrustLeftRight = arch._forceThrustLeftRight;
            _torqueYaw = arch._torqueYaw;
            _torquePitch = arch._torquePitch;
            _torqueRoll = arch._torqueRoll;
            _heatGenerationThrust = arch._heatGenerationThrust;
            _heatGenerationTorque = arch._heatGenerationTorque;
            _maxHeatCapacity = arch._maxHeatCapacity;
            _overheatMalfunctionChance = arch._overheatMalfunctionChance;
            _constPowerUsage = arch._constPowerUsage;
            _coolantEfficiency = arch._coolantEfficiency;
            _empResistance = arch._empResistance;
            _oneshotForwardThrust = arch._oneshotForwardThrust;
            _oneshotForwardThrustHeat = arch._oneshotForwardThrustHeat;
            _frictionTorque = arch._frictionTorque;
            _frictionVelocity = arch._frictionVelocity;
        }

        public override void ReadHeader(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
            {
                if (p.Check("sound"))
                {
                    sound = p.GetString(0);
                    continue;
                }
                else
                if (p.Check("exhaust"))
                {
                    _engineExhaustEffects.Add(new ArchEngineExhaustEffect(p.GetStrkey64(0), p.GetStrkey64(1)));
                    continue;
                }
                else
                if (p.Check("force_thrust_forward_backward"))
                {
                    _forceThrustForwardBackward = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("force_thrust_left_right"))
                {
                    _forceThrustLeftRight = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("force_thrust_up_down"))
                {
                    _forceThrustUpDown = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("force_torque_yaw"))
                {
                    _torqueYaw = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("force_torque_pitch"))
                {
                    _torquePitch = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("force_torque_roll"))
                {
                    _torqueRoll = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("friction_torque"))
                {
                    _frictionTorque = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("friction_velocity"))
                {
                    _frictionVelocity = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("heat_generation_thrust"))
                {
                    _heatGenerationThrust = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("heat_generation_torque"))
                {
                    _heatGenerationTorque = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("max_heat_capacity"))
                {
                    _maxHeatCapacity = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("overheat_malfunction_chance"))
                {
                    _overheatMalfunctionChance = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("const_power_usage"))
                {
                    _constPowerUsage = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("coolant_efficiency"))
                {
                    _coolantEfficiency = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("emp_resistance"))
                {
                    _empResistance = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("oneshot_forward_thrust"))
                {
                    _oneshotForwardThrust = p.GetFloat(0);
                    continue;
                }
                else
                if (p.Check("oneshot_forward_thrust_heat"))
                {
                    _oneshotForwardThrust = p.GetFloat(0);
                    continue;
                }
            }
        }
    }
}
