using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Customer
{
    [Table("table_client")]
    public class CustomerModel: BaseModel
    {
        [Column(Name = "client_id", IsPrimaryKey=true)]
        public string Id
        {
            get;
            set;
        }

        [Column(Name = "client_name")]
        public string ClientName
        {
            get;
            set;
        }

        [Column(Name = "client_status")]
        public string ClientStatus
        {
            get;
            set;
        }

        [Column(Name = "client_father_id")]
        public string ClientFatherId
        {
            get;
            set;
        }

        [Column(Name = "client_type")]
        public string ClientType
        {
            get;
            set;
        }

        [Column(Name = "client_contect")]
        public string ClientContect
        {
            get;
            set;
        }

        [Column(Name = "client_tel")]
        public string ClientTel
        {
            get;
            set;
        }

        [Column(Name = "client_email")]
        public string ClientEmail
        {
            get;
            set;
        }

        [Column(Name = "client_country")]
        public string ClientCountry
        {
            get;
            set;
        }


        [Column(Name = "client_currency")]
        public string ClientCurrency
        {
            get;
            set;
        }

        [Column(Name = "client_address")]
        public string ClientAddress
        {
            get;
            set;
        }

        [Column(Name = "update_date")]
        public DateTime UpdateDate
        {
            get;
            set;
        }

        [Column(Name = "updater")]
        public string Updater
        {
            get;
            set;
        }

        [Column(Name = "mobile_arr")]
        public string MobileArr
        {
            get;
            set;
        }

        public string FatherName
        {
            get;
            set;
        }
        /*
        public List<CustomerModel> children
        {
            get;
            set;
        }
        

        public string key
        {
            get;
            set;
        }
        */
    }
}
