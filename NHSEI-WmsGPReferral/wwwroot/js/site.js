// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.



// Handle cookie consent banner
function handleCloseCookies() {
  document.getElementById('cookiebanner').style.display = 'none';
  var cookieValue = encodeURIComponent("True");
  document.cookie = "CookieConsent=" + cookieValue + ";path=/";
}

document.getElementById("nhsuk-cookie-banner__link_accept_analytics")?.addEventListener("click", function () {
    handleCloseCookies()
});