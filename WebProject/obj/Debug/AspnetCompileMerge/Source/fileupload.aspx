<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fileupload.aspx.cs" Inherits="WebProject.fileupload" %>

<!DOCTYPE html>

<html>
<head>
    <!-- Force latest IE rendering engine or ChromeFrame if installed -->
    <!--[if IE]>
<meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
<![endif]-->
    <meta charset="utf-8">
    <title>File Upload </title>
    <meta name="description" content="File Upload widget with multiple file selection, drag&amp;drop support, progress bars, validation and preview images, audio and video for jQuery. Supports cross-domain, chunked and resumable file uploads and client-side image resizing. Works with any server-side platform (PHP, Python, Ruby on Rails, Java, Node.js, Go etc.) that supports standard HTML form file uploads.">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <!-- Bootstrap styles -->
    <%--<link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">--%>
    <!-- Generic page styles -->
    <%--<link rel="stylesheet" href="css/style.css">--%>
    <!-- blueimp Gallery styles -->
    <link rel="stylesheet" href="http://blueimp.github.io/Gallery/css/blueimp-gallery.min.css">
    <!-- CSS to style the file input field as button and adjust the Bootstrap progress bars -->
    <link rel="stylesheet" href="css/jquery.fileupload.css">
    <link rel="stylesheet" href="css/jquery.fileupload-ui.css">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css">
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">

    <!-- CSS adjustments for browsers with JavaScript disabled -->
    <noscript>
        <link rel="stylesheet" href="css/jquery.fileupload-noscript.css">
    </noscript>
    <noscript>
        <link rel="stylesheet" href="css/jquery.fileupload-ui-noscript.css">
    </noscript>
