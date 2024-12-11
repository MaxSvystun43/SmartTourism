import {MapContainer, Marker, Popup, TileLayer, useMapEvents} from "react-leaflet";
import RoutingMachine from "../components/RoutineMachine.tsx";
import L, {LatLng} from "leaflet";
import React, {useEffect, useState} from "react";
import 'leaflet/dist/leaflet.css';
import {Button} from "antd";
import DirectionList from "../components/DirectionList.tsx";
import Place from "../types/Place.ts";
import GeoApiModal from "../components/GeoApiModel.tsx";
import {GeoApiProvider} from "../components/contexts/GeoApiContext.tsx";
import {MarkerType} from "../types/MarkerType.ts";
import TypeDropdown from "../components/TypeDropdown.tsx";
import StartGeocoder from "../components/StartGeocoder.tsx";
import EndGeocoder from "../components/EndGeocoder.tsx";
import Point from "../types/Point.ts";

interface GeoApiRequest {
    categories: string[];
    filter: {
        longitude: number;
        latitude: number;
        radiusInMeters: number;
    };
    bias: {
        longitude: number;
        latitude: number;
    };
    limit: number;
}

interface RouteRequestPayload {
    start: {
        location: [number, number];
    };
    end: {
        location: [number, number];
    };
    GeoApiRequest: GeoApiRequest;
}



