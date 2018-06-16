using System;
using System.Collections.Generic;
using LibOpenNFS.Games.MW.Database.Types;
using LibOpenNFS.Utils;

namespace LibOpenNFS.Games.MW.Database
{
    public class VltTypeMap
    {
        private readonly Dictionary<uint, Type> _typeDictionary;

        public static readonly VltTypeMap Instance = new VltTypeMap();

        private VltTypeMap()
        {
            _typeDictionary =
                new Dictionary<uint, Type>
                {
                    {JenkinsHash.getHash32("EA::Reflection::Int8"), typeof(EaInt8)},
                    {JenkinsHash.getHash32("EA::Reflection::Int16"), typeof(EaInt16)},
                    {JenkinsHash.getHash32("EA::Reflection::Int32"), typeof(EaInt32)},
                    {JenkinsHash.getHash32("EA::Reflection::Int64"), typeof(EaInt64)},
                    {JenkinsHash.getHash32("EA::Reflection::UInt8"), typeof(EaUInt8)},
                    {JenkinsHash.getHash32("EA::Reflection::UInt16"), typeof(EaUInt16)},
                    {JenkinsHash.getHash32("EA::Reflection::UInt32"), typeof(EaUInt32)},
                    {JenkinsHash.getHash32("EA::Reflection::UInt64"), typeof(EaUInt64)},
                    {JenkinsHash.getHash32("EA::Reflection::Bool"), typeof(EaBoolean)},
                    {JenkinsHash.getHash32("EA::Reflection::Float"), typeof(EaFloat)},
                    {JenkinsHash.getHash32("EA::Reflection::Double"), typeof(EaDouble)},
                    {JenkinsHash.getHash32("EA::Reflection::Text"), typeof(EaText)},
                    {JenkinsHash.getHash32("Attrib::RefSpec"), typeof(RefSpec)},
                    {JenkinsHash.getHash32("Attrib::StringKey"), typeof(StringKey)},
                    {JenkinsHash.getHash32("Attrib::Types::Matrix"), typeof(Matrix)},
                    {JenkinsHash.getHash32("Attrib::Types::Vector2"), typeof(Vector2)},
                    {JenkinsHash.getHash32("Attrib::Types::Vector3"), typeof(Vector3)},
                    {JenkinsHash.getHash32("Attrib::Types::Vector4"), typeof(Vector4)},
                    {JenkinsHash.getHash32("Attrib::Blob"), typeof(Blob)},
                    {JenkinsHash.getHash32("GCollectionKey"), typeof(CollectionKey)},
                    {JenkinsHash.getHash32("AirSupport"), typeof(AirSupport)},
                    {JenkinsHash.getHash32("AxlePair"), typeof(AxlePair)},
                    
                };
        }

        public Type GetTypeForKey(uint typeHash)
        {
            return _typeDictionary.ContainsKey(typeHash) ? _typeDictionary[typeHash] : null;
        }
    }
}