<%@ Page Language="VB" AutoEventWireup="false" CodeFile="city.aspx.vb" Inherits="city" %>

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
    </style>

    <!-- ================== BEGIN PAGE LEVEL STYLE ================== -->
    <script type="text/javascript" src="assets/js/userjs/city.js"></script>
    <script type="text/javascript" src="assets/js/userjs/station.js"></script>
    <script type="text/javascript" src="assets/js/userjs/common.js"></script>
    <!-- ================== END PAGE LEVEL STYLE ================== -->
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
                    <li class="dropdown navbar-user">
                        <a href="javascript:;" class="dropdown-toggle" data-toggle="dropdown">
                            <asp:Literal ID="label_user_info" runat="server"></asp:Literal>                            
                        </a>
                        <ul class="dropdown-menu animated fadeInLeft">
                            <asp:Literal ID="literal_menu_items_base" runat="server"></asp:Literal>  
                            <asp:Literal ID="literal_menu_items" runat="server"></asp:Literal>                            
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
        <asp:Literal ID="page_header" runat="server"></asp:Literal>

        <div class="row">
            <div class="col-md-8 col-md-offset-2 col-lg-8 col-lg-offset-2">
                <div class="panel panel-inverse">
                    <div class="panel-heading">
                        <div class="panel-heading-btn">
							<a href="javascript:;" class="btn btn-sm btn-icon btn-circle btn-green" data-click="panel-expand"><i class="fa fa-expand"></i></a>
						</div>
						<h4 class="panel-title" style="font-size: 14px;"><asp:Literal ID="literal_edit" runat="server"></asp:Literal></h4>
                    </div>
                    <div class="panel-body">
                        <form id="formEdit" runat="server" class="form-horizontal">
                            <asp:ToolkitScriptManager ID="ToolkitScriptManager1" runat="server">
                                <Services>
                                    <asp:ServiceReference Path ="~/ServiceCity.asmx" />
                                </Services>
                            </asp:ToolkitScriptManager>
                            <fieldset>
                                <asp:Literal ID="literal_alert" runat="server"></asp:Literal>
                                <div class="form-group">
                                    <label for="input_name" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,name %>"></asp:Literal></label>
                                    <div class="col-md-6 col-lg-6">
                                        <asp:TextBox ID="input_name" runat="server" CssClass="form-control input-lg" Font-Size="Medium"></asp:TextBox>
                                        <asp:HiddenField ID="hidden_latitude" runat="server" />
                                        <asp:HiddenField ID="hidden_longitude" runat="server" />
                                    </div>                                    
                                </div>
                                <div class="form-group">
                                    <label for="input_district" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,district %>"></asp:Literal></label>
                                    <div class="col-md-2 col-lg-2">
                                        <asp:TextBox ID="input_district" runat="server" CssClass="form-control input-lg" Font-Size="Medium"></asp:TextBox>
                                    </div>                                    
                                </div>
                                <div class="form-group">
                                    <label for="input_state" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,state %>"></asp:Literal></label>
                                    <div class="col-md-6 col-lg-6">
                                        <asp:TextBox ID="input_state" runat="server" CssClass="form-control input-lg" Font-Size="Medium"></asp:TextBox>
                                    </div>                                    
                                </div>
                                <div class="form-group">
                                    <label for="input_citizens" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,summary_citizens %>"></asp:Literal></label> 
                                    <div class="col-md-2 col-lg-2">
                                        <asp:TextBox ID="input_citizens" runat="server" CssClass="form-control input-lg" Font-Size="Medium"></asp:TextBox>
                                    </div>                                 
                                </div>
                                <div class="form-group">
                                    <label for="input_units" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,summary_users %>"></asp:Literal></label> 
                                    <div class="col-md-2 col-lg-2">
                                        <asp:TextBox ID="input_units" runat="server" CssClass="form-control input-lg" Font-Size="Medium" ReadOnly="True"></asp:TextBox>
                                    </div>                                 
                                </div>
                                <div class="form-group">
                                    <label for="input_stations" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,summary_stations %>"></asp:Literal></label> 
                                    <div class="col-md-2 col-lg-2">
                                        <asp:TextBox ID="input_stations" runat="server" CssClass="form-control input-lg" Font-Size="Medium" ReadOnly="True"></asp:TextBox>
                                    </div>                                 
                                </div>
                                <div class="form-group">
                                    <label for="input_email" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,user %>"></asp:Literal></label>
                                    <div class="col-md-8 col-lg-8">
                                        <asp:TextBox ID="input_username" runat="server" CssClass="form-control"></asp:TextBox>
                                    </div>                                    
                                </div>
                                <div class="form-group">
                                    <div class="col-md-8 col-md-offset-4">
                                        <asp:Button ID="button_confirm" runat="server" CssClass="btn btn-sm btn-success" Text="<%$Resources: controls,button_submit %>" />
                                        <asp:HyperLink runat="server" ID="link_cancel" CssClass="btn btn-sm btn-white" Text="<%$Resources: controls,button_cancel %>"></asp:HyperLink>                                            
                                    </div>
                                </div>                                
                            </fieldset> 
                            <br />
                            <asp:Panel ID="panel_tags" runat="server">                                   
                                <fieldset>
                                    <legend><asp:Literal runat="server" Text="<%$Resources: controls,tag_management %>"></asp:Literal></legend>
                                    <asp:Panel ID="panel_error" runat="server"></asp:Panel>
                                    <div class="form-group">
                                        <label for="input_tag_code" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,tag_code %>"></asp:Literal></label>
                                        <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4">
                                            <asp:TextBox ID="input_tag_code" runat="server" CssClass="form-control"></asp:TextBox>
                                        </div>                                    
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-8 col-md-offset-4">
                                            <asp:Button ID="button_add" runat="server" CssClass="btn btn-sm btn-success" Text="<%$Resources: controls,button_add %>" />
                                            <asp:Button ID="button_check" runat="server" CssClass="btn btn-sm btn-info" Text="<%$Resources: controls,button_check %>" />
                                            <asp:HyperLink ID="link_delete" runat="server" CssClass="btn btn-sm btn-danger" data-toggle="modal" NavigateUrl="#modal_message_delete" Text="<%$Resources: controls,button_delete %>"></asp:HyperLink>                                                                                                                                  
                                            <img id="image_loading" alt="loading" src="assets/img/loading_small.gif" style="display: none;" />
                                        </div>
                                    </div>                                
                                </fieldset>
                            </asp:Panel> 
                            <asp:Panel ID="panel_upload" runat="server">
                                <fieldset>
                                    <legend><asp:Literal runat="server" Text="<%$Resources: controls,upload_tags %>"></asp:Literal></legend>
                                    <asp:Literal ID="literal_alert_upload_tags" runat="server"></asp:Literal>
                                    <div class="form-group">    
                                        <label for="input_file" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,select_file %>"></asp:Literal></label>                                                
                                        <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4">
                                            <asp:FileUpload ID="file_upload_tags" runat="server" CssClass="form-control"/>
                                        </div>                                    
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-8 col-md-offset-4">
                                            <asp:Button ID="button_upload_tags" runat="server" CssClass="btn btn-sm btn-success" Text="<%$Resources: controls,button_upload %>" />
                                        </div>
                                    </div>  
                                    <hr />
                                    <div class="form-group">
                                        <div class="col-md-8 col-md-offset-4">
                                            <asp:HyperLink ID="link_download_csv_tags" runat="server" Text="<%$Resources: controls,download_tags_csv %>" CssClass="btn btn-link p-0"></asp:HyperLink>
                                        </div>                                    
                                    </div>                             
                                </fieldset>
                                <fieldset>
                                    <legend><asp:Literal runat="server" Text="<%$Resources: controls,upload_stations %>"></asp:Literal></legend>
                                    <asp:Literal ID="literal_alert_upload_stations" runat="server"></asp:Literal>
                                    <div class="form-group">                                                    
                                        <label for="input_file" class="col-md-4 control-label"><asp:Literal runat="server" Text="<%$Resources: controls,select_file %>"></asp:Literal></label>                                                
                                        <div class="col-xs-5 col-sm-5 col-md-4 col-lg-4">
                                            <asp:FileUpload ID="file_upload_stations" runat="server" CssClass="form-control"/>
                                        </div>                                    
                                    </div>
                                    <div class="form-group">
                                        <div class="col-md-8 col-md-offset-4">
                                            <asp:Button ID="button_upload_stations" runat="server" CssClass="btn btn-sm btn-success" Text="<%$Resources: controls,button_upload %>" />
                                        </div>
                                    </div>  
                                    <br />
                                    <div class="form-group">
                                        <div class="col-md-8 col-md-offset-4">
                                            <asp:HyperLink ID="link_download_csv_stations" runat="server" Text="<%$Resources: controls,download_stations_csv %>" CssClass="btn btn-link p-0"></asp:HyperLink>
                                        </div>                                    
                                    </div>                             
                                </fieldset>
                            </asp:Panel>                                                     
                        </form>
                    </div>
                </div>
            </div>            
        </div>

        <asp:Panel ID="panel_stations" runat="server" CssClass="row">
            <div class="col-md-8 col-md-offset-2 col-lg-8 col-lg-offset-2">
                <div class="panel">
                    <div class="panel-body">
                        <div class="table-responsive">
                            <table id="data_table_stations" class="table table-striped table-bordered"></table>
                        </div>
                    </div>
                </div>                    
            </div>
        </asp:Panel>
    </div>

    <div class="modal modal-message fade" id="modal_message_delete" aria-hidden="true" style="z-index: 1200;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 id="modal_title_delete" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <p id="modal_text_delete"></p>
                </div>
                <div class="modal-footer">
                    <a id="link_modal_delete" class="btn btn-sm btn-success"></a>                    
                    <asp:HyperLink runat="server" CssClass="btn btn-sm btn-white" text="<%$Resources: controls,button_cancel %>" data-dismiss="modal"></asp:HyperLink>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-message fade" id="modal_message_check" aria-hidden="true" style="z-index: 1200;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <h4 id="modal_title_check" class="modal-title"></h4>
                </div>
                <div class="modal-body">
                    <p id="modal_text_check"></p>
                </div>
                <div class="modal-footer">                
                    <asp:HyperLink runat="server" CssClass="btn btn-sm btn-primary" text="<%$Resources: controls,button_close %>" data-dismiss="modal"></asp:HyperLink>
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
    <script type="text/javascript" src="assets/js/userjs/tables.js"></script>
    <!-- ================== END USER DEFINED JS ================== -->
    <script type="text/javascript" src="assets/js/apps.js"></script>	
    <!-- ================== END PAGE LEVEL JS ================== -->
	
    <script type="text/javascript">
        $(document).ready(function () {
            App.init();

            // Script generazione contenuti di gestione
            CreateTable("#data_table_stations",<%=mStationsHeaders%>,<%=mStationsData%>,2,<%=mTableLabels%>,<%=mDisplayItems%>);
        });
	</script>
</body>
</html>
