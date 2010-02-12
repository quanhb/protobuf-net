﻿using System;


namespace ProtoBuf.Serializers
{
    sealed class Int32Serializer : IProtoSerializer
    {
        public Type ExpectedType { get { return typeof(int); } }
        public void Write(object value, ProtoWriter dest)
        {
            dest.WriteInt32((int)value);
        }
#if FEAT_COMPILER
        void IProtoSerializer.EmitWrite(Compiler.CompilerContext ctx, Compiler.Local valueFrom)
        {
            ctx.EmitWrite("WriteInt32", typeof(int), valueFrom);
        }
#endif
    }
}
