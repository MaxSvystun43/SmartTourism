import { useMap } from "react-leaflet";
import { Geocoder, geocoders } from 'leaflet-control-geocoder';
import { useEffect } from "react";
import {LatLng} from "leaflet";

function GeocoderComponent({ setMarkers, fetchAddress }) {
    const map = useMap();

    useEffect(() => {
        const GeocoderControl = new Geocoder({
            geocoder: new geocoders.Nominatim(),
            position: 'topleft',
        });

        GeocoderControl.addTo(map);

        GeocoderControl.on("markgeocode", (e) => {
            const { center } = e.geocode;
            const latlng = new LatLng(center.lat, center.lng);
            setMarkers((prev) => [...prev, latlng]);
            fetchAddress(latlng);
            map.setView(latlng, 14);
        });

        return () => {
            GeocoderControl.remove();
        };
    }, [map]);

    return null;
}

export default GeocoderComponent;
