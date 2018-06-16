using System;
using System.Collections.Generic;
using System.IO;

namespace LibOpenNFS.Games.MW.Database.Blocks
{
    public class TableEndBlock : VltBlockContainer
    {
        private List<VltRowInfo> _unknownList;
        private List<VltRowInfo> _unknownList2;

        public Dictionary<int, VltRowInfo> InfoDictionary;
        
        public override void Read(BinaryReader reader)
        {
            _unknownList = new List<VltRowInfo>();
            _unknownList2 = new List<VltRowInfo>();
            
            InfoDictionary = new Dictionary<int, VltRowInfo>();

            var unknownFlag = false;

            VltRowInfo rowInfo;

            while (true)
            {
                rowInfo = new VltRowInfo();
                rowInfo.Read(reader);
                
                _unknownList.Add(rowInfo);

                if (rowInfo.Unknown2 == 2 && (rowInfo.Unknown3 == 0 || rowInfo.Unknown3 == 1))
                {
                    unknownFlag = rowInfo.Unknown3 == 0;
                }
                else
                {
                    if (rowInfo.Unknown2 == 1)
                    {
                        if (unknownFlag)
                        {
                            InfoDictionary[rowInfo.Address] = rowInfo;
                        }
                        else
                        {
                            _unknownList2.Add(rowInfo);
                        }
                    }
                    else
                    {
                        if (rowInfo.Unknown2 != 3 || rowInfo.Unknown3 != 1)
                        {
                            break;
                        }

                        if (unknownFlag)
                        {
                            InfoDictionary[rowInfo.Address] = rowInfo;
                        }
                        else
                        {
                            _unknownList2.Add(rowInfo);
                        }
                    }
                }
            }

            if (rowInfo.Address != 0)
            {
                throw new Exception("Unknown pointer type");
            }
        }
    }
}