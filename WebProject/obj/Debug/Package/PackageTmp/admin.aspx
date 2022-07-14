<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="admin.aspx.cs" Inherits="WebProject.admin" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>編輯廣告輪播</title>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"/>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"/>
    <meta content="noindex, nofollow" name="robots"/>
    <script src="Scripts/jquery-3.3.1.js"></script> 
    <script src="js/canvasjs.min.js"></script>

   
</head>
<body>
     <form id="form1" runat="server">
    <div>
        <nav class="navbar navbar-inverse">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a class="navbar-brand" href="#">資管4A影視平台</a>
                </div>
                <ul class="nav navbar-nav">
                    <li><a href="default.html">Home</a></li>
                    <li class="active" id="admin"><a id="adminhref" href="#">管理員</a></li>
                    <li><a id="personal" href="#">我的資料</a></li>
                    <li><a id="matcher" href="#">需求媒合</a></li>
                    <li><a id="communicate" href="#">技術交流</a></li>
                    <li><a id="upload" href="#">影片上傳</a></li>
                    <li><a id="donate" href="#"><span class="glyphicon glyphicon-usd"></span> 我要贊助</a></li>
                </ul>

                <ul class="nav navbar-nav navbar-right">
                    <li><a id="welcome" href="#"><span id="welcomespan" class="glyphicon glyphicon-user"></span>Sign Up</a></li>
                    <li><a id="logout" href="#"><span id="logoutspan" class="glyphicon glyphicon-log-in"></span>Login</a></li>
                    <!--<li><a id="welcome" href="#">Sign Up</a></li>
                    <li><a id="logout" href="#">Login</a></li>-->
                </ul>
            </div>
        </nav>
    </div>
    <div id="chartContainer" style="width: 100%; height:200px;font-size: large; color: #0000FF;"></div>
    <div id="ModContainer" style="width: 47%;background-color:cyan;float:left; margin-left:3%; font-size: large; color: #0000FF;"><%--"display :none">--%>
        <asp:RadioButtonList ID="FuncOP" runat="server" RepeatDirection="Horizontal" BackColor="Yellow" BorderStyle="Groove" Width="100%" OnSelectedIndexChanged="FuncOP_SelectedIndexChanged" AutoPostBack="True">
            <asp:ListItem Selected="True">新增</asp:ListItem>
            <asp:ListItem>編輯</asp:ListItem>
            <asp:ListItem>刪除</asp:ListItem>
        </asp:RadioButtonList>
        
            <br />
        
            <asp:Label ID="Label1" runat="server" Font-Bold="True" ForeColor="Blue" Text="委託人(公司)："></asp:Label>
            <asp:TextBox ID="txtPricipal" runat="server" Width="148px"></asp:TextBox>
            <br />
            <asp:Label ID="Label2" runat="server" Font-Bold="True" ForeColor="Blue" Text="託播日期："></asp:Label>
            <asp:Button ID="btnStart" runat="server" OnClick="btnStart_Click" Text="2018/08/30" Width="120px" />
            &nbsp;&nbsp;&nbsp;<asp:Label ID="Label3" runat="server" Font-Bold="True" ForeColor="Blue" Text="至"></asp:Label>
            &nbsp;&nbsp;
            <asp:Button ID="btnEnd" runat="server" OnClick="btnEnd_Click" Text="2018/08/30" Width="120px" />
            <br />
        
            <asp:Label ID="Label6" runat="server" Font-Bold="True" ForeColor="Blue" Text="委託人(公司)網址："></asp:Label>
            <br />
            <asp:TextBox ID="txtwebUrl" runat="server"  Width="70%" style="margin-left:10%" Font-Bold="True" Font-Size="Large"></asp:TextBox>
            <br />
            <asp:Label ID="Label4" runat="server" Font-Bold="True" ForeColor="Blue" Text="廣告詞："></asp:Label>
            <asp:TextBox ID="txtDesciption" runat="server"  Font-Bold="True" Font-Size="Large" Width="70%"></asp:TextBox>
            <br />
            <br />
           
            <asp:Label ID="Label5" runat="server" Font-Bold="True" ForeColor="Blue" Text="優先序："></asp:Label>
            <asp:DropDownList ID="dpPriority" runat="server">
                <asp:ListItem>1</asp:ListItem>
                <asp:ListItem>2</asp:ListItem>
                <asp:ListItem>3</asp:ListItem>
                <asp:ListItem>4</asp:ListItem>
                <asp:ListItem>5</asp:ListItem>
                <asp:ListItem></asp:ListItem>
            </asp:DropDownList>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Label ID="UploadStatusLabel" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <br />
        選取檔案上傳...<br />
            <asp:FileUpload ID="FileUpload1" runat="server" Font-Bold="True" Font-Size="Large" />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
             <asp:Button ID="btnConfirm"  runat="server" style="align-items:center" Font-Bold="True" Font-Size="Large" OnClick="Button1_Click" Text="資料確定與檔案上傳" Width="255px" />
            <asp:Calendar  style="Z-INDEX: 209; POSITION:relative; TOP: -252px; LEFT: 176px; overflow :hidden; height: 192px; width: 367px;" ID="Calendar1" runat="server" BackColor="#FFFFCC" BorderColor="#FFCC66" BorderWidth="1px" DayNameFormat="Shortest" Font-Names="Verdana" Font-Size="8pt" ForeColor="#663399" ShowGridLines="True" Visible="False" OnSelectionChanged="Calendar1_SelectionChanged" >
                <DayHeaderStyle BackColor="#FFCC66" Font-Bold="True" Height="1px" />
                <NextPrevStyle Font-Size="9pt" ForeColor="#FFFFCC" />
                <OtherMonthDayStyle ForeColor="#CC9966" />
                <SelectedDayStyle BackColor="#CCCCFF" Font-Bold="True" />
                <SelectorStyle BackColor="#FFCC66" />
                <TitleStyle BackColor="#990000" Font-Bold="True" Font-Size="9pt" ForeColor="#FFFFCC" />
                <TodayDayStyle BackColor="#FFCC66" ForeColor="White" />
            </asp:Calendar>
       <br/>
     </div>
 <div id="ModContainerR" style="width: 47%;background-color:pink;float:right;margin-right:2%; margin-left:1%; font-size: large; color: #0000FF;">
          <div id="GridArea" style="width: 100%;">

         <asp:GridView ID="GridView1" runat="server" Width="90%" AllowPaging="True" AutoGenerateColumns="False" OnSelectedIndexChanged="GridView1_SelectedIndexChanged" BackColor="#99FF33" PageSize="5">
             <AlternatingRowStyle BackColor="#FFFF66" />
             <Columns>
                 <asp:CommandField ButtonType="Button" CausesValidation="False" InsertVisible="False" ShowCancelButton="False" ShowSelectButton="True" />
                 <asp:BoundField DataField="principal" HeaderText="委託人" />
                 <asp:BoundField DataField="ShowDateStart" HeaderText="開始日期" DataFormatString="{0:yyyy/MM/dd }" />
                 <asp:BoundField DataField="ShowDateStop" HeaderText="結束日期" DataFormatString="{0:yyyy/MM/dd }" />
                 <asp:BoundField DataField="Priority" HeaderText="優先序" />
                 <asp:TemplateField Visible="False">
                     <ItemTemplate>
                         <asp:Label ID="lblfilename" runat="server" Text='<%# Bind("filename") %>'></asp:Label>
                     </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField Visible="False">
                     <ItemTemplate>
                         <asp:Label ID="lbldescription" runat="server" Text='<%# Bind("description") %>'></asp:Label>
                     </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField Visible="False">
                    <ItemTemplate>
                         <asp:Label ID="lblFileURL" runat="server" Text='<%# Bind("FileURL") %>'></asp:Label>
                     </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField Visible="False">
                     <ItemTemplate>
                         <asp:Label ID="lblMediaType" runat="server" Text='<%# Bind("MediaType") %>'></asp:Label>
                     </ItemTemplate>
                 </asp:TemplateField>
                 <asp:TemplateField Visible="False">
                     <ItemTemplate>
                         <asp:Label ID="lblWebURL" runat="server" Text='<%# Bind("WebURL") %>'></asp:Label>
                     </ItemTemplate>
                 </asp:TemplateField>
             </Columns>
             <HeaderStyle BackColor="Blue" />
             <SelectedRowStyle BackColor="#FF33CC" BorderColor="Red" />
         </asp:GridView>

        </div>
     <div>

         <br />
        
            <asp:Label ID="Label7" runat="server" Font-Bold="True" ForeColor="Blue" Text="佈告訊息："></asp:Label>
            <asp:Button ID="btnUpdate" runat="server" OnClick="btnUpdate_Click" Text="更新確定" Width="162px" />
         <br />
            <asp:TextBox ID="txtM1" runat="server"  Font-Bold="True" Font-Size="Large" Width="93%"></asp:TextBox>
            <br />
            <asp:TextBox ID="txtM2" runat="server"  Font-Bold="True" Font-Size="Large" Width="93%"></asp:TextBox>
            <br />
            <asp:TextBox ID="txtM3" runat="server"  Font-Bold="True" Font-Size="Large" Width="93%"></asp:TextBox>
            <br />

     </div>
         </div>

   <div class="tclearfix" style="height:80px;background-color:black;width: 100%;clear :both">
        <p style="color:white;top:20px;left:10px;">Copyright &copy; 2018 S.H.A.V.P影音平台</p>
    </div>
        

    </form>
   </body>
