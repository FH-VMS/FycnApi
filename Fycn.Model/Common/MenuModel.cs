using Fycn.Model.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fycn.Model.Common
{
    [Table("table_menu")]
    public class MenuModel
    {
        [Column(Name = "menu_id")]
        public string MenuId
        {
            get;
            set;
        }

        [Column(Name = "menu_name")]
        public string MenuName
        {
            get;
            set;
        }

        [Column(Name = "menu_father")]
        public string MenuFather
        {
            get;
            set;
        }

        [Column(Name = "url")]
        public string Url
        {
            get;
            set;
        }

        [Column(Name = "remark")]
        public string Remark
        {
            get;
            set;
        }

        private int _add = 1;
        public int Add
        {
            get{
                return _add;
            }

            set
            {
                _add = value;
            }
        }

        private int _del = 1;
        public int Del
        {
            get
            {
                return _del;
            }

            set
            {
                _del = value;
            }
        }

        private int _mod = 1;
        public int Mod
        {
            get
            {
                return _mod;
            }

            set
            {
                _mod = value;
            }
        }

        private int _sear = 1;
        public int Sear
        {
            get
            {
                return _sear;
            }

            set
            {
                _sear = value;
            }
        }

        private int _checked = 1;
        public int Checked
        {
            get
            {
                return _checked;
            }

            set
            {
                _checked = value;
            }
        }


        public List<MenuModel> Menus;
    }
}
