var HotImg=[];
var NewImg=[];
var HotVid = [];
var NewVid = [];
var HotArray = [];
var NewArray = [];
var Wrapper1;
var Wrapper2;
var rr;
function preload() {

    loadJSON(url  + "/scheduleRelated.ashx?qid=1", getdata);
}

function getdata(data) {
    HotArray = data.Hot;
    for (var i = 0; i < HotArray.length; i++) {
        HotImg.push(loadImage(url  +HotArray[i].url + HotArray[i].movieid + ".jpg"));
        var path=[];
        path.push(url  +HotArray[i].url + HotArray[i].movieid + ".mp4");

        HotVid.push(createVideo(path));
    }
    NewArray = data.New;

    for (var i = 0; i < NewArray.length; i++) {
        NewImg.push(loadImage( url  +NewArray[i].url + NewArray[i].movieid + ".jpg"))
        var path = [];
        path.push(url  +NewArray[i].url + NewArray[i].movieid + ".mp4");
        NewVid.push(createVideo(path));
    }
  
}

function setup() {
    noLoop();
    Wrapper1 = select('#Wrapper1');
    Wrapper2 = select('#Wrapper2');
    var appendWrap='<div class="item"> ';

    for (var i = 0; i < HotArray.length; i++) {
        if (i == 0) {
            var elmdiv = select('#' + str(i) + 'hitem');
            //elm.style('display',)
            elmdiv.show();
            var elmimg = select('#' + str(i) + 'himg');
            elmimg.attribute(
        }
 
    }


}

function draw() {
  // put drawing code here
}

//<a href="https://www.w3schools.com"><img src="smiley.gif" alt="Visit W3Schools.com!" width="42" height="42"></a>


//<div class="item active" style=" display:inline">
//    <img id="001img" src="./img/post/1.gif" width="300" height="200" alt="Car" style=" float:left">
//    <video id="01img" controls autoplay src="./img/post/v1.mp4" style=" float:right"></video>
//    <div class="carousel-caption">
//        <h3>熱播1</h3>
//        <p>活動式海報1.</p>
//    </div>
//</div>
