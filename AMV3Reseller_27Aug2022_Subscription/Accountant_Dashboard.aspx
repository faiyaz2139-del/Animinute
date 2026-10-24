<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Accountant_Dashboard.aspx.cs" Inherits="AccountantDashBoard2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script>
        window.Promise ||
            document.write(
                '<script src="https://cdn.jsdelivr.net/npm/promise-polyfill@8/dist/polyfill.min.js"><\/script>'
            )
        window.Promise ||
            document.write(
                '<script src="https://cdn.jsdelivr.net/npm/eligrey-classlist-js-polyfill@1.2.20171210/classList.min.js"><\/script>'
            )
        window.Promise ||
            document.write(
                '<script src="https://cdn.jsdelivr.net/npm/findindex_polyfill_mdn"><\/script>'
            )
    </script>

     <script src="https://cdn.jsdelivr.net/npm/apexcharts"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <%-- -----------------------------  Card Area------------------------------------------------%>
    <div class="row">
        <div class="col-md-3">
            <div class="panel panel-default" style="border-left:solid 5px #ff6a00">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-8" style="text-align:center;font-size:14px;font-weight:bold">
                           <label id="lblbuss" runat="server" style="font-size:14px;font-weight:bold;text-transform:capitalize;"></label>
                        </div>
                        <div class="col-md-4">
                            <i class="fa fa-building fa-3x" aria-hidden="true" style="color:#325e7e;opacity:0.5;"></i>
                        </div>
                    </div>
                    </div>
                </div>
        </div>
        <div class="col-md-3">
             <div class="panel panel-default" style="border-left:solid 5px #db4304">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-8" style="text-align:center;font-size:14px;font-weight:bold">
                            <label id="lblbussOwner" runat="server" style="font-size:14px;font-weight:bold;text-transform:capitalize;"></label>
                        </div>
                        <div class="col-md-4">
                            <i class="fa fa-briefcase fa-3x" aria-hidden="true" style="color:#325e7e;opacity:0.5"></i>
                        </div>
                    </div>
                    </div>
                </div>
        </div>
        <div class="col-md-3">
             <div class="panel panel-default" style="border-left:solid 5px #00ff21">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-8" style="text-align:center;font-size:14px;font-weight:bold">
                           <label id="lblep" runat="server" style="font-size:14px;font-weight:bold;text-transform:capitalize;"></label>
                        </div>
                        <div class="col-md-4">
                            <i class="fa fa-users fa-3x" aria-hidden="true" style="color:#325e7e;opacity:0.5"></i>
                        </div>
                    </div>
                    </div>
                </div>
        </div>
        <div class="col-md-3">
            <div class="panel panel-default" style="border-left:solid 5px #00ff90">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-8" style="text-align:center;font-size:14px;font-weight:bold">
                            <label id="lblsuper" runat="server" style="font-size:14px;font-weight:bold;text-transform:capitalize;"></label>
                        </div>
                        <div class="col-md-4">
                            <i class="fa fa-user-secret fa-3x" aria-hidden="true" style="color:#325e7e;opacity:0.5"></i>
                        </div>
                    </div>
                    </div>
                </div>
        </div>
    </div>
 <%-- -----------------------------  Card Area end------------------------------------------------%>


     <%-------------------------------- Graph Area ------------------------------------------------%>

    <div class="row">
        <div class="col-md-6">
             <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                    <%--<div style="font-weight:bold;text-align:center""> No. of Company Owner & Company - Weekly</div>--%>
                    <div style="font-weight:bold;text-align:center""> No. of Subscriber & Company - Weekly</div>
                    <div id="chartAccFig1">

                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-6">
             <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                    <div style="font-weight:bold;text-align:center"> No. of Subscriber & Company - Monthly</div>
                    <div id="chartAccFig2">

                    </div>
                </div>
            </div>
        </div>

         <div class="col-md-6">
             <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-2">
                            </div>
                       <div class="col-md-3">
                          
                          <asp:DropDownList ID="ddlfig3" runat="server" CssClass="form-control" Width="100%" Height="30px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlfig3_SelectedIndexChanged">
                        <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                          </asp:DropDownList>
                             
                       </div>
                       <div class="col-md-6" style="font-weight:bold;text-align:center;margin-top:7px;">
                           No. of Subscriber & Company - Quarterly
                       </div>
                   </div>
                    
                    <div id="chartAccFig3">

                    </div>
                </div>
            </div>
        </div>

         <div class="col-md-6">
             <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                    
                    <div class="row">
                        <div class="col-md-1" style="visibility:hidden;">
                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control" Width="100%" Height="30px">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-11" style="font-weight:bold;text-align:center;margin-top:7px;">
                            No. of Subscriber & Company - Yearly
                        </div>
                    </div>
                    
                    <div id="chartAccFig4">

                    </div>
                </div>
            </div>
        </div>
    </div>

    <%---------------------------------------------------Weekly----------------------------------------------------%>
    
    <asp:HiddenField ID="hfdCatagoriesfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig3categories = $("#<%= hfdCatagoriesfig3.ClientID %>").val();
        var fig3Submitted = $("#<%= hfdsubmitedfig3.ClientID %>").val();
        var fig3pending = $("#<%= hfdpendingfig3.ClientID %>").val();
        

        var jsonCategories = JSON.parse(fig3categories);
        var jsonSubmitted = JSON.parse(fig3Submitted);
        var jsonpending = JSON.parse(fig3pending);
       

        var Fig3 = {
            series: [{
                name: 'No. of Company',
                data: jsonSubmitted
            }, {
                name: 'No. of Company Owners',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            xaxis: {
                categories: jsonCategories,
            },
            yaxis: {
                title: {
                    text: ''
                }
            },
            fill: {
                opacity: 1
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartAccFig1"), Fig3);
        chart.render();


    </script>


    <%---------------------------------------------------Monthly----------------------------------------------------%>

    <asp:HiddenField ID="hfdCatagoriesYR" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfiYR" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingYR" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig3categories = $("#<%= hfdCatagoriesYR.ClientID %>").val();
        var fig3Submitted = $("#<%= hfdsubmitedfiYR.ClientID %>").val();
        var fig3pending = $("#<%= hfdpendingYR.ClientID %>").val();


        var jsonCategories = JSON.parse(fig3categories);
        var jsonSubmitted = JSON.parse(fig3Submitted);
        var jsonpending = JSON.parse(fig3pending);


        var Fig2 = {
            series: [{
                name: 'No. of Company',
                data: jsonSubmitted
            }, {
                name: 'No. of Company Owners',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            xaxis: {
                categories: jsonCategories,
            },
            yaxis: {
                title: {
                    text: ''
                    
                }
            },
            fill: {
                opacity: 1
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartAccFig2"), Fig2);
        chart.render();


    </script>


     <%---------------------------------------------------Qutarly----------------------------------------------------%>

    <asp:HiddenField ID="hfdCatagoriesQR" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfiQR" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingQR" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig3categories = $("#<%= hfdCatagoriesQR.ClientID %>").val();
        var fig3Submitted = $("#<%= hfdsubmitedfiQR.ClientID %>").val();
        var fig3pending = $("#<%= hfdpendingQR.ClientID %>").val();


        var jsonCategories = JSON.parse(fig3categories);
        var jsonSubmitted = JSON.parse(fig3Submitted);
        var jsonpending = JSON.parse(fig3pending);


        var Fig3 = {
            series: [{
                name: 'No. of Company',
                data: jsonSubmitted
            }, {
                name: 'No. of Company Owners',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            xaxis: {
                categories: jsonCategories,
            },
            yaxis: {
                title: {
                    text: ''
                }
            },
            fill: {
                opacity: 1
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartAccFig3"), Fig3);
        chart.render();


    </script>


     <%---------------------------------------------------Yearly----------------------------------------------------%>

    <asp:HiddenField ID="hfdCatagoriesYer" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfiYer" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingYer" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig3categories = $("#<%=hfdCatagoriesYer.ClientID %>").val();
        var fig3Submitted = $("#<%=hfdsubmitedfiYer.ClientID %>").val();
        var fig3pending = $("#<%=hfdpendingYer.ClientID %>").val();


        var jsonCategories = JSON.parse(fig3categories);
        var jsonSubmitted = JSON.parse(fig3Submitted);
        var jsonpending = JSON.parse(fig3pending);


        var Fig4 = {
            series: [{
                name: 'No. of Company',
                data: jsonSubmitted
            }, {
                name: 'No. of Company Owners',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '55%',
                    endingShape: 'rounded'
                },
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['transparent']
            },
            xaxis: {
                categories: jsonCategories,
            },
            yaxis: {
                title: {
                    text: ''
                }
            },
            fill: {
                opacity: 1
            },
            tooltip: {
                y: {
                    formatter: function (val) {
                        return val
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartAccFig4"), Fig4);
        chart.render();


    </script>
</asp:Content>

