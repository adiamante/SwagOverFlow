using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwagOverflowWPF.Controls;
using SwagOverflowWPF.Data;
using SwagOverflowWPF.Iterator;
using SwagOverflowWPF.Repository;
using SwagOverflowWPF.Utilities;
using SwagOverflowWPF.ViewModels;
using System;
using System.Collections.Generic;
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

                if (sdtDataTable.Columns != null)
                {
                    foreach (KeyValuePair<String, SwagDataColumn> sdcKvp in sdtDataTable.Columns)
                    {
                        dt.Columns.Add(sdcKvp.Value.DataColumn);
                    }
                }

                SwagItemPreOrderIterator<SwagItem> iterator = sdtDataTable.CreateIterator();
                for (SwagItem swagItem = iterator.First(); !iterator.IsDone; swagItem = iterator.Next())
                {
                    SwagDataRow swagDataRow = (SwagDataRow)swagItem;

                    if (swagDataRow.Value != null)      //Skip the root
                    {
                        JObject rowValues = (JObject)swagDataRow.Value;
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
                    }
                    
                    #region OLD
                    //if (!String.IsNullOrEmpty(swagDataRow.ValueTypeString) && swagDataRow.Parent != null)
                    //{
                    //    sdtDataTable.Descendants.Remove(swagDataRow);
                    //    work.DataRows.Delete(swagDataRow);

                    //    SwagDataRow newRow = (SwagDataRow)Activator.CreateInstance(typeof(SwagDataRow), swagDataRow);
                    //    newRow.Children = swagDataRow.Children;
                    //    swagDataRow.Parent.Children.Remove(swagDataRow);
                    //    swagDataRow.Parent.Children.Add(newRow);

                    //    JObject rowValues = JsonConvert.DeserializeObject<JObject>(swagDataRow.Value.ToString());
                    //    DataRow dr = dt.NewRow();

                    //    foreach (KeyValuePair<String, JToken> kvp in rowValues)
                    //    {
                    //        if (kvp.Value.Type != JTokenType.Null)
                    //        {
                    //            dr[kvp.Key] = kvp.Value;
                    //        }
                    //    }

                    //    newRow.DataRow = dr;
                    //    dt.Rows.Add(dr);
                    //    work.DataRows.Insert(newRow);
                    //}
                    #endregion Name
                }

                sdtDataTable.SetDataTable(dt, true);
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
