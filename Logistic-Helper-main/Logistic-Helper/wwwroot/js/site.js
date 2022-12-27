// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getValue() {
    alert("kooo")
    sessionStorage.setItem("favoriteMovie", "Shrek");

    var streetId = document.getElementById("InputId").value
    var cityId = document.getElementById("CityId").value
    localStorage.setItem("streetId",streetid )
    localStorage.setItem("cityId", cityid)
    alert("kooo")

}