function validateInput1()
{
    if ($("#txbWarehouses").val() > 50 || $("#txbFactories").val() > 50)
    {
        ShowErrorMsg("Upper limit for factories and warehouses is 50", "Okay");
        return false;
    }
    if ($("#txbWarehouses").val() < 2 || $("#txbFactories").val() < 2)
    {
        ShowErrorMsg("Lower limit for factories and warehouses is 2", "Okay");
        return false;
    }
}
function validateInput2()
{
    var costsValues = getInput2Values(".costs");
    var costs = costsValues.join(", ");
    $("#hdnCost").val(costs);

    var sumFactory = 0;
    var sumWarehouse = 0;

    var factoriesValues = getInput2Values(".factories");

    for (var i = 0; i < factoriesValues.length; i++) {
        sumFactory += factoriesValues[i] << 0;
    }
    
    
    var factories = factoriesValues.join(", ");
    $("#hdnFactories").val(factories);


    var warehousesValues = getInput2Values(".warehouses");

    for (var i = 0; i < warehousesValues.length; i++) {
        sumWarehouse += warehousesValues[i] << 0;
    }
    

    if (sumFactory == sumWarehouse)
    {
        //This is a balanced transportation problem;
    }
    else
    {
        ShowErrorMsg("You cannot have total warehouse demand different from total factory supply!", "Okay");
        return false;
    }
    var warehouses = warehousesValues.join(", ");
    $("#hdnWarehouses").val(warehouses);
     
}
function getInput2Values(cssClass)
{
    var values = [];
    $("input" + cssClass + "").each(function () {
        values.push($(this).val()); // this is the value of each textbox 
    })
    return values;
}

$(document).ready(function ()
{
    AddDigitCheck(".txbDigits");
    
});


function MakeMatrixEditable()
{
    $(".costs").html("<input  type='text' value='0' class='editable costs' >");
    $(".warehouses").html("<input  type='text' value='0' class='editable warehouses' >");
    $(".factories").html("<input  type='text' value='0' class='editable factories' >");

    AddDigitCheck(".editable");  

}

function AddDigitCheck(selector)
{
    //called when key is pressed in textbox
    $(selector).keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $(".errorDigits").html("Positive Digits Only").show().fadeOut("slow");
            return false;
        }
    });

    //called when leaving a textbox
    $(selector).change(function (e) {
        //if textbox is empty then put a zero "0"  so we don't get errors        
        
        if ($(this).val() == "") {            
            $(this).val("0");
        }
    });

}

function ShowErrorMsg(msg, buttonText)
{
    swal({ title: "Error!", text: msg, type: "error", confirmButtonText: buttonText});
}