<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>
<link href="css/sweetalert.css" rel="stylesheet" />
<script src="JavaScript/sweetalert.min.js"></script>
<script src="JavaScript/jquery-1.11.3.min.js"></script>
<script src="JavaScript/default.js"></script>
<link href="css/htmlTableStyle1.css" rel="stylesheet" />
<link href="css/default.css" rel="stylesheet" />
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div id="mainDiv" runat="server" class="mainDiv">
            <div id="statusDiv" class="statusDiv">
                <span id="errorDigits1" class="errorDigits"></span>

            </div>

            <div id="inputDiv1" runat="server" class="">
                <label class="label1" for="txbWarehouses">Warehouses</label>
                <asp:TextBox ID="txbWarehouses" runat="server" CssClass="txbDigits" Text="3"></asp:TextBox><br />

                <label class="label1" for="txbFactories">Factories:</label>
                <asp:TextBox ID="txbFactories" runat="server" CssClass="txbDigits" Text="3"></asp:TextBox><br />

                <asp:Button runat="server" ID="btnInput1" CssClass="buttons" Visible="false" Text="Submit" OnClick="btnInput1_Click" OnClientClick="return validateInput1();" />

            </div>
            <div id="outputDiv1" runat="server" class="matrixStyle1">
            </div>
            <asp:Button CssClass="buttons" runat="server" ID="btnInput2" Visible="false" Text="Submit" OnClick="btnInput2_Click" OnClientClick="return validateInput2();" />
            <asp:Button CssClass="buttons" runat="server" ID="btnInput3" Visible="false" Text="Solve" OnClick="btnInput3_Click" OnClientClick="return validateInput3();" />
            <br />
            <asp:Button CssClass="buttons" runat="server" ID="btnReset" Text="Reset" OnClick="btnReset_Click" />
            <asp:HiddenField ID="hdnCost" runat="server" Value="" />
            <asp:HiddenField ID="hdnFactories" runat="server" Value="" />
            <asp:HiddenField ID="hdnWarehouses" runat="server" Value="" />
        </div>
    </form>
</body>
</html>
