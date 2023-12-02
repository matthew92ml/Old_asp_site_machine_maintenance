<%@ Page Language="VB" AutoEventWireup="false" CodeFile="benefits.aspx.vb" Inherits="benefits" %>
<!--[if IE 8]> <html lang="en" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<!DOCTYPE html>
<!--<![endif]-->
<html lang="en">
<head runat="server">
    <title>I vantaggi del sistema di raccolta differenziata | Ecofil</title>
    <meta charset="utf-8" />
	<meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta name="description" content="Completo controllo dei dati di conferimento e raccolta, partnership virtuosa con la cittadinanza, progetti educativi rivolti alle nuove generazioni"/>
    <meta name="keywords" content="stazione ecologica, raccolta differenziata, rifiuti, conferimento rifiuti, smaltimento rifiuti, ecoself, decoro urbano, videosorveglianza, bonus, riduzione delle tasse"/>
    <meta name="revisit-after" content="7 days"/>
    <meta name="robots" content="all"/>

    <link rel="shortcut icon" href="assets/img/favicon.png">
	
	<!-- ================== BEGIN BASE CSS STYLE ================== -->
	<link href="http://fonts.googleapis.com/css?family=Open+Sans:300,400,600,700" rel="stylesheet" />	
    <!-- Bootstrap CSS -->
	<link href="assets/plugins/bootstrap-3.1.1/css/bootstrap.min.css" rel="stylesheet" />
	<link href="assets/plugins/font-awesome-4.1.0/css/font-awesome.min.css" rel="stylesheet" />
    <!-- Pretty Photo CSS -->
    <link href="assets/plugins/prettyPhoto/css/prettyPhoto.css" rel="stylesheet" />
	<!-- Parallax slider -->
    <link href="assets/plugins/parallax-slider/css/slider.css" rel="stylesheet" />
	<!-- ================== END BASE CSS STYLE ================== -->
	
    <!-- ================== BEGIN USER DEFINED CSS STYLE ================== -->
    <link href="assets/plugins/frontend/css/style.css" rel="stylesheet" />
    <link href="assets/plugins/frontend/css/green.css" rel="stylesheet" />
    <!-- ================== END USER DEFINED CSS STYLE ================== -->

    <style>
        .da-slider {
            background-image:url('assets/img/punto-raccolta-300.jpg');
        }

        .da-slider h2 {
            color: #FFFFFF;
        }

        .da-slider p {
            color: #FFFFFF;
        }

        .claim {
            margin-top: 25px;           
        }

        .how {
            margin-top:10px; 
            margin-right:10px; 
            font-weight:bold; 
            padding: 2px 5px 2px 5px; 
            background-color:#41bb19; 
            color:#FFFFFF; 
            display:inline;
        }
    </style>
</head>
<body>
    <header>
        <div class="container">
            <div class="row">
                <div class="col-md-4 col-sm-4">
                    <div class="logo"><a href="home.aspx"><img src="assets/img/logo.png" style="height: 55px;" /></a></div>
                </div>
                <div class="col-md-8 hidden-xs hidden-sm text-right">
                    <div class="claim"><h3><asp:Literal runat="server" Text="<%$Resources: pages,claim_text %>"></asp:Literal></h3></div>
                </div>
            </div>
        </div>
    </header>

    <div class="navbar bs-docs-nav" role="banner">
        <div class="container">
            <div class="navbar-header">
                <button class="navbar-toggle" type="button" data-toggle="collapse" data-target=".bs-navbar-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
            </div>
               
            <nav class="collapse navbar-collapse bs-navbar-collapse" role="navigation">
                <ul class="nav navbar-nav">
                    <li><a href="home">Home</a></li>                
                    <li><asp:HyperLink runat="server" NavigateUrl="<%$Resources: pages,how_work_link %>"><asp:Literal runat="server" Text="<%$Resources: pages,how_work_text %>"></asp:Literal></asp:HyperLink></li>
                    <li><asp:HyperLink runat="server" NavigateUrl="<%$Resources: pages,benefits_link %>"><asp:Literal runat="server" Text="<%$Resources: pages,benefits_text %>"></asp:Literal></asp:HyperLink></li>
                    <%--<li><asp:HyperLink runat="server" NavigateUrl="#"><asp:Literal runat="server" Text="<%$Resources: pages,services_text %>"></asp:Literal></asp:HyperLink></li>--%>                                                                                                     
                </ul>
                <ul class="nav navbar-nav navbar-right">                    
                    <li><asp:HyperLink runat="server" NavigateUrl="<%$Resources: pages,signin_link %>"><asp:Literal ID="Literal1" runat="server" Text="<%$Resources: pages,signin_text %>"></asp:Literal></asp:HyperLink></li>                  
                    <li><asp:HyperLink runat="server" NavigateUrl="~/login" ToolTip="<%$Resources: pages,login_text %>" style="padding-top: 11px; padding-bottom: 11px;"><i class="fa fa-user" style="font-size: 2em !important;"></i> <i class="fa fa-sign-in" style="font-size: 2em !important;"></i></asp:HyperLink></li>
                </ul>
            </nav>
        </div>
    </div>

    <div class="content">
        <div class="container">
            <div class="row hidden-sm hidden-xs">
                <div class="col-md-12">
                    <div id="da-slider" class="da-slider">
                        <h2 class="bold" style="padding-left:40px; padding-right:40px; margin-top:40px;"><asp:Literal runat="server" meta:resourcekey="slidertext"></asp:Literal></h2>
                        <h4 style="padding-left:40px; padding-right:40px; margin-top:20px; color:#FFFFFF;"><asp:Literal runat="server" meta:resourcekey="slidersubtext"></asp:Literal></h4>
                    </div>          
                </div>
            </div>

            <div class="row">
                <div class="col-md-12">
                    <h4 class="bold"><asp:Literal runat="server" meta:resourcekey="infotitle"></asp:Literal></h4>
                    <span class="how"><asp:Literal runat="server" meta:resourcekey="infosubtitle1"></asp:Literal></span>
                    <br /><br />
                    <p><asp:Literal runat="server" meta:resourcekey="infotext1"></asp:Literal></p>
                    <p class="text-justify"><span class="how">1</span><asp:Literal runat="server" meta:resourcekey="infotext2"></asp:Literal></p>
                    <p class="text-justify"><span class="how">2</span><asp:Literal runat="server" meta:resourcekey="infotext3"></asp:Literal></p>
                    <br />
                    <p class="how"><asp:Literal runat="server" meta:resourcekey="infosubtitle2"></asp:Literal></p>
                    <p class="text-justify"><asp:Literal runat="server" meta:resourcekey="infotext4"></asp:Literal></p>
                </div>  
            </div>
            <br />
        </div>
    </div>
        
    <footer>
        <div class="container">
            <div class="row"> 
                <div class="col-md-12">
                    <div class="copy">           
                        <p><asp:Literal runat="server" Text="<%$Resources: pages,footer_text %>"></asp:Literal></p>
                    </div>
                </div>
            </div>
            <div class="clearfix"></div>
        </div>
    </footer>

    <!-- Javascript files -->
	<!-- jQuery -->
	<script type="text/javascript" src="assets/plugins/jquery-1.8.2/jquery-1.8.2.min.js"></script>
	<!-- Bootstrap JS -->
	<script type="text/javascript" src="assets/plugins/bootstrap-3.1.1/js/bootstrap.min.js"></script>
    <!-- Google Analytics -->
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-45058039-4', 'auto');
        ga('send', 'pageview');
    </script>
</body>
</html>

