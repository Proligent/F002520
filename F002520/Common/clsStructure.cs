using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    public enum ModelID : int
    {
        NULL = 0,
        CT40,
        CT40P,
        CT45,
        CT45P,
        CT47,
        CW45   
    }

    public struct MFGData
    {
        public string SN;
        public string PN;
        public string Model;
        public string CN;
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

    public struct UnitDeviceInfo
    {
        public string SN;
        public string SKU;
        public string Model;
        public string IMEI;
        public string MEID;
        public string IMEI2;
        public string MEID2;
        public string Vendor;
        public string HWVersion;
        public string AndroidOS;
        public string AudioPAName;
        public string ConfigNumber;
        public string WLAN_MAC_ADDRESS;
        public string WLAN_AUX_MAC_ADDRESS;
        public string BLUETOOTH_DEVICE_ADDRESS;
        public string SECOND_BLUETOOTH_DEVICE_ADDRESS;
   
        // Before Calibration
        public string ACCEL_ZERO_OFFSET_BEFORE;
        public string ACCELEROMETER_CALIBRATION_BEFORE;
        public string GYRO_ZERO_OFFSET_BEFORE;
        public string GYROSCOPE_CALIBRATION_BEFORE;
        public string PROXIMITY_CALIBRATION_BEFORE;
        public string PROXIMITY_CALIBRATION_EXTEND_BEFORE;
        public string MAX98390L_TROOM_BEFORE;
        public string MAX98390L_RDC_BEFORE;

        // After Calibration
        public string ACCEL_ZERO_OFFSET_AFTER;
        public string ACCELEROMETER_CALIBRATION_AFTER;
        public string GYRO_ZERO_OFFSET_AFTER;
        public string GYROSCOPE_CALIBRATION_AFTER;
        public string PROXIMITY_CALIBRATION_AFTER;
        public string PROXIMITY_CALIBRATION_EXTEND_AFTER;
        public string MAX98390L_TROOM_AFTER;
        public string MAX98390L_RDC_AFTER;

        public string EID;
        public string WorkOrder;
        public string OSPN;   
        public string Status;
    }

    public struct ComPortSetting
    {
        public Int32 PortNum;
        public Int32 BaudRate;
        public Parity Parity;
        public Int32 DataBits;
        public Int32 StopBits;
    }

    #region MDCS

    public struct TestResult
    {
        public bool TestPassed;
        public int TestFailCode;
        public string TestFailMessage;
        public string TestStatus;
    }

    public struct TestRecord
    {
        public string ToolNumber;
        public string ToolRev;
        public string SKU;
        public string Model;
        public string SN;

        public string IMEI;
        public string MEID;
        public string IMEI2;
        public string MEID2;

        public string EID;
        public string WorkOrder;

        public string PCBAVendor;
        public string HWVersion;
        public string AndroidOSVersion;
        public string AudioPAName;
        public string ConfigurationNumber;
        public string BarometerOffsetValue;
        public string SoftwareVersionControl;

        // TestItem Result
        public string TestGSensorCalibration;
        public string TestGYROSensorCalibration;
        public string TestPSensorCalibration;
        public string TestPSensorFunction;
        public string TestAudioCalibration;
        public string TestBarometerCalibration;

        // Before Calibration
        public string ACCEL_ZERO_OFFSET_BEFORE;
        public string ACCELEROMETER_CALIBRATION_BEFORE;
        public string GYRO_ZERO_OFFSET_BEFORE;
        public string GYROSCOPE_CALIBRATION_BEFORE;
        public string PROXIMITY_CALIBRATION_BEFORE;
        public string PROXIMITY_CALIBRATION_EXTEND_BEFORE;
        public string MAX98390L_TROOM_BEFORE;
        public string MAX98390L_RDC_BEFORE;

        // After Calibration
        public string ACCEL_ZERO_OFFSET_AFTER;
        public string ACCELEROMETER_CALIBRATION_AFTER;
        public string GYRO_ZERO_OFFSET_AFTER;
        public string GYROSCOPE_CALIBRATION_AFTER;
        public string PROXIMITY_CALIBRATION_AFTER;
        public string PROXIMITY_CALIBRATION_EXTEND_AFTER;
        public string MAX98390L_TROOM_AFTER;
        public string MAX98390L_RDC_AFTER;

        public string TestTotalTime;
    }

    public struct TestSaveData
    {
        public TestResult TestResult;
        public TestRecord TestRecord;
    }

    #endregion


    #region Equipment

    public struct DAQ
    {
        public bool Enable;
        public string DeviceType;
        public string DeviceName;
    }

    #region Obsolote

    //public struct JYDAM_DAQ
    //{
    //    public bool Enable;
    //    public string DeviceName;
    //}

    #endregion

    public struct MOTOR
    {
        public bool Enable;
        public string DeviceType;
        public Int32 PortNum;
        public Int32 BaudRate;
        public Parity Parity;
        public Int32 DataBits;
        public Int32 StopBits;
        public string Home;
    }

    public struct PLC
    {
        public bool Enable;
        public string LocalIP;
        public string PLCIP;
        public int PLCPort;
        public int ReadDB;
        public int WriteDB;
        public string Location;
    }

    #endregion



}