function MapPage() {
    const ClickHandler = ({ setMarkers }: { setMarkers: React.Dispatch<React.SetStateAction<LatLng[]>> }) => {
        // Hook to capture map events
        useMapEvents({
            click(e) {
                if (clickType == MarkerType.Waypoint) {
                    const newMarker = e.latlng; // Get clicked LatLng
                    setMarkers((prevMarkers) => [...prevMarkers, newMarker]); // Add new marker to array
                }
                if (clickType == MarkerType.Start) {
                    setStartMarkers(e.latlng);
                }
                if (clickType == MarkerType.End) {
                    setEndMarkers(e.latlng);
                    console.log("end marker" + JSON.stringify(endMarker));
                }
                console.log(JSON.stringify([startMarker, ...markers, endMarker]));
            },
        });
        return null; // No rendering needed for this component
    };

    const createRoutePayload = (startMarker: L.LatLng | null, endMarker: L.LatLng | null, formData: GeoApiRequest): RouteRequestPayload | null => {
        if (!startMarker || !endMarker) {
            console.warn("Start or End marker is missing!");
            return null;
        }

        return {
            start: {
                location: [startMarker.lat, startMarker.lng],
            },
            end: {
                location: [endMarker.lat, endMarker.lng],
            },
            GeoApiRequest: formData,
        };
    };


    const calculateMidpoint = (start: L.LatLng, end: L.LatLng) => {
        const midLatitude = (start.lat + end.lat) / 2;
        const midLongitude = (start.lng + end.lng) / 2;
        return { latitude: midLatitude, longitude: midLongitude };
    };
    const handleGetRoutePoints = async () => {
        if (!formData || !startMarker || !endMarker) {
            console.warn("Start/End marker or form data is missing!");
            return;
        }

        const payload = createRoutePayload(startMarker, endMarker, formData);

        if (!payload) {
            return;
        }

        try {
            const response = await fetch("https://localhost:7148/api/geo/pathfinding/get-routes", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                throw new Error(`Error: ${response.statusText}`);
            }

            const routePoints: Point[] = await response.json();
            console.log("Fetched Route Points:", routePoints);

            setRoutePoints(routePoints); // Save the route points for display
        } catch (error) {
            console.error("Failed to fetch route points:", error);
        }
    };

    const startIcon = L.icon({
        iconUrl: "https://img.icons8.com/?size=100&id=13800&format=png",
        iconSize: [33, 37],
        iconAnchor: [15, 45],
        popupAnchor: [0, -45],
    });

    const endIcon = L.icon({
        iconUrl: "https://img.icons8.com/?size=100&id=13800&format=png",
        iconSize: [33, 37],
        iconAnchor: [15, 45],
        popupAnchor: [0, -45],
    });


    const [markers, setMarkers] = useState<LatLng[]>([]);
    const [routeVisible, setRouteVisible] = useState(false);
    const [refreshKey, setRefreshKey] = useState(0); // Key to refresh RoutingMachine
    const [instructions, setInstructions] = useState(""); // Key to refresh RoutingMachine
    //const [data, setData] = useState<Locations[]>(); // Key to refresh RoutingMachine
    const [places, setPlacesData] = useState<Place[]>(); // Key to refresh RoutingMachine
    const [formData, setFormData] = useState(null);
    const [startMarker, setStartMarkers] = useState<LatLng | null>(null);
    const [endMarker, setEndMarkers] = useState<LatLng | null>(null);
    const [clickType, setClickType] = useState<MarkerType>(MarkerType.Start);
    const [,setAddressMap] = useState<Map<string, string>>(new Map());
    const [routePoints, setRoutePoints] = useState<Point[]>([]);

    const handleFormSubmit = (data: any) => {
        console.log("Received data from GeoApiForm:", data);
        setFormData(data); // Save or process the form data as needed
    };


    const handleRouteButtonClick = () => {
        if (markers.length > 0) {
            setRouteVisible(true); // Show the RoutingMachine when the button is clicked
            setRefreshKey((prev) => prev + 1); // Increment the key to refresh RoutingMachine
        }
    };

    function handleChangeClickType(type : MarkerType){
        console.log("Selected type:", type);
        setClickType(type)
    }
    
    // const handleFindBestRouteClick = async () =>{
    //    
    //     const locations = {
    //         locations : markers.map((marker) => ({ location: [marker.lng, marker.lat] }))
    //     }
    //     console.log("FindBestRouteClick " + JSON.stringify(locations));        
    //    
    //     const requestOptions = {
    //         method: 'POST',
    //         headers: {
    //             'Content-Type': 'application/json'
    //         },
    //         body: JSON.stringify(locations)
    //     }        
    //     await fetch('https://localhost:7148/api/geo/get-routes', requestOptions)
    //         .then(async response => {
    //             const data : Locations[] = await response.json();
    //
    //             console.log(JSON.stringify(data))
    //             setData(data);
    //             const latLngArray: LatLng[] = data.map(item => new LatLng(item.location[1], item.location[0]));
    //             setMarkers(latLngArray)
    //            
    //             console.log("markers is " + JSON.stringify(markers));
    //         });
    // }

    const fetchAddress = async (latlng: LatLng) => {
        const url = `https://nominatim.openstreetmap.org/reverse?format=json&lat=${latlng.lat}&lon=${latlng.lng}`;
        try {
            const response = await fetch(url);
            const data = await response.json();
            const address = data.display_name || "Address not found";
            setAddressMap((prev) => new Map(prev).set(latlng.toString(), address));
        } catch (error) {
            console.error("Failed to fetch address:", error);
        }
    };
    
    const handleSomeNewButtonClick = async () => {
        if (!formData) {
            console.warn("Form data is missing!");
            return;
        }
       console.log("data handler is " + JSON.stringify(formData));        
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        }

        try {
            const response = await fetch("https://localhost:7148/api/geo/get-places", requestOptions);

            if (!response.ok) {
                throw new Error(`Error: ${response.statusText}`);
            }

            const data: Place[] = await response.json();
            console.log("Response data:", JSON.stringify(data));
            setPlacesData(data); // Save API response data to display or use elsewhere
        } catch (error) {
            console.error("Failed to fetch places:", error);
        }
    };

    const isValidLatLng = (lat: number | undefined, lng: number | undefined): boolean => {
        return typeof lat === "number" && !isNaN(lat) && typeof lng === "number" && !isNaN(lng);
    };

    const validateRouteData = (
        startMarker: L.LatLng | null,
        endMarker: L.LatLng | null,
        routePoints: Point[]
    ): boolean => {
        const issues: string[] = [];

        console.log(JSON.stringify(routePoints))
        // Validate start marker
        if (!startMarker || !isValidLatLng(startMarker.lat, startMarker.lng)) {
            issues.push("Invalid start marker");
        }

        // Validate end marker
        if (!endMarker || !isValidLatLng(endMarker.lat, endMarker.lng)) {
            issues.push("Invalid end marker");
        }

        // Validate each route point
        routePoints.forEach((point, index) => {
            if (!isValidLatLng(point.latitude, point.longitude)) {
                issues.push(`Invalid route point at index ${index}: ${JSON.stringify(point)}`);
            }
        });

        // Log the results
        if (issues.length > 0) {
            console.error("Route data validation failed with issues:", issues);
            return false;
        }

        console.log("Route data is valid");
        return true;
    };

    
    function addInstructions(instructions : any){
        console.log("AddInstruct")
        setInstructions(instructions);
    }

    useEffect(() => {
        if (routeVisible && markers.length > 0) {
            setRefreshKey((prev) => prev + 1); // Refresh the RoutingMachine
        }
    }, [markers]);
   
    return (
        <>
        <div className="map-grid">
            <div className="map-container">
                <MapContainer center={[50.6199, 26.2516]} zoom={13} scrollWheelZoom={false}>
                    <TileLayer
                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                    <ClickHandler setMarkers={setMarkers}/>
                    <StartGeocoder setStartMarker={setStartMarkers} fetchAddress={fetchAddress} />
                    <EndGeocoder setEndMarker={setEndMarkers} fetchAddress={fetchAddress} />

                    {markers.length > 0 && routePoints.length < 1 && markers.map((position, idx) => (
                        <Marker key={idx} position={position}>
                            <Popup>
                                Marker {idx + 1}
                            </Popup>
                        </Marker>
                    ))}
                    {places !== undefined  && routePoints.length < 1 && places.map((position, idx) => (
                        <Marker key={markers.length + idx} position={new LatLng(position.lat, position.lon)}>
                            <Popup>
                                <h2>{position.name}</h2>
                                <p>{JSON.stringify(position.categories)}</p>
                            </Popup>
                        </Marker>
                    ))}

                    {routePoints
                        .map((point, idx) => (
                            <Marker
                                key={idx}
                                position={new LatLng(point.latitude, point.longitude)}
                            >
                                <Popup>
                                    <h3>{point.name}</h3>
                                    <p>Categories: {point.categories}</p>
                                </Popup>
                            </Marker>
                        ))}

                    {startMarker && (
                        <Marker position={startMarker} icon={startIcon}>
                            <Popup>{"Start Location"}</Popup>
                        </Marker>
                    )}

                    {endMarker && (
                        <Marker position={endMarker} icon={endIcon}>
                            <Popup>{"End Location"}</Popup>
                        </Marker>
                    )}

                    {/*{routeVisible && startMarker && endMarker && routePoints.length < 0 && <RoutingMachine*/}
                    {/*    key={refreshKey}*/}
                    {/*    waypoints={[startMarker, ...markers, endMarker]}*/}
                    {/*    onClick={addInstructions}/>}*/}

                    {routePoints.length > 0 && startMarker && endMarker && validateRouteData(startMarker, endMarker, routePoints) && (
                        <RoutingMachine
                            key={refreshKey}
                            waypoints={[
                                new L.LatLng(startMarker.lat, startMarker.lng),
                                ...routePoints.map((point) => new LatLng(point.latitude, point.longitude)),
                                new L.LatLng(endMarker.lat, endMarker.lng),
                            ]}
                            onClick={addInstructions}
                        />
                    )}
                </MapContainer>
            </div>
                <div className="button-container">
                    <TypeDropdown onChoose={handleChangeClickType}/>
                    
                    <Button onClick={handleRouteButtonClick} disabled={markers.length === 0}>
                        Run route
                    </Button>                    
                    
                    {routeVisible && (instructions !== null || instructions !== "") && (
                        <div className="route-display">
                            <DirectionList instructions={instructions} />
                        </div>
                    )}
                    <Button onClick={handleGetRoutePoints}>
                        Find route
                    </Button>
                    {/*{(data !== undefined) && (*/}
                    {/*    <div>*/}
                    {/*        <ul>*/}
                    {/*            {data.map((route, idx) =>(*/}
                    {/*                <li key={idx}>*/}
                    {/*                    {JSON.stringify(route)}*/}
                    {/*                </li>*/}
                    {/*            ))}*/}
                    {/*        </ul>*/}
                    {/*    </div>*/}
                    {/*)}    */}
                    <Button onClick={handleSomeNewButtonClick}>
                        Find Places
                    </Button>
                    <GeoApiProvider>
                        <GeoApiModal onSubmit={handleFormSubmit} 
                         initialBias={
                         startMarker && endMarker
                             ? calculateMidpoint(startMarker, endMarker)
                             : {
                                 longitude: markers[0]?.lng ?? startMarker?.lng ?? endMarker?.lng ?? 50.6199,
                                 latitude: markers[0]?.lat ?? startMarker?.lat ?? endMarker?.lat ?? 26.2516,
                             }}
                        initialFilter={{
                            longitude : markers[0]?.lng ?? startMarker?.lng ?? endMarker?.lng ?? 50.6199,
                            latitude : markers[0]?.lat ?? startMarker?.lat ?? endMarker?.lat ?? 26.2516,
                            radiusInMeters : 5000
                        }}
                        />
                    </GeoApiProvider>
                </div>
            </div>
        </>
    );
}

export default MapPage;