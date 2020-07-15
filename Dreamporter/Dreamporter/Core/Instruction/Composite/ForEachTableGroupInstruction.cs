using Dreamporter.Core;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Dreamporter.Core
{
    public class ForEachTableGroupInstruction : GroupInstruction
    {
        #region Private Members
        String _query;
        Decimal _delayMinutes;
        #endregion Private Members

        #region Properties
        #region Type
        public override Type Type { get { return typeof(ForEachTableGroupInstruction); } }
        #endregion Type
        #region Query
        public String Query
        {
            get { return _query; }
            set { SetValue(ref _query, value); }
        }
        #endregion Query
        #region DelayMinutes
        public Decimal DelayMinutes
        {
            get { return _delayMinutes; }
            set { SetValue(ref _delayMinutes, value); }
        }
        #endregion DelayMinutes
        #endregion Properties

        #region Methods
        public override void RunHandler(RunContext context, RunParams rp)
        {
            String query = Query ?? "";
            query = Instruction.ResolveParameters(query, rp.Params);

            DataTable targetTable = context.Query(query);
            Boolean firstRowDone = false;
            foreach (DataRow dr in targetTable.Rows)
            {
                if (firstRowDone && DelayMinutes > 0.0m)
                {
                    Task.Delay((Decimal.ToInt32(DelayMinutes * 1000m)));
                }

                //String info = "";
                RunParams rpLocal = JsonHelper.Clone<RunParams>(rp);
                foreach (DataColumn dc in targetTable.Columns)
                {
                    if (rpLocal.Params.ContainsKey(dc.ColumnName))
                    {
                        rpLocal.Params[dc.ColumnName] = dr[dc].ToString();
                    }
                    else
                    {
                        rpLocal.Params.Add(dc.ColumnName, dr[dc].ToString());
                    }
                    //info += $"|{dr[dc].ToString()}|";
                }
                
                foreach (Instruction child in Children.OrderBy(c => c.Sequence))
                {
                    child.Run(context, rpLocal);
                }

                firstRowDone = true;
            }
        }
        #endregion Methods
    }
}
