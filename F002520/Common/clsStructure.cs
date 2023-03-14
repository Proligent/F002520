using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F002520
{
    public struct TestResult
    {
        public bool TestPassed;
        public int TestFailCode;
        public string TestFailMessage;
        public string TestStatus;
    }

    public struct TestRecord
    {
        public string modelNumber;
        public string sn;
        public string pn;
        public string cn;
        public string IMEI;
        public string MEID;
        public string BDA;
        public string MAC;
        public string AndroidLicenseKey;

        public string AndroidSN;//BIT APK: getprop "ro.serialno"
        public string SerialNumber;//HSMSN
        public string ConfigNumber;
        public string TestSWRev;
        public string PartNumber;

        public double TestTime;

        public string LCDTest;
        public string ChargerTest;
        public string SWVersionTest;
        public string AccelerometerTest;
        public string BacklightTest;
        public string BluetoothTest;
        public string CameraPreviewTest;
        public string CameraFlashTest;
        public string GyroscopeSensorTest;
        public string PressureSensorTest;
        public string ReceiverTest;
        public string SpeakerTest;
        public string VibratorTest;
        public string ECompassSensorTest;
        public string LightSensorTest;
        public string ProximitySensorTest;
        public string KeypadTest;
        public string MemoryCardTest;
        public string ScanTest;
        public string TouchPanelTest;
        public string MicrophoneTest;
        public string GPSTest;
        public string NFCReadTest;
        public string NFCWriteTest;
        public string WiFiTest;
        public string WiFi5GTest;
        public string DockTest;
        public string LEDTest;
        public string SIMTest;
        public string TouchEdgeTest;
        public string BatteryTest;
        public string PhoneCallTest;
        public string BrowserTest;
        public string KeypadBacklightTest;
        public string AllTestResult;
        public string AndroidSWVer;
        public string HallSensorTest;
    }

    public struct TestSaveData
    {
        public TestResult TestResult;
        public TestRecord TestRecord;
    }

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
