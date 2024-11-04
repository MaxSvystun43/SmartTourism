import DirectionStep from "../types/DirectionStep.tsx";


const DirectionsList: React.FC<{ instructions:string }> = ({ instructions }) => {
    
    if (instructions !== null && instructions.length > 0) {
        const directionsArray: DirectionStep[] = JSON.parse(instructions) ?? [];
        console.log(JSON.stringify(directionsArray))
    return(
    <div>
        <h2>Route Directions</h2>
        {/*<ul>*/}
        {/*    {directionsArray.map((step, index) => (*/}
        {/*        <li key={index}>*/}
        {/*            <strong>{step.text}</strong> ({step.distance} m, {Math.round(step.time)} sec)*/}
        {/*        </li>*/}
        {/*    ))}*/}
        {/*</ul>*/}
    </div>
        
)}
else
    {
        return <div>No route instructions available.</div>
    }};

export default DirectionsList;