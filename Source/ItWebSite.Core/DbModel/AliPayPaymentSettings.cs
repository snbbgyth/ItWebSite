using ItWebSite.Core.Model;

namespace ItWebSite.Core.DbModel
{
   public  class AliPayPaymentSettings:BaseTable
    {
        public virtual string SellerEmail { get; set; }
        public virtual string Key { get; set; }
        public virtual string Partner { get; set; }
        public virtual decimal AdditionalFee { get; set; }
    }
}
