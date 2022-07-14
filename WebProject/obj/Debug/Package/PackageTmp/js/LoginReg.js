var comingUrl = window.location.href.split('?')[1].split('&')[0].substring(4);
var attval = window.location.href.split('?')[1].split('&');
if (attval.length > 2) {
    for (i = 2; i < attval.length; i++) {
        if(i==2)
            comingUrl += "?" + attval[2].split('=')[1] + "=" + attval[2].split('=')[2];
        else {
            comingUrl += "&" + attval[i];
        }

    }

}
//var role=window.location.href.split('?')[1].split('&')[1];
//if (role = "login") {
//    $("#first").slideUp("slow", function () {
//        $("#second").slideDown("slow");
//    });
//} else {
//    $("#second").slideUp("slow", function () {
//        $("#first").slideDown("slow");
//    });
//}

$(document).ready(function () {
    // On Click SignIn Button Checks For Valid E-mail And All Field Should Be Filled
    $("#login").click(function () {
        var email = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$/i);
        if ($("#loginemail").val() == '' || $("#loginpassword").val() == '') {
            alert("請輸入所有欄位...!!!!!!");
        } else if (!($("#loginemail").val()).match(email)) {
            alert("請輸入正確電子信箱...!!!!!!");
        } else {
            var loginemail = $("#loginemail").val();
            var loginpassword = $("#loginpassword").val();

            $.post("dprocess.ashx?role=login", { qid: 2, email: loginemail, password: loginpassword },
                       function (data) {
                           if (data == 'Email or Password is wrong...') {
                               $('input[type="text"],input[type="password"]').css({ "border": "2px solid red", "box-shadow": "0 0 3px red" });
                               alert('Email 或密碼錯誤...');
                           } else if (data == 'Successfully Logged in...') {
                               $("form")[0].reset();
                               $('input[type="text"],input[type="password"]').css({ "border": "2px solid #00F5FF", "box-shadow": "0 0 5px #00F5FF" });
                               alert('您已成功登入');
                               window.location.href = comingUrl;
                           }
                       });
        }
    });
        $("#register").click(function () {
            var emailFilter = new RegExp(/^[+a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+.[a-zA-Z]{2,4}$/i);
            var email = $("#registeremail").val();
            var password = $("#registerpassword").val();
            var cpassword = $("#registercpassword").val();
            var name = $("#name").val();
            var contact = $("#contact").val();
           if ($("#name").val() == '' || $("#registeremail").val() == '' || $("#registerpassword").val() == '' || $("#registercpassword").val() == '' || $("#contact").val() == '') {
                alert("請輸入所有欄位...!!!!!!");
           } else if (!(email.match(emailFilter))) {
                alert("請輸入正確電子信箱...!!!!!!");
            } else if (!(password).match(cpassword)) {
                alert("密碼不匹配. 重試?");
            } else {
                $.post("dprocess.ashx?role=reg", {
                    qid: 1,
                    name: name,
                    email: email,
                    password: password,
                    contact: contact
                }, function (data) {
                    if (data == "already exist...") {
                        alert('輸入的電子信箱已被使用...');
                    } else if (data == 'OK Registered...') {
                        alert("您已註冊成功，現在可以登入了...!!!!!!");
                        $("#form")[0].reset();
                        $("#second").slideUp("slow", function () {
                            $("#first").slideDown("slow");
                        });
                    }
                });
            }
        });
        // On Click SignUp It Will Hide Login Form and Display Registration Form
        $("#signup").click(function () {
            $("#first").slideUp("slow", function () {
                $("#second").slideDown("slow");
            });
        });
        // On Click SignIn It Will Hide Registration Form and Display Login Form
        $("#signin").click(function () {
            $("#second").slideUp("slow", function () {
                $("#first").slideDown("slow");
            });
        });
    });
