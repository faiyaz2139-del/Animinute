<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Dashboard_Schedular.aspx.cs" Inherits="Report3" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style>
        th{
            width:350px !important;
            text-align:center !important
        }
        .table-bordered>thead>tr>th {
            width:350px !important;
        }
        .table-bordered>tbody>tr>th {
            width:350px !important;
        }
        
        .table{
           /* width:100% !important;*/
        }
        th{
            width:350px !important;
        }

        .datacenter
        {
            text-align:center !important;
        }
        .Grid{
            border: 1px solid #FFFFFF;
        }
        .Labeltextalign
        {
            text-align:right;
        }
    </style>

    

    <script>
        function MergeGridCells() {
            var dimension_cells = new Array();
            var dimension_col = null;
            var columnCount = $("#grdTotal tr:first th").length;
            for (dimension_col = 0; dimension_col <= columnCount; dimension_col++) {
                // first_instance holds the first instance of identical td
                var first_instance = null;
                var rowspan = 1;
                // iterate through rows
                $("#grdTotal").find('tr').each(function () {

                    // find the td of the correct column (determined by the dimension_col set above)
                    var dimension_td = $(this).find('td:nth-child(' + dimension_col + ')');



                    if (first_instance === null) {
                        // must be the first row
                        first_instance = dimension_td;
                    } else if (dimension_td.text() === first_instance.text()) {
                        // the current td is identical to the previous
                        // remove the current td
                        // dimension_td.remove();
                        dimension_td.attr('hidden', true);
                        ++rowspan;
                        // increment the rowspan attribute of the first instance
                        first_instance.attr('rowspan', rowspan);
                    } else {
                        // this cell is different from the last
                        first_instance = dimension_td;
                        rowspan = 1;
                    }
                });
            }
        }

        
        
        $(document).ready(function () {
            $('#GVBusiness').DataTable({
            });
            try {

                var tb = $('#grdTotal th');
                for (var i = 0; i < tb.length; i++) {
                    tb[i].innerHTML = "<b>" + tb[i].innerHTML.replace("@", "<br/>") + "</b>";
                }
           
                MergeGridCells();
            } catch (e) {

            }




            $('#multi-filter-select').DataTable({
                "pageLength": 5,
                initComplete: function () {
                    this.api().columns().every(function () {
                        var column = this;
                        var select = $('<select class="form-control"><option value=""></option></select>')
                            .appendTo($(column.footer()).empty())
                            .on('change', function () {
                                var val = $.fn.dataTable.util.escapeRegex(
                                    $(this).val()
                                );

                                column
                                    .search(val ? '^' + val + '$' : '', true, false)
                                    .draw();
                            });

                        column.data().unique().sort().each(function (d, j) {
                            select.append('<option value="' + d + '">' + d + '</option>')
                        });
                    });
                }
            });

            // Add Row
            $('#add-row').DataTable({
                "pageLength": 5,
            });

            var action = '<td> <div class="form-button-action"> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-primary btn-lg" data-original-title="Edit Task"> <i class="fa fa-edit"></i> </button> <button type="button" data-toggle="tooltip" title="" class="btn btn-link btn-danger" data-original-title="Remove"> <i class="fa fa-times"></i> </button> </div> </td>';

            $('#addRowButton').click(function () {
                $('#add-row').dataTable().fnAddData([
                    $("#addName").val(),
                    $("#addPosition").val(),
                    $("#addOffice").val(),
                    action
                ]);
                $('#addRowModal').modal('hide');

            });
        });
    </script>

    


    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@9"></script>
    <link type="text/css" rel="https://cdn.datatables.net/fixedcolumns/4.2.1/css/fixedColumns.dataTables.min.css" />
    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script type="text/javascript" src='https://cdnjs.cloudflare.com/ajax/libs/jquery/2.1.3/jquery.min.js'></script>
    <script type="text/javascript" src="https://code.jquery.com/ui/1.12.1/jquery-ui.js" defer></script>
      <script scr="https://cdn.datatables.net/rowgroup/1.3.0/js/dataTables.rowGroup.min.js"></script>
      

    <link type="text/css" rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css" />
    
    <script>
        $(document).ready(function () {
            $('#ddlBusiness').on('change', function () {
                $('#<%=hfSelected.ClientID%>').val($(this).val());
            });
        });
    </script>
    <script type="text/javascript">
        $(document).ready(function () {
            
            $("#<%= txtFromDate.ClientID %>").datepicker({
                numberOfMonths: 1,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                onSelect: function () {
                    
                    var dt = new Date($(this).val());
                    $("#<%= txtFromDate.ClientID %>").datepicker();

                    //dt.setDate(dt.getDate());
                    // $("#<%= txtToDate.ClientID %>").datepicker("option", "minDate", dt);
                    var index = $("#<%=hdnrange.ClientID%>").val();
                    if (index == 1) {
                        
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()));
                        $("#<%= txtFromDate.ClientID %>").datepicker('setDate', todate);
                        todate.setDate((todate.getDate() - todate.getDay()) + 6);
                        $("#<%= txtToDate.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 2) {
                        
                        var todayedt = new Date($(this).val());
                        //todate.setDate((todate.getDate() - todate.getDay()) - 21);
                        //alert(todate);
                        $("#<%= txtFromDate.ClientID %>").datepicker('setDate', dt);
                        todate = new Date(todayedt.setMonth(todayedt.getMonth() + 1))
                        //todate.setDate((todate.getDate() - todate.getDay()) + 6);
                        $("#<%= txtToDate.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 3) {
                        
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 28);
                        $("#<%= txtFromDate.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        $("#<%= txtToDate.ClientID %>").datepicker('setDate', todate);
                        $("#<%=hdnDate1.ClientID%>").val() = $("#<%= txtToDate.ClientID %>").val();
                    }
                    if (index == 4) {
                        
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 58);
                        $("#<%= txtFromDate.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        $("#<%= txtToDate.ClientID %>").datepicker('setDate', todate);
                        $("#<%=hdnDate1.ClientID%>").val() = $("#<%= txtToDate.ClientID %>").val();
                    }
                    if (index == 5) {
                        
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 88);
                        $("#<%= txtFromDate.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        $("#<%= txtToDate.ClientID %>").datepicker('setDate', todate);
                        $("#<%=hdnDate1.ClientID%>").val() = $("#<%= txtToDate.ClientID %>").val();
                    }
                    if (index == 6) {
                        
                        var date = new Date(dt);
                        var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
                        var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

                        var firstDayWithSlashes = new Date(firstDay.getFullYear() + '/' + (lastDay.getMonth() + 1) + '/' + (firstDay.getDate()));
                        $("#<%= txtFromDate.ClientID %>").datepicker('setDate', firstDayWithSlashes);

                        var lastDayWithSlashes = new Date(lastDay.getFullYear() + '/' + (lastDay.getMonth() + 1) + '/' + (lastDay.getDate()));
                        $("#<%= txtToDate.ClientID %>").datepicker('setDate', lastDayWithSlashes);
                        $("#<%=hdnDate1.ClientID%>").val() = $("#<%= txtToDate.ClientID %>").val();

                    }

                }
            });

           
          var  dtString1 = $("#<%=hdnDate.ClientID%>").val();
            dtString1 = dtString1.split(',');
            var defaultDate = new Date(dtString1);
            $("#<%= txtFromDate.ClientID %>").datepicker("setDate", defaultDate);



        });
        $(document).ready(function () {
            
            $("#<%= txtToDate.ClientID %>").datepicker({
                numberOfMonths: 1,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                onSelect: function () {
                    var dt = new Date($(this).val());
                    $("#<%= txtToDate.ClientID %>").datepicker();

                    dt.setDate(dt.getDate());

                }
            });
            debugger;
            dtString = $("#<%=hdnDate1.ClientID%>").val();
            dtString = dtString.split(',');
            var defaultDate = new Date(dtString);
            $("#<%= txtToDate.ClientID %>").datepicker("setDate", defaultDate);
            //defaultDate.setDate(defaultDate.getDate());
        });

    </script>

    <script type="text/javascript">
        $(document).ready(function () {
            
            $("#<%= txtFromDate2.ClientID %>").datepicker({
                numberOfMonths: 1,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                onSelect: function () {
                    
                    var dt = new Date($(this).val());
                    $("#<%= txtFromDate2.ClientID %>").datepicker();

                    dt.setDate(dt.getDate());
                    // $("#<%= txtToDate2.ClientID %>").datepicker("option", "minDate", dt);
                    var index = $("#<%=hdnrange2.ClientID%>").val();
                    if (index == 1) {
                        
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()));
                        $("#<%= txtFromDate2.ClientID %>").datepicker('setDate', todate);
                        todate.setDate((todate.getDate() - todate.getDay()) + 6);
                        $("#<%= txtToDate2.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 2) {
                        
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 21);
                        //alert(todate);
                        $("#<%= txtFromDate2.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) + 6);
                        $("#<%= txtToDate2.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 3) {
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 28);
                        $("#<%= txtFromDate2.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        $("#<%= txtToDate2.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 4) {
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 58);
                        $("#<%= txtFromDate2.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        $("#<%= txtToDate2.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 5) {
                        var todate = new Date(dt);
                        todate.setDate((todate.getDate() - todate.getDay()) - 88);
                        $("#<%= txtFromDate2.ClientID %>").datepicker('setDate', todate);
                        todate = new Date(dt);
                        $("#<%= txtToDate2.ClientID %>").datepicker('setDate', todate);
                    }
                    if (index == 6) {
                        var date = new Date(dt);
                        var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
                        var lastDay = new Date(date.getFullYear(), date.getMonth() + 1, 0);

                        var firstDayWithSlashes = new Date(firstDay.getFullYear() + '/' + (lastDay.getMonth() + 1) + '/' + (firstDay.getDate()));
                        $("#<%= txtFromDate2.ClientID %>").datepicker('setDate', firstDayWithSlashes);

                        var lastDayWithSlashes = new Date(lastDay.getFullYear() + '/' + (lastDay.getMonth() + 1) + '/' + (lastDay.getDate()));
                        $("#<%= txtToDate2.ClientID %>").datepicker('setDate', lastDayWithSlashes);

                    }

                }
            });

            dtString = $("#<%=hdndate3.ClientID%>").val();
            dtString = dtString.split(',');
            var defaultDate = new Date(dtString[0], dtString[1], dtString[2]);
            $("#<%= txtFromDate2.ClientID %>").datepicker("setDate", defaultDate);



        });
        $(document).ready(function () {
            
            $("#<%= txtToDate2.ClientID %>").datepicker({
                numberOfMonths: 2,
                changeMonth: true,
                changeYear: true,
                dateFormat: 'yy/mm/dd',
                onSelect: function () {
                    var dt = new Date($(this).val());
                    $("#<%= txtToDate2.ClientID %>").datepicker();

                    dt.setDate(dt.getDate());
                    $("#<%=hdndate4.ClientID%>").val(dt.getDate())
                }
            });
            
            dtString = $("#<%=hdndate4.ClientID%>").val();
            dtString = dtString.split(',');
            var defaultDate = new Date(dtString[0], dtString[1], dtString[2]);
            $("#<%= txtToDate2.ClientID %>").datepicker("setDate", defaultDate);


        });

    </script>
    <style>
        .GridPager a, .GridPager span {
            display: block;
            height: 35px;
            width: 35px;
            font-weight: bold;
            text-align: center;
            text-decoration: none;
            padding: 10px;
        }

        .GridPager a {
            background-color: #fff;
            color: #969696;
            border: 1px solid #dedede;
        }

            .GridPager a:hover {
                color: #337ab7;
            }

        .GridPager span {
            background-color: #337ab7;
            color: #fff;
            border: 1px solid #dedede;
        }

        .buttonhyperlink {
            /*text-align: center;
                    padding-top: 10px;
                    font-family: sans-serif;
                    font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #607DF2;
            border-radius: 0px;
            color: white;
            padding: 5px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .buttonsquarelink222:hover {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }


        .buttonhyperlink222 {
            border: 1px solid #607DF2;
            display: block;
            background-color: white;
            border-radius: 1px;
            color: #607DF2;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .dropdownlist {
            /*text-align: center;
                    padding-top: 10px;
                    font-family: sans-serif;
                    font-size: medium;*/
            border: 1px solid #607DF2;
            display: block;
            background-color: #ECF0F3;
            border-radius: 30px;
            color: black;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            display: block;
            margin-top: -1px;
            font-family: sans-serif;
            font-size: medium;
        }

        .text {
            background: url(/images/calendar.png) no-repeat right;
            border: 1px solid #808080;
            height: 40px;
            background-color: white;
            display: block;
            width: 122%;
            padding: 6px 12px;
            font-size: 14px;
            line-height: 1.42857143;
            color: #555;
            background-color: #fff;
            border-radius: 4px;
        }
    </style>
    <script>

        $("#menu-toggle").click(function () {
            alert("The paragraph was clicked.");
        });
    </script>
    <script>
        function ShowDocument(hdfselected, dtfrom, dtto, businessname, counter, empid, empname, type) {
            var url = "document3.aspx?hfSelected=" + hdfselected + "&dtfrom=  " + dtfrom + "&dtto=  " + dtto + "&businessname=  " + businessname + "&counter=  " + counter + "&empid=  " + empid + " &empname=  " + empname + "&type=  " + type;
            document.location.href = url;
            return false;

            // location.reload();

        }

    </script>

    <script>
        $(document).ready(function () {
            $("#<%=ddl_Business.ClientID%>").select2({
                   placeholder: "Select Business",
                   allowClear: true
               });
               $("#<%=ddlemployee.ClientID%>").select2({
                   placeholder: "Select Employee",
                   allowClear: true
               });
           });
    </script>
    <%--  <link href="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/css/bootstrap-multiselect.css" rel="stylesheet" type="text/css" />
     <script src="http://cdn.rawgit.com/davidstutz/bootstrap-multiselect/master/dist/js/bootstrap-multiselect.js" type="text/javascript"></script>--%>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="panel panel-default" id="cardbus" runat="server" style="display:none;">
        <div class="panel-body" style="margin-top: 0px;">
            <div class="row">
                <div class="col-md-6">
                    <h2 style="margin-top: 12px;">View Schedule</h2>
                </div>
                <div class="col-md-6">
                    <asp:HyperLink ID="HyperLink1______" runat="server" CssClass="buttonhyperlink222"
                        Style="margin-top: 12px; float: right;">Schedule
                    </asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <div style="position: relative">
        <div class="row">
            <div class="col-md-3">
                <a href="Home_Page.aspx" id="A1" runat="server" style="font-size:large;">
                    Home
                </a><br /><br />
                <asp:HyperLink ID="HyperLink1" runat="server" Text="Schedule"></asp:HyperLink>
            </div>
            <div class="col-md-6" style="text-align: center; margin-top:-7px;">
                <asp:Label Font-Bold="true" Font-Size="X-Large" Text="View Schedule" runat="server" />
            </div>
            <div class="col-md-3" style="margin-top: 12px; text-align: right;">
                <p>
                    
                </p>
            </div>
        </div>
    </div>

    <div class="panel panel-default" id="Div1" runat="server" style="margin-top: 0px; height: 80%;">
        <div class="panel-body" style="margin-top: 0px; height: 80%;">

            <div class="row">
                <div class="col-md-12">
                    <div class="form-group">

                        <div class="row">

                            <div class="col-md-3">
                                <div class="form-group">
                                    <label for="Name">Business</label>

                                    <asp:DropDownList ID="ddl_Business" runat="server" CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" Width="100%" ClientIDMode="Static" OnSelectedIndexChanged="ddl_Business_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hfdMotherId" runat="server" />
                                </div>
                            </div>
                            <div class="col-md-3">
                                <%--<label style="font-weight: bold; font-size: 16px; color: black">Employee</label>--%>
                                <label>User</label>
                                <asp:DropDownList ID="ddlemployee" Width="100%" ClientIDMode="Static" required runat="server" CssClass="form-control js-example-basic-single" AppendDataBoundItems="true">
                                    <asp:ListItem Text="Select" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hfSelected" runat="server" />

                                <asp:HiddenField ID="hdnDate" runat="server" />
                                <asp:HiddenField ID="hdnDate1" runat="server" />

                            </div>


                        </div>


                    </div>
                </div>
            </div>


        </div>
    </div>



    <div class="panel panel-default" runat="server" style="background-color: #ECF0F3">
        <div class="panel-body" style="align-items: baseline;">

            <div class="row" style="background-color: #ECF0F3">
                <div class="col-md-12">
                    <div class="form-group">
                        <div class="row">
                            <%--<div class="col-md-1" style="padding: 10px; text-align: right;">
                                <label style="font-weight: bold; font-size: 16px; color: black; text-transform: capitalize;">Date Range 1</label>
                            </div>--%>
                            <div class="col-md-3">
                                <label>Date Range</label>
                                <asp:DropDownList ID="ddrange" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddrange_SelectedIndexChanged">
                                    <asp:ListItem  Value="Select" Text="Select"></asp:ListItem>
                                    <asp:ListItem Value="Weekly" Text="Weekly"></asp:ListItem>
                                    <asp:ListItem Value="Monthly" Text="Monthly"></asp:ListItem>
                                    <%--<asp:ListItem Value="Last 4 Weeks" Text="Last 4 Weeks"></asp:ListItem>
                                    <asp:ListItem Value="Last 30 Days" Text="Last 30 Days"></asp:ListItem>
                                    <asp:ListItem Value="Last 60 Days" Text="Last 60 Days"></asp:ListItem>
                                    <asp:ListItem Value="Last 90 Days" Text="Last 90 Days"></asp:ListItem>--%>
                                    <%--<asp:ListItem Value="Monthly" Text="Monthly"></asp:ListItem>--%>
                                    <%--<asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>--%>
                                </asp:DropDownList>
                                <asp:HiddenField ID="hdnrange" runat="server" />
                            </div>


                            <div class="col-md-2">
                                <label>From Date</label>
                                <asp:TextBox ID="txtFromDate" required runat="server" CssClass="form-control" Max="9999-12-31"></asp:TextBox>
                                <asp:HiddenField ID="hdnfrmdt" runat="server" />

                            </div>


                            <div class="col-md-2">
                                <label>To Date</label>
                                <asp:TextBox ID="txtToDate" required runat="server" CssClass="form-control" Max="9999-12-31"></asp:TextBox>
                                <asp:HiddenField ID="hdntodt" runat="server" />

                            </div>


                            <div class="col-md-2">
                                <label>&nbsp;</label>
                                <asp:Button ID="btnshow" runat="server" Text="View" class="buttonhyperlink"
                                    OnClick="btnshow_Click" />
                                <%--Width="100%"--%>
                            </div>
                            <div class="col-md-2">
                                <label>&nbsp;</label>
                                <div class="dropdown" style="display:none">
                                    <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" style="background-color: #ECF0F3; border-radius: 0px; border: 1px solid #607DF2; color: black; font-family: sans-serif; font-size: small;" aria-expanded="true">
                                        Select Export Type
    <span class="caret"></span>
                                    </button>
                                    <ul class="dropdown-menu" aria-labelledby="dropdownMenu1" style="display:none">
                                        <li>
                                            <asp:LinkButton ID="lnkExcel" runat="server" Text="Excel" OnClick="lnkExcel_Click"></asp:LinkButton></li>
                                        <li>
                                            <asp:LinkButton ID="lnkDirectPdf" runat="server" Text="PDF" OnClick="lnkDirectPdf_Click"></asp:LinkButton></li>

                                    </ul>
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <%--<div class="col-md-2" style="padding: 10px; text-align: center;">
                            </div>--%>
                            <div class="col-md-3">
                                <asp:LinkButton ID="lnkbtn" Style="display:none" OnClick="lnkbtn_Click" runat="server" Text="Compare to prior period"></asp:LinkButton>
                                <asp:HiddenField ID="hdnvisible" runat="server" Value="NO" />
                            </div>
                        </div>
                        <div class="row" style="background-color: #ECF0F3" id="divrange2" runat="server">
                            <div class="col-md-12">
                                <div class="form-group">
                                    <div class="row">
                                        <div class="col-md-2" style="padding: 5px; text-align: right;">
                                            <label style="font-weight: bold; font-size: 16px; color: black; text-transform: capitalize;">Date Range 2</br>(Prior Period)</label>
                                        </div>
                                        <div class="col-md-3">
                                            <asp:DropDownList ID="ddrange2" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddrange2_SelectedIndexChanged">
                                                <asp:ListItem Value="Select" Text="Select"></asp:ListItem>
                                                <asp:ListItem Value="Weekly" Text="Weekly"></asp:ListItem>
                                                <asp:ListItem Value="Last 4 Weeks" Text="Last 4 Weeks"></asp:ListItem>
                                                <asp:ListItem Value="Last 30 Days" Text="Last 30 Days"></asp:ListItem>
                                                <asp:ListItem Value="Last 60 Days" Text="Last 60 Days"></asp:ListItem>
                                                <asp:ListItem Value="Last 90 Days" Text="Last 90 Days"></asp:ListItem>
                                                <asp:ListItem Value="Monthly" Text="Monthly"></asp:ListItem>
                                                <asp:ListItem Value="Custom" Text="Custom"></asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:HiddenField ID="hdnrange2" runat="server" />
                                        </div>


                                        <div class="col-md-1">
                                            <asp:TextBox ID="txtFromDate2" required runat="server" class="text" Max="9999-12-31"></asp:TextBox>
                                            <asp:HiddenField ID="hdnfrmdt2" runat="server" />

                                        </div>


                                        <div class="col-md-1">

                                            <asp:TextBox ID="txtToDate2" required runat="server" class="text" Max="9999-12-31"></asp:TextBox>
                                            <asp:HiddenField ID="HiddenField3" runat="server" />
                                            <asp:HiddenField ID="hdntodt2" runat="server" />
                                        </div>



                                        <div class="col-md-1">

                                            <asp:ImageButton ID="ImageButton2" runat="server" formnovalidate="formnovalidate" ImageUrl="~/images/delete3.png"
                                                OnClick="ImageButton2_Click" Height="27px" Width="29px" />
                                        </div>
                                        <div class="col-md-2">
                                            <asp:HiddenField ID="hdndate3" runat="server" />
                                            <asp:HiddenField ID="hdndate4" runat="server" />
                                        </div>

                                    </div>


                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>


    <div class="panel panel-default" runat="server">
        <div class="panel-body">
            <%--<div class="row">
                 <div class="col-md-2" style="display:none;">
                    <div class="form-group">
                        <label style="margin-top:15px;"></label>
                        <asp:Button ID="btnExport" runat="server" formnovalidate="formnovalidate" Text="Export" OnClick="btnExport_Click" CssClass="btn btn-primary" 
                            Width="100%" />                        
                    </div>
                </div>
            </div>--%>

            <div class="row" runat="server" id="grdHeaderDay">
                <div class="col-md-12">
                <div class="row" style="text-align:center">
                     <asp:Label ID="lblHeader" runat="server" Visible="false" Text=" Weekly Scheduled Hours"></asp:Label>
                        <asp:Label ID="lblHeaderMonthly" runat="server" Visible="false" Text=" Monthly Scheduled Hours"></asp:Label>
                </div>
                    </div>
                <div class="col-md-12" >

                    <%--<div class="col-sm-2">
                        <asp:Label ID="lblHeader" runat="server" Visible="false" Text=" Weekly Scheduled"></asp:Label>
                        <asp:Label ID="lblHeaderMonthly" runat="server" Visible="false" Text=" Monthly Scheduled"></asp:Label>
                    </div>
                        <div class="col-sm-10">--%>
                   
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdHeaderDays" ClientIDMode="Static" runat="server"
                             OnRowDataBound="grdHeaderDays_RowDataBound" AutoGenerateColumns="true" CssClass="Grid" EmptyDataText="No records Found" OnPreRender="grdHeaderDays_PreRender" Visible="false"
                            >
                        </asp:GridView>
                    </div>

                    
                <%--</div>--%>
                    </div>
            </div>



            <div class="row" runat="server" id="grdTotals">
                <div class="col-md-12" >
                   <%-- <div class="col-md-2" runat="server" style="text-align:center; margin-top: 1%; border:medium;  color:black;">
                       
                     
                       
                    </div>
                        <div class="col-sm-10">--%>
                   
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdTotal" ClientIDMode="Static" runat="server"
                             OnRowDataBound="grdTotal_RowDataBound" AutoGenerateColumns="true" EmptyDataText="No records Found" OnPreRender="grdTotal_PreRender" Visible="false"
                            >
                        </asp:GridView>
                    </div>

                    
                <%--</div>--%>
                    </div>
            </div>
                        <%--firoz 21--%>
            <div class="row" runat="server" id="grdHalfMonths">
               <div class="col-md-12" >
                    
                         <%--<div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                       
                       
                    </div>
                       
                 
                    <div class="col-md-10">--%>
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdHalfMonth" ClientIDMode="Static" runat="server" OnRowDataBound="grdHalfMonth_RowDataBound" OnPreRender="grdHalfMonth_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                       <%-- </div>--%>
                </div>
            </div>

            <div class="row" runat="server" id="grdQterlys">
                <div class="col-md-12">
                   
                        <%-- <div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                      
                       
                    </div>
                      
                    
                    <div class="col-md-10">--%>
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdQterly" ClientIDMode="Static" runat="server" OnRowDataBound="grdQterly_RowDataBound" OnPreRender="grdQterly_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                       <%-- </div>--%>
                </div>
            </div>

            <div class="row" runat="server" id="grdFourWeeks">
                <div class="col-md-12" >
                     <%--<div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                       
                       
                    </div>
                   
                    <div class="col-md-10">--%>
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdFourWeek" ClientIDMode="Static" runat="server" OnRowDataBound="grdFourWeek_RowDataBound" OnPreRender="grdFourWeek_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                        <%--</div>--%>
                </div>
            </div>

            <div class="row" runat="server" id="grdFives" >
                <div class="col-md-12" >
                   
                   <%-- <div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                       
                       
                    </div>
                    <div class="col-md-10">--%>
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdFive" ClientIDMode="Static" runat="server" CssClass="datacenter"   OnRowDataBound="grdFive_RowDataBound" OnPreRender="grdFive_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                       <%-- </div>--%>
                </div>
            </div>

            <div class="row" runat="server" id="grdsixs" >
                <div class="col-md-12" >
                    <%--<div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                      
                       
                    </div>
                    <div class="col-md-10">--%>
                    
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdsix" ClientIDMode="Static" runat="server" CssClass="datacenter"  OnRowDataBound="GridView1_RowDataBound" OnPreRender="GridView1_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                       <%-- </div>--%>
                </div>
            </div>

            <div class="row" runat="server" id="grdSevens" >
                <div class="col-md-12" >
                   <%-- <div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                        
                       
                    </div>
                    <div class="col-md-10">--%>
                   
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdSeven" ClientIDMode="Static" runat="server" CssClass="datacenter" OnRowDataBound="grdSeven_RowDataBound" OnPreRender="grdSeven_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                       <%-- </div>--%>
                </div>
            </div>

            <div class="row" runat="server" id="grdEights" >
                <div class="col-md-12" >
                    <%--<div class="col-md-2" style="text-align: center; margin-top: 1%; border:medium;  color:black;">
                       
                       
                    </div>
                    <div class="col-md-10">--%>
                    <%--<div style="overflow-x:auto ; width: 363px;">--%>
                    <div style="overflow-x:auto ;">
                        <asp:GridView ID="grdEight" ClientIDMode="Static" runat="server" CssClass="datacenter" OnRowDataBound="grdEight_RowDataBound" OnPreRender="grdEight_PreRender"
                              AutoGenerateColumns="true" EmptyDataText="No records Found" Visible="false"
                            >
                        </asp:GridView>
                    </div>
                       <%-- </div>--%>
                </div>
            </div>






            <div class="row">
                <div class="col-md-12 col-lg-12">
                   <%-- <div style="overflow-x: auto;">--%>
                        <asp:GridView ID="GVBusiness" ClientIDMode="Static" runat="server" Width="100%"
                            CssClass="table nowrap" AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender="GVBusiness_PreRender" Visible="false"
                            AllowPaging="True" OnPageIndexChanging="GVBusiness_PageIndexChanging" OnRowCommand="GVBusiness_RowCommand" CellPadding="4" ForeColor="#333333" GridLines="None">

                            <%--<HeaderStyle CssClass="bg-clouds" />--%>

                            <AlternatingRowStyle BackColor="LightGray" />

                            <Columns>

                                <asp:TemplateField HeaderText="Sl. No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex + 1%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Company Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Company Name")%>' />
                                        <asp:HiddenField ID="hdncomid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "Company ID")%>' />
                                        <asp:HiddenField ID="hfdEmpid" runat="server" Value='<%#DataBinder.Eval(Container.DataItem, "EmployeeID")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Gross Hours">
                                    <ItemTemplate>
                                        <asp:Label ID="LblRegular" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Gross Hours")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Break Time">
                                    <ItemTemplate>
                                        <asp:Label ID="LblBreakTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Break Time")%>' />
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Net Hours">
                                    <ItemTemplate>
                                        <asp:Label ID="LblTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Net")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <%--                                                                       <asp:TemplateField  HeaderText="View Report">
                                                                                        <ItemTemplate>
                                                                                           <asp:ImageButton  ID="imgExcel" runat="server" Height="30px" ImageUrl="images/excel1.png" ToolTip="Excel Download"   CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Excel" />
                                                                                           <asp:ImageButton  ID="imgpdf" runat="server" Height="30px" ImageUrl="~/images/pdf1.png" ToolTip="PDF Download" CommandArgument='<%# ((GridViewRow) Container).RowIndex %>' CommandName="Pdf" />
                                                                                            </ItemTemplate>
                                                                                    </asp:TemplateField>--%>
                                <asp:TemplateField HeaderText="View Report">
                                    <ItemTemplate>

                                        <div class="dropdown" style="Display:none">
                                            <button class="btn btn-default dropdown-toggle" type="button" id="dropdownMenu1" data-toggle="dropdown" aria-haspopup="true" style="background-color: #ECF0F3; border-radius: 0px; border: 1px solid #607DF2; color: black; font-family: sans-serif; font-size: small;" aria-expanded="true">
                                                Select Export Type
                <span class="caret"></span>
                                            </button>
                                            <ul class="dropdown-menu" aria-labelledby="dropdownMenu1" style="display:none">
                                                <li>
                                                    <asp:LinkButton ID="lnkGvExcel" runat="server" Text="Excel" CommandName="Excel" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton></li>
                                                <li>
                                                    <asp:LinkButton ID="lnkPdf" runat="server" Text="PDF" CommandName="PDF" CommandArgument='<%# Container.DataItemIndex %>'></asp:LinkButton></li>

                                            </ul>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EditRowStyle BackColor="#2461BF" />
                            <EmptyDataTemplate>
                                <div>No records found.</div>
                            </EmptyDataTemplate>

                            <PagerStyle CssClass="GridPager" BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />

                            <RowStyle BackColor="white" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#4870BE" />

                        </asp:GridView>
                    <%--</div>--%>
                </div>
            </div>

            <div class="col-md-12">
                <div class="row">

                    <div class="col-md-6" style="text-align: left; justify-content: left; justify-items: left;">


                        <asp:GridView ID="GVCompare1" ClientIDMode="Static" runat="server" Width="100%"
                            CssClass="table nowrap" AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender="GVCompare1_PreRender" Visible="false"
                            AllowPaging="True" OnPageIndexChanging="GVCompare1_PageIndexChanging" ForeColor="#333333" GridLines="None">

                            <%--<HeaderStyle CssClass="bg-clouds" />--%>

                            <AlternatingRowStyle BackColor="LightGray" Font-Names="Verdana" Font-Size="10px" />

                            <Columns>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lbldate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Date")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Day">
                                    <ItemTemplate>
                                        <asp:Label ID="lbldate" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Day")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Employee Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeName" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Company Name")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Gross Hours">
                                    <ItemTemplate>
                                        <asp:Label ID="LblRegular" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Gross Hours")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Break Time">
                                    <ItemTemplate>
                                        <asp:Label ID="LblBreakTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Break Time")%>' />
                                    </ItemTemplate>

                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="Net Hours">
                                    <ItemTemplate>
                                        <asp:Label ID="LblTotal" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Net")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>


                            </Columns>
                            <EditRowStyle BackColor="#2461BF" />
                            <EmptyDataTemplate>
                                <div>No records found.</div>
                            </EmptyDataTemplate>



                            <RowStyle BackColor="white" Font-Names="Verdana" Font-Size="10px" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#4870BE" />

                        </asp:GridView>
                    </div>
                    <div class="col-md-6">

                        <asp:GridView ID="GVCompare2" ClientIDMode="Static" runat="server" Width="100%"
                            CssClass="table nowrap" AutoGenerateColumns="False" EmptyDataText="No records Found" OnPreRender="GVCompare2_PreRender" Visible="false"
                            AllowPaging="True" OnPageIndexChanging="GVCompare2_PageIndexChanging" ForeColor="#333333" GridLines="None">

                            <%--<HeaderStyle CssClass="bg-clouds" />--%>

                            <AlternatingRowStyle BackColor="LightGray" Font-Names="Verdana" Font-Size="10px" />


                            <Columns>
                                <asp:TemplateField HeaderText="Date">
                                    <ItemTemplate>
                                        <asp:Label ID="lbldate2" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Date")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Day">
                                    <ItemTemplate>
                                        <asp:Label ID="lblday2" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Day")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Employee Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblEmployeeName2" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Company Name")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="Net Hours">
                                    <ItemTemplate>
                                        <asp:Label ID="LblTotal2" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Net")%>' />
                                    </ItemTemplate>
                                </asp:TemplateField>



                            </Columns>
                            <EditRowStyle BackColor="#2461BF" Font-Names="Verdana" Font-Size="10px" />
                            <EmptyDataTemplate>
                                <div>No records found.</div>
                            </EmptyDataTemplate>



                            <RowStyle BackColor="white" Font-Names="Verdana" Font-Size="10px" />
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#F5F7FB" />
                            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                            <SortedDescendingCellStyle BackColor="#E9EBEF" />
                            <SortedDescendingHeaderStyle BackColor="#4870BE" />


                        </asp:GridView>
                    </div>

                </div>
            </div>
            <asp:GridView runat="server" ID="GvExport" CellPadding="4" ForeColor="#333333" GridLines="None" CellSpacing="4">
                <AlternatingRowStyle BackColor="LightGray" HorizontalAlign="Center" />
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="white" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
            <asp:GridView runat="server" ID="GvExport1" CellPadding="4" ForeColor="#333333" GridLines="None" CellSpacing="4">
                <AlternatingRowStyle BackColor="LightGray" HorizontalAlign="Center" />
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="white" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>

        </div>
    </div>
</asp:Content>
