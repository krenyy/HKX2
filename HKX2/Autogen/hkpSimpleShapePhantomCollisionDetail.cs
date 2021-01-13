using System.Collections.Generic;
using System.Numerics;

namespace HKX2
{
    public partial class hkpSimpleShapePhantomCollisionDetail : IHavokObject
    {
        public virtual uint Signature { get => 0; }
        
        public hkpCollidable m_collidable;
        
        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_collidable = des.ReadClassPointer<hkpCollidable>(br);
        }
        
        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteClassPointer<hkpCollidable>(bw, m_collidable);
        }
    }
}
