function CreateGoogleMap(citylan, citylon, cityzoom, mapdata) {
    "use strict";
    var map;

    if (citylan == null || citylon == null || mapdata == null) return;

    function initialize() {
        var mapOptions = {
            zoom: cityzoom,
            center: new google.maps.LatLng(citylan, citylon),
            mapTypeId: google.maps.MapTypeId.ROADMAP,
        };

        map = new google.maps.Map(document.getElementById("google_map"), mapOptions);

        var i = 0;
        var markers = [];

        for (i = 0; i < mapdata.length; i++) {
            var position = new google.maps.LatLng(mapdata[i].latitudine, mapdata[i].longitudine);
            var marker = new google.maps.Marker({
                position: position,
                map: map,
                title: mapdata[i].title
            });

            markers.push(marker);
        }        
        var markerCluster = new google.maps.MarkerClusterer(map, markers);
    }

    google.maps.event.addDomListener(window, "load", initialize);

    $(window).resize(function () {
        if (map) {
            google.maps.event.trigger(map, "resize");

            if ($("#panel_body_map").css("position") == "absolute") {
                $("#panel_map").css("height", "100%");
            } else {
                $("#panel_map").css("height", "");
            }
        }
    });
}