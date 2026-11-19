<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ModalPopUpSelectEffectiveDate.aspx.cs" Inherits="ModalPopUpSelectEffectiveDate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<link href="css/AnyminuteTypography.css?v=20261119v91" rel="stylesheet" /><link href="css/AnyminuteReference.css?v=20261119v91" rel="stylesheet" /><link href="css/AnyminuteFlat.css?v=20261119v91" rel="stylesheet" /></head>
<body class="am-reference am-typography">
    <form id="form1" runat="server">
        <div>
            <asp:Panel ID="PnlEffectiveDate" runat="server" CssClass="modalPopup_addproject" Style="display: none;">
        <div class="header">
            <asp:Label ID="Label5" runat="server"></asp:Label>
            <asp:HiddenField ID="hdnEmpId" runat="server" />
            <asp:HiddenField ID="hdnEmpStatus" runat="server" />
            <asp:HiddenField ID="hdnEffectiveDate" runat="server" />
            <div class="pull-right" style="margin-right: 5px;">
                <button id="btnHide13" class="btn btn-danger">
                    <i class="fa fa-close"></i>
                </button>
            </div>
        </div>
                    
            <div class="body">

                    <div class="row">
                        <div class="col-md-12">
                            <div class="form-group">
                                <asp:Label ID="Label6" text="Effective Date: " runat="server"></asp:Label>
                                <asp:TextBox ID="TextEffectiveDate" Width="400px"  ClientIDMode="Static"
                                    type="date"  AutoPostBack="true" 
                                                                                
                                    runat="server" CssClass="form-control" onkeypress="hideIt()"></asp:TextBox>
                                
                            </div>
                        </div>          
                    </div>
                                    
                    
                    
                    <div class="row">
                        <div class="col-md-12">                         
                            <div class="form-group">
                                <center>
                                  <asp:Button runat="server" ID="BtnSaveEffectiveDate" Text="Save" CssClass="buttonhyperlink22" />
                                 
                                </center>
                            </div>
                        </div>
                    </div>
                 
            </div>
     </asp:Panel>
        </div>
    </form>
</body>
</html>
