using NHPortal.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NHPortal
{
    /// <summary>
    /// Summary description for DownloadHandler
    /// </summary>
    public class DownloadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string dataType = context.Request.QueryString["data"];
 
            if (dataType == "dlfile")
            {
                string file = context.Request.QueryString["file"];
                SendDownloadFile(file);
            }
        }

        private static void SendDownloadFile(string file)
        {
            string fileName = String.Empty;
            string partialPath = String.Empty;

            if (file == "denialkey")
            {
                fileName = "StickerDenialReasonKey.csv";
                partialPath = "DownloadFiles\\StickerDenial\\";
            }

            string fullPath = System.IO.Path.Combine(NHPortalUtilities.WebRoot + partialPath, fileName);

            System.Web.HttpResponse response = System.Web.HttpContext.Current.Response;
            response.ClearContent();
            response.Clear();
            response.ContentType = "text/plain";
            response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ";");
            response.TransmitFile(fullPath);
            response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}