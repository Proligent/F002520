using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    public struct MFGData
    {
        public string modelNumber;
        public string sn;
        public string pn;
        public string cn;
        public string IMEI;
        public string MEID;
        public string BDA;
        public string SecBDA;
        public string MAC;
        public string SecMAC;
        public string ZigBeeAddress;
    }

    public struct MCFData
    {
        public string PartNumber;
        public string Model;
        public string BOMVersion;
        public string Description;
        public string ProductLabelName;
        public string PackageLabelName;
    }

    public struct ComPortSetting
    {
        public Int32 PortNum;
        public Int32 BaudRate;
        public string Parity;
        public Int32 DataBits;
        public Int32 StopBits;
    }

 








}
