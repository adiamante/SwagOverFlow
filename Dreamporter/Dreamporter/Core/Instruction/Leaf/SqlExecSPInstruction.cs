using Dreamporter.Core;
using SwagOverFlow.Clients;
using SwagOverFlow.Utils;
using System;
using System.Collections.Generic;
using System.Data;

namespace Dreamporter.Core
{
    public class SqlExecSPInstruction : Instruction
    {
        #region Private Members
        String _dbContext, _storedProcedure, _targetTable;
        List<SqlParam> _sqlParams = new List<SqlParam>();
        #endregion Private Members

        #region Properties
        #region Type
        public override Type Type { get { return typeof(SqlExecSPInstruction); } }
        #endregion Type
        #region DBContext
        public String DBContext
        {
            get { return _dbContext; }
            set { SetValue(ref _dbContext, value); }
        }
        #endregion DBContext
        #region StoredProcedure
        public String StoredProcedure
        {
            get { return _storedProcedure; }
            set { SetValue(ref _storedProcedure, value); }
        }
        #endregion StoredProcedure
        #region TargetTable
        public String TargetTable
        {
            get { return _targetTable; }
            set { SetValue(ref _targetTable, value); }
        }
        #endregion TargetTable
        #region SqlParams
        public List<SqlParam> SqlParams
        {
            get { return _sqlParams; }
            set { SetValue(ref _sqlParams, value); }
        }
        #endregion SqlParams
        #endregion Properties

        #region Methods
        public override void Execute(RunContext context, Dictionary<String, String> parameters)
        {
            Dictionary<String, SqlParam> sqlParams = new Dictionary<string, SqlParam>();
            if (SqlParams != null)
            {
                foreach (SqlParam p0 in SqlParams)
                {
                    SqlParam p = JsonHelper.Clone<SqlParam>(p0);
                    String key = p.Key, value = p.Value == null ? "" : p.Value.ToString();
                    key = Instruction.ResolveParameters(key, parameters);
                    value = Instruction.ResolveParameters(value, parameters);

                    p.Key = key;
                    if (p.Type == SqlDbType.Structured)
                    {
                        DataTable dt = context.Query(value);
                        p.Value = dt;
                    }
                    else if (p.Type == SqlDbType.Text && value.StartsWith("SELECT"))
                    {
                        DataTable dt = context.Query(value);
                        p.Value = JsonHelper.ToJsonString(dt);
                    }
                    else
                    {
                        p.Value = value;
                    }
                    sqlParams.Add(key, p);
                }
            }

            String targetTable = TargetTable ?? "";

            try
            {
                //TODO: FIX through RuntimeContext with new SqlDBContext class (does not exist yet;
                DataSet ds = new DataSet(); //context.SqlConnections[ConnectionName].ExecuteStoredProcedure(StoredProcedure, sqlParams);
                foreach (DataTable dt in ds.Tables)
                {
                    dt.TableName = targetTable;
                    context.AddTables(dt);
                }

                foreach (KeyValuePair<String, SqlParam> kvpOutParam in sqlParams)
                {
                    if (kvpOutParam.Value.Direction == ParameterDirection.Output)
                    {
                        if (parameters.ContainsKey(kvpOutParam.Value.Key))
                        {
                            parameters[kvpOutParam.Value.Key] = kvpOutParam.Value.Value.ToString();
                        }
                        else
                        {
                            parameters.Add(kvpOutParam.Value.Key, kvpOutParam.Value.Value.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exception exOuter = new Exception($"SQL Params: {JsonHelper.ToJsonString(SqlParams)}\nparameters: {JsonHelper.ToJsonString(parameters)}", ex);
                throw exOuter;
            }
        }
        #endregion Methods
    }
}
