using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFactory.ExternalDLL;

namespace F002520
{
    class clsUploadMES
    {

        public clsUploadMES()
        {   
        }

        ~clsUploadMES()
        { 
        }

        #region Obsolote

        //public bool MESUploadData(string strSN, bool bPassFailFlag, ref string strErrorMessage)
        //{
        //    strErrorMessage = "";
        //    string strResult = "";

        //    try
        //    {
        //        if (FrmMain.MES_ud.EID == "" || FrmMain.MES_ud.StationName == "" || FrmMain.MES_ud.WorkOrder == "" || strSN == "")
        //        {
        //            strErrorMessage = "Invalid MES data.";
        //            return false;
        //        }
        //        if (bPassFailFlag == true)
        //        {
        //            strResult = "PASS";
        //        }
        //        else
        //        {
        //            strResult = "Failure";
        //        }

        //        SmartFactory.ExternalDLL.UploadData data = new SmartFactory.ExternalDLL.UploadData()
        //        {
        //            EID = FrmMain.MES_ud.EID,
        //            StationName = FrmMain.MES_ud.StationName,
        //            WorkOrder = FrmMain.MES_ud.WorkOrder,
        //            SN = strSN,
        //            TestResult = strResult
        //        };

        //        SmartFactory.ExternalDLL.Result result = SmartFactory.ExternalDLL.LineDashboard.UploadTestValue(data);
        //        if (result.code == 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            strErrorMessage = "Fail: code=" + result.code + ". Message: " + result.message;
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        strErrorMessage = "Exception:" + ex.Message;
        //        return false;
        //    }
        //}

        //public bool MESCheckData(ref string strErrorMessage)
        //{
        //    strErrorMessage = "";
        //    string strResult = "Pass";

        //    try
        //    {
        //        if (FrmMain.MES_ud.EID == "" || FrmMain.MES_ud.StationName == "" || FrmMain.MES_ud.WorkOrder == "")
        //        {
        //            strErrorMessage = "Invalid MES data.";
        //            return false;
        //        }

        //        SmartFactory.ExternalDLL.UploadData data = new SmartFactory.ExternalDLL.UploadData()
        //        {
        //            EID = FrmMain.MES_ud.EID,
        //            StationName = FrmMain.MES_ud.StationName,
        //            WorkOrder = FrmMain.MES_ud.WorkOrder,
        //            SN = "1234567890",
        //            TestResult = strResult
        //        };

        //        SmartFactory.ExternalDLL.Result result = SmartFactory.ExternalDLL.LineDashboard.UploadTestValue(data);
        //        if (result.code == 0)
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            strErrorMessage = "Fail: code=" + result.code + ". Message: " + result.message;
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        strErrorMessage = "Exception:" + ex.Message;
        //        return false;
        //    }
        //}

        #endregion

        #region AutoChangeOver

        public static bool MESCheckData(string strEID, string strStation, string strWorkOrder, ref string strErrorMessage)
        {
            strErrorMessage = "";

            try
            {
                #region Check MES Data

                if (strEID == "")
                {
                    strErrorMessage = "Invalid EID.";
                    return false;
                }
                if (strStation == "")
                {
                    strErrorMessage = "Invalid StationName.";
                    return false;
                }
                if (strWorkOrder == "")
                {
                    strErrorMessage = "Invalid WorkOrder.";
                    return false;
                }

                #endregion

                UploadData data = new UploadData()
                {
                    EID = strEID,
                    StationName = strStation,
                    WorkOrder = strWorkOrder
                };

                Result result = LineDashboard.CheckTestValid(data);
                if (result.code == 0)
                {
                    return true;
                }
                else
                {
                    strErrorMessage = "FailCode: " + result.code.ToString() + ",  Message: " + result.message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "MESCheckData Exception:" + ex.Message;
                return false;
            }
        }

        public static bool MESUploadData(string strEID, string strStation, string strWorkOrder, string strSN, bool bPassFailFlag, ref string strErrorMessage)
        {
            strErrorMessage = "";
            string strResult = "";

            try
            {
                #region Check MES Data

                if (strEID == "")
                {
                    strErrorMessage = "Invalid EID.";
                    return false;
                }
                if (strStation == "")
                {
                    strErrorMessage = "Invalid StationName.";
                    return false;
                }
                if (strWorkOrder == "")
                {
                    strErrorMessage = "Invalid WorkOrder.";
                    return false;
                }
                if (strSN == "")
                {
                    strErrorMessage = "Invalid SN.";
                    return false;
                }

                #endregion

                if (bPassFailFlag == true)
                {
                    strResult = "PASS";
                }
                else
                {
                    strResult = "Failure";
                }

                UploadData data = new UploadData()
                {
                    EID = strEID,
                    StationName = strStation,
                    WorkOrder = strWorkOrder,
                    SN = strSN,
                    TestResult = strResult
                };

                Result result = LineDashboard.UploadTestValue(data);
                if (result.code == 0)
                {
                    return true;
                }
                else
                {
                    strErrorMessage = "Fail: code=" + result.code.ToString() + ", Message: " + result.message;
                    return false;
                }
            }
            catch (Exception ex)
            {
                strErrorMessage = "MESUploadData Exception:" + ex.Message;
                return false;
            }
        }
        
        #endregion

    }
}
