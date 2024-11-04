import * as L from "leaflet";
import {createControlComponent} from "@react-leaflet/core";
import "leaflet-routing-machine";

// Define the routing control component without unnecessary props


const createRoutineMachineLayer = ({waypoints, onClick}) => {
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
        createMarker: () => null, // No marker created
    });

    instance.on('routesfound', function (e : any) {
        const distance = e.routes[0].instructions;
        onClick(JSON.stringify(distance));
    });

    return instance;
};


const RoutingMachine = createControlComponent(createRoutineMachineLayer);

export default RoutingMachine;
