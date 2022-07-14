var ratingValue = 0;

$(document).ready(function () {

    /* 1. Visualizing things on Hover - See next part for action on click */
    $('#stars li').mouseover(function() {
        var onStar = parseInt($(this).data('value'), 10); // The star currently mouse on

        // Now highlight all the stars that's not after the current hovered star
        $(this).parent().children('li.star').each(function (e) {
            if (e < onStar) {
                $(this).addClass('hover');
            }
            else {
                $(this).removeClass('hover');
            }
        });

    });
        $('#stars li').mouseout(function () {
        $(this).parent().children('li.star').each(function (e) {
            $(this).removeClass('hover');
        });
    });


    /* 2. Action to perform on click */
    $('#stars li').click(function () {
        if (CurMovieID == '') {
            alert("還未選影片觀看！");
            return;
        }
        var onStar = parseInt($(this).data('value'), 10); // The star currently selected
        var stars = $(this).parent().children('li.star');

        for (i = 0; i < stars.length; i++) {
            $(stars[i]).removeClass('selected');
        }

        for (i = 0; i < onStar; i++) {
            $(stars[i]).addClass('selected');
        }

        // JUST RESPONSE (Not needed)
        ratingValue = parseInt($('#stars li.selected').last().data('value'), 10);
        switch (ratingValue){
            case 1:
                $("#ratingBtn").css("background-color", '#000000');
                $("#ratingBtn").css("color", '#ffffff');
                break;
            case 2:
                $("#ratingBtn").css("background-color","grey");
                $("#ratingBtn").css("color", "white");
                break;
            case 3:
                $("#ratingBtn").css("background-color", "yellow");
                $("#ratingBtn").css("color", 'black');
                break;
            case 4:
                $("#ratingBtn").css("background-color", "magenta");
                $("#ratingBtn").css("color", 'black');
               break;
            case 5:
                $("#ratingBtn").css("background-color", "red");
                $("#ratingBtn").css("color", '#ffffff');
                break;
        }
                $("#ratingBtn").show();
        var msg = "";
        if (ratingValue > 1) {
            msg = "感謝! 您給評 " + ratingValue + " 顆星.";
        }
        else {
            msg = "作者將繼續努力改善. 您給評 " + ratingValue + " 顆星.";
        }
        responseMessage(msg);

    });


});


function responseMessage(msg) {
    $('.success-box').fadeIn(200);
    //$('.success-box div.text-message').html("<span>" + msg + "</span>");
    $('.success-box div.text-message').html( msg );
}