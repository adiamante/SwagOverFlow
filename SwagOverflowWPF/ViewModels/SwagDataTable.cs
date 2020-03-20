using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Text;

namespace SwagOverflowWPF.ViewModels
{
    public class SwagDataRow : SwagItemViewModel<Object[]>
    {
        DataRow _dataRow;

        #region DataRow
        [NotMapped]
        public DataRow DataRow
        {
            get { return _dataRow; }
            set 
            {
                if (_dataRow != null)
                {
                    _value = _dataRow.ItemArray;
                }
                SetValue(ref _dataRow, value); 
            }
        }
        #endregion DataRow

        #region Value
        public override object Value
        {
            get
            {
                if (_dataRow != null)
                {
                    return _dataRow.ItemArray;
                }
                return _value;
            }
            set { SetValue(ref _value, value); }
        }
        #endregion Value

        public SwagDataRow()
        {

        }

        public SwagDataRow(DataRow dataRow)
        {
            _dataRow = dataRow;
            _value = dataRow.ItemArray;
        }

        public SwagDataRow(SwagDataRow row) : base()
        {
            PropertyCopy.Copy(row, this);
        }
    }


    public class SwagDataTable  : SwagGroupViewModel<SwagDataRow>
    {
        String _columnsString;
        DataTable _dataTable;
        SwagContext _context;
        Dictionary<DataRow, SwagDataRow> _dictChildren = new Dictionary<DataRow, SwagDataRow>();

        #region DataTable
        public DataTable DataTable
        {
            get { return _dataTable; }
            set 
            {
                SetValue(ref _dataTable, value);

                //Clear child rows
                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    this.ColumnsString = ColumnsString;

                    foreach (SwagDataRow row in RootGeneric.Children)
                    {
                        work.DataRows.Delete(row);
                    }
                    work.DataTables.Update(this);
                    work.Complete();
                }
                RootGeneric.Children.Clear();

                foreach (DataRow dr in _dataTable.Rows)
                {
                    SwagDataRow row = new SwagDataRow(dr);
                    row.Value = row.Value;
                    row.ValueTypeString = row.ValueTypeString;
                    RootGeneric.Children.Add(row);
                }

                if (_context != null)
                {
                    SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                    work.DataTables.Update(this);
                    work.Complete();
                }
            }
        }
        #endregion DataTable

        #region ColumnsString
        public String ColumnsString
        {
            get 
            {
                if (_dataTable != null)
                {
                    DataColumn[] cols = new DataColumn[_dataTable.Columns.Count];
                    _dataTable.Columns.CopyTo(cols, 0);
                    return JsonHelper.ToJsonString(cols);
                }
                return _columnsString;
            }
            set 
            {
                //Set _dataTable.Columns here
                SetValue(ref _columnsString, value);
            }
        }
        #endregion ColumnsString

        public SwagDataTable()
        {
            
        }

        public SwagDataTable(DataTable dt)
        {
            _dataTable = dt;

            foreach (DataRow dr in dt.Rows)
            {
                SwagDataRow row = new SwagDataRow(dr);
                RootGeneric.Children.Add(row);
            }
        }

        public void Init()
        {
            _dataTable.RowChanged += _dataTable_RowChanged;
            _dataTable.ColumnChanged += _dataTable_ColumnChanged;
            _dictChildren.Clear();
            foreach (SwagDataRow row in RootGeneric.Children)
            {
                _dictChildren.Add(row.DataRow, row);
            }
        }

        private void _dataTable_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (_context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                this.ColumnsString = ColumnsString;
                work.DataTables.Update(this);
                work.Complete();
            }
        }

        private void _dataTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (_context != null)
            {
                SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
                SwagDataRow row = _dictChildren[e.Row];
                row.Value = row.Value;
                work.DataRows.Update(row);
                work.Complete();
            }
        }

        public void SetContext(SwagContext context)
        {
            _context = context;
        }

        public void SetDataTableSilent(DataTable dt)
        {
            SetValue(ref _dataTable, dt);
        }
    }
}
