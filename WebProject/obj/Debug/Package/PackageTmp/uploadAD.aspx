<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="uploadAD.aspx.cs" Inherits="WebProject.uploadAD" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>檔案上傳 ＃2</title>
    <style type="text/css">
        .style1 {
            color: #FF0000;
        }
        .style2
        {
            color: #FF0000;
            font-weight: bold;
        }
        .style3
        {
            color: #003399;
            font-weight: bold;
        }
        .style4
        {
            background-color: #FFFF00;
        }
        .style5
        {
            color: #003399;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div style="background-color:lightcyan">
       <h4>選擇欲附加廣告影片檔案上傳&nbsp; (Select a file to upload) :</h4>
        <p>&nbsp;</p>
   
            請先選取檔案，然後再上傳：<asp:FileUpload id="FileUpload1" runat="server" Height="34px">
       </asp:FileUpload>
            
       <br /><br />
       
       <asp:Button id="Button1" Text="檔案上傳" runat="server" onclick="Button1_Click" Height="33px">
       </asp:Button>    
        <br />
        <span class="style1"><br />
       
       </span>
       
        <span class="style5">檔名相同的話，目前上傳的檔名（如：abc.gif），前面會用數字來代替（如：<span class="style4">2_</span>abc.gif）。
       
       </span>
       
       <hr />
       
       <asp:Label id="Label1" runat="server"></asp:Label>    
        </div>
<script >
    
    function updateOpener(mvid) {
        window.opener.document.getElementById(mvid+"btnADUpload").innerHTML = "廣告片已上傳";
        window.opener.document.getElementById(mvid + "btnADUpload").style.background = "black";
        window.opener.document.getElementById(mvid + "btnADUpload").style.color = "white";
        window.opener.document.getElementById(mvid + "btnADUpload").onclick = "";
        window.close();

    }
</script>     </form>

</body>
</html>
