$(document).ready(function () {
    console.log("ready");
    //wyszukiwanie
    $('#search').click(function (event) {
        buttonSearchClick()
    });
    //zapisywanie
    $('#save').click(function (event) {
        buttonSaveClick()
    });
});

function buttonSearchClick() {
    var geocoder = new google.maps.Geocoder();
    var street = document.getElementById('txtStreet').value;
    var city = document.getElementById('txtCity').value;
    
    var com = city + "," + street;
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
}

function buttonSaveClick() {
    var street = document.getElementById('txtStreet').value;
    var city = document.getElementById('txtCity').value;
    sendDataToController(street, city);
}

function sendDataToController(street, city) {
    $.ajax({
        url: "/Account/GetDataFromView",
        dataType: 'json',
        data: {street: street, city: city},
        type: "post",
        success: function (savingStatus) {
            console.log("Wyslano", savingStatus);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            console.error("Blad: ", thrownError, xhr, ajaxOptions);
        }
    });
}
