using Dreamporter.Builds;
using System;
using System.Collections.Generic;
using System.Data;

namespace Dreamporter.Instructions
{
    public class QueryInstruction : Instruction
    {
        #region Private Members
        String _query, _targetTable;
        #endregion Private Members

        #region Properties
        #region Type
        public override Type Type { get { return typeof(QueryInstruction); } }
        #endregion Type
        #region Query
        public String Query
        {
            get { return _query; }
            set { SetValue(ref _query, value); }
        }
        #endregion Query
        #region TargetTable
        public String TargetTable
        {
            get { return _targetTable; }
            set { SetValue(ref _targetTable, value); }
        }
        #endregion TargetTable
        #endregion Properties

        public override void Execute(RuntimeContext context, Dictionary<String, String> parameters)
        {
            String query = Query,
                targetTable = TargetTable ?? "";

            foreach (KeyValuePair<String, String> kvp in parameters)
            {
                targetTable = targetTable.Replace($"{{{kvp.Key}}}", kvp.Value);
                query = query.Replace($"{{{kvp.Key}}}", kvp.Value);
            }

            if (targetTable == "")
            {
                context.ExecuteNonQuery(query);
            }
            else
            {
                DataTable result = context.Query(query);
                result.TableName = targetTable;
                context.AddTables(result);
            }
        }

        //Sample Query
        //SELECT *, date('now', '-' || n  || ' day') AS Date,
        // CASE WHEN n <= 4 THEN 'Sales,' ELSE '' END ||
        // CASE WHEN n <= 8 THEN 'Deposits,' ELSE '' END ||
        // CASE WHEN n <= 16 THEN 'Labor' ELSE '' END
        // AS Tags
        //FROM util.numbers
        //WHERE n between 1 and 16
    }
}
