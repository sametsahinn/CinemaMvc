$(document).ready(function () {
    $("#slider-1").slider({
        animate: true,
        value: 1,
        min: 0,
        max: 10,
        step: 1,
        slide: function (event, ui) {
            update(1, ui.value); //changed
        }
    });

    $("#slider-2").slider({
        animate: true,
        value: 0,
        min: 0,
        max: 10,
        step: 1,
        slide: function (event, ui) {
            update(2, ui.value); //changed
        }
    });

    //Added, set initial value.
    $("#tam").val(0);
    $("#öğrenci").val(0);
    $("#tam-label").text(0);
    $("#öğrenci-label").text(0);

    update();
});

//changed. now with parameter
function update(slider, val) {
    //changed. Now, directly take value from ui.value. if not set (initial, will use current value.)
    var $amount = slider == 1 ? val : $("#tam").val();
    var $duration = slider == 2 ? val : $("#öğrenci").val();

    var $TPRICE = $("#tam #txtPRICE").val();
    var $OPRICE = $("#öğrenci #txtPRICE").val();
    
    /* commented
    $amount = $( "#slider" ).slider( "value" );
    $duration = $( "#slider2" ).slider( "value" );
     */
    $total = 0;
    if ($duration == 0) {
        $total = ($amount * parseInt($TPRICE)) + " ₺";
    }
    else if ($amount == 0) {
        $total = ($duration * parseInt($OPRICE)) + " ₺";
    }
    else {
        var $amounttotal = ($amount * parseInt($TPRICE));
        var $durationtotal = ($duration * parseInt($OPRICE));
        $total = 0;
        var $totalsum = ($amounttotal + $durationtotal);
        $total = $totalsum + " ₺";
    }

    //$total = "₺ " + ($amount * 19);
    $("#tam").val($amount);
    $("#tam-label").text($amount);
    $("#öğrenci").val($duration);
    $("#öğrenci-label").text($duration);
    $("#total").val($total);
    $("#total-label").text($total);

    var $tamPiece = $("#tam").val();
    var $öğrenciPiece = $("#öğrenci").val();

    $("#totalPiece").val(parseInt($tamPiece) + parseInt($öğrenciPiece));
    $("#totalPiece").text(parseInt($tamPiece) + parseInt($öğrenciPiece));

    $(".totalPiece").val(parseInt($tamPiece) + parseInt($öğrenciPiece));
    $(".totalPiece").text(parseInt($tamPiece) + parseInt($öğrenciPiece));

    $(".totalTicket").val($total);
    $(".totalTicket").text($total);

    //$("#totalBtn").val($total);
    //$("#totalBtn").text($total);
    //$(".totalValue").val($total);
    //$(".totalValue").text($total);

    $('#slider-1 a').html('<label><span class="glyphicon"><i class="fa fa-chevron-left" aria-hidden="true"></i></span> ' + $amount + ' <span class="glyphicon"><i class="fa fa-chevron-right" aria-hidden="true"></i></span></label>');
    $('#slider-2 a').html('<label><span class="glyphicon"><i class="fa fa-chevron-left" aria-hidden="true"></i></span> ' + $duration + ' <span class="glyphicon"><i class="fa fa-chevron-right" aria-hidden="true"></i></span></label>');
}