<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Cancel.aspx.cs" Inherits="payment_Cancel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Payment cancelled</title>
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
        .error-notification {
            background-color: #ddaeae;
            color: black;
            padding: 20px;
            font-size: 14px;
            border-radius:10px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="error-notification">
                <h4>Payment cancelled</h4>
                <p>We couldn't process the payment request. Please try again.</p>
            </div>
        </div>
    </form>
</body>
</html>
