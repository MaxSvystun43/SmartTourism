import { useMap } from "react-leaflet";
import { Geocoder, geocoders } from 'leaflet-control-geocoder';
import { useEffect } from "react";
import { LatLng } from "leaflet";
import L from "leaflet";

function EndGeocoder({ setEndMarker, fetchAddress }) {
    const map = useMap();

    useEffect(() => {
        const geocoderControl = new Geocoder({
            geocoder: new geocoders.Nominatim(),
            defaultMarkGeocode: false,
            position: 'topleft',
            placeholder: 'Search End Location...',
        });

        geocoderControl.addTo(map);

        geocoderControl.on("markgeocode", (e) => {
            const { center } = e.geocode;
            const latlng = new LatLng(center.lat, center.lng);
            setEndMarker(latlng);
            fetchAddress(latlng);
            map.setView(latlng, 14);

            // Optionally, add a marker to the map
            L.marker(latlng).addTo(map);
        });

        return () => {
            geocoderControl.remove();
        };
    }, [map]);

    return null;
}

export default EndGeocoder;
