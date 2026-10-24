<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Success.aspx.cs" Inherits="payment_Success" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payment Info</title>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <%--Added By firoz--%>
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/jquery.min.js" type="text/javascript"></script>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>
    <%--end--%>

    <link rel="icon" href="Assets/assets/img/logo.jpg" type="image/x-icon" />

    <script src="Assets/assets/js/SweetAlert.js"></script>
    <script src="js/jquery.min.js"></script>
    <style>
        body {
            font-family: sans-serif;
        }

        .container {
            margin: 100px auto;
            width: 50%;

            @media (max-width: 700px) {
                .container {
                    width: 90%;
                }
            }
        }

        .success-notification {
            background-color: #ccddbe;
            color: black;
            padding: 20px;
            font-size: 14px;
            border-radius:10px;
        }

        .error-notification {
            background-color: #edacac;
            color: black;
            padding: 20px;
            font-size: 14px;
            border-radius:10px;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function DisableBackButton() {
            window.history.forward()
        }
        DisableBackButton();
        window.onload = DisableBackButton;
        window.onpageshow = function (evt) { if (evt.persisted) DisableBackButton() }
        window.onunload = function () { void (0) }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="row">
                <div class="col-md-12 col-sm-12 col-xs-12">
                    <img src="../Assets/assets/img/logo.jpg" />
                    <h5 style="font-style: italic; color: dimgray;">Record from Anywhere!</h5>
                </div>
            </div>
            <div class="success-notification" id="divSuccess" runat="server" visible="false">
                <h3>Payment done</h3>
                <p>Thank you for choosing the plan with AnyMinute.</p>
                <p>Your payment has been successfully completed. Please <a href="/login.aspx">login</a> to AnyMinute.</p>
            </div>
            <div>
                <label id="lblErrorSms" runat="server"></label>
                <label id="lblErrorSmstwo" runat="server"></label>
                <label id="lblErrorSmsthree" runat="server"></label>
            </div>
            <div class="error-notification" id="divError" runat="server" visible="false">
                <h3>Exception occured</h3>
                <p>You have paid the amount for seleted plan</p>
                <p>We encountered some issue while handling your request. Please be with us, we will resolve it soon.</p>
            </div>
            
        </div>
    </form>
</body>
</html>
