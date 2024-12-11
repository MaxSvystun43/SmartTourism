import {useState} from "react";

function Home () {
    const [location, setLocation] = useState(null);

    function handleLocationClick() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(success, error);
        } else {
            console.log("Geolocation not supported");
        }
    }

    function success(position) {
        const latitude = position.coords.latitude;
        const longitude = position.coords.longitude;
        setLocation({latitude, longitude});
        console.log(`Latitude: ${latitude}, Longitude: ${longitude}`);
    }

    function error() {
        console.log("Unable to retrieve your location");
    }
    
    return (
        <div>
            {!location ? <button onClick={handleLocationClick}>Get Location</button> : null}
        </div>
    )
}

export default Home;
