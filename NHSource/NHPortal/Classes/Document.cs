using GDCoreUtilities;
using GDDatabaseClient.Oracle;
using Oracle.DataAccess.Client;
using PortalFramework.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace NHPortal.Classes
{
    /// <summary>Used for storing and accessing documents stored in the database.</summary>
    public static class DocumentMaster
    {
        static DocumentMaster()
        {
            Initialize();
            DocumentDirectory = PortalFramework.PortalIniSettings.Values.Directories.Documents;
            ArchiveDirectory = PortalFramework.PortalIniSettings.Values.Directories.DocumentArchive;
        }

        /// <summary>Gets all documents from the database to store in memory.</summary>
        public static void Initialize()
        {
            string qry = "SELECT  doc.doc_sys_no, doc.doc_filename, doc.doc_description, doc.doc_deleted" + Environment.NewLine
                       + "FROM    nh_document doc" + Environment.NewLine
                       + "ORDER BY doc.DOC_SYS_NO";

            m_all = new List<Document>();
            OracleResponse resp = ODAP.GetDataTable(qry, DatabaseTarget.Adhoc);
            if (resp.Successful)
            {
                foreach (DataRow dr in resp.ResultsTable.Rows)
                {
                    m_all.Add(new Document(dr));
                }
            }
        }

        /// <summary>Adds a document to the array of documents.</summary>
        /// <param name="doc">Document to add.</param>
        public static void Add(Document doc)
        {
            m_all.Add(doc);
        }

        /// <summary>Removes a document from the array of documents.</summary>
        /// <param name="doc">Document to remove.</param>
        public static void Remove(Document doc)
        {
            m_all.Remove(doc);
        }

        /// <summary>Returns a document matching the system number provided.</summary>
        /// <param name="sysNo">System number of the document to find.</param>
        /// <returns>Document matching the system number, or null if no match found.</returns>
        public static Document Find(int sysNo)
        {
            Document doc = null;
            foreach (Document d in All)
            {
                if (d.SysNo.Equals(sysNo))
                {
                    doc = d;
                    break;
                }
            }
            return doc;
        }



        private static List<Document> m_all;
        /// <summary>Gets an array of all documents uploaded.</summary>
        public static Document[] All
        {
            get
            {
                if (m_all == null)
                {
                    Initialize();
                }
                return m_all.ToArray();
            }
        }

        /// <summary>Gets an array of all documents not marked as deleted, ordered by file name.</summary>
        public static Document[] AllAvailableByName
        {
            get { return All.Where(d => d.Deleted == false).OrderBy(d => d.Filename).ToArray(); }
        }

        /// <summary>Gets the location of uploaded documents.</summary>
        public static string DocumentDirectory { get; private set; }

        /// <summary>Gets the location of deleted documents.</summary>
        public static string ArchiveDirectory { get; private set; }
    }



    /// <summary>Stores information about an uploaded document.</summary>
    public class Document
    {
        /// <summary>Instantiates a new instance of the Document class.</summary>
        /// <param name="dr">DataRow containing information about the document.</param>
        public Document(DataRow dr)
        {
            SysNo = NullSafe.ToInt(dr["DOC_SYS_NO"]);
            Filename = NullSafe.ToString(dr["DOC_FILENAME"]);
            Description = NullSafe.ToString(dr["DOC_DESCRIPTION"]);
            Deleted = NullSafe.ToBoolean(dr["DOC_DELETED"]);
        }

        /// <summary>Instantiates a new instance of the Document class.</summary>
        /// <param name="filename">File name of the document.</param>
        /// <param name="description">Description of the document.</param>
        public Document(string filename, string description)
        {
            SysNo = 0;
            Filename = filename;
            Description = description;
            Deleted = false;
        }

        /// <summary>Create a new record in the database for the document.</summary>
        /// <param name="usrName">Name of the user creating the document.</param>
        /// <returns>True if the document record was successfully created, false otherwise.</returns>
        public bool Save(string usrName)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("sysNo", OracleDbType.Int32, 4, SysNo, ParameterDirection.InputOutput));
            oraParameters.Add(new OracleParameter("filename", OracleDbType.Varchar2, 50, Filename, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("description", OracleDbType.Varchar2, 250, Description, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("createUsr", OracleDbType.Varchar2, 30, usrName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("CREATE_DOCUMENT", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                SysNo = NullSafe.ToInt(procRsp.ReturnParameters["sysNo"]);
                success = true;
            }
            return success;
        }

        /// <summary>Marks a document as deleted in the database.</summary>
        /// <param name="usrName">Name of the user deleting the document.</param>
        /// <returns>True if the document was successfully deleted, false otherwise.</returns>
        public bool Delete(string usrName)
        {
            List<OracleParameter> oraParameters = new List<OracleParameter>();
            oraParameters.Add(new OracleParameter("sysNo", OracleDbType.Int32, 4, SysNo, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("usrName", OracleDbType.Varchar2, 30, usrName, ParameterDirection.Input));
            oraParameters.Add(new OracleParameter("rspMsg", OracleDbType.Varchar2, 2000, null, ParameterDirection.Output));
            oraParameters.Add(new OracleParameter("rspCode", OracleDbType.Int32, 4, null, ParameterDirection.Output));

            bool success = false;
            OracleResponse procRsp = PortalFramework.Database.ODAP.CallProcedure("DELETE_DOCUMENT", oraParameters.ToArray(), PortalFramework.Database.DatabaseTarget.Adhoc);
            if (NHProcedureUtilities.WasSuccessful(procRsp))
            {
                Deleted = true;
                success = true;
            }
            return success;
        }



        /// <summary>Gets the unique system number assigned to the document.</summary>
        public int SysNo { get; private set; }

        /// <summary>Gets the document's filename.</summary>
        public string Filename { get; private set; }

        /// <summary>Gets the description of the document.</summary>
        public string Description { get; private set; }

        /// <summary>Gets whether or not the document is marked as deleted.</summary>
        public bool Deleted { get; private set; }

        /// <summary>Gets information about the file for the document.</summary>
        public FileInfo FileInformation
        {
            get
            {
                string fullpath = Path.Combine(DocumentMaster.DocumentDirectory, Filename);
                FileInfo fi = null;
                GDCoreUtilities.IO.FileHelper.TryGetFileInfo(fullpath, out fi);
                return fi;
            }
        }
    }
}