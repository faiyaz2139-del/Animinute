<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Business_Owner_Dashboard.aspx.cs" Inherits="BusinessOwnerDashboard2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">





    <div class="panel panel-default">
        <div class="panel-body">
            <%--firoz--%>
            <%--<div class="row" style="width: 62%; margin-left: 70%;">
                <div class="col-md-12">

                    <div class="col-sm-2">
                        
                    </div>
                    <div class="col-sm-2">
                        
                    </div>
                    <div class="col-sm-2">
                        
                    </div>

                </div>
            </div>--%>


            <%--firoz--%>


            <div class="row">
                <div class="col-md-2"></div>
                <div class="col-md-4">
                    <div class="form-group">
                        <label>Business Name</label>
                        <asp:DropDownList ID="ddlBussName" runat="server" CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBussName_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                </div>
                <div class="col-md-4">
                    <div class="form-group" style="display:none;">
                        <asp:RadioButton ID="radioblue" runat="server" AutoPostBack="true" Text="V-Graph " GroupName="my" OnCheckedChanged="radioblue_CheckedChanged" />
                        <asp:RadioButton ID="radiored" runat="server" AutoPostBack="true" Text="H-Graph " GroupName="my" OnCheckedChanged="radiored_CheckedChanged" />
                        <asp:RadioButton ID="radioLine" runat="server" AutoPostBack="true" Text="L-Graph" GroupName="my" OnCheckedChanged="radioLine_CheckedChanged" />
                    </div>                    
                    <div class="form-group">
                        <label>Select Graph Type</label>
                        <asp:DropDownList ID="DropDownList_GraphName" runat="server" CssClass="form-control js-example-basic-single" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="DropDownList_GraphName_SelectedIndexChanged">
                            <%--<asp:ListItem Value="0" Text="Please Select"></asp:ListItem>--%>
                            <asp:ListItem Value="1" Text="Bar Graph - Vertical" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="2" Text="Bar Graph - Horizontal"></asp:ListItem>
                            <asp:ListItem Value="3" Text="Line Graph"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-md-2"></div>
            </div>
        </div>
    </div>

    <div class="row" id="VarticalGraph" runat="server" visible="true">


        <div class="col-md-6">
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">

                    <div class="row">
                        <div class="col-md-11" style="font-weight:bold;text-align:center;">
                            No of Hours Worked By User - Weekly
                        </div>
                        <div class="col-md-1" style="visibility:hidden;">
                            <asp:DropDownList ID="DropDownList1" runat="server" CssClass="form-control" Height="30px">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
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
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                   
                    <div class="row">
                        <div style="font-weight:bold;text-align:center;">
                           No of Hours Worked By User - Monthly
                        </div>
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
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-2">
                        </div>
                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlfig4" runat="server" CssClass="form-control" Height="30px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlfig4_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                       </div>
                       <div class="col-md-6" style="font-weight:bold;text-align:center;margin-top:6px;">
                            No of Hours Worked By User - Quarterly
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


            <div class="col-md-6" >
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                   
                    <div class="row">
                        <div class="col-md-1" style="visibility:hidden;">
                            <asp:DropDownList ID="DropDownList6" runat="server" CssClass="form-control" Height="30px">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-11" style="font-weight:bold;text-align:center;margin-top:6px;">
                             No of Hours Worked By User - Yearly
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig5">

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
                        <div style="font-weight:bold;text-align:center;">
                            No. of User by Business                            
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig1">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>

        </div>
    

    <script>
        var colors = [
            '#008ffb'
        ]
    </script>
    <asp:HiddenField ID="fig1Series" runat="server" />
     <asp:HiddenField ID="figcategories" runat="server" />
    <script>
        var litfig1series = $("#<%= fig1Series.ClientID %>").val();
        var hfdcategories = $("#<%= figcategories.ClientID %>").val();
        var jsoncategories = JSON.parse(hfdcategories);
        var jsonseries = JSON.parse(litfig1series);
        var options = {
            series: [{
                data: jsonseries,
                lable: jsoncategories
            }],
            chart: {
                height: 300,
                type: 'bar',

            },
            colors: colors,
            plotOptions: {
                bar: {
                    columnWidth: '10%',
                    distributed: true,
                    endingShape: 'rounded'
                }
            },
            dataLabels: {
                enabled: false
            },
            legend: {
                show: false
            },
            yaxis: {
                title: {
                    text: ''
                }
            },
            xaxis: {
                categories: jsoncategories,
                labels: {
                    style: {
                        colors: '',
                        fontSize: '12px'
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartfig1"), options);
        chart.render();


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
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
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
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
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


        //var Fig3 = {
        //    series: [
        //        {
        //            name: 'No. of User',
        //            data: jsonSubmitted
        //        },
        //        {
        //            name: 'Total Working Hr',
        //            data: jsonpending
        //        }],
        //    chart: {
        //        type: 'bar',
        //        height: 300
        //    },

        //    plotOptions: {
        //        bar: {

        //            horizontal: true,
        //            //columnWidth: '55%',
        //            rowWidth: '55%',
        //            endingShape: 'rounded'
        //        },
        //    },
        //    dataLabels: {
        //        enabled: false
        //    },
        //    stroke: {
        //        show: true,
        //        width: 2,
        //        colors: ['transparent']
        //    },
        //    xaxis: {
        //        categories: jsonCategories,
        //    },
        //    yaxis: {
        //        title: {
        //            text: ''
        //        }
        //    },
        //    fill: {
        //        opacity: 1
        //    },
        //    tooltip: {
        //        y: {
        //            formatter: function (val) {
        //                return val
        //            }
        //        }
        //    }
        //};

        var chart = new ApexCharts(document.querySelector("#chartfig3"), Fig3);
        chart.render();



        //var Fig3 = {
        //    series: [

        //        {
        //            name: 'No. of User',
        //            data: jsonSubmitted
        //        },
        //        {
        //            name: 'Total Working Hr',
        //            data: jsonpending
        //        }],


        //    chart: {
        //        //type: 'bar',
        //        //height: 300

        //        legend: {
        //            layout: 'horizontal',
        //            align: 'center',
        //            verticalAlign: 'bottom'
        //        }
        //    },

        //    plotOptions: {
        //        series: {
        //            label: {
        //                connectorAllowed: false
        //            },
        //            pointStart: 2010
        //        }

        //        //bar: {

        //        //    horizontal: true,
        //        //    //columnWidth: '55%',
        //        //    rowWidth:'55%',
        //        //    endingShape: 'rounded'
        //        //},
        //    },
        //    dataLabels: {
        //        enabled: false
        //    },
        //    stroke: {
        //        show: true,
        //        width: 2,
        //        colors: ['red']
        //    },
        //    xaxis: {
        //        categories: {
        //            jsonCategories,
        //        }
        //    },
        //    //xaxis: {
        //    //    accessibility: {
        //    //        rangeDescription: 'Range: 2010 to 2020'
        //    //    }
        //    //},

        //    //yaxis: {
        //    //    title: {
        //    //        text: 'Number of Employees in IT Department'
        //    //    }
        //    //},

        //    yaxis: {
        //        title: {
        //            text: ''
        //        }
        //    },
        //    fill: {
        //        opacity: 1
        //    },
        //    //tooltip: {
        //    //    y: {
        //    //        formatter: function (val) {
        //    //            return val
        //    //        }
        //    //    }
        //    //}
        //};

        //var chart = new ApexCharts(document.querySelector("#chartfig3"), Fig3);
        //chart.render();


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
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
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
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
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

        var chart = new ApexCharts(document.querySelector("#chartfig5"), Fig5);
        chart.render();


    </script>


    <%----------------------Horizontal graph----------------%>

  <div class="row" id="HorizontalGraph" runat="server" visible="false">


      <div class="col-md-6">
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">

                    <div class="row">
                        <div style="font-weight:bold;text-align:center;">
                            No of Hours Worked By User - Weekly
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig20">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>


          <div class="col-md-6">
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                   
                    <div class="row">
                        <div style="font-weight:bold;text-align:center;">
                           No of Hours Worked By User - Monthly 
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig30">

                            </div>
                        </div>
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
                            <asp:DropDownList ID="DropDownList3" runat="server" CssClass="form-control" Height="30px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlfig4_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                       </div>
                       <div class="col-md-6" style="font-weight:bold;text-align:center;margin-top:6px;">
                            No of Hours Worked By User - Quarterly
                       </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig40">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

            <div class="col-md-6" >
            <div class="panel panel-default" style="height:400px;">
                <div class="panel-body">
                   
                    <div class="row">
                        <div class="col-md-1" style="visibility:hidden;">
                            <asp:DropDownList ID="DropDownList7" runat="server" CssClass="form-control" Height="30px">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-11" style="font-weight:bold;text-align:center;margin-top:6px;">
                             No of Hours Worked By User - Yearly
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig50">

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
                        <div style="font-weight:bold;text-align:center;">
                            No. of User by Business                            
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig10">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>

        </div>
    

    <script>
        var colors = [
            '#008ffb'
        ]
    </script>
    <asp:HiddenField ID="HiddenField1" runat="server" />
     <asp:HiddenField ID="HiddenField2" runat="server" />
    <script>
        var litfig1series = $("#<%= fig1Series.ClientID %>").val();
        var hfdcategories = $("#<%= figcategories.ClientID %>").val();
        var jsoncategories = JSON.parse(hfdcategories);
        var jsonseries = JSON.parse(litfig1series);
        var options = {
            series: [{
                data: jsonseries,
                lable: jsoncategories
            }],
            chart: {
                height: 300,
                type: 'bar',

            },
            colors: colors,
            plotOptions: {
                bar: {

                    horizontal: true,
                    rowWidth: '10%',
                    endingShape: 'rounded'

                    //columnWidth: '10%',
                    //distributed: true,
                    //endingShape: 'rounded'
                }
            },
            dataLabels: {
                enabled: false
            },
            legend: {
                show: false
            },
            yaxis: {
                title: {
                    text: ''
                }
            },
            xaxis: {
                categories: jsoncategories,
                labels: {
                    style: {
                        colors: '',
                        fontSize: '12px'
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartfig10"), options);
        chart.render();


    </script>

    <%-----------------------------------------------------------------------------------------------%>

    <asp:HiddenField ID="HiddenField3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField4" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField5" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig2.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig2.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig2.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig2 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: true,
                    rowWidth: '55%',
                    endingShape: 'rounded'
                    //horizontal: false,
                    //columnWidth: '55%',
                    //endingShape: 'rounded'
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

        var chart = new ApexCharts(document.querySelector("#chartfig20"), Fig2);
        chart.render();


    </script>

     <%-----------------------------------------------------------------------------------------------%>

     <asp:HiddenField ID="HiddenField6" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField7" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField8" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig3.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig3.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig3.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        //var Fig3 = {
        //    series: [
        //        {
        //            name: 'No. of User',
        //        data: jsonSubmitted
        //    },
        //        {
        //        name: 'Total Working Hr',
        //        data: jsonpending
        //    }],
        //    chart: {
        //        type: 'bar',
        //        height: 300
        //    },
        //    plotOptions: {
        //        bar: {
        //            horizontal: false,
        //            columnWidth: '55%',
        //            endingShape: 'rounded'
        //        },
        //    },
        //    dataLabels: {
        //        enabled: false
        //    },
        //    stroke: {
        //        show: true,
        //        width: 2,
        //        colors: ['transparent']
        //    },
        //    xaxis: {
        //        categories: jsonCategories,
        //    },
        //    yaxis: {
        //        title: {
        //            text: ''
        //        }
        //    },
        //    fill: {
        //        opacity: 1
        //    },
        //    tooltip: {
        //        y: {
        //            formatter: function (val) {
        //                return val
        //            }
        //        }
        //    }
        //};


        var Fig3 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                type: 'bar',
                height: 300
            },

            plotOptions: {
                bar: {

                    horizontal: true,
                    //columnWidth: '55%',
                    rowWidth: '55%',
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

        var chart = new ApexCharts(document.querySelector("#chartfig30"), Fig3);
        chart.render();



        //var Fig3 = {
        //    series: [

        //        {
        //            name: 'No. of User',
        //            data: jsonSubmitted
        //        },
        //        {
        //            name: 'Total Working Hr',
        //            data: jsonpending
        //        }],


        //    chart: {
        //        //type: 'bar',
        //        //height: 300

        //        legend: {
        //            layout: 'horizontal',
        //            align: 'center',
        //            verticalAlign: 'bottom'
        //        }
        //    },

        //    plotOptions: {
        //        series: {
        //            label: {
        //                connectorAllowed: false
        //            },
        //            pointStart: 2010
        //        }

        //        //bar: {

        //        //    horizontal: true,
        //        //    //columnWidth: '55%',
        //        //    rowWidth:'55%',
        //        //    endingShape: 'rounded'
        //        //},
        //    },
        //    dataLabels: {
        //        enabled: false
        //    },
        //    stroke: {
        //        show: true,
        //        width: 2,
        //        colors: ['red']
        //    },
        //    xaxis: {
        //        categories: {
        //            jsonCategories,
        //        }
        //    },
        //    //xaxis: {
        //    //    accessibility: {
        //    //        rangeDescription: 'Range: 2010 to 2020'
        //    //    }
        //    //},

        //    //yaxis: {
        //    //    title: {
        //    //        text: 'Number of Employees in IT Department'
        //    //    }
        //    //},

        //    yaxis: {
        //        title: {
        //            text: ''
        //        }
        //    },
        //    fill: {
        //        opacity: 1
        //    },
        //    //tooltip: {
        //    //    y: {
        //    //        formatter: function (val) {
        //    //            return val
        //    //        }
        //    //    }
        //    //}
        //};

        //var chart = new ApexCharts(document.querySelector("#chartfig3"), Fig3);
        //chart.render();


    </script>


      <%-----------------------------------------------------------------------------------------------%>
     <asp:HiddenField ID="HiddenField9" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField10" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField11" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig4.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig4.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig4.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig4 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: true,
                    rowWidth: '55%',
                    endingShape: 'rounded'
                    //horizontal: false,
                    //columnWidth: '55%',
                    //endingShape: 'rounded'
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

        var chart = new ApexCharts(document.querySelector("#chartfig40"), Fig4);
        chart.render();


    </script>


     <%-----------------------------------------------------------------------------------------------%>

     <asp:HiddenField ID="HiddenField12" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField13" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField14" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig5.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig5.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig5.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig5 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                type: 'bar',
                height: 300
            },
            plotOptions: {
                bar: {
                    horizontal: true,
                    rowWidth: '55%',
                    endingShape: 'rounded'
                    //horizontal: false,
                    //columnWidth: '55%',
                    //endingShape: 'rounded'
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

        var chart = new ApexCharts(document.querySelector("#chartfig50"), Fig5);
        chart.render();


    </script>

    <%------------------Line-----------------%>

    

  <div class="row" id="lineGraph" runat="server" visible="false">

      <div class="col-md-6">
            <div class="panel panel-default" style="height:500px;">
                <div class="panel-body">

                    <div class="row">
                        <div style="font-weight:bold;text-align:center;">
                            No of Hours Worked By User - Weekly
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig02">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>

          <div class="col-md-6">
            <div class="panel panel-default" style="height:500px;">
                <div class="panel-body">
                   
                    <div class="row">
                        <div style="font-weight:bold;text-align:center;">
                           No of Hours Worked By User - Monthly 
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig03">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>  


      <div class="col-md-6">
            <div class="panel panel-default" style="height:500px;">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-2">
                        </div>
                        <div class="col-md-3">
                            <asp:DropDownList ID="DropDownList5" runat="server" CssClass="form-control" Height="30px" AppendDataBoundItems="true" AutoPostBack="true" OnSelectedIndexChanged="ddlfig4_SelectedIndexChanged">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                       </div>
                       <div class="col-md-6" style="font-weight:bold;text-align:center;margin-top:6px;">
                            No of Hours Worked By User - Quarterly
                       </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig04">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

            <div class="col-md-6" >
            <div class="panel panel-default" style="height:500px;">
                <div class="panel-body">
                   
                    <div class="row">
                        <div class="col-md-1" style="visibility:hidden;">
                            <asp:DropDownList ID="DropDownList4" runat="server" CssClass="form-control" Height="30px">
                                <asp:ListItem Value="" Text="Select Year"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-11" style="font-weight:bold;text-align:center;margin-top:6px;">
                             No of Hours Worked By User - Yearly
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig05">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

      
          


         

       
          <div class="col-md-6">
            <div class="panel panel-default" style="height:500px">
                <div class="panel-body">
                   
                    <div class="row">
                        <div style="font-weight:bold;text-align:center;">
                            No. of User by Business                            
                        </div>
                    </div>
                   
                    <div class="row">
                        <div class="col-md-12">
                            <div id="chartfig01">

                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>

        </div>
    

    <script>
        var colors = [
            '#008ffb'
        ]
    </script>
    <asp:HiddenField ID="HiddenField15" runat="server" />
     <asp:HiddenField ID="HiddenField16" runat="server" />
    <script>
        var litfig1series = $("#<%= fig1Series.ClientID %>").val();
        var hfdcategories = $("#<%= figcategories.ClientID %>").val();
        var jsoncategories = JSON.parse(hfdcategories);
        var jsonseries = JSON.parse(litfig1series);
        var options = {
            series: [{
                data: jsonseries,
                lable: jsoncategories
            }],
            chart: {
                //height: 300,
                //type: 'bar',

                legend: {
                    layout: 'horizontal',
                    align: 'center',
                    verticalAlign: 'bottom'
                }

            },
            colors: colors,
            plotOptions: {
                //bar: {

                //    horizontal: true,
                //    rowWidth: '10%',
                //    endingShape: 'rounded'

                //    //columnWidth: '10%',
                //    //distributed: true,
                //    //endingShape: 'rounded'
                //}
            },
            dataLabels: {
                enabled: false
            },
            legend: {
                show: false
            },
            yaxis: {
                title: {
                    text: ''
                }
            },
            xaxis: {
                categories: jsoncategories,
                labels: {
                    style: {
                        colors: '',
                        fontSize: '12px'
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartfig01"), options);
        chart.render();


    </script>

    <%-----------------------------------------------------------------------------------------------%>

    <asp:HiddenField ID="HiddenField17" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField18" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField19" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig2.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig2.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig2.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig2 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                //type: 'bar',
                //height: 300
                legend: {
                    layout: 'horizontal',
                    align: 'center',
                    verticalAlign: 'bottom'
                }
            },
            plotOptions: {
                bar: {
                    horizontal: true,
                    rowWidth: '55%',
                    endingShape: 'rounded'
                    //horizontal: false,
                    //columnWidth: '55%',
                    //endingShape: 'rounded'
                },
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['red']
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

        var chart = new ApexCharts(document.querySelector("#chartfig02"), Fig2);
        chart.render();


    </script>

     <%-----------------------------------------------------------------------------------------------%>

     <asp:HiddenField ID="HiddenField20" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField21" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField22" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig3.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig3.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig3.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        //var Fig3 = {
        //    series: [
        //        {
        //            name: 'No. of User',
        //        data: jsonSubmitted
        //    },
        //        {
        //        name: 'Total Working Hr',
        //        data: jsonpending
        //    }],
        //    chart: {
        //        type: 'bar',
        //        height: 300
        //    },
        //    plotOptions: {
        //        bar: {
        //            horizontal: false,
        //            columnWidth: '55%',
        //            endingShape: 'rounded'
        //        },
        //    },
        //    dataLabels: {
        //        enabled: false
        //    },
        //    stroke: {
        //        show: true,
        //        width: 2,
        //        colors: ['transparent']
        //    },
        //    xaxis: {
        //        categories: jsonCategories,
        //    },
        //    yaxis: {
        //        title: {
        //            text: ''
        //        }
        //    },
        //    fill: {
        //        opacity: 1
        //    },
        //    tooltip: {
        //        y: {
        //            formatter: function (val) {
        //                return val
        //            }
        //        }
        //    }
        //};

        /*-----------------*/

        //var Fig3 = {
        //    series: [
        //        {
        //            name: 'No. of User',
        //            data: jsonSubmitted
        //        },
        //        {
        //            name: 'Total Working Hr',
        //            data: jsonpending
        //        }],
        //    chart: {
        //        type: 'bar',
        //        height: 300
        //    },

        //    plotOptions: {
        //        bar: {

        //            horizontal: true,
        //            //columnWidth: '55%',
        //            rowWidth: '55%',
        //            endingShape: 'rounded'
        //        },
        //    },
        //    dataLabels: {
        //        enabled: false
        //    },
        //    stroke: {
        //        show: true,
        //        width: 2,
        //        colors: ['transparent']
        //    },
        //    xaxis: {
        //        categories: jsonCategories,
        //    },
        //    yaxis: {
        //        title: {
        //            text: ''
        //        }
        //    },
        //    fill: {
        //        opacity: 1
        //    },
        //    tooltip: {
        //        y: {
        //            formatter: function (val) {
        //                return val
        //            }
        //        }
        //    }
        //};

        //var chart = new ApexCharts(document.querySelector("#chartfig03"), Fig3);
        //chart.render();



        var Fig3 = {
            series: [

                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],


            chart: {
                //type: 'bar',
                //height: 300

                legend: {
                    layout: 'horizontal',
                    align: 'center',
                    verticalAlign: 'bottom'
                }
            },

            plotOptions: {
                series: {
                    label: {
                        connectorAllowed: false
                    },
                    pointStart: 2010
                }

                //bar: {

                //    horizontal: true,
                //    //columnWidth: '55%',
                //    rowWidth:'55%',
                //    endingShape: 'rounded'
                //},
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['red']
            },
            xaxis: {
                categories: {
                    jsonCategories,
                }
            },
            //xaxis: {
            //    accessibility: {
            //        rangeDescription: 'Range: 2010 to 2020'
            //    }
            //},

            //yaxis: {
            //    title: {
            //        text: 'Number of Employees in IT Department'
            //    }
            //},

            yaxis: {
                title: {
                    text: ''
                }
            },
            fill: {
                opacity: 1
            },
            //tooltip: {
            //    y: {
            //        formatter: function (val) {
            //            return val
            //        }
            //    }
            //}
        };

        var chart = new ApexCharts(document.querySelector("#chartfig03"), Fig3);
        chart.render();


    </script>


      <%-----------------------------------------------------------------------------------------------%>
     <asp:HiddenField ID="HiddenField23" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField24" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField25" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig4.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig4.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig4.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig4 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                //type: 'bar',
                //height: 300

                legend: {
                    layout: 'horizontal',
                    align: 'center',
                    verticalAlign: 'bottom'
                }
            },
            plotOptions: {
                //bar: {
                //    horizontal: true,
                //    rowWidth: '55%',
                //    endingShape: 'rounded'
                //    //horizontal: false,
                //    //columnWidth: '55%',
                //    //endingShape: 'rounded'
                //},
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['green']
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

        var chart = new ApexCharts(document.querySelector("#chartfig04"), Fig4);
        chart.render();


    </script>


     <%-----------------------------------------------------------------------------------------------%>

     <asp:HiddenField ID="HiddenField26" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField27" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="HiddenField28" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig2categories = $("#<%= hfdCatagoriesfig5.ClientID %>").val();
        var fig2Submitted = $("#<%= hfdsubmitedfig5.ClientID %>").val();
        var fig2pending = $("#<%= hfdpendingfig5.ClientID %>").val();


        var jsonCategories = JSON.parse(fig2categories);
        var jsonSubmitted = JSON.parse(fig2Submitted);
        var jsonpending = JSON.parse(fig2pending);


        var Fig5 = {
            series: [
                {
                    name: 'No. of User',
                    data: jsonSubmitted
                },
                {
                    name: 'Total Working Hr',
                    data: jsonpending
                }],
            chart: {
                //type: 'bar',
                //height: 300

                legend: {
                    layout: 'horizontal',
                    align: 'center',
                    verticalAlign: 'bottom'
                }
            },
            plotOptions: {
                //bar: {
                //    horizontal: true,
                //    rowWidth: '55%',
                //    endingShape: 'rounded'
                //    //horizontal: false,
                //    //columnWidth: '55%',
                //    //endingShape: 'rounded'
                //},
            },
            dataLabels: {
                enabled: false
            },
            stroke: {
                show: true,
                width: 2,
                colors: ['red']
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

        var chart = new ApexCharts(document.querySelector("#chartfig05"), Fig5);
        chart.render();


    </script>


</asp:Content>

