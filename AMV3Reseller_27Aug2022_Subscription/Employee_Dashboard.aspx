<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Employee_Dashboard.aspx.cs" Inherits="EmpDashBoard2" %>

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
    


    <div class="row" style="display:none;">
        <div class="col-lg-12">
            <div class="panel panel-default">
                <div class="row">
                    <div class="col-md-12">
                    </div>
                </div>                
                <div class="row">
                    <div class="col-md-12" style="margin-left:20px">
                        <asp:Label ID="Label_Approved_Timesheet" Text="" ClientIDMode="Static" ForeColor="Green" Font-Bold="true" runat="server" />
                        <asp:Label Text="day(s) time sheet has been approved" ClientIDMode="Static" ForeColor="Green" Font-Bold="true" runat="server" />
                        <asp:LinkButton ID="LinkButton_Approved_Timesheet" runat="server" ClientIDMode="Static" ForeColor="Green" Font-Bold="true" OnClick="LinkButton_Approved_Timesheet_Click" Text="Click Here" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12" style="margin-left:20px">
                        <asp:Label ID="Label_Rejected_Timesheet" Text="" ClientIDMode="Static" ForeColor="Red" Font-Bold="true" runat="server" />
                        <asp:Label Text="day(s) time sheet has been rejected" ClientIDMode="Static" ForeColor="Red" Font-Bold="true" runat="server" />
                        <asp:LinkButton ID="LinkButton_Rejected_Timesheet" runat="server" ClientIDMode="Static" ForeColor="Red" Font-Bold="true" OnClick="LinkButton_Rejected_Timesheet_Click" Text="Click Here"/>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12" style="margin-left:20px">
                        <asp:Label ID="Label_Pending_Timesheet" Text="" ClientIDMode="Static" ForeColor="#ff9900" Font-Bold="true" runat="server" />
                        <asp:Label Text="day(s) time sheet are pending" ClientIDMode="Static" ForeColor="#ff9900" Font-Bold="true" runat="server" />
                        <asp:LinkButton ID="LinkButton_Pending_Timesheet" runat="server" ClientIDMode="Static" ForeColor="#ff9900" Font-Bold="true" OnClick="LinkButton_Rejected_Timesheet_Click" Text="Click Here"/>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12" style="margin-left:20px">
                        <asp:Label ID="Label_Absent_Timesheet" Text="" ClientIDMode="Static" ForeColor="#666699" Font-Bold="true" runat="server" />
                        <asp:Label Text="day(s) time sheet is absent" ClientIDMode="Static" ForeColor="#666699" Font-Bold="true" runat="server" />
                        <asp:LinkButton ID="LinkButton_Absent_Timesheet" runat="server" ClientIDMode="Static" ForeColor="#666699" Font-Bold="true" OnClick="LinkButton_Absent_Timesheet_Click" Text="Click Here"/>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12" style="margin-left:20px">
                        <asp:Label ID="Label_Blank_Timesheet" Text="" ClientIDMode="Static" ForeColor="#337ab7" Font-Bold="true" runat="server" />
                        <asp:Label Text="day(s) time sheet has not been filled" ClientIDMode="Static" ForeColor="#337ab7" Font-Bold="true" runat="server" />
                        <asp:LinkButton ID="LinkButton_Blank_Timesheet" runat="server" ClientIDMode="Static" ForeColor="#337ab7" Font-Bold="true" OnClick="LinkButton_Blank_Timesheet_Click" Text="Click Here" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="panel panel-default">
        <div class="panel-body">
            <div class="row">
                <div class="col-md-4"></div>
                <div class="col-md-4" style="align-items:center;">
                    <asp:DropDownList ID="ddlBussName" runat="server" CssClass="form-control js-example-basic-single" Width="100%" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBussName_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="col-md-4"></div>
            </div>               
        </div>
    </div>


    <div class="row">

        <div class="col-md-6">
            <div class="panel panel-default" style="height:400px">
                <div class="panel-body">
                   
                    <div style="font-weight:bold;text-align:center;">
                        Hours Worked by Businesses - Weekly
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig2">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="panel panel-default" style="height:400px">
                <div class="panel-body">
                   
                    <div style="font-weight:bold;text-align:center;">
                        Hours Worked by Businesses - Monthly
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig3">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="panel panel-default" style="height:400px">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-2">
                        </div>
                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlfig4" runat="server" CssClass="form-control"  Width="100%" Height="30px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlfig4_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                       </div>
                       <div class="col-md-6" style="font-weight:bold;text-align:center;margin-top:6px;">
                           Hours Worked by Businesses - Quarterly
                       </div>
                   </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig4">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-md-6">
            <div class="panel panel-default" style="height:400px">
                <div class="panel-body">

                    <div class="row">
                        <div class="col-md-1" style="visibility:hidden;">
                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control"  Width="100%" Height="30px">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                       </div>
                       <div class="col-md-11" style="font-weight:bold;text-align:center;margin-top:6px;">
                           Hours Worked by Businesses - Yearly
                       </div>
                   </div>
                   
                    <%--<div style="font-weight:bold;text-align:center;">
                        Hours Worked by Businesses - Yearly
                    </div>--%>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig5">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        

    </div>


    <div class="row">

    </div>
    

    <script>
        var colors = [
            '#008ffb'
        ]
    </script>
    


    <%-----------------------------------------------------------------------------------------------%>

    <asp:HiddenField ID="hfdCatagoriesfig2" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfig2" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingfig2" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig2.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig2.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig2.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig2 = {
            series: [{
                name: 'Total Working Hr',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '35%',
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

        var chart = new ApexCharts(document.querySelector("#chartfig2"), Fig2);
        chart.render();


    </script>

     <%-----------------------------------------------------------------------------------------------%>

     <asp:HiddenField ID="hfdCatagoriesfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig3.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig3.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig3.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig3 = {
            series: [{
                name: 'Total Working Hr',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '35%',
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

        var chart = new ApexCharts(document.querySelector("#chartfig3"), Fig3);
        chart.render();


    </script>


      <%-----------------------------------------------------------------------------------------------%>
     <asp:HiddenField ID="hfdCatagoriesfig4" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfig4" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingfig4" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig4.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig4.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig4.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig4 = {
            series: [{
                name: 'Total Working Hr',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '35%',
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

        var chart = new ApexCharts(document.querySelector("#chartfig4"), Fig4);
        chart.render();


    </script>


     <%-----------------------------------------------------------------------------------------------%>

     <asp:HiddenField ID="hfdCatagoriesfig5" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdsubmitedfig5" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpendingfig5" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig5.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig5.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig5.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig5 = {
            series: [{
                name: 'Total Working Hr',
                data: jsonpending
            }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                    columnWidth: '35%',
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

        var chart = new ApexCharts(document.querySelector("#chartfig5"), Fig5);
        chart.render();


    </script>

</asp:Content>