</html>
<script>
    var url;
    var urlS;

    var gurl = window.location.protocol + "//" + window.location.host;
    if (window.location.host.indexOf("localhost") >= 0) {
        url = gurl + "/";
        urlS = window.location.protocol + "//localhost/";
    }
    else {
        url = gurl + "/";
    }

    $.get(url + "/scheduleRelated.ashx?qid=2", function (mydata, status) {//get 廣告
        var obj = JSON.parse(mydata);
        var chart = new CanvasJS.Chart("chartContainer",
	    {
	        title: {
	            text: "廣告輪播排程 "
	        },
	        axisY: {
	            includeZero: false,
	            interval: 1,
	            valueFormatString: "第#0天"
	        },
	        //axisX: { //廣告主
	        //    interval: 1
	        //    //valueFormatString: "#0.## °C"
	        //},
	        data: [
            {
                type: "rangeBar",
                yValueFormatString: "第#0天",
                dataPoints: obj
            }
	        ]
	    });
        //chart.data[0].dataPoints = obj;
        chart.render();
        $("#ModContainer").show();
    });
    var UserName, Account, auth;
    $.post("dprocess.ashx", { qid: '3' }, function (data) {
        if (data != 'null') {//已登入成功
            UserName = data.split(",")[0];
            Account = data.split(",")[1];
            $("#welcome").text("歡迎:" + UserName)
            $("#welcome").attr("href", "#");
            $("#logout").text("Log out")
            $("#logout").attr("href", "logout.html");
            auth = 'Yes';
            $("#match").attr("href", "match.html");
            $("#communicate").attr("href", "communicate.html");
            //$("#upload").attr("href", "fileupload.aspx");
            $("#donate").attr("href", "donate.html");



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