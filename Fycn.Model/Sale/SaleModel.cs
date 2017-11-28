using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fycn.Model.Sale
{
     [Table("table_sales_cashless")]
    public class SaleModel
    {
         [Column(Name = "sales_ic_id")]
         public string SalesIcId
         {
             get;
             set;
         }

         [Column(Name = "machine_id")]
         public string MachineId
         {
             get;
             set;
         }

         [Column(Name = "sales_date")]
         public DateTime SalesDate
         {
             get;
             set;
         }

         [Column(Name = "sales_number")]
         public int SalesNumber
         {
             get;
             set;
         }

         [Column(Name = "pay_date")]
         public DateTime PayDate
         {
             get;
             set;
         }

         [Column(Name = "pay_type")]
         public string PayType
         {
             get;
             set;
         }

         [Column(Name = "pay_interface")]
         public string PayInterface
         {
             get;
             set;
         }

         [Column(Name = "acquiring_merchant")]
         public string AcquiringMerchant
         {
             get;
             set;
         }

         [Column(Name = "trade_no")]
         public string TradeNo
         {
             get;
             set;
         }


         [Column(Name = "payer")]
         public string Payer
         {
             get;
             set;
         }


         [Column(Name = "goods_id")]
         public string GoodsId
         {
             get;
             set;
         }

         [Column(Name = "com_id")]
         public string ComId
         {
             get;
             set;
         }

         [Column(Name = "trade_amount")]
         public double TradeAmount
         {
             get;
             set;
         }

         [Column(Name = "service_charge")]
         public double ServiceCharge
         {
             get;
             set;
         }

         [Column(Name = "trade_status")]
         public int TradeStatus
         {
             get;
             set;
         }

         [Column(Name = "random_id")]
         public string RandomId
         {
             get;
             set;
         }

         [Column(Name = "reality_sale_number")]
         public int RealitySaleNumber
         {
             get;
             set;
         }

         [Column(Name = "wares_id")]
         public string WaresId
         {
             get;
             set;
         }

         public string SaleDateStart
         {
             get;
             set;
         }

         public string SaleDateEnd
         {
             get;
             set;
         }

         public string ClientName
         {
             get;
             set;
         }

          public string DeviceId
          {
              get;
              set;
          }

         public int PageIndex
         {
             get;
             set;
         }

         public int PageSize
         {
             get;
             set;
         }

    }
}
