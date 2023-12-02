<%@ Page Language="VB" AutoEventWireup="false" CodeFile="login.aspx.vb" Inherits="login" %>

<!DOCTYPE html>
<!--[if IE 8]> <html lang="en" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<html lang="it">
<!--<![endif]-->
<head runat="server">
    <meta charset="utf-8" />
    <title></title>
    <meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta name="title" content=""/>
    <meta name="subject" content=""/>
    <meta name="rating" content=""/>
    <meta name="description" content=""/>
    <meta name="abstract" content=""/>
    <meta name="keywords" content=""/>
    <meta name="revisit-after" content="7 days"/>
    <meta name="language" content="it"/>
    <meta name="robots" content="all"/>

    <link rel="shortcut icon" href="assets/img/favicon.png">
  
    <!-- ================== BEGIN BASE CSS STYLE ================== -->
    <link href="http://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />
    <link href="assets/plugins/jquery-ui-1.10.4/themes/base/minified/jquery-ui.min.css" rel="stylesheet" />
    <link href="assets/plugins/bootstrap-3.1.1/css/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/plugins/font-awesome-4.1.0/css/font-awesome.min.css" rel="stylesheet" />
    <link href="assets/css/animate.min.css" rel="stylesheet" />
    <link href="assets/css/style.min.css" rel="stylesheet" />
    <link href="assets/css/style-responsive.min.css" rel="stylesheet" />
    <!-- ================== END BASE CSS STYLE ================== -->
  
    <!-- ================== BEGIN USER DEFINED CSS STYLE ================== -->
    <link href="assets/css/style-user-defined.css" rel="stylesheet" />
    <!-- ================== END USER DEFINED CSS STYLE ================== -->

    <!-- ================== BEGIN PAGE LEVEL STYLE ================== -->
    <link href="assets/plugins/jquery-jvectormap/jquery-jvectormap-1.2.2.css" rel="stylesheet" />
    <link href="assets/plugins/bootstrap-datepicker/css/datepicker.css" rel="stylesheet" />
    <link href="assets/plugins/bootstrap-datepicker/css/datepicker3.css" rel="stylesheet" />
    <link href="assets/plugins/gritter/css/jquery.gritter.css" rel="stylesheet" />  
    <!-- ================== END PAGE LEVEL STYLE ================== -->

    <!-- ================== BEGIN USER DEFINED JS ================== -->
    <script type="text/javascript" src="assets/js/userjs/common.js"></script>
    <!-- ================== END USER DEFINED JS ================== -->
</head>
<body>
    <!-- begin #page-loader -->
    <div id="page-loader" class="fade in hide"><span class="spinner"></span></div>
    <!-- end #page-loader -->
	
    <!-- begin #page-container -->
    <div id="page-container" class="fade in">
    <!-- begin login -->
    <div class="login bg-black animated fadeInDown">
    <!-- begin brand -->
    <div class="login-header">
        <div class="brand">
            <a href="home"><img src="assets/img/logo.png" style="height: 55px;" /></a>
            <asp:Literal runat="server" meta:resourcekey="logo_subtext"></asp:Literal>
        </div>
        <div class="icon">
            <i class="fa fa-sign-in"></i>
        </div>
    </div>
    <!-- end brand -->
    <div class="login-content">
        <form runat="server" class="margin-bottom-0">
            <asp:Literal ID="literal_alert" runat="server"></asp:Literal>
            <div class="form-group m-b-20">
                <asp:TextBox ID="input_username" runat="server" CssClass="form-control input-lg" Font-Size="Medium" placeholder="<%$Resources: controls,user %>" ></asp:TextBox>
            </div>
            <div class="form-group m-b-20">
                <asp:TextBox ID="input_password" runat="server" CssClass="form-control input-lg" Font-Size="Medium" placeholder="<%$Resources: controls,password %>" TextMode="Password"></asp:TextBox>
            </div>
            <label class="checkbox m-b-20">
                <asp:CheckBox ID="check_remember" runat="server" Text="<%$Resources: controls,remember %>" />
            </label>
            <div class="login-buttons">
                <asp:Button ID="button_submit" CssClass="btn btn-green btn-block btn-lg m-b-20" runat="server" Text="<%$Resources: controls,button_submit %>" />
            </div> 
            <asp:HyperLink ID="link_recovery" CssClass="m-b-20" runat="server" ></asp:HyperLink>                                
        </form>
    </div>
    </div>
    <!-- end login -->
    </div>
    <!-- end page container -->
	
    <!-- ================== BEGIN BASE JS ================== -->
    <script type="text/javascript" src="assets/plugins/jquery-1.8.2/jquery-1.8.2.min.js"></script>
    <script type="text/javascript" src="assets/plugins/jquery-ui-1.10.4/ui/minified/jquery-ui.min.js"></script>
    <script type="text/javascript" src="assets/plugins/bootstrap-3.1.1/js/bootstrap.min.js"></script>
    <!--[if lt IE 9]>
        <script type="text/javascript" src="assets/crossbrowserjs/html5shiv.js"></script>
        <script type="text/javascript" src="assets/crossbrowserjs/respond.min.js"></script>
        <script type="text/javascript" src="assets/crossbrowserjs/excanvas.min.js"></script>
    <![endif]-->
    <script type="text/javascript" src="assets/plugins/slimscroll/jquery.slimscroll.min.js"></script>
    <!-- ================== END BASE JS ================== -->
	
    <!-- ================== BEGIN PAGE LEVEL JS ================== -->
    <script type="text/javascript" src="assets/js/apps.js"></script>
    <!-- ================== END PAGE LEVEL JS ================== -->
    <script type="text/javascript">
        $(document).ready(function () {
            App.init();
        });
    </script>
</body>
</html>
