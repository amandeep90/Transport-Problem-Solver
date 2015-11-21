function validateInput1()
{
    ($("#txbWarehouses").val() > 50 || ("#txbFactories").val() > 50)
    {
        alert("Maximum upper limit for factories and warehouses is 50");
        return false;
    }
}
function validateInput2()
{
    var values = getInput2Values(".costs");
    alert(values);
    return false;
}
function getInput2Values(cssClass)
{
    var values = [];
    $("input" + cssClass).each(function () {
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
    
    txtBox = "<input  type='text' value='0' class='txbDigits' >"
    $(".editable").html(txtBox);
    AddDigitCheck(".txbDigits");

}

function AddDigitCheck(selector)
{
    //called when key is pressed in textbox
    $(selector).keypress(function (e) {
        //if the letter is not digit then display error and don't type anything
        if (e.which != 8 && e.which != 0 && (e.which < 48 || e.which > 57)) {
            //display error message
            $(".errorDigits").html("Digits Only").show().fadeOut("slow");
            return false;
        }
    });

}