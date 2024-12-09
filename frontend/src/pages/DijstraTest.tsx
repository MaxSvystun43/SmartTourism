import {MapContainer, Marker, Popup, TileLayer, useMapEvents} from "react-leaflet";
import RoutingMachine from "../components/RoutineMachine.tsx";
import {LatLng} from "leaflet";
import React, {useEffect, useState} from "react";
import 'leaflet/dist/leaflet.css';
import {Button} from "antd";
import DirectionList from "../components/DirectionList.tsx";
import Locations from "../types/Locations.ts";
import TypeDropdown from "../components/TypeDropdown.tsx";
import {MarkerType} from "../types/MarkerType.ts";
import "leaflet/dist/leaflet.css";
import "leaflet-control-geocoder/dist/Control.Geocoder.css";
import GeocoderComponent from "../components/GeocoderComponent.tsx";
import StartGeocoder from "../components/StartGeocoder.tsx";
import EndGeocoder from "../components/EndGeocoder.tsx";
import L from 'leaflet';
import '../styles/Geocoder.css';




function DijstraMapTest() {
    const ClickHandler = ({ setMarkers }: { setMarkers: React.Dispatch<React.SetStateAction<LatLng[]>> }) => {
        // Hook to capture map events
        useMapEvents({
            click(e) {
                if (clickType == MarkerType.Waypoint) {
                    const newMarker = e.latlng; // Get clicked LatLng
                    setMarkers((prevMarkers) => [...prevMarkers, newMarker]); // Add new marker to array
                    fetchAddress(newMarker);
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
    const [startMarker, setStartMarkers] = useState<LatLng | null>(null);
    const [endMarker, setEndMarkers] = useState<LatLng | null>(null);
    const [routeVisible, setRouteVisible] = useState(false);
    const [refreshKey, setRefreshKey] = useState(0); // Key to refresh RoutingMachine
    const [instructions, setInstructions] = useState(""); // Key to refresh RoutingMachine
    const [data, setData] = useState<Locations[]>(); // Key to refresh RoutingMachine
    const [clickType, setClickType] = useState<MarkerType>(MarkerType.Start);
    const [addressMap, setAddressMap] = useState<Map<string, string>>(new Map());


    function handleChangeClickType(type : MarkerType){
        console.log("Selected type:", type);
        setClickType(type)
    }

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
    
    const handleRouteButtonClick = () => {
        if (markers.length > 0) {
            setRouteVisible(true); // Show the RoutingMachine when the button is clicked
            setRefreshKey((prev) => prev + 1); // Increment the key to refresh RoutingMachine
        }
    };
    
    const handleFindBestRouteClick = async () =>{
        
        const locations = {
            locations : markers.map((marker) => ({ location: [marker.lng, marker.lat] }))
        }
        console.log("FindBestRouteClick " + JSON.stringify(locations));        
        
        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(locations)
        }        
        await fetch('https://localhost:7148/api/geo/get-routes', requestOptions)
            .then(async response => {
                const data : Locations[] = await response.json();

                console.log(JSON.stringify(data))
                setData(data);
                const latLngArray: LatLng[] = data.map(item => new LatLng(item.location[1], item.location[0]));
                setMarkers(latLngArray)
                
                console.log("markers is " + JSON.stringify(markers));
            });
    }
    
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
                <MapContainer center={[50.6199, 26.2516]} zoom={13}>
                    <TileLayer
                        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
                        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                    />
                    <ClickHandler setMarkers={setMarkers}/>
                    <GeocoderComponent setMarkers={setMarkers} fetchAddress={fetchAddress} />
                    <StartGeocoder setStartMarker={setStartMarkers} fetchAddress={fetchAddress} />
                    <EndGeocoder setEndMarker={setEndMarkers} fetchAddress={fetchAddress} />
                    {markers.map((position, idx) => (
                        <Marker key={idx} position={position}>
                            <Popup>{addressMap.get(position.toString()) || "Loading address..."}</Popup>
                        </Marker>
                    ))}                 
                    {routeVisible && startMarker && endMarker && <RoutingMachine 
                        key={refreshKey} 
                        waypoints={[startMarker, ...markers, endMarker]} 
                        onClick={addInstructions}/>}

                    {startMarker && (
                        <Marker position={startMarker} icon={startIcon}>
                            <Popup>{addressMap.get(startMarker.toString()) || "Start Location"}</Popup>
                        </Marker>
                    )}

                    {endMarker && (
                        <Marker position={endMarker} icon={endIcon}>
                            <Popup>{addressMap.get(endMarker.toString()) || "End Location"}</Popup>
                        </Marker>
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
                    <Button onClick={handleFindBestRouteClick}>
                        Find route
                    </Button>
                    {(data !== undefined) && (
                        <div>
                            <ul>
                                {data.map((route, idx) =>(
                                    <li key={idx}>
                                        {JSON.stringify(route)}
                                    </li>
                                ))}
                            </ul>
                        </div>
                    )}    
                 </div>
            </div>
        </>
    );
}

export default DijstraMapTest;