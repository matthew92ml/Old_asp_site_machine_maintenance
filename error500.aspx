<%@ Page Language="VB" AutoEventWireup="false" CodeFile="error500.aspx.vb" Inherits="errorPage500" %>

<!DOCTYPE html>

<html lang="en"><!--<![endif]-->
<head runat="server">
	<meta charset="utf-8">
	<title>Ecofil | 500</title>
	<meta content="width=device-width, initial-scale=1.0" name="viewport">
	
	<!-- ================== BEGIN BASE CSS STYLE ================== -->
	<link href="http://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet">
	<link href="assets/plugins/jquery-ui-1.10.4/themes/base/minified/jquery-ui.min.css" rel="stylesheet">
	<link href="assets/plugins/bootstrap-3.1.1/css/bootstrap.min.css" rel="stylesheet">
	<link href="assets/plugins/font-awesome-4.1.0/css/font-awesome.min.css" rel="stylesheet">
	<link href="assets/css/animate.min.css" rel="stylesheet">
	<link href="assets/css/style.min.css" rel="stylesheet">
	<link href="assets/css/style-responsive.min.css" rel="stylesheet">
	<!-- ================== END BASE CSS STYLE ================== -->

    <!-- ================== BEGIN USER DEFINED CSS STYLE ================== -->
    <link href="assets/css/style-user-defined.css" rel="stylesheet" />
    <!-- ================== END USER DEFINED CSS STYLE ================== -->
</head>
<body>
	<!-- begin #page-loader -->
	<div id="page-loader" class="fade in hide"><span class="spinner"></span></div>
	<!-- end #page-loader -->
	
	<!-- begin #page-container -->
	<div id="page-container" class="fade in">
        <div class="error">
            <div class="error-code m-b-10">500 <i class="fa fa-bug"></i></div>
            <div class="error-content">
                <img src="assets/img/logo.png" style="width: 250px;" />
                <div class="error-message">
                    <asp:Literal ID="literal_title" runat="server" meta:resourcekey="title"></asp:Literal>
                </div>
                <div class="error-desc m-b-20">
                    <asp:Literal ID="literal_message" runat="server" meta:resourcekey="message"></asp:Literal>
                </div>                
                <div>
                    <asp:HyperLink runat="server" NavigateUrl="~/home" Text="<%$Resources: controls,back_home %>" CssClass="btn btn-green"></asp:HyperLink>
                </div>
            </div>
        </div>
		<!-- begin scroll to top btn -->
		<a href="javascript:;" class="btn btn-icon btn-circle btn-success btn-scroll-to-top fade" data-click="scroll-top"><i class="fa fa-angle-up"></i></a>
		<!-- end scroll to top btn -->
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
