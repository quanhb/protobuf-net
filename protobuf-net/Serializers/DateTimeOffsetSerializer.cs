#if !NO_RUNTIME
using System;



namespace ProtoBuf.Serializers
{
    sealed class DateTimeOffsetSerializer : IProtoSerializer
    {
        public Type ExpectedType { get { return typeof(DateTimeOffset); } }
        public void Write(object value, ProtoWriter dest)
        {
            BclHelpers.WriteDateTimeOffset((DateTimeOffset)value, dest);
        }
        bool IProtoSerializer.RequiresOldValue { get { return false; } }
        bool IProtoSerializer.ReturnsValue { get { return true; } }
        public object Read(object value, ProtoReader source)
        {
            Helpers.DebugAssert(value == null); // since replaces
            return BclHelpers.ReadDateTimeOffset(source);
        }
#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitWrite(typeof(BclHelpers), "WriteDateTimeOffset", valueFrom);
        }
        void IProtoSerializer.EmitRead(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitBasicRead(typeof(BclHelpers), "ReadDateTimeOffset", ExpectedType);
        }
#endif

    }
}
#endif