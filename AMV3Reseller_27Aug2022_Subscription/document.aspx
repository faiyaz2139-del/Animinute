<%@ Page Title="" MasterPageFile="~/Main.master" Language="C#"  AutoEventWireup="true" CodeFile="document.aspx.cs" Inherits="document" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <script src="Assets/assets/js/SweetAlert.js"></script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
      <div class="panel panel-default" id="Div1" runat="server" style="margin-top: 0px; height: 80%;">
        <div class="panel-body">
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
            </div>
          </div>    
   
</asp:Content>
            

