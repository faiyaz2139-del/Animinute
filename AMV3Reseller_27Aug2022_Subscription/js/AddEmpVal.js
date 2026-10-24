function addPerVal() {

    var gridView = document.getElementById("Gv_RolePermission");

    if ($("#txtFirstName").val().length == 0) {
        $("#txtFirstName").focus();
        swal('Error!', 'Please Enter First Name', 'error');
    }
    else if ($("#txtLastName").val().length == 0) {
        $("#txtLastName").focus();
        swal('Error!', 'Please Enter Last Name', 'error');
    }
    else if ($("#txtHiredate").val().length == 0) {
        $("#txtHiredate").focus();
        swal('Error!', 'Please Enter Hire Date', 'error');
    }

    else if ($("#ddlBusiness").val().length == 0) {
        $("#ddlBusiness").focus();
        swal('Error!', 'Please Select Business', 'error');
    }

    else if ($("#ddlRole").val().length == 0) {
        $("#ddlRole").focus();
        swal('Error!', 'Please Select Role', 'error');
    }
	
	else if (!gridView) {
        swal('Error!', 'Please Click Add Button', 'error');
        return false;
    }
	
    else if (!gridView) {
        if ($("#ddlRole").val().length > 0 && $("#ddlRole").val().length > 0) {
            swal('Error!', 'Please Click on Add Button', 'error');
        }
        else {
            swal('Error!', 'Please Select Assign Business And Assign Role And Then Click on Add Button', 'error');
        }
        return false;
    }
    
    //else if ($("#txtMainEmail").val().length == 0) {
    //    $("#txtMainEmail").focus();
    //    swal('Error!', 'Please Enter Email Id', 'error');
    //}
    else if ($("#txtMainPhone").val().length == 0) {
        $("#txtMainPhone").focus();
        swal('Error!', 'Please Enter Contact Phone', 'error');
    }
    //else if ($("#txtSearchPostalCode").val().length == 0) {
    //    $("#txtSearchPostalCode").focus();
    //    swal('Error!', 'Please Search Postal Code', 'error');
    //}
    //else if ($("#txtStreetNo").val().length == 0) {
    //    $("#txtStreetNo").focus();
    //    swal('Error!', 'Please Click On Search Button', 'error');
    //}
    //else if ($("#txtStreetName").val().length == 0) {
    //    $("#txtStreetName").focus();
    //    swal('Error!', 'Please Enter Address Line 1', 'error');
    //}
    //else if ($("#txtCity").val().length == 0) {
    //    $("#txtCity").focus();
    //    swal('Error!', 'Please Enter City', 'error');
    //}
    //else if ($("#txtProvince").val().length == 0) {
    //    $("#txtProvince").focus();
    //    swal('Error!', 'Please Enter Province', 'error');
    //}
    //else if ($("#txtPostalCode").val().length == 0) {
    //    $("#txtPostalCode").focus();
    //    swal('Error!', 'Please Enter Postal Code', 'error');
    //}
    else {
        //document.cookie = "tabid=#permission";
        //$("#permission").attr("class", "tab-pane fade in active show");
        //$("#home").attr("class", "tab-pane fade in");
        //$("#ChangeBuss").attr("class", "tab-pane fade in");
        //$("#LichangeBuss").attr("class", "");
        //$("#Lirolemid").attr("class", "active");
        //$("#Lihomemid").attr("class", "");

        document.cookie = "tabid=#project";
        $("#project").attr("class", "tab-pane fade in active show");
        $("#home").attr("class", "tab-pane fade in");
        $("#ChangeBuss").attr("class", "tab-pane fade in");
        //$("#LichangeBuss").attr("class", "");
        $("#LiProject").attr("class", "active");
        $("#Lihomemid").attr("class", "");
    }
    return false;
 }
//function addRoleVal() {
//    document.cookie = "tabid=#project";
//    $("#project").attr("class", "tab-pane fade in active show");
//    //$("#permission").attr("class", "tab-pane fade in");
//    $("#home").attr("class", "tab-pane fade in");
//    $("#ratedetils").attr("class", "tab-pane fade in");
//    $("#LiRateDetails").attr("class", "");
//    $("#LiProject").attr("class", "active");
//    $("#Lihomemid").attr("class", "");
//    $("#Lirolemid").attr("class", "");
//    return false;
//}
function addProjectVal() {
    document.cookie = "tabid=#ratedetils";
    //$("#permission").attr("class", "tab-pane fade in");
    $("#project").attr("class", "tab-pane fade in");
    $("#home").attr("class", "tab-pane fade in");
    $("#ratedetils").attr("class", "tab-pane fade in active show");
    $("#LiRateDetails").attr("class", "active");
    $("#LiProject").attr("class", "");
    //$("#Lirolemid").attr("class", "");
    $("#Lihomemid").attr("class", "");
    return false;
}

function addProjectValPrev() {
    debugger;
    document.cookie = "tabid=#home";
    //$("#permission").attr("class", "tab-pane fade in");
    $("#project").attr("class", "tab-pane fade in");
    $("#ratedetils").attr("class", "tab-pane fade in");
    $("#home").attr("class", "tab-pane fade in active show");
    $("#LiRateDetails").attr("class", "");
    $("#LiProject").attr("class", "");
    //$("#Lirolemid").attr("class", "");
    $("#Lihomemid").attr("class", "active");
    return false;
}

function addDetailsValPrev() {
    debugger;
    document.cookie = "tabid=#project";
    //$("#permission").attr("class", "tab-pane fade in");
    $("#project").attr("class", "tab-pane fade in");
    $("#ratedetils").attr("class", "tab-pane fade in");
    $("#project").attr("class", "tab-pane fade in active show");
    $("#LiRateDetails").attr("class", "");
    $("#LiProject").attr("class", "active");
    //$("#Lirolemid").attr("class", "");
    $("#Lihomemid").attr("class", "");
    return false;
}