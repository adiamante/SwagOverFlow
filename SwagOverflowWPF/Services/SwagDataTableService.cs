using Newtonsoft.Json;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.ViewModels;
using System;
using System.Data;
using System.Linq;

namespace SwagOverflowWPF.Services
{
    public class SwagDataTableService
    {
        private readonly SwagContext _context;

        public SwagDataTableService(SwagContext context) => this._context = context;

        public SwagDataTable GetDataTableByName(String groupName)
        {
            SwagDataTable sdtDataTable = null;
            SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);

            sdtDataTable = work.DataTables.Get(sg => sg.Name == groupName, null, "Root").FirstOrDefault();
            if (sdtDataTable != null)
            {
                #region Load SwagDataTableUnitOfWork
                work.DataRows.RecursiveLoadChildren(sdtDataTable.RootGeneric);
                DataTable dt = new DataTable();

                if (sdtDataTable.ColumnsString != null)
                {
                    DataColumn[] cols = JsonConvert.DeserializeObject<DataColumn[]>(sdtDataTable.ColumnsString);
                    foreach (DataColumn dc in cols)
                    {
                        dt.Columns.Add(dc);
                    }
                }

                SwagItemPreOrderIterator<SwagItemViewModel> iterator = sdtDataTable.CreateIterator();
                for (SwagItemViewModel swagItem = iterator.First(); !iterator.IsDone; swagItem = iterator.Next())
                {
                    SwagDataRow swagItemOriginal = (SwagDataRow)swagItem;

                    if (!String.IsNullOrEmpty(swagItemOriginal.ValueTypeString) && swagItemOriginal.Parent != null)
                    {
                        sdtDataTable.Descendants.Remove(swagItemOriginal);
                        work.DataRows.Delete(swagItemOriginal);

                        SwagDataRow newRow = (SwagDataRow)Activator.CreateInstance(typeof(SwagDataRow), swagItemOriginal);
                        newRow.Children = swagItemOriginal.Children;
                        swagItemOriginal.Parent.Children.Remove(swagItemOriginal);
                        swagItemOriginal.Parent.Children.Add(newRow);

                        Type valueType = JsonConvert.DeserializeObject<Type>(swagItemOriginal.ValueTypeString);
                        Object[] itemsArray = JsonConvert.DeserializeObject<Object[]>(swagItemOriginal.Value.ToString());
                        DataRow dr = dt.NewRow();
                        dr.ItemArray = itemsArray;
                        newRow.DataRow = dr;
                        dt.Rows.Add(dr);
                        work.DataRows.Insert(newRow);
                        newRow.Value = itemsArray;
                    }
                }

                sdtDataTable.SetDataTableSilent(dt);
                #endregion Load SwagDataTableUnitOfWork
            }

            if (sdtDataTable == null)
            {
                #region Create SwagDataTable
                sdtDataTable = new SwagDataTable();
                sdtDataTable.Name = sdtDataTable.AlternateId = groupName;
                work.DataTables.Insert(sdtDataTable);
                work.Complete();
                #endregion Create SwagDataTable
            }

            sdtDataTable.SetContext(_context);
            return sdtDataTable;
        }
    }
}
