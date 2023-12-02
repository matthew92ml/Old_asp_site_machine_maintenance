<%@ Page Language="VB" AutoEventWireup="false" CodeFile="home.aspx.vb" Inherits="home" %>
<!--[if IE 8]> <html lang="en" class="ie8"> <![endif]-->
<!--[if !IE]><!-->
<!DOCTYPE html>
<!--<![endif]-->
<html lang="en">
<head runat="server">
    <title>Sistema di raccolta differenziata intelligente | Ecofil</title>
    <meta charset="utf-8" />
	<meta content="width=device-width, initial-scale=1.0" name="viewport" />
    <meta name="description" content="Ecofil è un sistema avanzato per la raccolta differenziata per pubbliche amministrazioni ed aziende multiservizi"/>
    <meta name="keywords" content="stazione ecologica, raccolta differenziata, rifiuti, conferimento rifiuti, smaltimento rifiuti, ecoself, decoro urbano, sistema di prossimità, RSU, nuclei familiari, tracciabilità, RFID"/>
    <meta name="revisit-after" content="7 days"/>
    <meta name="robots" content="all"/>
    <meta name="google-site-verification" content="cZsjt8h6UYpDglkdnAA8Iy2-4WkE8mLPmuQoPiqJOt4" />

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
        header {
            border-top: 2px solid #41bb19;
        }
        .da-slider {
            background-image:url('assets/img/green-leaves-300-a.jpg');
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

        .service p {
            font-size: 14px;
        }
    </style>
</head>
<body>
    <header>
        <div class="container">
            <div class="row">
                <div class="col-md-4">
                    <div class="logo"><a href="home.aspx"><img src="assets/img/logo.png" style="height: 55px;" alt="ecofil differenzia con intelligenza"/></a></div>
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
                    <%--<li><asp:HyperLink runat="server" NavigateUrl="<%$Resources: pages,services_link %>"><asp:Literal runat="server" Text="<%$Resources: pages,services_text %>"></asp:Literal></asp:HyperLink></li>--%>                                                                                                     
                </ul>
                <ul class="nav navbar-nav navbar-right">                    
                    <li><asp:HyperLink runat="server" NavigateUrl="<%$Resources: pages,signin_link %>"><asp:Literal runat="server" Text="<%$Resources: pages,signin_text %>"></asp:Literal></asp:HyperLink></li>                  
                    <li><asp:HyperLink runat="server" NavigateUrl="~/login" style="padding-top: 11px; padding-bottom: 11px;">
                        <i class="fa fa-user" style="font-size: 2em !important;"></i>&nbsp 
                        <i class="fa fa-sign-in" style="font-size: 2em !important;"></i>&nbsp
                        <asp:Literal Text="<%$Resources: pages,login_text %>" runat="server"></asp:Literal>                        
                     </asp:HyperLink></li>
                </ul>
            </nav>
        </div>
    </div>

    <div class="content">
        <div class="container">
            <div class="row hidden-sm hidden-xs">
                <div class="col-md-12">
                    <div id="da-slider" class="da-slider">
                        <div id="da-slider" class="da-slider">
                        <h1 style="padding-left:40px; padding-right:40px; margin-top:40px; color:#666;"><asp:Literal runat="server" meta:resourcekey="slidertext"></asp:Literal></h1>
                        <h4 style="padding-left:40px; padding-right:40px; margin-top:20px; color:#666;"><asp:Literal runat="server" meta:resourcekey="slidersubtext"></asp:Literal></h4>
                    </div> 
                    </div>          
                </div>
            </div>        

            <div class="row">
                <div class="col-md-12">
                    <h4 class="bold"><asp:Literal runat="server" meta:resourcekey="maininfo"></asp:Literal></h4>
                    <h6><asp:Literal runat="server" meta:resourcekey="mainsubinfo"></asp:Literal></h6>
                    <hr />
                </div>  
            </div>

            <div class="services" style="background-color: #f4f5f0; padding: 10px;">
                <div class="row">
                    <div class="col-md-12">
                        <h3 class="bold"><asp:Literal runat="server" meta:resourcekey="servicesinfo"></asp:Literal></h3>
                        <h6><asp:Literal runat="server" meta:resourcekey="servicessubinfo"></asp:Literal></h6>
                    </div>

                    <div class="col-md-4">                
                        <div class="service">
                            <div class="b-orange serv-block">
                                <i class="fa fa-certificate"></i>
                                <h3><asp:Literal runat="server" meta:resourcekey="info1"></asp:Literal></h3>
                            </div>
                            <p><asp:Literal runat="server" meta:resourcekey="text1"></asp:Literal></p>
                        </div>
                    </div>
              
                    <div class="col-md-4">                
                        <div class="service">      
                            <div class="b-purple serv-block">
                                <i class="fa fa-leaf"></i>
                                <h3><asp:Literal runat="server" meta:resourcekey="info2"></asp:Literal></h3>
                            </div>     
                            <p><asp:Literal runat="server" meta:resourcekey="text2"></asp:Literal></p>
                        </div>
                    </div>

                    <div class="col-md-4">                
                        <div class="service">
                            <div class="b-green serv-block">
                                <i class="fa fa-money"></i>
                                <h3><asp:Literal runat="server" meta:resourcekey="info3"></asp:Literal></h3>
                            </div>
                            <p><asp:Literal runat="server" meta:resourcekey="text3"></asp:Literal></p>
                        </div>
                    </div>                         
                </div>

                <div class="row">
                    <div class="col-md-4">                
                        <div class="service">
                            <div class="b-lblue serv-block">
                                <i class="fa fa-circle"></i>
                                <h3><asp:Literal runat="server" meta:resourcekey="info4"></asp:Literal></h3>
                            </div>
                            <p><asp:Literal runat="server" meta:resourcekey="text4"></asp:Literal></p>                    
                        </div>
                    </div>
              
                    <div class="col-md-4">                
                        <div class="service"> 
                            <div class="b-blue serv-block">
                                <i class="fa fa-gift"></i>
                                <h3><asp:Literal runat="server" meta:resourcekey="info5"></asp:Literal></h3>
                            </div>        
                            <p><asp:Literal runat="server" meta:resourcekey="text5"></asp:Literal></p>                   
                        </div>
                    </div>

                    <div class="col-md-4">                
                        <div class="service">      
                            <div class="b-red serv-block">
                                <i class="fa fa-users"></i>
                                <h3><asp:Literal runat="server" meta:resourcekey="info6"></asp:Literal></h3>
                            </div>     
                            <p><asp:Literal runat="server" meta:resourcekey="text6"></asp:Literal></p>                    
                        </div>
                    </div>
                </div>
            </div>
            <hr />  

            <div class="services">
                <div class="row">
                    <div class="col-md-12">
                        <h4 class="bold"><asp:Literal runat="server" meta:resourcekey="customerinfo"></asp:Literal></h4>
                    </div>

                    <asp:Literal ID="literal_cities" runat="server"></asp:Literal>                          
                </div>
            </div>
            <hr />

            <div class="border"></div>
            <div class="prod">
                <div class="row">
                    <div class="col-md-6">
                        <h6><asp:Literal runat="server" meta:resourcekey="contactinfo"></asp:Literal></h6>
                    </div>
                    <div class="col-md-6"> 
                        <div class="home-product">                               
                            <h6 style="color:#FFF;">tel. +39 0736.847281</h6>
                            <h6 style="color:#FFF;">fax +39 0736.847281</h6>
                            <h6 style="color:#FFF;">email info@ecofilgreen.it</h6>
                            <div class="clearfix"></div>
                        </div>
                    </div>
                </div>
            </div>  
        </div>
    </div>

<%--<div class="social-links">
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <p class="big"><span>Seguici su</span> <a href="#"><i class="fa fa-facebook"></i>Facebook</a> <a href="#"><i class="fa fa-twitter"></i>Twitter</a> <a href="#"><i class="fa fa-google-plus"></i>Google Plus</a> <a href="#"><i class="fa fa-linkedin"></i>LinkedIn</a></p>
                </div>
            </div>    
        </div>
    </div>--%>

    <footer>
        <div class="container">
            <div class="row">
                <div class="widgets">
                    <div class="col-md-4">
                        <div class="fwidget">
                            <h4 class="bold"><asp:Literal runat="server" meta:resourcekey="footertitle1"></asp:Literal></h4>
                            <hr />
                            <p class="text-justify"><asp:Literal runat="server" meta:resourcekey="footertext1"></asp:Literal></p>
                        </div>
                    </div> 
          
                    <div class="col-md-4">
                        <div class="fwidget">
                            <h4 class="bold"><asp:Literal runat="server" meta:resourcekey="footertitle2"></asp:Literal></h4>
                            <hr />
                            <p class="text-justify"><asp:Literal runat="server" meta:resourcekey="footertext2"></asp:Literal></p>
                        </div>
                    </div>  
         
                    <div class="col-md-4">
                        <div class="fwidget">
                            <h4 class="bold"><asp:Literal runat="server" meta:resourcekey="footertitle3"></asp:Literal></h4>
                            <hr />
                            <p class="text-justify"><asp:Literal runat="server" meta:resourcekey="footertext3"></asp:Literal></p>
                        </div>
                    </div>                  
                </div>
            </div>
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
