<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="ManagerDashboard.aspx.cs" Inherits="ManagerDashboard" %>

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
     <div class="row">
        <div class="col-md-4">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-6" style="font-weight:bold;">
                            Business VS No. of Employee
                            
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
        <div class="col-md-4">
             <div class="panel panel-default">
                <div class="panel-body">
                    <div style="font-weight:bold; margin-bottom:15px;" >Week wise Comparison of Working Hr. </div>
                     <div class="row">
                        <div class="col-md-6">
                           
                           <asp:DropDownList ID="ddlfig1" runat="server" Visible="true" CssClass="form-control" Height="30px" AutoPostBack="true" OnSelectedIndexChanged="ddlfig1_SelectedIndexChanged"></asp:DropDownList>
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

        <div class="col-md-4">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div style="font-weight:bold; margin-bottom:15px;" >Week wise Comparison of Working Employee </div>
                   <div class="row">
                        <div class="col-md-6">
                           
                           <asp:DropDownList ID="ddlfig3" runat="server" Visible="true" CssClass="form-control" Height="30px" AutoPostBack="true" OnSelectedIndexChanged="ddlfig3_SelectedIndexChanged"></asp:DropDownList>
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
    </div>


     <script>
         var colors = [
             '#008FFB',
             '#00E396',
             '#FEB019',
             '#FF4560',
             '#775DD0',
             '#546E7A',
             '#26a69a',
             '#D10CE8'
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
                height: 350,
                type: 'bar',

            },
            colors: colors,
            plotOptions: {
                bar: {
                    columnWidth: '15%',
                    distributed: true
                }
            },
            dataLabels: {
                enabled: true
            },
            legend: {
                show: false
            },
            yaxis: {
                title: {
                    text: 'No. of Employees'
                }
            },
            xaxis: {
                categories: jsoncategories,
                labels: {
                    style: {
                        colors: colors,
                        fontSize: '12px'
                    }
                }
            }
        };

        var chart = new ApexCharts(document.querySelector("#chartfig1"), options);
        chart.render();


    </script>




    <asp:HiddenField ID="hfdCatagoriesfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdcurrentweek" runat="server" ClientIDMode="Static"></asp:HiddenField>
    <asp:HiddenField ID="hfdpPrevweek" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>

        var fig3categories = $("#<%= hfdCatagoriesfig3.ClientID %>").val();
        var fig3Submitted = $("#<%= hfdcurrentweek.ClientID %>").val();
        var fig3pending = $("#<%= hfdpPrevweek.ClientID %>").val();


        var jsonCategories = JSON.parse(fig3categories);
        var jsonSubmitted = JSON.parse(fig3Submitted);
        var jsonpending = JSON.parse(fig3pending);


        var Fig3 = {
            series: [{
                name: 'Current Week',
                data: jsonSubmitted
            }, {
                name: 'Previous Week',
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
                    text: 'No. of Working Hr.'
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

        var chart = new ApexCharts(document.querySelector("#chartfig2"), Fig3);
        chart.render();


    </script>



    
    <asp:HiddenField ID="hfdcurrentweekfig3" runat="server" ClientIDMode="Static"></asp:HiddenField>
    
    <script>
        var fig3Submitted = $("#<%= hfdcurrentweekfig3.ClientID %>").val();




        var jsonSubmitted = JSON.parse(fig3Submitted);


        var optionsfig3 = {
            series: jsonSubmitted,
            chart: {
                height: 335,
                type: 'radialBar',
            },
            plotOptions: {
                radialBar: {
                    dataLabels: {
                        name: {
                            fontSize: '14px',
                        },

                        value: {
                            fontSize: '12px',
                            formatter: function (val) {
                                return val + ' Employees'
                            }
                        },
                        total: {
                            show: false,
                            label: '',
                            formatter: function (w) {
                                // By default this function returns the average of all series. The below is just an example to show the use of custom formatter function
                                return 88
                            }
                        }
                    }
                }
            },
            labels: ['Current Week', 'Preveious Week'],
        };

        var chart = new ApexCharts(document.querySelector("#chartfig3"), optionsfig3);
        chart.render();


    </script>

</asp:Content>

