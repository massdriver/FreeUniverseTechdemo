using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace FreeUniverse.Core.Serialization
{
    public sealed class FastSerializableAttribute : Attribute
    {

    }

    public class BaseFastSerializable<T>
    {
        public static FastSerializer serializer = new FastSerializer(typeof(T));
    }

    public abstract class SerializationProvider
    {
        public SerializationProvider(Type type)
        {
            this.type = type;
        }

        public Type type { get; private set; }

        public abstract void Serialize(object obj, BinaryWriter writer);
        public abstract void Deserialize(out object obj, BinaryReader reader);
    }

    public sealed class FastSerializer
    {
        public const int DEFAULT_BUFFER_SIZE = 4096 * 8;
        public Type type { get; private set; }
        private byte[] buffer { get; set; }
        private Dictionary<Type, SerializationProvider> serializationProviders { get; set; }
        private MemoryStream outputStream { get; set; }
        private BinaryWriter binaryWriter { get; set; }
        public bool useChecksum { get; set; }

        public FastSerializer(Type type)
        {
            this.type = type;
            useChecksum = false;

            if (!type.IsClass) throw new Exception("FastSerializer cant operate over non-class object");
            if (!type.IsDefined(typeof(FastSerializableAttribute), false)) throw new Exception("FastSerializerAttribute should be defined over serializable class");

            buffer = new byte[DEFAULT_BUFFER_SIZE];
            outputStream = new MemoryStream(buffer);
            binaryWriter = new BinaryWriter(outputStream);

            BuildSerialization();
        }

        void BuildSerialization()
        {
            serializationProviders = new Dictionary<Type, SerializationProvider>();
            serializationProviders[type] = new SerializationProviderClass(type, serializationProviders);
        }

        public string Serialize(object obj)
        {
            return Convert.ToBase64String(SerializeToBytes(obj), 0, (int)binaryWriter.BaseStream.Position);
        }

        public object Deserialize(string data)
        {
            return DeserializeFromBytes(Convert.FromBase64String(data));
        }

        public byte[] SerializeToBytes(object obj)
        {
            outputStream.Seek(0, SeekOrigin.Begin);
            serializationProviders[type].Serialize(obj, binaryWriter);
            byte[] outArray = new byte[(int)binaryWriter.BaseStream.Position];
            Array.Copy(buffer, outArray, outArray.Length);

            return outArray;
        }

        public object DeserializeFromBytes(byte[] data)
        {
            object obj = null;

            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);
            serializationProviders[type].Deserialize(out obj, reader);

            return obj;
        }
    }

    public sealed class SerializationProviderString : SerializationProvider
    {
        public SerializationProviderString() : base(typeof(String)) { }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            if (obj != null)
                writer.Write((string)obj);
            else
                writer.Write("");
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = reader.ReadString();
        }
    }

    public sealed class SerializationProviderClass : SerializationProvider
    {
        public SerializationProviderClass(Type objType, Dictionary<Type, SerializationProvider> serializers)
            : base(objType)
        {
            objectProperties = new Dictionary<uint, Property>();
            this.sharedSerializers = serializers;
            BuildSerialization();
        }

        void BuildSerialization()
        {
            PropertyInfo[] props = type.GetProperties();

            foreach (PropertyInfo propertyInfo in props)
            {
                if (!propertyInfo.IsDefined(typeof(FastSerializableAttribute), false))
                    continue;

                if( !propertyInfo.GetGetMethod().IsPublic || !propertyInfo.GetSetMethod().IsPublic )
                    throw new Exception("get and set accessors should be public for " + type.Name + "'s property: " + propertyInfo.Name);

                uint hash = FastHash.StringToUINT32(propertyInfo.Name);

                if (objectProperties.ContainsKey(hash))
                    throw new Exception("duplicate property hash found, rename " + type.Name + "'s property: " + propertyInfo.Name);

                objectProperties[hash] = new Property(propertyInfo, BuildSerializationProvider(propertyInfo));

            }

            return;
        }

        SerializationProvider BuildSerializationProvider(PropertyInfo prop)
        {
            Type propertyType = prop.PropertyType;

            SerializationProvider provider = SerializationProviderFactory.MakeSerializationProvider(propertyType, sharedSerializers);

            if (provider == null) throw new Exception("Unable to create serializer for type: " + type.FullName);

            sharedSerializers[propertyType] = provider;
            return provider;
        }

        private sealed class Property
        {
            public Property(PropertyInfo p, SerializationProvider s) { propertyInfo = p; provider = s; getter = p.GetGetMethod(); setter = p.GetSetMethod(); }
            public PropertyInfo propertyInfo { get; set; }
            public SerializationProvider provider { get; set; }
            public MethodInfo getter { get; set; }
            public MethodInfo setter { get; set; }
        };

        private Dictionary<uint, Property> objectProperties { get; set; }
        private Dictionary<Type, SerializationProvider> sharedSerializers { get; set; }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            foreach (KeyValuePair<uint, Property> k in objectProperties)
            {
                writer.Write(k.Key);
                k.Value.provider.Serialize(k.Value.propertyInfo.GetValue(obj, null), writer);
            }
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = Activator.CreateInstance(type);

            int propsRead = 0;

            while (propsRead < objectProperties.Count && reader.BaseStream.Position < reader.BaseStream.Length)
            {
                uint propkey = reader.ReadUInt32();
                Property prop = objectProperties[propkey];
                object propValue = null;
                prop.provider.Deserialize(out propValue, reader);
                prop.propertyInfo.SetValue(obj, propValue, null);
                propsRead++;
            }
        }
    }

    public sealed class SerializationProviderFactory
    {
        public static SerializationProvider MakeSerializationProvider(Type objType, Dictionary<Type, SerializationProvider> serializers)
        {
            SerializationProvider provider;
            serializers.TryGetValue(objType, out provider);

            if (provider != null) return provider;
            if (objType == typeof(uidkey)) return new SerializationProviderUIDKEY();
            if (objType == typeof(UInt64)) return new SerializationProviderUINT64();
            if (objType == typeof(String)) return new SerializationProviderString();
            if (objType == typeof(UInt32)) return new SerializationProviderUINT32();
            if (objType == typeof(DateTime)) return new SerializationProviderDateTime();
            if (objType == typeof(Boolean)) return new SerializationProviderBool();
            if (objType == typeof(float)) return new SerializationProviderFloat();
            if (objType.FullName.StartsWith("System.Collections.Generic.Dictionary")) return new SerializationProviderDictionary(objType, serializers);
            if (objType.FullName.StartsWith("System.Collections.Generic.List")) return new SerializationProviderList(objType, serializers);
            if (objType.IsClass) return new SerializationProviderClass(objType, serializers);
            return null;
        }
    }

    public sealed class SerializationProviderFloat : SerializationProvider
    {
        public SerializationProviderFloat() : base(typeof(float)) { }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            writer.Write((float)obj);
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = reader.ReadSingle();
        }
    }

    public sealed class SerializationProviderBool : SerializationProvider
    {
        public SerializationProviderBool() : base(typeof(Boolean)) { }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            writer.Write((Boolean)obj);
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = reader.ReadBoolean();
        }
    }

    public sealed class SerializationProviderDateTime : SerializationProvider
    {
        public SerializationProviderDateTime()
            : base(typeof(DateTime))
        {

        }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            writer.Write(((DateTime)obj).ToBinary());
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = DateTime.FromBinary(reader.ReadInt64());
        }
    }

    public sealed class SerializationProviderList : SerializationProvider
    {
        public SerializationProviderList(Type objType, Dictionary<Type, SerializationProvider> serializers)
            : base(objType)
        {
            valueProvider = SerializationProviderFactory.MakeSerializationProvider(objType.GetGenericArguments()[0], serializers);
        }

        private SerializationProvider valueProvider { get; set; }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            IList lst = (IList)obj;

            writer.Write((int)lst.Count);

            foreach (object e in lst)
                valueProvider.Serialize(e, writer);
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = Activator.CreateInstance(type);
            IList lst = (IList)obj;
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                object val = null;
                valueProvider.Deserialize(out val, reader);
                lst.Add(val);
            }
        }
    }

    public sealed class SerializationProviderDictionary : SerializationProvider
    {
        public SerializationProviderDictionary(Type objType, Dictionary<Type, SerializationProvider> serializers)
            : base(objType)
        {
            Type keyType = objType.GetGenericArguments()[0];
            Type valueType = objType.GetGenericArguments()[1];

            keyProvider = SerializationProviderFactory.MakeSerializationProvider(keyType, serializers);
            valueProvider = SerializationProviderFactory.MakeSerializationProvider(valueType, serializers);
        }

        private SerializationProvider valueProvider { get; set; }
        private SerializationProvider keyProvider { get; set; }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            IDictionary dict = (IDictionary)obj;

            writer.Write((int)dict.Count);

            foreach (DictionaryEntry e in dict)
            {
                keyProvider.Serialize(e.Key, writer);
                valueProvider.Serialize(e.Value, writer);
            }
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = Activator.CreateInstance(type);

            IDictionary dict = (IDictionary)obj;

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                object key, val;
                keyProvider.Deserialize(out key, reader);
                valueProvider.Deserialize(out val, reader);
                dict.Add(key, val);
            }
        }
    }

    public sealed class SerializationProviderUINT64 : SerializationProvider
    {
        public SerializationProviderUINT64() : base(typeof(UInt64)) { }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            writer.Write((UInt64)obj);
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = reader.ReadUInt64();
        }
    }

    public sealed class SerializationProviderUIDKEY : SerializationProvider
    {
        public SerializationProviderUIDKEY() : base(typeof(uidkey)) { }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            uidkey key = (uidkey)obj;
            writer.Write(key.id0);
            writer.Write(key.id1);
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            ulong id0 = reader.ReadUInt64();
            ulong id1 = reader.ReadUInt64();
            obj = new uidkey(id0, id1);
        }
    }

    public sealed class SerializationProviderUINT32 : SerializationProvider
    {
        public SerializationProviderUINT32() : base(typeof(UInt32)) { }

        public override void Serialize(object obj, BinaryWriter writer)
        {
            writer.Write((UInt32)obj);
        }

        public override void Deserialize(out object obj, BinaryReader reader)
        {
            obj = reader.ReadUInt32();
        }
    }

    public sealed class FastHash
    {
        public static uint StringToUINT32(string s)
        {
            return Hash(System.Text.Encoding.UTF8.GetBytes(s));
        }

        public static UInt32 Hash(Byte[] dataToHash)
        {
            Int32 dataLength = dataToHash.Length;
            if (dataLength == 0)
                return 0;
            UInt32 hash = Convert.ToUInt32(dataLength);
            Int32 remainingBytes = dataLength & 3; // mod 4
            Int32 numberOfLoops = dataLength >> 2; // div 4
            Int32 currentIndex = 0;
            while (numberOfLoops > 0)
            {
                hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                UInt32 tmp = (UInt32)(BitConverter.ToUInt16(dataToHash, currentIndex + 2) << 11) ^ hash;
                hash = (hash << 16) ^ tmp;
                hash += hash >> 11;
                currentIndex += 4;
                numberOfLoops--;
            }

            switch (remainingBytes)
            {
                case 3:
                    hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                    hash ^= hash << 16;
                    hash ^= ((UInt32)dataToHash[currentIndex + 2]) << 18;
                    hash += hash >> 11;
                    break;
                case 2:
                    hash += BitConverter.ToUInt16(dataToHash, currentIndex);
                    hash ^= hash << 11;
                    hash += hash >> 17;
                    break;
                case 1:
                    hash += dataToHash[currentIndex];
                    hash ^= hash << 10;
                    hash += hash >> 1;
                    break;
                default:
                    break;
            }

            /* Force "avalanching" of final 127 bits */
            hash ^= hash << 3;
            hash += hash >> 5;
            hash ^= hash << 4;
            hash += hash >> 17;
            hash ^= hash << 25;
            hash += hash >> 6;

            return hash;
        }
    }
}