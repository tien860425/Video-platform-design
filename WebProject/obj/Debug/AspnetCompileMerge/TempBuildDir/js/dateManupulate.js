function DateAdd(interval, number, date) {
    switch (interval.toLowerCase()) {
        case "y":
            date.setFullYear(date.getFullYear() + number);
            break;
        case "m":
            date.setMonth(date.getMonth() + number);
            break;
        case "d":
            date.setDate(date.getDate() + number);
            break;
        case "h":
            date.setHours(date.getHours() + number);
            break;
        case "n":
            date.setMinutes(date.getMinutes() + number);
            break;
        case "s":
            date.setSeconds(date.getSeconds() + number);
            break;
        default:
    }
    return date
}

function DateDiff(interval, date1, date2) {
    var part = date2.getTime() - date1.getTime(); //相差毫秒
    switch (interval.toLowerCase()) {
        case "y":
            return parseInt(date2.getFullYear() - date1.getFullYear());
        case "m":
            return parseInt((date2.getFullYear() - date1.getFullYear()) * 12 + (date2.getMonth() - date1.getMonth()));
        case "d":
            return parseInt(part / 1000 / 60 / 60 / 24);
        case "w":
            return parseInt(part / 1000 / 60 / 60 / 24 / 7);
        case "h":
            return parseInt(part / 1000 / 60 / 60);
        case "n":
            return parseInt(part / 1000 / 60);
        case "s":
            return parseInt(part / 1000);
        case "l":
            return parseInt(part);
    }
}

function FormatDateTime(date, format) {
    var obj = {
        "M+": date.getMonth() + 1,
        "d+": date.getDate(),
        "h+": date.getHours(),
        "m+": date.getMinutes(),
        "s+": date.getSeconds(),
        "q+": Math.floor((date.getMonth() + 3) / 3),
        "S": date.getMilliseconds()
    }
    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length))
    }
    for (var k in obj) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? obj[k] : ("00" + obj[k]).substr(("" + obj[k]).length))
        }
    }
    return format
}

function isDate(obj) {
    return toString.call(obj) === "[object Date]";
}

function js_yyyy_mm_dd_hh_mm_ss() {
    now = new Date();
    year = "" + now.getFullYear();
    month = "" + (now.getMonth() + 1); if (month.length == 1) { month = "0" + month; }
    day = "" + now.getDate(); if (day.length == 1) { day = "0" + day; }
    hour = "" + now.getHours(); if (hour.length == 1) { hour = "0" + hour; }
    minute = "" + now.getMinutes(); if (minute.length == 1) { minute = "0" + minute; }
    second = "" + now.getSeconds(); if (second.length == 1) { second = "0" + second; }
    return year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
}

function Format(a, nodigit) {
    stra = "" + a;
    //if (stra.length >= nodigit) return stra;
    for (var i = 0; i < nodigit - stra.length ; i++) {
        stra = "0" + stra;
    }
    return stra;
}