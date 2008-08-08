﻿
using System;
namespace ProtoBuf.Property
{
    internal sealed class PropertyDouble<TSource> : Property<TSource, double>
    {
        public override string DefinedType
        {
            get { return ProtoFormat.FIXED64; }
        }
        public override WireType WireType { get { return WireType.Fixed64; } }

        public override int Serialize(TSource source, SerializationContext context)
        {
            double value = GetValue(source);
            if (IsOptional && value == DefaultValue) return 0;
            byte[] raw = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian) SerializationContext.Reverse8(raw);
            int len = WritePrefix(context);
            context.Write(raw, 0, 8);
            return len + 8;
        }

        public override double DeserializeImpl(TSource source, SerializationContext context)
        {
            context.ReadBlock(8);
            if (!BitConverter.IsLittleEndian) SerializationContext.Reverse8(context.Workspace);
            return BitConverter.ToDouble(context.Workspace, 0);
        }
    }
}
