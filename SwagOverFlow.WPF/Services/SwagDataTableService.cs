using Newtonsoft.Json.Linq;
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
                sdtDataTable = new SwagDataTableWPF(sdtStored);

                work.DataTables.Delete(sdtStored);
                work.DataTables.Insert(sdtDataTable);

                sdtDataTable.DelaySave = true;

                #region Load SwagDataTableUnitOfWork
                work.DataTables.LoadChildren(sdtDataTable);
                DataTable dt = new DataTable();

                //Find all the columns first
                SwagItemPreOrderIterator<SwagData> iterator = sdtDataTable.CreateIterator();
                List<SwagData> dataToRemove = new List<SwagData>();
                List<SwagData> dataToAdd = new List<SwagData>();
                for (SwagData swagData = iterator.First(); !iterator.IsDone; swagData = iterator.Next())
                {
                    switch (swagData)
                    {
                        case SwagDataColumn swagDataColumn:
                            SwagDataColumnWPF swagDataColumnWpf = new SwagDataColumnWPF(swagDataColumn);
                            dataToRemove.Add(swagDataColumn);
                            dataToAdd.Add(swagDataColumnWpf);
                            sdtDataTable.Columns.Add(swagDataColumnWpf.ColumnName, swagDataColumnWpf);
                            dt.Columns.Add(swagDataColumnWpf.DataColumn);
                            if (swagDataColumnWpf.ColSeq < dt.Columns.Count)
                            {
                                dt.Columns[swagDataColumnWpf.ColumnName].SetOrdinal(swagDataColumnWpf.ColSeq);
                            }
                            break;
                    }
                }
                for (SwagData swagData = iterator.First(); !iterator.IsDone; swagData = iterator.Next())
                {
                    switch (swagData)
                    {
                        case SwagDataRow swagDataRow:
                            dataToRemove.Add(swagDataRow);
                            dataToAdd.Add(swagDataRow);
                            JObject rowValues = (JObject)swagDataRow.ObjValue;
                            DataRow dr = dt.NewRow();

                            foreach (KeyValuePair<String, JToken> kvp in rowValues)
                            {
                                if (kvp.Value.Type != JTokenType.Null)
                                {
                                    dr[kvp.Key] = kvp.Value;
                                }
                            }

                            swagDataRow.DataRow = dr;
                            dt.Rows.Add(dr);
                            break;
                    }
                }
                iterator = sdtDataTable.CreateIterator();

                
                work.Complete();    //This clears the Children

                for (int i = 0; i < dataToRemove.Count; i++)    //Fills the Children
                {
                    work.Data.Delete(dataToRemove[i]);
                    work.Data.Detach(dataToRemove[i]);
                    work.Data.Attach(dataToAdd[i]);
                }

                sdtDataTable.SetDataTable(dt, true);
                #endregion Load SwagDataTableUnitOfWork

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
