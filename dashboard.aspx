<%@ Page Language="VB" AutoEventWireup="false" CodeFile="dashboard.aspx.vb" Inherits="dashboard" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

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

    <style type="text/css">
        #google_map { height: 100% }
    </style>

    <script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=AIzaSyBxu581tgc59858UvW7VMSV3OiiZ-EEz3U&v=3.exp&amp;sensor=false"></script>

    <!-- ================== BEGIN USER DEFINED JS ================== -->
    <script type="text/javascript" src="assets/js/userjs/common.js"></script>
    <!-- ================== END USER DEFINED JS ================== -->
</head>
<body>
	<div id="page-loader" class="fade in"><span class="spinner"></span></div>

	<div id="page-container" class="fade in">
		<div id="header" class="header navbar navbar-default navbar-fixed-top">
            <div class="container-fluid">
                <div class="navbar-header">
                    <a href="#" class="navbar-brand"><span><img src="assets/img/logo.png" height="35"/></span></a>                    
                </div>                
                				
                <ul class="nav navbar-nav navbar-right"> 
                    <li class="border-left-1 border-right-1 p-l-5 p-r-5">
                        <form id="form1" class="form-inline hidden-xs" runat="server">
                            <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server" EnableScriptGlobalization="true" ></asp:ToolkitScriptManager>

                            <asp:Panel ID="panel_date_range" runat="server">
                                <div class="form-group">
                                    <asp:TextBox ID="input_start_date" runat="server" class="form-control text-center m-10" autocomplete="off"></asp:TextBox>
                                    <asp:CalendarExtender ID="calendar_start_date" runat="server" TargetControlID="input_start_date"></asp:CalendarExtender>                                   
                                </div>
                                <div class="form-group">
                                    <asp:TextBox ID="input_end_date" runat="server" class="form-control text-center m-10" autocomplete="off"></asp:TextBox> 
                                    <asp:CalendarExtender ID="calendar_end_date" runat="server" TargetControlID="input_end_date"></asp:CalendarExtender> 
                                </div>
                                <button class="btn btn-sm btn-primary m-5" onclick="UpdateInteractiveData(); return false;"><i class="fa fa-search"></i></button>   
                                <asp:HyperLink ID="link_clear_filter" class="btn btn-sm btn-danger m-5" runat="server"><i class="fa fa-trash-o"></i></asp:HyperLink>                         
                            </asp:Panel>   
                            <asp:HiddenField ID="hidden_garbage_type" runat="server" />                                         
	                    </form>
                    </li>              
                    <li class="dropdown navbar-user">                        
                        <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                            <asp:Literal ID="label_user_info" runat="server"></asp:Literal>                            
                        </a>
                        <ul class="dropdown-menu animated fadeInLeft">
                            <asp:Literal ID="literal_menu_items" runat="server"></asp:Literal>
                            <asp:Literal ID="literal_menu_items_base" runat="server"></asp:Literal> 
                            <li class="divider"></li>                                                         
                            <li><asp:HyperLink ID="link_reload" runat="server" Text="<%$Resources: controls,reload %>"></asp:HyperLink></li>	
                            <li class="divider"></li>						                             
                            <li><asp:HyperLink runat="server" NavigateUrl="home?logoff=1" Text="<%$Resources: controls,exit %>"></asp:HyperLink></li>
                        </ul>
                    </li>
                </ul>
            </div>
        </div>
    </div>   

	<div id="content" class="content">
		<ul class="breadcrumb pull-right">            
            <asp:Literal ID="literal_breadcrumb" runat="server"></asp:Literal>				
		</ul>			

        <asp:Literal ID="page_header" runat="server"></asp:Literal>

        <div class="row">
            <asp:Literal ID="literal_summary" runat="server"></asp:Literal>
		</div>

		<div class="row">
            <asp:Panel ID="panel_list_1" runat="server" class="col-md-12 col-lg-12">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
							<a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
						</div>
						<h4 class="panel-title" style="font-size: 14px;"><asp:Literal ID="literal_list_1" runat="server"></asp:Literal></h4>
					</div>
					<div class="panel-body">
						<div class="table-responsive">
                            <table id="data_table_list_1" class="table table-striped table-bordered" style="font-size: 11px;"></table>
                        </div>
					</div>
				</div>
            </asp:Panel>

            <asp:Panel ID="panel_list_2" runat="server" class="col-md-12 col-lg-12">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
							<a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
						</div>
						<h4 class="panel-title" style="font-size: 14px;"><asp:Literal ID="literal_list_2" runat="server"></asp:Literal></h4>
					</div>
                    <asp:Literal ID="literal_body_list_2" runat="server"></asp:Literal>					
				</div>
            </asp:Panel>
            
            <asp:Panel ID="panel_1" runat="server" class="col-md-6">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
							<a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
						</div>
						<h4 class="panel-title" style="font-size: 14px;"><asp:Literal runat="server" meta:resourcekey="panel_table_types"></asp:Literal></h4>
					</div>
					<div class="panel-body">
                        <asp:Literal ID="literal_garbage_summary" runat="server"></asp:Literal>						
					</div>
				</div>
            </asp:Panel>

            <asp:Panel ID="panel_2" runat="server" class="col-md-6">
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
            </asp:Panel>

            <asp:Panel ID="panel_3" runat="server" class="col-md-12">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
							<a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
						</div>
						<h4 class="panel-title" style="font-size: 14px;"><asp:Literal runat="server" meta:resourcekey="panel_timeline"></asp:Literal></h4>
					</div>
					<div class="panel-body">       
                        <asp:Literal ID="literal_garbage_buttons" runat="server"></asp:Literal>
                        <h2><asp:Literal runat="server" meta:resourcekey="panel_timeline_day"></asp:Literal></h2>
                        <div id="interactive-chart-multi-day" class="height-sm" style="padding: 0px; position: relative;">
                            <canvas class="flot-base" style="direction: ltr; position: absolute; left: 0px; top: 0px;"></canvas>                            
                            <canvas class="flot-overlay" style="direction: ltr; position: absolute; left: 0px; top: 0px;"></canvas>                                                        
						</div>   
                        <h2><asp:Literal runat="server" meta:resourcekey="panel_timeline_sum"></asp:Literal></h2>
                        <div id="interactive-chart-multi-sum" class="height-sm" style="padding: 0px; position: relative;">
                            <canvas class="flot-base" style="direction: ltr; position: absolute; left: 0px; top: 0px;"></canvas>                            
                            <canvas class="flot-overlay" style="direction: ltr; position: absolute; left: 0px; top: 0px;"></canvas>                           
						</div>                                     
					</div>
				</div>  
            </asp:Panel>

            <asp:Panel ID="panel_4" runat="server" class="col-md-12 col-lg-12">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
						<div class="panel-heading-btn">
						    <a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
					    </div>                        
					    <h4 class="panel-title" style="font-size: 14px;"><asp:Literal ID="literal_panel_4" runat="server"></asp:Literal></h4>
					</div>
                    <asp:Literal ID="literal_panel_body_4" runat="server"></asp:Literal>									    
			    </div>
            </asp:Panel>
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
    <script type="text/javascript" src="assets/js/userjs/map-google.js"></script>
    <!-- ================== END USER DEFINED JS ================== -->
    <script type="text/javascript" src="assets/js/form-plugins.js"></script>
    <script type="text/javascript" src="assets/js/apps.js"></script>	
    <!-- ================== END PAGE LEVEL JS ================== -->
	
    <script type="text/javascript">
        $(document).ready(function () {
            App.init();

            // Script generazione contenuti dashboard
            CreateInteractiveChart("#interactive-chart-multi-day",<%=mTimelineGarbageTypesDataDay%>, <%=mTimelineGarbageMinYDay%>, <%=mTimelineGarbageMaxYDay%>);
            CreateInteractiveChart("#interactive-chart-multi-sum",<%=mTimelineGarbageTypesDataSum%>, null, null);
            CreateDonutChart(<%=mGarbageDataDonut%>);
            CreateTable("#panel_table_3",<%=mContributionsHeaders%>,<%=mContributionsData%>,1,<%=mTableLabels%>,<%=mDisplayItems%>);
            CreateTable("#data_table_list_1",<%=mCitiesHeaders%>,<%=mCitiesData%>,2,<%=mTableLabels%>,<%=mDisplayItems%>);
            CreateTable("#data_table_list_1",<%=mStationsHeaders%>,<%=mStationsData%>,2,<%=mTableLabels%>,<%=mDisplayItems%>);

            CreateGoogleMap(<%=mMapCityLan%>,<%=mMapCityLon%>, <%=mMapCityZoom%>,<%=mMapData%>);            
        });
	</script>
</body>
</html>
