using GDDatabaseClient.Oracle;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace NHPortal.Classes
{
    ///// <summary>Database Access Protocol used for communicating with an Oracle database.</summary>
    //public static class DAP
    //{
    //    /// <summary>Queries a database and returns the first result found.</summary>
    //    /// <param name="query">Query string.</param>
    //    /// <param name="targetDB">Database to query.</param>
    //    /// <returns>DataRow containing the query results, or null if no results found.</returns>
    //    public static DataRow GetDataRow(string query, DatabaseTarget targetDB)
    //    {
    //        string connString = ResolveDatabaseTarget(targetDB);
    //        OracleCommandInfo info = new OracleCommandInfo(query, connString);

    //        DataRow dr = OracleWorker.QueryDataRow(query, connString);
    //        return dr;
    //    }

    //    /// <summary>Queries a database and returns the first result found.</summary>
    //    /// <param name="cmdInfo">Information about the query and connection information.</param>
    //    /// <returns>DataRow containing the query results, or null if no results found.</returns>
    //    public static DataRow GetDataRow(OracleCommandInfo cmdInfo)
    //    {
    //        return OracleWorker.QueryDataRow(cmdInfo);
    //    }

    //    /// <summary>Queries a database with the provided query string and returns the results.</summary>
    //    /// <param name="query">Query string.</param>
    //    /// <param name="targetDB">Database to query.</param>
    //    /// <returns>DataTable containing the query results.</returns>
    //    public static DataTable GetDataTable(string query, DatabaseTarget targetDB)
    //    {
    //        string connString = ResolveDatabaseTarget(targetDB);
    //        OracleCommandInfo info = new OracleCommandInfo(query, connString);

    //        DataTable dt = OracleWorker.QueryDataTable(info);
    //        return dt;
    //    }

    //    /// <summary>Queries a database with the provided query strings and returns the resulting DataSet.</summary>
    //    /// <param name="queryStrings">Query strings to send to the database.</param>
    //    /// <param name="targetDB">Database to query.</param>
    //    /// <returns>DataSet containing the query results.</returns>
    //    public static DataSet GetDataSet(string[] queryStrings, DatabaseTarget targetDB)
    //    {
    //        string connString = ResolveDatabaseTarget(targetDB);
    //        List<OracleCommandInfo> cmds = new List<OracleCommandInfo>();
    //        foreach (string query in queryStrings)
    //        {
    //            cmds.Add(new OracleCommandInfo(query, connString));
    //        }

    //        return GetDataSet(cmds.ToArray());
    //    }

    //    /// <summary>Queries a database with the provided query strings and returns the resulting DataSet.</summary>
    //    /// <param name="queryCommands">Commands containing query and database connection information.</param>
    //    /// <returns>Resulting dataset containing the query results.</returns>
    //    public static DataSet GetDataSet(OracleCommandInfo[] queryCommands)
    //    {
    //        return OracleWorker.QueryDataSet(queryCommands);
    //    }

    //    /// <summary>Queries a database and returns an iterator for each DataRow in the results.</summary>
    //    /// <param name="query">Query string.</param>
    //    /// <param name="targetDB">Database to query.</param>
    //    /// <returns>DataRow enumerator.</returns>
    //    public static IEnumerable<DataRow> GetDataRowEnumerator(string query, DatabaseTarget targetDB)
    //    {
    //        string connString = ResolveDatabaseTarget(targetDB);

    //        OracleCommandInfo cmdInfo = new OracleCommandInfo(query, connString);
    //        foreach (DataRow dr in OracleWorker.QueryEnumerable(cmdInfo))
    //        {
    //            yield return dr;
    //        }
    //    }

    //    /// <summary>Gets the connection string for the target database.</summary>
    //    /// <param name="targetDB">Database to get the connection string of.</param>
    //    /// <returns>Connection string for the target database.</returns>
    //    public static string ResolveDatabaseTarget(DatabaseTarget targetDB)
    //    {
    //        string connectionString = String.Empty;
    //        switch (targetDB)
    //        {
    //            case DatabaseTarget.Adhoc:
    //                connectionString = PortalFramework.IniSettings.Values.AdhocConnectionString;
    //                break;
    //            case DatabaseTarget.OLTP:
    //                connectionString = PortalFramework.IniSettings.Values.OLTPConnectionString;
    //                break;
    //        }
    //        return connectionString;
    //    }

    //    /// <summary>Builds an OracleCommandInfo object.</summary>
    //    /// <param name="commandText">Command text.</param>
    //    /// <param name="targetDB">Target database.</param>
    //    /// <returns>OracleCommandInfo object.</returns>
    //    public static OracleCommandInfo BuildOracleCommand(string commandText, DatabaseTarget targetDB)
    //    {
    //        OracleCommandInfo cmdInfo = new OracleCommandInfo();
    //        cmdInfo.Text = commandText;
    //        cmdInfo.ConnectionString = ResolveDatabaseTarget(targetDB);
    //        return cmdInfo;
    //    }
    //}

    ///// <summary>Represents a target database defined via the ini settings file.</summary>
    //public enum DatabaseTarget
    //{
    //    /// <summary>Database defined as the primary OLTP database.</summary>
    //    OLTP,
    //    /// <summary>Database defined as the primary Adhoc database.</summary>
    //    Adhoc,
    //}
}