</head>
<body  style="background-repeat:no-repeat; background:url(global.jpg);
      background-attachment: fixed;
      background-position: center;
      background-size: cover;">
    <div>
        <nav class="navbar navbar-inverse">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a class="navbar-brand" href="#"style="border-right:1px black solid">影視人</a>
                </div>
                <ul class="nav navbar-nav">
                    <li><a href="default.html">Home</a></li>
                    <li><a id="personal" href="#">我的資料</a></li>
                    <%--                    <li><a id="match" href="#">需求媒合</a></li>
                    <li><a id="communicate" href="#">技術交流</a></li>--%>
                    <li class="active"><a id="upload" href="#">影片上傳</a></li>
                </ul>

                <ul class="nav navbar-nav navbar-right">
                    <li><a id="welcome" href="#"><span class="glyphicon glyphicon-user"></span>Sign Up</a></li>
                    <li><a id="logout" href="#"><span class="glyphicon glyphicon-log-in"></span>Login</a></li>
                </ul>
            </div>
        </nav>
    </div>

    <div class="container">
        <h1 style="text-align: center">影片上傳</h1>
        <!-- The file upload form used as target for the file upload widget -->
        <form id="fileupload" action="UploadProgress.aspx" method="POST" enctype="multipart/form-data">
            <!-- Redirect browsers with JavaScript disabled to the origin page -->
            <!--      <noscript><input type="hidden" name="redirect" value="http://blueimp.github.io/jQuery-File-Upload/"></noscript>-->
            <!-- The fileupload-buttonbar contains buttons to add/delete files and start/cancel the upload -->
            <div class="row fileupload-buttonbar">
                <div class="col-lg-7">
                    <!-- The fileinput-button span is used to style the file input field as button -->
                    <span class="btn btn-success fileinput-button">
                        <i class="glyphicon glyphicon-plus"></i>
                        <span>選擇檔案</span>
                        <input type="file" name="files[]" multiple>
                    </span>
                    <button type="submit" class="btn btn-primary start">
                        <i class="glyphicon glyphicon-upload"></i>
                        <span>檔案上傳</span>
                    </button>
                    <%-- <span class="btn btn-success fileinput-button">
                        <i class="glyphicon glyphicon-plus"></i>
                        <span>上傳廣告內容並連接FB</span>
                        <input type="file" name="files[]" multiple>
                    </span>--%>
                    <button type="reset" class="btn btn-warning cancel">
                        <i class="glyphicon glyphicon-ban-circle"></i>
                        <span>取消上傳</span>
                    </button>
                    <%--                <button type="button" class="btn btn-danger delete">
                    <i class="glyphicon glyphicon-trash"></i>
                    <span>刪除檔案</span>
                </button>
                <input type="checkbox" class="toggle">
                <!-- The global file processing state -->
                <span class="fileupload-process"></span>--%>
                </div>
                <!-- The global progress state -->
                <div class="col-lg-5 fileupload-progress fade">
                    <!-- The global progress bar -->
                    <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100">
                        <div class="progress-bar progress-bar-success" style="width: 1%;"></div>
                    </div>
                    <!-- The extended global progress state -->
                    <div class="progress-extended">&nbsp;</div>
                </div>
            </div>
            <!-- The table listing the files available for upload/download -->
            <table id="presentTbl" role="presentation" class="table table-striped">
                <tbody class="files"></tbody>
            </table>
        </form>

        <br>
        <div class="panel panel-default">
            <div class="panel-heading">
                <h3 class="panel-title">上傳說明</h3>
            </div>

            <div class="panel-body">
                <ul>
                    <li>上傳檔案大小限制在 <strong>500 MB</strong> 以下。</li>
                    <li>限制上傳檔案類型： (<strong>avi, mp4, flv, webm, wmv檔案</strong>) .</li>
                    <!--上傳類型的限制設定在 main.js-->
                </ul>
            </div>
        </div>
    </div>
    <div class="tclearfix" style="height: 60px; background-color: black; bottom: 0px; right: 0px; width: 100%; text-align: center; padding-top: 20px; clear: both">
        <p style="color: white; top: 20px; left: 10px;">Copyright &copy; 2018 S.H.V.P影音平台</p>
    </div>

    <!-- The blueimp Gallery widget -->

    <!-- The template to display files available for upload -->
    <script id="template-upload" type="text/x-tmpl">
       {% for (var i=0, file; file=o.files[i]; i++) { 
          var str= file.name.split(' ').join(''); %}
       <tr class="template-upload fade">
            <td>
                <span class="preview"></span>
            </td>
            <td>
                <p class="name">{%=file.name%}</p>
                        <strong class="error text-danger"></strong>
                 <p class="title">片名</p>
                <input name={%="Title" + str%} type="text"> <%--只是為區別--%>
            </td>
            <td>
                <p class="size">Processing...</p>
                <div class="progress progress-striped active" role="progressbar" aria-valuemin="0" aria-valuemax="100" aria-valuenow="0"><div class="progress-bar progress-bar-success" style="width:0%;"></div></div>
            </td>
            <td>
                <p class="vClass">類別</p>
                <strong class="error text-danger"></strong>
                  <select name={%="Vtype" + str%}>
<%--                        <option>電影</option>
                        <option>動漫</option>
                        <option>微電影</option>
                         <option>動作</option>
                         <option>喜劇</option>
                         <option >愛情</option>
                        <option>科幻</option>
                        <option>恐怖</option>
                        <option>教學</option>
                        <option>其他</option>--%>

                        <option value="Suspense">懸疑</option>
                        <option value="cartoon">動漫</option>
                        <option value="micrfilm">微電影</option>
                         <option value="action">動作</option>
                         <option value="comedy">喜劇</option>
                         <option value="love" >愛情</option>
                        <option value="Sci-fi">科幻</option>
                        <option value="horro">恐怖</option>
                        <option value="education">教學</option>
                        <option value="others">其他</option>
                  </select>
            </td>
        <td>
                <p class="cut">剪裁</p>
                <strong class="error text-danger"></strong>
                  <select name={%="Vcut" + str%}>
<%--                        <option>電影</option>
                        <option>動漫</option>
                        <option>微電影</option>
                         <option>動作</option>
                         <option>喜劇</option>
                         <option >愛情</option>
                        <option>科幻</option>
                        <option>恐怖</option>
                        <option>教學</option>
                        <option>其他</option>--%>

                        <option value="cutone">每部三十秒</option>
                        <option value="cutfive">每部一分鐘</option>
                        <option value="cutten">每部五分鐘</option>

                  </select>
            </td>
            <td>
                <p class="intro">簡介</p>
                <strong class="error text-danger"></strong>
                <textarea style="overflow:scroll;" name={%="intro" +  str%} rows="3" cols="30">
                   </textarea>
            </td>
            <td>
                <p class="actor">演員</p>
                <strong class="error text-danger"></strong>
                   <input name={%="actor" +  str%} type="text">
            </td>

            <td>
                {% if (!i && !o.options.autoUpload) { %}
                <button class="btn btn-primary start" disabled>
                    <i class="glyphicon glyphicon-upload"></i>
                    <span>Start</span>
                </button>
                {% } %}
                {% if (!i) { %}
                <button class="btn btn-warning cancel">
                    <i class="glyphicon glyphicon-ban-circle"></i>
                    <span>Cancel</span>
                </button>
                {% } %}
            </td>
        </td>

        </tr>
        {% } %}
    </script>
    <!-- The template to display files available for download -->
    <script id="template-download" type="text/x-tmpl">
        {% for (var i=0, file; file=o.files[i]; i++) { %}

        <tr class="template-download fade">
            <td>
                <span class="preview">
                    {% if (file.thumbnailUrl) { %}
                    <a href="{%=file.url%}" target="_blank" title="{%=file.name%}" download="{%=file.name%}" data-gallery><img src="{%=file.thumbnailUrl%}"></a>
                    {% } %}
                </span>
            </td>
            <td>
                <p class="name">
                    {% if (file.url) { %}
                    <a href="{%=file.url%}" target="_blank" title="{%=file.name%}" download="{%=file.name%}" {%=file.thumbnailUrl?'data-gallery':''%}>{%=file.name%}</a>
                    {% } else { %}
                    <span>{%=file.name%}</span>
                    {% } %}
                </p>
                {% if (file.error) { %}
                <div><span class="label label-danger">Error</span> {%=file.error%}</div>
                {% } %}
                 <p class="title">片名</p>
                <span>{%=file.title%}</span>

            </td>
            <td>
                <p class="mvid">影片編號</p>
                <span class="size">{%=file.mvid%}</span>
            </td>
            <td>
                <span class="size">{%=o.formatFileSize(file.size)%}</span>
            </td>
            <td>
                <p class="vClass">類別</p>
                <span class="type">{%=file.Vtype%}</span>
            </td>
        {% if (!file.error) { %}
            <td>
               <button type="button" class="vAD" id="{%=file.mvid+'btnADUpload' %}" onclick='window.open("uploadAD.aspx?path={%=file.path%}&mvid={%=file.mvid%}&cut={%=file.scut%}","_bank","width=500,height=350,top=150,left=400,location=0,menubar=0,resizable=0,scrollbars=0,status=0,titlebar=0,toolbar=0");'>上傳廣告片</button>  <%--this.onclick="";this.text("已上傳廣告片")--%>
            </td>
                {% } %}

<%--            <td>
                {% if (file.deleteUrl) { %}
                <button class="btn btn-danger delete" data-type="{%=file.deleteType%}" data-url="{%=file.deleteUrl%}" {% if (file.deletewithcredentials) { %} data-xhr-fields='{"withCredentials":true}' {% } %}>
                    <i class="glyphicon glyphicon-trash"></i>
                    <span>Delete</span>
                </button>
                <input type="checkbox" name="delete" value="1" class="toggle">                                                
               {% } else { %}><button class="btn btn-warning cancel">
                    <i class="glyphicon glyphicon-ban-circle"></i>
                    <span>Cancel</span>
                </button>
                {% } %}
            </td>--%>
        </tr>
        {% } %}
    </script>
    <script src="js/jquery_1.11.0.min.js"></script>
    <!-- The jQuery UI widget factory, can be omitted if jQuery UI is already included -->
    <!-- jquery.ui.widget.js 不能省略，否則已上傳及要上傳之清單會看不到-->
    <script src="js/vendor/jquery.ui.widget.js"></script>
    <!-- The Templates plugin is included to render the upload/download listings -->
    <script src="http://blueimp.github.io/JavaScript-Templates/js/tmpl.min.js"></script>
    <!-- The Load Image plugin is included for the preview images and image resizing functionality -->
    <script src="http://blueimp.github.io/JavaScript-Load-Image/js/load-image.all.min.js"></script>
    <!-- The Canvas to Blob plugin is included for image resizing functionality -->
    <script src="http://blueimp.github.io/JavaScript-Canvas-to-Blob/js/canvas-to-blob.min.js"></script>
    <!-- Bootstrap JS is not required, but included for the responsive demo navigation -->
    <script src="css/bootstrap-3.1.1-dist/js/bootstrap.min.js"></script>
    <!-- blueimp Gallery script -->
    <script src="http://blueimp.github.io/Gallery/js/jquery.blueimp-gallery.min.js"></script>
    <!-- The Iframe Transport is required for browsers without support for XHR file uploads -->
    <script src="js/jquery.iframe-transport.js"></script>
    <!-- The File Upload 基本檔案上傳套件 -->
    <script src="js/jquery.fileupload.js"></script>
    <!-- The File Upload 檔案上傳處理套件 (processing plugin) -->
    <script src="js/jquery.fileupload-process.js"></script>
    <!-- The File Upload 檔案上傳圖片檔預覽套件 (image preview & resize plugin) -->
    <script src="js/jquery.fileupload-image.js"></script>
    <!-- The File Upload 檔案上傳音樂檔預覽套件 (audio preview plugin)  若不允許上傳影片，可以不用載入-->
    <script src="js/jquery.fileupload-audio.js"></script>
    <!-- The File Upload 檔案上傳影片預覽套件 (video preview plugin) 若不允許上傳影片，可以不用載入 -->
    <script src="js/jquery.fileupload-video.js"></script>
    <!-- The File Upload 檔案上傳驗證套件 (validation plugin) -->
    <script src="js/jquery.fileupload-validate.js"></script>
    <!-- The File Upload 檔案上傳使用者介面套件 (user interface plugin) -->
    <script src="js/jquery.fileupload-ui.js"></script>
    <script>var fileuploadurl = "UploadProgress.aspx";</script>
    <script src="js/main.js"></script>


</body>
</html>
<script>
    var url;
    var urlS;
    var gurl = window.location.protocol + "//" + window.location.host;
    if (window.location.host.indexOf("localhost") >= 0) {
        url = gurl + "/";
        urlS = window.location.protocol + "//localhost";
    }
    else {
        url = gurl + "/";
        urlS = url;
    }

    var UserName, Account, auth;
    $.post("dprocess.ashx", { qid: '3' }, function (data) {
        if (data != 'null') {//已登入成功
            UserName = data.split(",")[0];
            Account = data.split(",")[1];
            $("#welcome").text("歡迎:" + UserName)
            $("#welcome").attr("href", "#");
            $("#logout").text("Log out")
            auth = 'Yes';
            //$("#match").attr("href", "match.html");
            //$("#communicate").attr("href", "communicate.html");
            //$("#upload").attr("href", "fileupload.aspx");
            $("#personal").attr("href", url + "personel.html");



        }
        else {
            window.location.href = "/default.html";

        }
    });
    $("#logout").click(function () {
        $.post("dprocess.ashx", { qid: '51' }, function (data) {
            if (data != 'null') {//登出成功

                window.location.href = url + "default.html";
            }
        });
    });


</script>
