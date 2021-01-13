using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpVehicleInstance : hkpUnaryAction
    {
        public override uint Signature { get => 0; }
        
        public hkpVehicleData m_data;
        public hkpVehicleDriverInput m_driverInput;
        public hkpVehicleSteering m_steering;
        public hkpVehicleEngine m_engine;
        public hkpVehicleTransmission m_transmission;
        public hkpVehicleBrake m_brake;
        public hkpVehicleSuspension m_suspension;
        public hkpVehicleAerodynamics m_aerodynamics;
        public hkpVehicleWheelCollide m_wheelCollide;
        public hkpTyremarksInfo m_tyreMarks;
        public hkpVehicleVelocityDamper m_velocityDamper;
        public hkpVehicleSimulation m_vehicleSimulation;
        public List<hkpVehicleInstanceWheelInfo> m_wheelsInfo;
        public hkpVehicleDriverInputStatus m_deviceStatus;
        public List<bool> m_isFixed;
        public float m_wheelsTimeSinceMaxPedalInput;
        public bool m_tryingToReverse;
        public float m_torque;
        public float m_rpm;
        public float m_mainSteeringAngle;
        public float m_mainSteeringAngleAssumingNoReduction;
        public List<float> m_wheelsSteeringAngle;
        public bool m_isReversing;
        public sbyte m_currentGear;
        public bool m_delayed;
        public float m_clutchDelayCountdown;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_data = des.ReadClassPointer<hkpVehicleData>(br);
            m_driverInput = des.ReadClassPointer<hkpVehicleDriverInput>(br);
            m_steering = des.ReadClassPointer<hkpVehicleSteering>(br);
            m_engine = des.ReadClassPointer<hkpVehicleEngine>(br);
            m_transmission = des.ReadClassPointer<hkpVehicleTransmission>(br);
            m_brake = des.ReadClassPointer<hkpVehicleBrake>(br);
            m_suspension = des.ReadClassPointer<hkpVehicleSuspension>(br);
            m_aerodynamics = des.ReadClassPointer<hkpVehicleAerodynamics>(br);
            m_wheelCollide = des.ReadClassPointer<hkpVehicleWheelCollide>(br);
            m_tyreMarks = des.ReadClassPointer<hkpTyremarksInfo>(br);
            m_velocityDamper = des.ReadClassPointer<hkpVehicleVelocityDamper>(br);
            m_vehicleSimulation = des.ReadClassPointer<hkpVehicleSimulation>(br);
            m_wheelsInfo = des.ReadClassArray<hkpVehicleInstanceWheelInfo>(br);
            m_deviceStatus = des.ReadClassPointer<hkpVehicleDriverInputStatus>(br);
            m_isFixed = des.ReadBooleanArray(br);
            m_wheelsTimeSinceMaxPedalInput = br.ReadSingle();
            m_tryingToReverse = br.ReadBoolean();
            br.ReadUInt16();
            br.ReadByte();
            m_torque = br.ReadSingle();
            m_rpm = br.ReadSingle();
            m_mainSteeringAngle = br.ReadSingle();
            m_mainSteeringAngleAssumingNoReduction = br.ReadSingle();
            m_wheelsSteeringAngle = des.ReadSingleArray(br);
            m_isReversing = br.ReadBoolean();
            m_currentGear = br.ReadSByte();
            m_delayed = br.ReadBoolean();
            br.ReadByte();
            m_clutchDelayCountdown = br.ReadSingle();
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer<hkpVehicleData>(bw, m_data);
            s.WriteClassPointer<hkpVehicleDriverInput>(bw, m_driverInput);
            s.WriteClassPointer<hkpVehicleSteering>(bw, m_steering);
            s.WriteClassPointer<hkpVehicleEngine>(bw, m_engine);
            s.WriteClassPointer<hkpVehicleTransmission>(bw, m_transmission);
            s.WriteClassPointer<hkpVehicleBrake>(bw, m_brake);
            s.WriteClassPointer<hkpVehicleSuspension>(bw, m_suspension);
            s.WriteClassPointer<hkpVehicleAerodynamics>(bw, m_aerodynamics);
            s.WriteClassPointer<hkpVehicleWheelCollide>(bw, m_wheelCollide);
            s.WriteClassPointer<hkpTyremarksInfo>(bw, m_tyreMarks);
            s.WriteClassPointer<hkpVehicleVelocityDamper>(bw, m_velocityDamper);
            s.WriteClassPointer<hkpVehicleSimulation>(bw, m_vehicleSimulation);
            s.WriteClassArray<hkpVehicleInstanceWheelInfo>(bw, m_wheelsInfo);
            s.WriteClassPointer<hkpVehicleDriverInputStatus>(bw, m_deviceStatus);
            s.WriteBooleanArray(bw, m_isFixed);
            bw.WriteSingle(m_wheelsTimeSinceMaxPedalInput);
            bw.WriteBoolean(m_tryingToReverse);
            bw.WriteUInt16(0);
            bw.WriteByte(0);
            bw.WriteSingle(m_torque);
            bw.WriteSingle(m_rpm);
            bw.WriteSingle(m_mainSteeringAngle);
            bw.WriteSingle(m_mainSteeringAngleAssumingNoReduction);
            s.WriteSingleArray(bw, m_wheelsSteeringAngle);
            bw.WriteBoolean(m_isReversing);
            bw.WriteSByte(m_currentGear);
            bw.WriteBoolean(m_delayed);
            bw.WriteByte(0);
            bw.WriteSingle(m_clutchDelayCountdown);
        }
    }
}
