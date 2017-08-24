using GDCoreUtilities.IO;
using GDCoreUtilities.Logging;
using GDWebUtilities;
using NHPortal.Classes;
using NHPortal.Classes.User;
using PortalFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace NHPortal
{
    public partial class Documents : PortalPage
    {
        private const string CHECKBOX_PREFIX = "chk-";
        private HtmlBuilder m_builder;
        private bool m_fullAccess;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PortalUser != null)
            {
                m_fullAccess = PortalUser.HasFullAccess(UserPermissions.Documents.Code);
            }

            if (IsPostBack)
            {
                // TODO: should be a way to not have to disable/enable validators on client post back
                // Disabling the validators here prevents the required field validators from displaying
                // Page.Validator() appears to be called, so we disable now and re-enable in page load complete
                DisableValidators();
                ProcessPostBack();
            }
            else
            {
                RenderDocuments();
                SetMaxLimitText();
                Master.SetHeaderText("Available Documents");
                if (!m_fullAccess)
                {
                    // hide delete button, upload div
                    pnlDelete.Visible = false;
                    pnlUpload.Visible = false;
                }
            }
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            EnableValidators();
        }

        private void ProcessPostBack()
        {
            switch (Master.HidActionValue)
            {
                case "DOWNLOAD_DOCUMENT":
                    DownloadDocument();
                    break;
                case "DELETE_DOCUMENTS":
                    DeleteSelectedDocuments();
                    break;
            }
        }

        private void DisableValidators()
        {
            foreach (RequiredFieldValidator v in Page.Validators)
            {
                if (v.Enabled)
                {
                    v.Enabled = false;
                    NHPortalUtilities.LogSessionMessage("Disabled validator " + v.ClientID, LogSeverity.Debug);
                }
            }
        }

        private void EnableValidators()
        {
            foreach (RequiredFieldValidator v in Page.Validators)
            {
                if (!v.Enabled)
                {
                    v.Enabled = true;
                    NHPortalUtilities.LogSessionMessage("Enabled validator " + v.ClientID, LogSeverity.Debug);
                }
            }
        }

        private void DownloadDocument()
        {
            int sysNo = GDCoreUtilities.NullSafe.ToInt(hidDocSysNo.Value);
            Document doc = DocumentMaster.Find(sysNo);
            if (doc != null)
            {
                bool downloadSuccess = PortalFramework.Utilities.DownloadFile(doc.Filename, doc.FileInformation.FullName, Response);
                if (!downloadSuccess)
                {
                    Master.SetMessagePrompt(String.Format("Unable to download file {0}", doc.Filename));
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFile)
            {
                LogMessage("Attempting to upload file...", LogSeverity.Information);

                string savePath = System.IO.Path.Combine(DocumentMaster.DocumentDirectory, Server.HtmlEncode(fileUpload.FileName));
                LogMessage("Save path: " + savePath, LogSeverity.Information);
                if (File.Exists(savePath))
                {
                    string msg = "File " + fileUpload.FileName + " already exists.";
                    Master.SetMessagePrompt(msg);
                    LogMessage(msg, LogSeverity.Information);
                }
                else
                {
                    SaveFileToServer(fileUpload.FileName, savePath);
                }
            }
            else
            {
                Master.SetMessagePrompt("No file specified for upload");
            }
        }

        private void SaveFileToServer(string filename, string savePath)
        {
            // TODO: way to stream large files up without hitting max content length ?
            try
            {
                fileUpload.SaveAs(savePath);
                //SaveFile(savePath);
                if (SaveNewDocument(filename))
                {
                    LogMessage("Document successfully saved.", LogSeverity.Information);
                    RenderDocuments();
                    Master.SetMessagePrompt("File Successfully Uploaded");
                }
                else
                {
                    LogMessage("Document failed to save.", LogSeverity.Information);
                    Master.SetMessagePrompt("File could not be uploaded at this time -- Please try again later.");
                }
            }
            catch (HttpException ex)
            {
                LogException(ex);
                Master.SetMessagePrompt("File could not be uploaded at this time -- Please try again later.");
            }
        }

        private bool SaveNewDocument(string fileName)
        {
            bool success = false;
            Document doc = new Document(fileName, tbDescription.Text);
            if (doc.Save(PortalUser.UserName))
            {
                DocumentMaster.Add(doc);
                tbDescription.Text = String.Empty;
                success = true;
            }
            return success;
        }

        private void DeleteSelectedDocuments()
        {
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith(CHECKBOX_PREFIX))
                {
                    int sysNo = GDCoreUtilities.NullSafe.ToInt(key.Replace(CHECKBOX_PREFIX, ""));
                    Document doc = DocumentMaster.Find(sysNo);
                    if (doc != null)
                    {
                        DeleteDocument(doc);
                    }
                }
            }

            RenderDocuments();
        }

        private void DeleteDocument(Document doc)
        {
            if (doc != null)
            {
                string path = Path.Combine(DocumentMaster.DocumentDirectory, doc.Filename);
                string toPath = Path.Combine(DocumentMaster.ArchiveDirectory, doc.Filename);

                if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(toPath))
                {
                    doc.Delete(PortalUser.UserName);
                    DocumentMaster.Remove(doc);

                    try
                    {
                        string deletedFile = FileHelper.GetAvailableFileNameForCreation(toPath);
                        if (!String.IsNullOrEmpty(deletedFile))
                        {
                            File.Move(path, deletedFile);
                        }
                    }
                    catch (IOException ex)
                    {
                        LogException(ex);
                    }
                }
            }
        }

        private void RenderDocuments()
        {
            m_builder = new HtmlBuilder();
            if (DocumentMaster.AllAvailableByName.Length > 0)
            {
                m_builder.AddOpeningTag(HtmlTags.Table);
                RenderTableHeader();
                foreach (var doc in DocumentMaster.AllAvailableByName)
                {
                    RenderDocumentRow(doc);
                }
                m_builder.AddClosingTag(HtmlTags.Table);
            }
            else
            {
                m_builder.AddOpeningTag(HtmlTags.Div);
                m_builder.AddAttribute(HtmlAttributes.Class, "centered centered-text");
                m_builder.AddElement(HtmlTags.Label, RenderOptions.Complete, "No Documents Available");
                m_builder.AddClosingTag(HtmlTags.Div);
            }
            ltrlDocuments.Text = m_builder.RenderBuilder();
        }

        private void RenderTableHeader()
        {
            m_builder.AddAttribute(HtmlAttributes.Class, "doc-section__table");
            m_builder.AddElement(HtmlTags.TableHeader, RenderOptions.Complete, "Filename");
            m_builder.AddAttribute(HtmlAttributes.Class, "doc-section__table-col1");
            m_builder.AddElement(HtmlTags.TableHeader, RenderOptions.Complete, "Description");
            m_builder.AddAttribute(HtmlAttributes.Class, "doc-section__table-col2");

            if (m_fullAccess)
            {
                m_builder.AddElement(HtmlTags.TableHeader, RenderOptions.Complete, "Delete");
                m_builder.AddAttribute(HtmlAttributes.Class, "doc-section__table-col3");
            }
        }

        private void RenderDocumentRow(Document doc)
        {
            m_builder.AddOpeningTag(HtmlTags.TableRow);
            m_builder.AddOpeningTag(HtmlTags.TableData);
            m_builder.AddElement(HtmlTags.Anchor, RenderOptions.Complete, doc.Filename);
            m_builder.AddAttribute(HtmlAttributes.Href, String.Format("javascript: downloadFile({0});", doc.SysNo));
            m_builder.AddClosingTag(HtmlTags.TableData);

            m_builder.AddElement(HtmlTags.TableData, RenderOptions.Complete, doc.Description);

            if (m_fullAccess)
            {
                m_builder.AddOpeningTag(HtmlTags.TableData);
                m_builder.AddAttribute(HtmlAttributes.Class, "centered-text");
                m_builder.AddCheckbox("", "", CHECKBOX_PREFIX + doc.SysNo);
                m_builder.AddClosingTag(HtmlTags.TableData);
            }

            m_builder.AddClosingTag(HtmlTags.TableRow);
        }

        private void SetMaxLimitText()
        {
            System.Configuration.Configuration config =
                System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            System.Web.Configuration.HttpRuntimeSection section =
                config.GetSection("system.web/httpRuntime") as System.Web.Configuration.HttpRuntimeSection;
            double maxFileSize = Math.Round(section.MaxRequestLength / 1024.0, 1);
            lblMaxLength.Text = String.Format("Maximum File Size: {0:0.#} MB", maxFileSize);
        }
    }
}