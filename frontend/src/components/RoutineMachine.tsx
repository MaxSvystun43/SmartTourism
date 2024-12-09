import * as L from "leaflet";
import {createControlComponent} from "@react-leaflet/core";
import "leaflet-routing-machine";
import React from "react";

interface RoutingProps {
    waypoints?: L.LatLng[];
    onClick: (distance: string) => void;
}


const createRoutineMachineLayer: React.FC<RoutingProps> = ({waypoints, onClick}) => {
    // @ts-ignore
    const instance = L.Routing.control({
        waypoints: waypoints,
        lineOptions: {
            styles: [{color: "red", weight: 4}],
        },
        show: false,
        addWaypoints: false,
        routeWhileDragging: true,
        draggableWaypoints: true,
        fitSelectedRoutes: true,
        showAlternatives: false,
        createMarker: () => null, // No marker created,
        geocoder : L.Control.Geocoder.nominatim()
    });

    instance.on('routesfound', function (e: any) {
        const distance = e.routes[0].instructions;
        onClick(JSON.stringify(distance));
    });

    return instance;
};


// @ts-ignore
const RoutingMachine : React.FC<RoutingProps> = createControlComponent(createRoutineMachineLayer);

export default RoutingMachine;
