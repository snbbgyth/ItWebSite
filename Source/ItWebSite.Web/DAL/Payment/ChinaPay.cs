//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using ChinaPay_Net;

//namespace SpringSoftware.Web.DAL
//{
//    public class Chinapay
//    {
//        string strUrl = HttpContext.Current.Request.PhysicalApplicationPath; //获取网站根目录物理路径
//        public Chinapay()
//        {
//        }
//        /////////////////////////////////////////////////////////////////////////////////////////
//        //                        二次开发 杨祥 2009.12.02
//        /////////////////////////////////////////////////////////////////////////////////////////
//        /// <summary>
//        /// 订单签名函数sign
//        /// </summary>
//        /// <param name="MerId">商户号，长度为15个字节的数字串，由ChinaPay或清算银行分配</param>
//        /// <param name="OrdId">订单号，长度为16个字节的数字串，由用户系统/网站生成，失败的订单号允许重复支付</param>
//        /// <param name="TransAmt">交易金额，长度为12个字节的数字串，例如：数字串"000000001234"表示12.34元</param>
//        /// <param name="CuryId">货币代码, 长度为3个字节的数字串，目前只支持人民币，取值为"156"</param>
//        /// <param name="TransDate">交易日期，长度为8个字节的数字串，表示格式为：YYYYMMDD</param>
//        /// <param name="TransType">交易类型，长度为4个字节的数字串，取值范围为："0001"和"0002"， 其中"0001"表示消费交易，"0002"表示退货交易</param>
//        /// <returns>string CheckValue[256]  即NetPayClient根据上述输入参数生成的商户数字签名，长度为256字节的字符串</returns>
//        public string getSign(string MerId, string OrdId, string TransAmt, string CuryId, string TransDate, string TransType)
//        {
//            NetPayClientClass npc = new NetPayClientClass(); //实例NetPay签名
//            //npc.setMerKeyFile("Bin/MerPrK.key");          //设置商户密钥文件地址 d:\\MerPrK.key
//            npc.setMerKeyFile(strUrl + "\\key\\MerPrK.key");
//            string strChkValue = "";                         //chinapay返回的商户数字签名
//            strChkValue = npc.sign(MerId, OrdId, TransAmt, CuryId, TransDate, TransType);
//            return strChkValue.Trim();
//        }
//        /// <summary>
//        /// 对一段字符进行签名 signData
//        /// </summary>
//        /// <param name="MerId">商户号，长度为15个字节的数字串，由ChinaPay分配</param>
//        /// <param name="SignMsg">用于要签名的字符串</param>
//        /// <returns>String CheckValue[256]即NetPayClient根据上述输入参数生成的商户数字签名，长度为256字节的字符串</returns>
//        public string signData(string MerId, string SignMsg)
//        {
//            NetPayClientClass npc = new NetPayClientClass(); //实例NetPay签名
//            //npc.setMerKeyFile("Bin/MerPrK.key");          //设置商户密钥文件地址 d:\\MerPrK.key
//            npc.setMerKeyFile(strUrl + "\\key\\MerPrK.key");
//            string strChkValueData = "";
//            strChkValueData = npc.signData(MerId, SignMsg);
//            return strChkValueData.Trim();
//        }

//        /// <summary>
//        /// 验证交易应答函数check
//        /// </summary>
//        /// <param name="MerId">商户号，长度为15个字节的数字串，由ChinaPay分配</param>
//        /// <param name="OrdId">订单号，长度为16个字节的数字串，由商户系统生成，失败的订单号允许重复支付</param>
//        /// <param name="TransAmt">交易金额，长度为12个字节的数字串，例如：数字串"000000001234"表示12.34元</param>
//        /// <param name="CuryId">货币代码, 长度为3个字节的数字串，目前只支持人民币，取值为"156"</param>
//        /// <param name="TransDate">交易日期，长度为8个字节的数字串，表示格式为： YYYYMMDD</param>
//        /// <param name="TransType">交易类型，长度为4个字节的数字串，取值范围为："0001"和"0002"， 其中"0001"表示消费交易，"0002"表示退货交易</param>
//        /// <param name="OrderStatus">交易状态，长度为4个字节的数字串。详见交易状态码说明</param>
//        /// <param name="CheckValue">校验值，即ChinaPay对交易应答的数字签名，长度为256字节的字符串</param>
//        /// <returns>true 表示成功，即该交易应答为ChinaPay所发送，商户根据“交易状态”进行后续处理；否则表示失败，即无效应答，商户可忽略该应答</returns>
//        public bool getCheck(string MerId, string OrdId, string TransAmt, string CuryId, string TransDate, string TransType, string OrderStatus, string CheckValue)
//        {
//            NetPayClientClass npc = new NetPayClientClass(); //实例NetPay签名
//            //npc.setPubKeyFile("Bin/PgPubk.key");          //设置chinapay公共密钥文件地址 d:\\PgPubk.key
//            npc.setPubKeyFile(strUrl + "\\key\\PgPubk.key");
//            string strFlag = "";
//            bool bolFlag = false;
//            strFlag = npc.check(MerId, OrdId, TransAmt, CuryId, TransDate, TransType, OrderStatus, CheckValue); // ChkValue 为ChinaPay返回给商户的域段内容
//            if (strFlag == "0") //“0”表示验签成功
//            {
//                bolFlag = true;
//            }
//            return bolFlag;
//        }
//        /// <summary>
//        /// 对一段字符串进行签名验证 checkData
//        /// </summary>
//        /// <param name="PlainData">用于数字签名的字符串</param>
//        /// <param name="CheckValue">校验值，要验证的字符串的数字签名，长度为256字节的字符串</param>
//        /// <returns>true 表示验证通过成功；否则表示失败</returns>
//        public bool checkData(string PlainData, string CheckValue)
//        {
//            NetPayClientClass npc = new NetPayClientClass(); //实例NetPay签名
//            //npc.setPubKeyFile("Bin/PgPubk.key");          //设置chinapay公共密钥文件地址 d:\\PgPubk.key
//            npc.setPubKeyFile(strUrl + "\\key\\PgPubk.key");
//            string strFlagData = "";
//            bool bolFlagData = false;
//            strFlagData = npc.checkData(PlainData, CheckValue);
//            if (strFlagData == "true")
//            {
//                bolFlagData = true;
//            }
//            return bolFlagData;
//        }
//    }
//}