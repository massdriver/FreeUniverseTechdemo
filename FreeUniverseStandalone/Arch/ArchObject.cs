using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FreeUniverse.Arch
{
    public class ArchObject
    {
        protected ulong _id;
        protected string _nickname;
        protected string _strName;
        protected string _strInfo;
        protected string _strDesc;
        protected string _model;
        public int index { get; set; }

        public ulong id
        {
            get { return _id; }
        }

        public string nickname
        {
            get { return _nickname; }
        }

        public string strName
        {
            get { return _strName; }
        }

        public string strInfo
        {
            get { return _strInfo; }
        }

        public string strDesc
        {
            get { return _strDesc; }
        }

        public string model
        {
            get { return _model; }
        }

        public const int INVALID_INDEX = -1;

        public ArchObject()
        {
            index = INVALID_INDEX;
        }

        public virtual void ReadHeader(INIReaderHeader header)
        {
            foreach (INIReaderParameter p in header.parameters)
                ReadParameter(p);
        }

        public virtual void ReadParameter(INIReaderParameter parameter)
        {
            if (parameter.Check("id"))
            {
                _id = HashUtils.StringToUINT64(parameter.GetString(0));
                _nickname = parameter.GetString(0);
            }
            else if (parameter.Check("strName"))
            {
                _strName = ArchModel.GetString(parameter.GetInt(0));
            }
            else if (parameter.Check("strInfo"))
            {
                _strInfo = ArchModel.GetString(parameter.GetInt(0));
            }
            else if (parameter.Check("strDesc"))
            {
                _strDesc = ArchModel.GetString(parameter.GetInt(0));
            }
            else if (parameter.Check("model"))
            {
                _model = parameter.GetString(0);
            }
        }
    }
}
