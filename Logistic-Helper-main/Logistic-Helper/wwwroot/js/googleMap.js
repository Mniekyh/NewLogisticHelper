

$(document).ready(function () {
    $('#btn').click(function (event) {
        alert("Hello! its me!!");
        $("#btn").click(function () {
            var geocoder = new google.maps.Geocoder();
            var con = document.getElementById('txtCon').value;
            var city = document.getElementById('txtCity').value;
            var com = city + "," + con;
            geocoder.geocode({ 'address': com }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    var x = results[0].geometry.location.lat();
                    var y = results[0].geometry.location.lng();
                    var latlng = new google.maps.LatLng(x, y);
                    var myOptions = {
                        zoom: 8,
                        center: latlng,
                        mapTypeId: google.maps.MapTypeId.ROADMAP
                    };
                    map = new google.maps.Map(document.getElementById("map-canvas"), myOptions);
                    var marker = new google.maps.Marker({
                        position: new google.maps.LatLng(x, y),
                        map: map,
                        title: com
                    });
                    var infowindow = new google.maps.InfoWindow({
                        content: com
                    });
                    infowindow.open(map, marker);
                    google.maps.event.addDomListener(window, 'load');
                } else {
                    res.innerHTML = "Enter correct Details: " + status;
                }
            });
        });
    });

    });
