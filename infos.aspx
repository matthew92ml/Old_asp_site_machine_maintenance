<%@ Page Language="VB" AutoEventWireup="false" CodeFile="infos.aspx.vb" Inherits="infos" %>

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

    <style type="text/css">.jqstooltip { position: absolute;left: 0px;top: 0px;visibility: hidden;background: rgb(0, 0, 0) transparent;background-color: rgba(0,0,0,0.6);filter:progid:DXImageTransform.Microsoft.gradient(startColorstr=#99000000, endColorstr=#99000000);-ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr=#99000000, endColorstr=#99000000)";color: white;font: 10px arial, san serif;text-align: left;white-space: nowrap;padding: 5px;border: 1px solid white;z-index: 10000;}.jqsfield { color: white;font: 10px arial, san serif;text-align: left;}</style>
    <style type="text/css">            
        .content {
            margin-left: 0;
        }

        .map {
            top: 0;
            left: 0;                
            height: 400px;
            position: relative;
        }

        .map-content {
            padding: 0;
        }

        @media (max-width: 767px) {
            .map {
                top: 0!important;                    
            }
        }
    </style>
</head>
<body>
    <div id="page-loader" class="fade in"><span class="spinner"></span></div>

	<div id="page-container" class="fade in">
		<div id="header" class="header navbar navbar-default navbar-fixed-top">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a href="#" class="navbar-brand"><span><img src="assets/img/logo.png" height="35"/></span></a>
                </div>
				
                <a href="home" class="btn btn-warning btn-lg m-10 pull-right"><i class="fa fa-home"></i> Home</a>
            </div>
        </div>
    </div>

    <div id="content" class="content">
        <asp:Literal ID="page_header" runat="server"></asp:Literal>

		<div class="row">
            <asp:Literal ID="literal_summary" runat="server"></asp:Literal>
		</div>

        <div class="row">
            <div class="col-md-6">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
							<a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
						</div>
						<h4 class="panel-title" style="font-size: 14px;"><asp:Literal runat="server" meta:resourcekey="panel_timeline_types"></asp:Literal></h4>
					</div>
					<div class="panel-body">
                        <asp:Literal ID="literal_garbage_summary" runat="server"></asp:Literal>						
					</div>
				</div>
            </div>

            <div class="col-md-6">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
						    <a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
					    </div>
					    <h4 class="panel-title" style="font-size: 14px;"><asp:Literal runat="server" Text="<%$Resources: controls,garbage_types %>"></asp:Literal></h4>
					</div>
				    <div class="panel-body">
                        <div id="donut-chart" class="height-sm" style="padding: 0px; position: relative;">
                            <canvas class="flot-base" width="316" height="300" style="direction: ltr; position: absolute; left: 0px; top: 0px; width: 316px; height: 300px;"></canvas>
                            <canvas class="flot-overlay" width="316" height="300" style="direction: ltr; position: absolute; left: 0px; top: 0px; width: 316px; height: 300px;"></canvas>                            
                        </div>
				    </div>
			    </div>
            </div>
        </div>
    </div>

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
	<script type="text/javascript" src="assets/plugins/gritter/js/jquery.gritter.js"></script>
	<script type="text/javascript" src="assets/plugins/flot/jquery.flot.min.js"></script>
	<script type="text/javascript" src="assets/plugins/flot/jquery.flot.time.min.js"></script>
	<script type="text/javascript" src="assets/plugins/flot/jquery.flot.resize.min.js"></script>
	<script type="text/javascript" src="assets/plugins/flot/jquery.flot.pie.min.js"></script>
	<script type="text/javascript" src="assets/plugins/sparkline/jquery.sparkline.js"></script>
	<script type="text/javascript" src="assets/plugins/bootstrap-datepicker/js/bootstrap-datepicker.js"></script>	
	<script type="text/javascript" src="/assets/plugins/DataTables-1.9.4/js/jquery.dataTables.js"></script>
	<script type="text/javascript" src="assets/plugins/DataTables-1.9.4/js/data-table.js"></script>           
    <!-- ================== BEGIN USER DEFINED JS ================== -->
    <script type="text/javascript" src="assets/js/userjs/graphics.js"></script>
    <script type="text/javascript" src="assets/js/userjs/tables.js"></script>
    <!-- ================== END USER DEFINED JS ================== -->
    <script type="text/javascript" src="assets/js/apps.js"></script>	
    <!-- ================== END PAGE LEVEL JS ================== -->
	
    <script type="text/javascript">
        $(document).ready(function () {
            App.init();

            // Script generazione contenuti dashboard
            CreateDonutChart(<%=mGarbageDataDonut%>);
        });
	</script>
</body>
</html>
