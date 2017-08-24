<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Documents.aspx.cs" Inherits="NHPortal.Documents"
    MasterPageFile="~/MasterPages/PortalMaster.master" Title="Documents" %>

<%@ MasterType VirtualPath="~/MasterPages/PortalMaster.master" %>

<asp:Content runat="server" ID="content" ContentPlaceHolderID="PageContent">
    <link type="text/css" rel="stylesheet" href="Style/Document.css" />

    <div class="doc-section">
        <asp:Literal runat="server" ID="ltrlDocuments"></asp:Literal>

        <asp:Panel runat="server" ID="pnlDelete" ClientIDMode="Static" CssClass="centered-text padded">
            <input type="button" value="Delete Selected" onclick="javascript: deleteDocs();" />
        </asp:Panel>
    </div>

    <asp:Panel runat="server" ID="pnlUpload" CssClass="upload-section" ClientIDMode="Static">
        <span class="upload-section__span">Upload Document:</span>
        <asp:FileUpload runat="server" ID="fileUpload" CssClass="document-uploader" />
        <asp:Button runat="server" ID="btnUpload" Text="Upload Document" OnClick="btnUpload_Click" />
        <asp:Label runat="server" ID="lblMaxLength" CssClass="max-length-text"></asp:Label>

        <br />

        <span class="upload-section__span">Description:</span>
        <asp:TextBox runat="server" ID="tbDescription" CssClass="doc-description-txtbox" MaxLength="250"></asp:TextBox>
    </asp:Panel>

    <div class="centered centered-text error">
        <div>
            <asp:RequiredFieldValidator runat="server"
                ID="rvalUpload"
                ControlToValidate="fileUpload"
                Text="No file selected for upload."
                CssClass="error"
                Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
        <div>
            <asp:RequiredFieldValidator runat="server"
                ID="rvalDesc"
                ControlToValidate="tbDescription"
                Text="Description cannot be blank."
                CssClass="error"
                Display="Dynamic"></asp:RequiredFieldValidator>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hidDocSysNo" />

    <script type="text/javascript">
        function deleteDocs() {
            var proceed = confirm('Delete selected documents?');
            if (proceed) {
                doPostBack('DELETE_DOCUMENTS');
            }
        }

        function downloadFile(docSysNo) {
            document.getElementById('<%=hidDocSysNo.ClientID%>').value = docSysNo;
            doPostBack('DOWNLOAD_DOCUMENT');
        }
    </script>
</asp:Content>
