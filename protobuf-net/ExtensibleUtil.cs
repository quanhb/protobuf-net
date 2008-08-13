﻿//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Reflection;
//using ProtoBuf.Property;

//namespace ProtoBuf
//{
//    /// <summary>
//    /// This class acts as an internal wrapper allowing us to do a dynamic
//    /// methodinfo invoke; an't put into Serializer as don't want on public
//    /// API; can't put into Serializer&lt;T&gt; since we need to invoke
//    /// accross classes, which isn't allowed in Silverlight)
//    /// </summary>
//    internal static class ExtensibleUtil
//    {
//        /// <summary>
//        /// All this does is call GetExtendedValuesTyped with the correct type for "instance";
//        /// this ensures that we don't get issues with subclasses declaring conflicting types -
//        /// the caller must respect the fields defined for the type they pass in.
//        /// </summary>
//        internal static IEnumerable<TValue> GetExtendedValues<TValue>(IExtensible instance, int tag, DataFormat format, bool singleton)
//        {
//            return (IEnumerable<TValue>)typeof(ExtensibleUtil)
//                .GetMethod("GetExtendedValuesTyped", BindingFlags.Public | BindingFlags.Static)
//                .MakeGenericMethod(instance.GetType(), typeof(TValue))
//                .Invoke(null, new object[] { instance, tag, format, singleton });
//        }

//        /// <summary>
//        /// Reads the given value(s) from the instance's stream; the serializer
//        /// is inferred from TValue and format. For singletons, each occurrence
//        /// is merged [only applies for sub-objects], and the composed
//        /// value if yielded once; otherwise ("repeated") each occurrence
//        /// is yielded separately.
//        /// </summary>
//        /// <remarks>Needs to be public to be callable thru reflection in Silverlight</remarks>
//        public static IEnumerable<TValue> GetExtendedValuesTyped<TSource, TValue>(
//            TSource instance, int tag, DataFormat format, bool singleton)
//            where TSource : class, IExtensible, new()
//        {
//            if (instance == null) throw new ArgumentNullException("instance");

//            Serializer<TSource>.CheckTagNotInUse(tag);
//            Property<TValue,TValue> prop = PropertyFactory.CreatePassThru<TValue>(tag, format);

//            IExtension extn = instance.GetExtensionObject(false);
//            if (extn == null) yield break;

//            Stream stream = extn.BeginQuery();
//            TValue lastValue = default(TValue);
//            bool hasValue = false;
//            try
//            {
//                SerializationContext ctx = new SerializationContext(stream, null);
//                uint fieldPrefix;
//                while ((fieldPrefix = ctx.TryReadFieldPrefix()) > 0)
//                {
//                    if (fieldPrefix == prop.FieldPrefix)
//                    {
//                        TValue value = prop.DeserializeImpl(lastValue, ctx);
//                        hasValue = true;
//                        if (singleton)
//                        {
//                            // merge with later values before returning
//                            lastValue = value;
//                        }
//                        else
//                        {
//                            // return immediately; no merge
//                            yield return value;
//                        }
//                    }
//                    else
//                    {
//                        int readTag;
//                        WireType wireType;
//                        Serializer.ParseFieldToken(fieldPrefix, out wireType, out readTag);
//                        // skip all other tags
//                        Serializer.SkipData(ctx, readTag, wireType);
//                    }
//                }
//            }
//            finally
//            {
//                extn.EndQuery(stream);
//            }

//            if (singleton && hasValue)
//            {
//                yield return lastValue;
//            }
//        }

//        /// <summary>
//        /// All this does is call AppendExtendValueTyped with the correct type for "instance";
//        /// this ensures that we don't get issues with subclasses declaring conflicting types -
//        /// the caller must respect the fields defined for the type they pass in.
//        /// </summary>
//        internal static void AppendExtendValue<TValue>(IExtensible instance, int tag, DataFormat format, object value)
//        {
//            if (instance == null) throw new ArgumentNullException("instance");
//            typeof(ExtensibleUtil)
//                .GetMethod("AppendExtendValueTyped", BindingFlags.Public | BindingFlags.Static)
//                .MakeGenericMethod(instance.GetType(), typeof(TValue))
//                .Invoke(null, new object[] { instance, tag, format, value });
//        }

//        /// <summary>
//        /// Stores the given value into the instance's stream; the serializer
//        /// is inferred from TValue and format.
//        /// </summary>
//        /// <remarks>Needs to be public to be callable thru reflection in Silverlight</remarks>
//        public static void AppendExtendValueTyped<TSource, TValue>(
//            TSource instance, int tag, DataFormat format, TValue value)
//            where TSource : class, IExtensible, new()
//        {
//            Serializer<TSource>.CheckTagNotInUse(tag);
            
//            ISerializer<TValue> serializer = SerializerCache<TValue>.GetSerializer(format);

//            // check the length of the new value, using the null stream as a stop-gap
//            SerializationContext nullCtx = new SerializationContext(Stream.Null);

//            Stream stream = instance.BeginAppend();
//            try
//            {
//                SerializationContext ctx = new SerializationContext(stream);
//                ctx.Push(instance); // for recursion detection
//                Serializer.WriteFieldToken(tag, serializer.WireType, ctx);
//                serializer.Serialize(value, ctx);
//                instance.EndAppend(stream, true);
//                ctx.Pop(instance);
//            }
//            catch
//            {
//                instance.EndAppend(stream, false);
//                throw;
//            }
//        }
//    }
//}
