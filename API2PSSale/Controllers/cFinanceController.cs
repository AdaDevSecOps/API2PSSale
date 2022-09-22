﻿using API2PSSale.Class;
using API2PSSale.Class.Standard;
using API2PSSale.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.Threading;

namespace API2PSSale.Controllers
{
    /// <summary>
    ///     Service upload.
    /// </summary>
    //[RoutePrefix(cCS.tCS_APIVer + "/Service")]
    //*Ton 64-05-18
    [ApiController]
    [Route(cCS.tCS_APIVer + "/Service")]
    public class cFinanceController : ControllerBase
    {
        /// <summary>
        /// Upload Finance
        /// </summary>
        /// <param name="poTFNTBnkDpl"></param>
        /// <returns></returns>
        [Route("Upload/Finance")]
        [HttpPost]
        public cResult<int> POST_UPLoFinanceUpload(cmlDataUpload poDataUpload)
        {
            //TransactionScope oTrans = null/* TODO Change to default(_) if this is not a reference type */;
            cResult<int> oResult = new cResult<int>();
            cMS oMsg;
            cSP oSP;
            string tErrCode, tErrDesc, tErrAPI;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                oMsg = new cMS();
                oSP = new cSP();

                oResult.roItem = 0;
                oResult.rnCount = 0;
                oResult.rtMsg = "";
                oResult.rtCode = "";

                if (poDataUpload == null)
                {
                    oResult.rtCode = cMS.tMS_RespCode700;
                    oResult.rtMsg = cMS.tMS_RespDesc700;
                    return oResult;
                }
                if (poDataUpload.ptData == null)
                {
                    oResult.rtCode = cMS.tMS_RespCode700;
                    oResult.rtMsg = cMS.tMS_RespDesc700;
                    return oResult;
                }

                #region Check API Key and check comnnect database
                //*Ton 64-05-18
                if (oSP.SP_CHKbKeyApi(HttpContext, out tErrAPI) == false)
                {
                    if (tErrAPI == "-1")
                    {
                        oResult.rtCode = cMS.tMS_RespCode905;
                        oResult.rtMsg = cMS.tMS_RespDesc905;
                        return oResult;
                    }
                    else
                    {
                        oResult.rtCode = cMS.tMS_RespCode904;
                        oResult.rtMsg = cMS.tMS_RespDesc904;
                        return oResult;
                    }
                }
                #endregion

                    string tQueueFinance = string.Format("BR_QTransfer" + poDataUpload.ptFilter);
                    cRabbitMQ oRQ = new cRabbitMQ();
                    oRQ.C_GETbLoadConfigMQ();
                    if (!string.IsNullOrEmpty(cRabbitMQ.tC_HostName))
                    {
                        if (tQueueFinance != "BR_QTransfer")
                        {
                        //*Ton 64-05-18
                        string tConnStr = cAppSetting.Default.tConnDB;
                            poDataUpload.ptConnStr = tConnStr;
                            string tMsgJson = Newtonsoft.Json.JsonConvert.SerializeObject(poDataUpload);
                            //string tMsgJson = oRQ.C_CRTtMsgDataUpload(Newtonsoft.Json.JsonConvert.SerializeObject(poDataUpload), tConnStr);
                            if (oRQ.C_PRCbSendData2Srv(tMsgJson, tQueueFinance))
                            {
                                oResult.rtCode = cMS.tMS_RespCode001;
                                oResult.rtMsg = cMS.tMS_RespDesc001;
                            }
                            else
                            {
                                oResult.rtCode = cMS.tMS_RespCode907;
                                oResult.rtMsg = cMS.tMS_RespDesc907;
                            }
                        }
                        else
                        {
                            oResult.rtCode = cMS.tMS_RespCode907;
                            oResult.rtMsg = cMS.tMS_RespDesc907;
                        }
                    }
                    else
                    {
                        oResult.rtCode = cMS.tMS_RespCode907;
                        oResult.rtMsg = cMS.tMS_RespDesc907;
                    }
                
                
            }
            catch (Exception oEx)
            {
                oResult.rtCode = cMS.tMS_RespCode900;
                oResult.rtMsg = oEx.Message.ToString();
            }
            finally
            {
                oMsg = null;
                oSP = null;
            }
            return oResult;

        }

    }
}

