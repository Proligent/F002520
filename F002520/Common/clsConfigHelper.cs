using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace F002520
{
    public class clsConfigHelper
    {
        #region Variable

        // Equipment Param
        public static DAQ Daq = new DAQ();
        public static MOTOR servoMotor = new MOTOR();
        public static PLC plcConfig = new PLC();

        // Store TestItem Param
        public Dictionary<string, bool> dicTestItemList = new Dictionary<string, bool>();   // TestItem Enable Param
        public Dictionary<string, Dictionary<string, string>> dicTestItemParamList = new Dictionary<string, Dictionary<string, string>>();  // TestItemList Param
        public Dictionary<string, Dictionary<string, string>> dicTestConfig = new Dictionary<string, Dictionary<string, string>>(); // Config Param

        #endregion

        #region Construct

        public clsConfigHelper()
        {
    
        }

        #endregion

        #region \TestConfig\TestItem\Model\SensorK_TestItem.xml

        /// <summary>
        /// Load TestItemList and Enable Param to dicTestItemList
        /// </summary>
        /// <param name="strXmlFile"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool LoadTestItemList(string strXmlFile, ref string strErrorMessage)
        {
            try
            {
                #region Check File Exist

                if (File.Exists(strXmlFile) == false)
                {
                    strErrorMessage = "File not exist:" + strXmlFile;
                    return false;
                }
                dicTestItemList.Clear();

                #endregion

                #region //Configuration/TestItemList

                // 初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();

                // 加载xml文件
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true; // 忽略注释
                XmlReader reader = XmlReader.Create(strXmlFile, settings);
                myXmlDoc.Load(reader);

                // xml root
                XmlNode root = myXmlDoc.SelectSingleNode("//Configuration/TestItemList");

                // 获取到所有root的子节点
                XmlNodeList nodeList = root.ChildNodes;

                string strItemName = "";
                string strItemEnabled = "";
                XmlNode node;
                XmlElement Element;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    node = nodeList[i];
                    Element = (XmlElement)node;
                    strItemName = Element.GetAttribute("Name").ToString().Trim();
                    strItemEnabled = Element.GetAttribute("Enable").ToString().Trim().ToUpper();

                    if (strItemName == "")
                    {
                        strErrorMessage = "Invalid Item Name." + strXmlFile;
                        return false;
                    }
                    if (strItemEnabled != "TRUE" && strItemEnabled != "FALSE")
                    {
                        strErrorMessage = "Invalid Item Enabled." + strXmlFile;
                        return false;
                    }

                    dicTestItemList.Add(strItemName, bool.Parse(strItemEnabled));
                }

                #endregion
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load TestItem Param to dicTestItemParamList
        /// </summary>
        /// <param name="strXmlFile"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool LoadTestItemParameter(string strXmlFile, ref string strErrorMessage)
        {
            try
            {
                #region Check File Exist

                if (File.Exists(strXmlFile) == false)
                {
                    strErrorMessage = "File not exist." + strXmlFile;
                    return false;
                }
                dicTestItemParamList.Clear();

                #endregion

                #region //Configuration/TestItemParameterList

                // 初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();

                // 加载xml文件
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(strXmlFile, settings);
                myXmlDoc.Load(reader);

                // xml root
                XmlNode root = myXmlDoc.SelectSingleNode("//Configuration/TestItemParameterList");

                // 获取到所有root的子节点
                Dictionary<string, string> dic_Temp = new Dictionary<string, string>();
                XmlNodeList nodeList = root.ChildNodes;

                string strItemName = "";
                XmlNode node;
                XmlNodeList childNodeList;
                XmlNode childNode;
                string strNodeName = "";
                string strNodeValue = "";
                for (int i = 0; i < nodeList.Count; i++)
                {
                    node = nodeList[i];
                    strItemName = node.Name.ToString().Trim();
                    if (strItemName == "")
                    {
                        strErrorMessage = "Invalid Item Name." + strXmlFile;
                        return false;
                    }
        
                    dic_Temp.Clear();
                    childNodeList = node.ChildNodes;
                    for (int j = 0; j < childNodeList.Count; j++)
                    {
                        childNode = childNodeList[j];
                        strNodeName = childNode.Name.ToString().Trim();
                        strNodeValue = childNode.InnerText.ToString().Trim();

                        dic_Temp[strNodeName] = strNodeValue;
                    }

                    dicTestItemParamList[strItemName] = dic_Temp;
                }

                #endregion
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load TestConfig Param Under the Configuration node
        /// </summary>
        /// <param name="strXmlFile"></param>
        /// <param name="strErrorMessage"></param>
        /// <returns></returns>
        public bool LoadTestConfig(string strXmlFile, ref string strErrorMessage)
        {
            try
            {
                #region Check File Exist

                if (File.Exists(strXmlFile) == false)
                {
                    strErrorMessage = "File not exist." + strXmlFile;
                    return false;
                }
                dicTestConfig.Clear();

                #endregion

                #region //Configuration/TestItemList

                // 初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();

                // 加载xml文件
                // myXmlDoc.Load(strXmlFile);
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
                XmlReader reader = XmlReader.Create(strXmlFile, settings);
                myXmlDoc.Load(reader);

                // xml root
                XmlNode root = myXmlDoc.SelectSingleNode("//Configuration");

                // 获取到所有root的子节点
                Dictionary<string, string> dic_Temp = new Dictionary<string, string>();
                XmlNodeList nodeList = root.ChildNodes;

                string strItemName = "";
                XmlNode node;
                XmlNodeList childNodeList;
                XmlNode childNode;
                string strNodeName = "";
                string strNodeValue = "";
                for (int i = 0; i < nodeList.Count; i++)
                {
                    node = nodeList[i];
                    strItemName = node.Name.ToString().Trim();
                    if (strItemName == "")
                    {
                        strErrorMessage = "Invalid Item Name." + strXmlFile;
                        return false;
                    }
   
                    dic_Temp.Clear();
                    childNodeList = node.ChildNodes;
                    for (int j = 0; j < childNodeList.Count; j++)
                    {
                        childNode = childNodeList[j];
                        strNodeName = childNode.Name.ToString().Trim();
                        strNodeValue = childNode.InnerText.ToString().Trim();

                        dic_Temp[strNodeName] = strNodeValue;
                    }

                    dicTestConfig.Add(strItemName, dic_Temp);
                }

                #endregion
            }
            catch (Exception ex)
            {
                strErrorMessage = "Exception:" + ex.Message.ToString();
                return false;
            }

            return true;
        }

        #endregion

        #region \TestConfig\Equipment\EquipmentConfig.xml

        public static bool LoadEquipmentParams(ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                #region Check File Exist

                if (File.Exists(frmMain.m_strEquipmentXMLFile) == false)
                {
                    strErrorMessage = "File not Exist: " + frmMain.m_strEquipmentXMLFile;
                    return false;
                }

                #endregion

                #region DeviceList

                if (LoadDAQParam(frmMain.m_strEquipmentXMLFile, "DAQ", ref Daq, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load DeviceList NI-DAQ fail: " + strErrorMessage;
                    return false;
                }

                if (LoadMotorParam(frmMain.m_strEquipmentXMLFile, "MOTOR", ref servoMotor, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load DeviceList MOTOR fail: " + strErrorMessage;
                    return false;
                }

                if (LoadPLCParam(frmMain.m_strEquipmentXMLFile, "PLC", ref plcConfig, ref strErrorMessage) == false)
                {
                    strErrorMessage = "Load DeviceList MOTOR fail: " + strErrorMessage;
                    return false;
                }

                #endregion
            }
            catch(Exception ex)
            {
                strErrorMessage = "Exception: " + ex.Message;
                return false;
            }

            return true;
        }

        private static bool LoadDAQParam(string strXMLFile, string sNodeName, ref DAQ tempParam, ref string strErrorMessage)
        {
            try
            {
                if (File.Exists(strXMLFile) == false)
                {
                    return false;
                }

                //初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true; // 忽略注释
                XmlReader reader = XmlReader.Create(strXMLFile, settings);

                // 加载xml文件，参数为reader
                myXmlDoc.Load(reader);

                XmlNode root = myXmlDoc.SelectSingleNode("//Configuration/DeviceList/" + sNodeName);

                //获取到所有 sNode 的子节点
                XmlNodeList nodeList = root.ChildNodes;

                tempParam.Enable = bool.Parse(nodeList[0].InnerText.ToString());
                tempParam.DeviceType = nodeList[1].InnerText.ToString();
                tempParam.DeviceName = nodeList[2].InnerText.ToString();
            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private static bool LoadMotorParam(string strXMLFile, string sNodeName, ref MOTOR tempParam, ref string strErrorMessage)
        {
            try
            {
                if (File.Exists(strXMLFile) == false)
                {
                    return false;
                }

                //初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true; // 忽略注释
                XmlReader reader = XmlReader.Create(strXMLFile, settings);

                // 参数为reader
                myXmlDoc.Load(reader);

                XmlNode root = myXmlDoc.SelectSingleNode("//Configuration/DeviceList/" + sNodeName);

                //获取到所有 sNode 的子节点
                XmlNodeList nodeList = root.ChildNodes;

                tempParam.Enable = bool.Parse(nodeList[0].InnerText.ToString());
                tempParam.DeviceType = nodeList[1].InnerText.ToString();
                tempParam.PortNum = int.Parse(nodeList[2].InnerText.ToString());
                tempParam.BaudRate = int.Parse(nodeList[3].InnerText.ToString());
                tempParam.Parity = (nodeList[4].InnerText.ToString() == "Even" ? Parity.Even : Parity.None);
                tempParam.DataBits = int.Parse(nodeList[5].InnerText.ToString());
                tempParam.StopBits = int.Parse(nodeList[6].InnerText.ToString());
                tempParam.Home = nodeList[7].InnerText.ToString(); 
            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        private static bool LoadPLCParam(string strXMLFile, string sNodeName, ref PLC tempParam, ref string strErrorMessage)
        {
            try
            {
                if (File.Exists(strXMLFile) == false)
                {
                    return false;
                }

                //初始化一个xml实例
                XmlDocument myXmlDoc = new XmlDocument();
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true; // 忽略注释
                XmlReader reader = XmlReader.Create(strXMLFile, settings);

                // 加载xml文件，参数为reader
                myXmlDoc.Load(reader);

                XmlNode root = myXmlDoc.SelectSingleNode("//Configuration/DeviceList/" + sNodeName);

                //获取到所有 sNode 的子节点
                XmlNodeList nodeList = root.ChildNodes;

                tempParam.Enable = bool.Parse(nodeList[0].InnerText.ToString());
                tempParam.LocalIP = nodeList[1].InnerText.ToString();
                tempParam.PLCIP = nodeList[2].InnerText.ToString();
                tempParam.PLCPort = int.Parse(nodeList[3].InnerText.ToString());
                tempParam.ReadDB = int.Parse(nodeList[4].InnerText.ToString());
                tempParam.WriteDB = int.Parse(nodeList[5].InnerText.ToString());
                tempParam.Location = nodeList[6].InnerText.ToString();
            }
            catch (Exception ex)
            {
                strErrorMessage = ex.Message;
                return false;
            }

            return true;
        }

        #endregion
  
    }
}
