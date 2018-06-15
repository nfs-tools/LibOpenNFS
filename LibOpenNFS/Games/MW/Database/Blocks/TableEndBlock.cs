using System;
using System.Collections.Generic;
using System.IO;

namespace LibOpenNFS.Games.MW.Database.Blocks
{
    public class TableEndBlock : VltBlockContainer
    {
        private List<VltUnknown> _unknownList;
        private List<VltUnknown> _unknownList2;

        public Dictionary<int, VltUnknown> UnknownDictionary;
        
        public override void Read(BinaryReader reader)
        {
            _unknownList = new List<VltUnknown>();
            _unknownList2 = new List<VltUnknown>();
            
            UnknownDictionary = new Dictionary<int, VltUnknown>();

            var unknownFlag = false;

            VltUnknown unknown;

            while (true)
            {
                unknown = new VltUnknown();
                unknown.Read(reader);
                
                _unknownList.Add(unknown);

                if (unknown.Unknown2 == 2 && (unknown.Unknown3 == 0 || unknown.Unknown3 == 1))
                {
                    unknownFlag = unknown.Unknown3 == 0;
                }
                else
                {
                    if (unknown.Unknown2 == 1)
                    {
                        if (unknownFlag)
                        {
                            UnknownDictionary[unknown.Address] = unknown;
                        }
                        else
                        {
                            _unknownList2.Add(unknown);
                        }
                    }
                    else
                    {
                        if (unknown.Unknown2 != 3 || unknown.Unknown3 != 1)
                        {
                            break;
                        }

                        if (unknownFlag)
                        {
                            UnknownDictionary[unknown.Address] = unknown;
                        }
                        else
                        {
                            _unknownList2.Add(unknown);
                        }
                    }
                }
            }

            if (unknown.Address != 0)
            {
                throw new Exception("Unknown pointer type");
            }
        }
    }
}