using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ItWebSite.Web.DAL.Payment
{
    public partial class Alipay
    {
        // Sample
        //protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        //{
        //    string zfb_ddh = "0001";//order number
        //    string count = "1";
        //    Response.Redirect(img("0001", "包月", "影视包月  10元/月", zfb_ddh, "10.00",
        //        Membership.GetUser().UserName.ToUpper(), count));
        //}

        private string img(string strcmd, string strSub, string strSubinfo, string strid, string strMoney,
            string strUser, string strNum)
        {
            string strsellerEmail = "341081@qq.com"; //卖家支付宝帐号
            string strAc = ""; //卖家支付宝安全校验码
            string INTERFACE_URL = "https://www.alipay.com/payto:";
            string strCmd = strcmd; //命令字
            string strSubject = strSub; //商品名
            string strBody = strSubinfo; //商品描述
            string strOrder_no = strid; //商户订单号
            string strPrice = strMoney; //商品单价 0.01～50000.00
            string rurl = "http://"; //商品展示网址
            string strType = "2"; //type支付类型    1：商品购买2：服务购买3：网络拍卖4：捐赠
            string strNumber = strNum; //购买数量
            string strTransport = "3"; //发货方式        1：平邮2：快递3：虚拟物品
            string strOrdinary_fee = ""; //平邮运费
            string strExpress_fee = ""; //快递运费
            string strReadOnly = "true"; //交易信息是否只读
            string strBuyer_msg = ""; //买家给卖家的留言

            string strBuyer = ""; //买家EMAIL
            string strBuyer_name = strUser; //买家姓名
            string strBuyer_address = ""; //买家地址
            string strBuyer_zipcode = ""; //买家邮编
            string strBuyer_tel = ""; //买家电话号码
            string strBuyer_mobile = ""; //买家手机号码
            string strPartner = ""; //合作伙伴ID    保留字段

            return CreatUrl(strsellerEmail, strAc, INTERFACE_URL, strCmd, strSubject, strBody,
                strOrder_no, strPrice, rurl, strType, strNumber, strTransport,
                strOrdinary_fee, strExpress_fee, strReadOnly, strBuyer_msg, strBuyer,
                strBuyer_name, strBuyer_address, strBuyer_zipcode, strBuyer_tel,
                strBuyer_mobile, strPartner);
        }

        private string CreatUrl(
            string strsellerEmail,
            string strAc,
            string INTERFACE_URL,
            string strCmd,
            string strSubject,
            string strBody,
            string strOrder_no,
            string strPrice,
            string rurl,
            string strType,
            string strNumber,
            string strTransport,
            string strOrdinary_fee,
            string strExpress_fee,
            string strReadOnly,
            string strBuyer_msg,
            string strBuyer,
            string strBuyer_name,
            string strBuyer_address,
            string strBuyer_zipcode,
            string strBuyer_tel,
            string strBuyer_mobile,
            string strPartner)
        {
            //以下参数值不能留空

            string str2CreateAc = "";
            str2CreateAc += "cmd" + strCmd + "subject" + strSubject;
            str2CreateAc += "body" + strBody;
            str2CreateAc += "order_no" + strOrder_no;
            str2CreateAc += "price" + strPrice;
            str2CreateAc += "url" + rurl;
            str2CreateAc += "type" + strType;
            str2CreateAc += "number" + strNumber;
            str2CreateAc += "transport" + strTransport;
            str2CreateAc += "ordinary_fee" + strOrdinary_fee;
            str2CreateAc += "express_fee" + strExpress_fee;
            str2CreateAc += "readonly" + strReadOnly;
            str2CreateAc += "buyer_msg" + strBuyer_msg;
            str2CreateAc += "seller" + strsellerEmail;
            str2CreateAc += "buyer" + strBuyer;
            str2CreateAc += "buyer_name" + strBuyer_name;
            str2CreateAc += "buyer_address" + strBuyer_address;
            str2CreateAc += "buyer_zipcode" + strBuyer_zipcode;
            str2CreateAc += "buyer_tel" + strBuyer_tel;
            str2CreateAc += "buyer_mobile" + strBuyer_mobile;
            str2CreateAc += "partner" + strPartner;
            str2CreateAc += strAc;
            string acCode = GetMD5(str2CreateAc);
            string parameter = "";
            parameter += INTERFACE_URL + strsellerEmail + "?cmd=" + strCmd;
            parameter += "&subject=" +HttpUtility.UrlEncode(strSubject);
            parameter += "&body=" + HttpUtility.UrlEncode(strBody);
            parameter += "&order_no=" + strOrder_no;
            parameter += "&url=" + rurl;
            parameter += "&price=" + strPrice;
            parameter += "&type=" + strType;
            parameter += "&number=" + strNumber;
            parameter += "&transport=" + strTransport;
            parameter += "&ordinary_fee=" + strOrdinary_fee;
            parameter += "&express_fee=" + strExpress_fee;
            parameter += "&readonly=" + strReadOnly;
            parameter += "&buyer_msg=" + strBuyer_msg;
            parameter += "&buyer=" + strBuyer;
            parameter += "&buyer_name=" + HttpUtility.UrlEncode(strBuyer_name);
            parameter += "&buyer_address=" + strBuyer_address;
            parameter += "&buyer_zipcode=" + strBuyer_zipcode;
            parameter += "&buyer_tel=" + strBuyer_tel;
            parameter += "&buyer_mobile=" + strBuyer_mobile;
            parameter += "&partner=" + strPartner;
            parameter += "&ac=" + acCode;
            return parameter;
        }

        private static string GetMD5(string s)
        {
            var md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(s));
            var sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
    }

    public class AlipayHandler
    {
        private string returnTxt = "N";              //返回给支付宝通知接口的结果
        private string alipayNotifyURL = "http://notify.alipay.com/trade/notify_query.do?";    //支付宝查询接口URL
        private string myalipayEmail = "341081@qq.com";            //商户的支付宝Email
        private string constPaySecurityCode = "";                  //码

        private string GetMD5(string s)
        {
            var md5 = new MD5CryptoServiceProvider();
            var t = md5.ComputeHash(Encoding.GetEncoding("gb2312").GetBytes(s));
            var sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        private String Get_Http(String a_strUrl, int timeout)
        {
            string strResult;
            try
            {
                var myReq = (HttpWebRequest)HttpWebRequest.Create(a_strUrl);
                myReq.Timeout = timeout;
                var HttpWResp = (HttpWebResponse)myReq.GetResponse();
                var myStream = HttpWResp.GetResponseStream();
                var sr = new StreamReader(myStream, Encoding.Default);
                var strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine() + "\r\n");
                }
                strResult = strBuilder.ToString();
            }
            catch (Exception exp)
            {
                strResult = "错误：" + exp.Message;
            }
            return strResult;
        }


        private void Page_Load(object sender, System.EventArgs e)
        {
            //检查支付宝通知接口传递过来的参数是否合法
            var msg_id = HttpContext.Current.Request["msg_id"];
            var order_no = HttpContext.Current.Request["order_no"];
            var gross = HttpContext.Current.Request["gross"];
            var buyer_email = HttpContext.Current.Request["buyer_email"];
            var buyer_name = HttpContext.Current.Request["buyer_name"];
            var buyer_address = HttpContext.Current.Request["buyer_address"];
            var buyer_zipcode = HttpContext.Current.Request["buyer_zipcode"];
            var buyer_tel = HttpContext.Current.Request["buyer_tel"];
            var buyer_mobile = HttpContext.Current.Request["buyer_mobile"];
            var action = HttpContext.Current.Request["action"];
            var s_date = HttpContext.Current.Request["date"];
            var ac = HttpContext.Current.Request["ac"];
            var notify_type = HttpContext.Current.Request["notify_type"];
            alipayNotifyURL = alipayNotifyURL + "msg_id=" + msg_id + "&email=" + myalipayEmail + "&order_no=" + order_no;

            //获取支付宝ATN返回结果，true和false都是正确的订单信息，invalid 是无效的
            var responseTxt = Get_Http(alipayNotifyURL, 120000);
            var str = "msg_id" + msg_id + "order_no" + order_no + "gross" + gross + "buyer_email" + buyer_email + "buyer_name" + buyer_name + "buyer_address" + buyer_address + "buyer_zipcode" + buyer_zipcode + "buyer_tel" + buyer_tel + "buyer_mobile" + buyer_mobile + "action" + action + "date" + s_date + constPaySecurityCode;

            string ac_code = GetMD5(str);

            if (action == "test") //支付宝接口测试是否有效
            {
                returnTxt = "Y";
            }
            if (action == "sendOff")  //发货通知
            {
                if (responseTxt.Substring(0, 4) == "true"
                    || responseTxt.Substring(0, 4) == "fals")//ATN，验证消息是否支付宝发过来 
                {
                    if (ac_code == ac)//验证消息是否被修改
                    {
                        //数据库操作
                    }
                }
            }
            if (action == "checkOut")  //交易完成通知 
            {
                returnTxt = "N";
                if (responseTxt.Substring(0, 4) == "true"
                    || responseTxt.Substring(0, 4) == "fals")//ATN，验证消息是否支付宝发过来 
                {
                    if (ac_code == ac)//验证消息是否被修改
                    {
                        //数据库操作    
                    }
                }
            }
            HttpContext.Current.Response.Write(returnTxt);
        }
    }
}