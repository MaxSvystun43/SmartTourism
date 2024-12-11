// import { useState } from "react";
// import { MapContainer, Marker, Popup, TileLayer } from "react-leaflet";
// import { LatLng } from "leaflet";
// import 'leaflet/dist/leaflet.css';
//
// function TestMap() {
//     const [startMarker, setStartMarker] = useState<LatLng | null>(null);
//     const [endMarker, setEndMarker] = useState<LatLng | null>(null);
//     const [contextMenu, setContextMenu] = useState<{ position: LatLng, x: number, y: number } | null>(null);
//
//     const handleMapRightClick = (event: any) => {
//         // Debug: Check if the event is firing
//         console.log("Right-click detected on the map:", event);
//
//         const { latlng } = event; // Get lat/lng from the event
//         const { clientX: x, clientY: y } = event.originalEvent; // Get mouse position for displaying the menu
//
//         // Set context menu state with map position and screen coordinates
//         setContextMenu({ position: latlng, x, y });
//     };
//
//     const handleSetMarker = (type: "start" | "end") => {
//         if (contextMenu?.position) {
//             if (type === "start") {
//                 setStartMarker(contextMenu.position);
//             } else if (type === "end") {
//                 setEndMarker(contextMenu.position);
//             }
//         }
//         setContextMenu(null); // Close the menu
//     };
//
//     return (
//         <div onContextMenu={(e) => e.preventDefault()} style={{ position: "relative" }}>
//             <MapContainer
//                 center={[50.6199, 26.2516]}
//                 zoom={13}
//                 scrollWheelZoom={false}
//                 whenCreated={(map) => {
//                     map.on("contextmenu", handleMapRightClick); // Bind right-click event
//                 }}
//                 style={{ height: "600px", width: "100%" }}
//             >
//                 <TileLayer
//                     attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
//                     url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
//                 />
//
//                 {/* Render Start Marker */}
//                 {startMarker && (
//                     <Marker position={startMarker}>
//                         <Popup>Start Marker</Popup>
//                     </Marker>
//                 )}
//
//                 {/* Render End Marker */}
//                 {endMarker && (
//                     <Marker position={endMarker}>
//                         <Popup>End Marker</Popup>
//                     </Marker>
//                 )}
//             </MapContainer>
//
//             {/* Context Menu */}
//             {contextMenu && (
//                 <div
//                     className="context-menu"
//                     style={{
//                         position: "absolute",
//                         top: `${contextMenu.y}px`,
//                         left: `${contextMenu.x}px`,
//                         zIndex: 1000,
//                         background: "white",
//                         border: "1px solid #ccc",
//                         borderRadius: "4px",
//                         padding: "5px",
//                         boxShadow: "0px 0px 10px rgba(0,0,0,0.1)"
//                     }}
//                 >
//                     <div
//                         onClick={() => handleSetMarker("start")}
//                         style={{ cursor: "pointer", padding: "5px", marginBottom: "5px" }}
//                     >
//                         Set as Start
//                     </div>
//                     <div
//                         onClick={() => handleSetMarker("end")}
//                         style={{ cursor: "pointer", padding: "5px" }}
//                     >
//                         Set as End
//                     </div>
//                 </div>
//             )}
//         </div>
//     );
// }
//
// export default TestMap;
