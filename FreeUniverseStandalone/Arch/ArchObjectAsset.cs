using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Arch
{
    public class ArchObjectAsset : ArchObject
    {
        uint _assetType;
        string _path;

        override public void ReadParameter(INIReaderParameter parameter)
        {
            base.ReadParameter(parameter);

            if (parameter.Check("asset_type"))
            {
                _assetType = parameter.GetStrkey(0);
            }
            else if (parameter.Check("path"))
            {
                _path = parameter.GetString(0);
            }
        }
    }
}
