(function () {
    var map;

    function getJSON(url, successHandler, errorHandler) {
        var xhr = typeof XMLHttpRequest !== 'undefined' ?
            new XMLHttpRequest() : new ActiveXObject('Microsoft.XMLHTTP');
        xhr.open('get', url, true);
        xhr.onreadystatechange = function () {
            var status;
            var data;
            // http://xhr.spec.whatwg.org/#dom-xmlhttprequest-readystate
            if (xhr.readyState === 4) { // `DONE`
                status = xhr.status;
                if (status === 200) {
                    data = JSON.parse(xhr.responseText);

                    if (successHandler) {
                        successHandler(data);
                    }
                } else {
                    if (errorHandler) {
                        errorHandler(status);
                    }
                }
            }
        };
        xhr.send();
    }

    function onEachFeature(feature, layer) {
        if (feature.properties && feature.properties.title && feature.geometry.coordinates.length === 2) {
            layer.bindPopup('<h4 style="color: #000">' + feature.properties.title + '</h4>'
                + '<p>Koordinaatit: ' + feature.geometry.coordinates[1] + ', ' + feature.geometry.coordinates[0] + '</p>');
        }
    }

    ko.bindingHandlers.map = {
        init: function (element, valueAccessor) {
            if (typeof L === 'undefined') {
                throw new Error("Leaflet.js is not included, cannot bind map");
            }

            if (element.children.length) {
                // Map has been initialized already, just exit
                return;
            }

            // Init map
            var args = ko.utils.unwrapObservable(valueAccessor());

            map = L.map(element, {
                center: [args.lat, args.lon],
                zoom: args.zoom
            });

            // On map popup open send page event. If more information is needed for logging 
            // get it from popupEvent given as first argument.
            map.on('popupopen', function () {
                Ksx.Track.pageEvent("map", "click", "marker");
            });

            // Create own icon for location
            var RunnerIcon = L.Icon.extend({
                options: {
                    //shadowUrl: 'leaf-shadow.png',
                    iconSize: [62, 60],
                    //shadowSize: [50, 64],
                    iconAnchor: [36, 60],
                    //shadowAnchor: [4, 62],
                    popupAnchor: [15, -60]
                }
            });

            var orangeRunningIcon = new RunnerIcon({ iconUrl: '/images/orange-runner.png' });

            // Map users current location if device allows
            map.on('locationfound', function (e) {
                var radius = e.accuracy / 2;

                L.marker(e.latlng, { icon: orangeRunningIcon }).addTo(map)
                    .bindPopup("Olet " + Math.round(radius) + " metrin sisällä tästä pisteestä.").openPopup();

                L.circle(e.latlng, radius).addTo(map);
            });

            map.locate({ setView: false });

            L.Icon.Default.imagePath = '/images/';

            // add an OpenStreetMap tile layer
            L.tileLayer('//{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors'
            }).addTo(map);

            // Add GeoJson from server
            if (args.geoJson) {
                getJSON(args.geoJson, function (data) {

                    var geoJsonLayer = L.geoJson(data, {
                        onEachFeature: onEachFeature
                    }).addTo(map);

                    if (!!args.zoomToRoute) {
                        map.fitBounds(geoJsonLayer.getBounds());
                    }

                }, function (status) {
                    console.error('Retrieving GeoJson data failed.', status);
                });
            }
        },
        update: function (element, valueAccessor) {
            var args = ko.utils.unwrapObservable(valueAccessor());

            // Close all popups if any open
            map.closePopup();

            // Center the (already initialized) map to given coordinates, and reset zoom
            map.setView([args.lat, args.lon], args.zoom);
        }
    };
})();