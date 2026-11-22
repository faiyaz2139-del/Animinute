<%@ Page Language="C#" AutoEventWireup="true" CodeFile="document2.aspx.cs" Inherits="document2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<link href="css/AnyminuteTypography.css?v=20261122v94" rel="stylesheet" /><link href="css/AnyminuteReference.css?v=20261122v94" rel="stylesheet" /><link href="css/AnyminuteFlat.css?v=20261122v94" rel="stylesheet" /></head>
<body class="am-reference am-typography">
    <form id="form1" runat="server">
             <div Style="visibility:hidden;display:none;">
              <asp:GridView runat="server" ID="GvExport" CellPadding="4" ForeColor="#333333" GridLines="None" CellSpacing="4">
             <AlternatingRowStyle BackColor="LightGray"  HorizontalAlign="Center" />
            
             <RowStyle BackColor="white" HorizontalAlign="Center" />
           
            </asp:GridView>
         <asp:GridView runat="server" ID="GvExport1" CellPadding="4" ForeColor="#333333" GridLines="None" CellSpacing="4">
            <AlternatingRowStyle BackColor="LightGray"  HorizontalAlign="Center" />
             <EditRowStyle BackColor="#2461BF" />
             
             <RowStyle BackColor="white" HorizontalAlign="Center" />
            
            </asp:GridView>
        </div>
    </form>
</body>
</html>
