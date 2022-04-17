var placeSearch, pickup, destiny;

function initAutocomplete() {
    // Create the autocomplete object, restricting the search to geographical
    // location types.
    pickup = new google.maps.places.Autocomplete(
                /** @type {!HTMLInputElement} */(document.getElementById('pickup')),
        { types: ['geocode'] });
    destiny = new google.maps.places.Autocomplete(
                /** @type {!HTMLInputElement} */(document.getElementById('destiny')),
        { types: ['geocode'] });
};

// Bias the autocomplete object to the user's geographical location,
// as supplied by the browser's 'navigator.geolocation' object.
function geolocate() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var geolocation = {
                lat: position.coords.latitude,
                lng: position.coords.longitude
            };
            var circle = new google.maps.Circle({
                center: geolocation,
                radius: position.coords.accuracy
            });
            pickup.setBounds(circle.getBounds());
            destiny.setBounds(circle.getBounds());
        });
    }
};
