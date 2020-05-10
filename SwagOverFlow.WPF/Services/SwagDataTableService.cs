﻿using Newtonsoft.Json.Linq;
using SwagOverFlow.Iterator;
using SwagOverFlow.WPF.Data;
using SwagOverFlow.WPF.Repository;
using SwagOverFlow.WPF.ViewModels;
using SwagOverFlow.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SwagOverFlow.WPF.Services
{
    public class SwagDataTableService
    {
        private readonly SwagContext _context;

        public SwagDataTableService(SwagContext context) => this._context = context;

        public SwagDataTableWPF GetDataTableByName(String groupName)
        {
            SwagDataTableWPF sdtDataTable = null;
            SwagDataTableUnitOfWork work = new SwagDataTableUnitOfWork(_context);
            SwagDataTable sdtStored = work.DataTables.Get(sg => sg.Name == groupName, null).FirstOrDefault();

            if (sdtStored != null)
            {
                work.DataTables.LoadChildren(sdtStored);

                #region Get Rows and Columns
                List<SwagDataColumn> columns = new List<SwagDataColumn>();
                List<SwagDataRow> rows = new List<SwagDataRow>();
                SwagItemPreOrderIterator<SwagData> iterator = sdtStored.CreateIterator();
                for (SwagData swagData = iterator.First(); !iterator.IsDone; swagData = iterator.Next())
                {
                    switch (swagData)
                    {
                        case SwagDataColumn swagDataColumn:
                            columns.Add(swagDataColumn);
                            break;
                        case SwagDataRow swagDataRow:
                            rows.Add(swagDataRow);
                            break;
                    }
                }
                #endregion Get Rows and Columns

                sdtDataTable = new SwagDataTableWPF(sdtStored) { DelaySave = true };
                work.DataTables.Detach(sdtStored);
                work.DataTables.Attach(sdtDataTable);

                DataTable dt = new DataTable();

                #region Resolve Columns
                foreach (SwagDataColumn originalColumn in columns)
                {
                    SwagDataColumnWPF newColumn = new SwagDataColumnWPF(originalColumn);
                    sdtDataTable.Columns.Add(newColumn.ColumnName, newColumn);
                    dt.Columns.Add(newColumn.DataColumn);
                    if (newColumn.ColSeq < dt.Columns.Count)
                    {
                        dt.Columns[newColumn.ColumnName].SetOrdinal(newColumn.ColSeq);
                    }

                    sdtDataTable.Children.Remove(originalColumn);
                    work.Data.Detach(originalColumn);
                    work.Data.Attach(newColumn);
                }
                #endregion Resolve Columns

                #region Resolve Rows
                foreach (SwagDataRow row in rows)
                {
                    JObject rowValues = (JObject)row.ObjValue;
                    DataRow dr = dt.NewRow();

                    foreach (KeyValuePair<String, JToken> kvp in rowValues)
                    {
                        if (kvp.Value.Type != JTokenType.Null)
                        {
                            dr[kvp.Key] = kvp.Value;
                        }
                    }

                    row.DataRow = dr;
                    dt.Rows.Add(dr);
                }
                #endregion Resolve Rows

                sdtDataTable.SetDataTable(dt, true);
                sdtDataTable.DelaySave = false;
            }

            if (sdtDataTable == null)
            {
                #region Create SwagDataTable
                sdtDataTable = new SwagDataTableWPF();
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
