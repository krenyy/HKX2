namespace HKX2
{
    public partial class hkpLimitedHingeConstraintData : hkpConstraintData
    {
        public override uint Signature => 0x75AAE5A3;

        public hkpLimitedHingeConstraintDataAtoms m_atoms;
        
        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.ReadUSize();
            m_atoms = new hkpLimitedHingeConstraintDataAtoms();
            m_atoms.Read(des, br);
        }
        
        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUSize(0);
            m_atoms.Write(s, bw);
        }
    }
}
