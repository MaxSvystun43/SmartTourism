// import {MapContainer, Marker, TileLayer, useMapEvents} from "react-leaflet";
// import RoutingMachine from "../components/RoutineMachine.tsx";
// import {LatLng} from "leaflet";
// import {useEffect, useState} from "react";
// import 'leaflet/dist/leaflet.css';
// import {Button} from "antd";
//
// function FindRoute() {
//     const ClickHandler = ({ setMarkers }: { setMarkers: React.Dispatch<React.SetStateAction<LatLng[]>> }) => {
//         // Hook to capture map events
//         useMapEvents({
//             click(e) {
//                 const newMarker = e.latlng; // Get clicked LatLng
//                 setMarkers((prevMarkers) => [...prevMarkers, newMarker]); // Add new marker to array
//             },
//         });
//         return null; // No rendering needed for this component
//     };
//
//     const [markers, setMarkers] = useState<LatLng[]>([]);
//     const [routeVisible, setRouteVisible] = useState(false);
//     const [refreshKey, setRefreshKey] = useState(0); // Key to refresh RoutingMachine
//
//     const handleRouteButtonClick = () => {
//         if (markers.length > 0) {
//             setRouteVisible(true); // Show the RoutingMachine when the button is clicked
//             setRefreshKey((prev) => prev + 1); // Increment the key to refresh RoutingMachine
//         }
//     };
//
//     useEffect(() => {
//         if (routeVisible && markers.length > 0) {
//             setRefreshKey((prev) => prev + 1); // Refresh the RoutingMachine
//         }
//     }, [markers]);
//
//
//     return (
//         <>
//             <MapContainer center={[50.6199, 26.2516]} zoom={13} scrollWheelZoom={false}>
//                 <TileLayer
//                     attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
//                     url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
//                 />
//                 <ClickHandler setMarkers={setMarkers} />
//
//                 {markers.map((position, idx) => (
//                     <Marker key={idx} position={position}></Marker>
//                 ))}
//                 {routeVisible && <RoutingMachine key={refreshKey} waypoints={markers} />} {/* Render RoutingMachine conditionally */}
//
//             </MapContainer>
//             <Button onClick={handleRouteButtonClick} disabled={markers.length === 0}>
//                 Run route
//             </Button>
//         </>
//     );
// }
//
// export default FindRoute;