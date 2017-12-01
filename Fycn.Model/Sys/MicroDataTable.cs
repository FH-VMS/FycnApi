using System;
using System.Collections.Generic;
using System.Text;

namespace Fycn.Model.Sys
{
    public class MicroDataTable
    { /// <summary>
      /// 整个查询语句结果的总条数，而非本DataTable的条数
      /// </summary>
      
        public int TotalCount { get; set; }
        public List<MicroDataColumn> Columns { get; set; } = new List<MicroDataColumn>();
        public List<MicroDataRow> Rows { get; set; } = new List<MicroDataRow>();
        public MicroDataColumn[] PrimaryKey { get; set; }
        public MicroDataRow NewRow()
        {
            return new MicroDataRow(this.Columns, new object[Columns.Count]);
        }
        
        
    }
    public class MicroDataColumn
    {
        private string _columnName;
        public string ColumnName {
            get
            {
                return _columnName;
            }
            set
            {
                _columnName = value;
            }
        }
        public Type ColumnType { get; set; }
    }
    public class MicroDataRow
    {
        private object[] _ItemArray;
        public List<MicroDataColumn> Columns { get; private set; }
        public MicroDataRow(List<MicroDataColumn> columns, object[] itemArray)
        {
            this.Columns = columns;
            this._ItemArray = itemArray;
        }
        public object this[int index]
        {
            get { return _ItemArray[index]; }
            set { _ItemArray[index] = value; }
        }
        public object this[string columnName]
        {
            get
            {
                int i = 0;
                foreach (MicroDataColumn column in Columns)
                {
                    if (column.ColumnName == columnName)
                        break;
                    i++;
                }
                return _ItemArray[i];
            }
            set
            {
                int i = 0;
                foreach (MicroDataColumn column in Columns)
                {
                    if (column.ColumnName == columnName)
                        break;
                    i++;
                }
                _ItemArray[i] = value;
            }
        }
    }
}
