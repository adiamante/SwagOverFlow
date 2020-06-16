using Dreamporter.Builds;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamporter.Instructions
{
    public class ForEachTableGroupInstruction : GroupInstruction
    {
        #region Properties
        #region Type
        public override Type Type { get { return typeof(ForEachTableGroupInstruction); } }
        #endregion Type
        public String Query { get; set; }
        public Decimal DelayMinutes { get; set; } = 0.0m;
        #endregion Properties

        #region Methods
        public override void Execute(RuntimeContext context, Dictionary<String, String> parameters)
        {
            String query = Query ?? "";
            query = Instruction.ResolveParameters(query, parameters);

            DataTable targetTable = context.Query(query);
            Boolean firstRowDone = false;
            foreach (DataRow dr in targetTable.Rows)
            {
                if (firstRowDone && DelayMinutes > 0.0m)
                {
                    Task.Delay((Decimal.ToInt32(DelayMinutes * 1000m)));
                }

                //String info = "";
                Dictionary<String, String> localParameters = JsonHelper.Clone<Dictionary<String, String>>(parameters);
                foreach (DataColumn dc in targetTable.Columns)
                {
                    if (localParameters.ContainsKey(dc.ColumnName))
                    {
                        localParameters[dc.ColumnName] = dr[dc].ToString();
                    }
                    else
                    {
                        localParameters.Add(dc.ColumnName, dr[dc].ToString());
                    }
                    //info += $"|{dr[dc].ToString()}|";
                }

                //if (localParameters.ContainsKey("Info"))
                //{
                //    localParameters["Info"] = localParameters["Info"] + info;
                //}
                //else
                //{
                //    localParameters.Add("Info", info);
                //}

                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    child.Run(context, localParameters);
                }

                firstRowDone = true;
            }
        }
        #endregion Methods
    }
}
