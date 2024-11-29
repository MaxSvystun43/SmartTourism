import React, {useState} from "react";
import {MarkerType} from "../types/MarkerType.ts";

interface TypeDropdownProps {
    onChoose: (value: MarkerType) => void;
}

function TypeDropdown({ onChoose }: TypeDropdownProps) {
    const [selectedType, setSelectedType] = useState<MarkerType>(MarkerType.Start); // Default value set to "start"

    const handleChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        setSelectedType(event.target.value as MarkerType);
        onChoose(event.target.value as MarkerType);
    };

    return (
        <div>
            <label htmlFor="type-dropdown" style={{marginRight: "10px"}}>
                Select Type:
            </label>
            <select
                id="type-dropdown"
                value={selectedType}
                onChange={handleChange}
                style={{padding: "5px", borderRadius: "4px", border: "1px solid #ccc"}}
            >
                <option value={MarkerType.Start}>Start</option>
                <option value={MarkerType.End}>End</option>
                <option value={MarkerType.Waypoint}>Waypoint</option>
            </select>

            {selectedType && (
                <p style={{marginTop: "10px"}}>
                    Selected Type: <strong>{selectedType}</strong>
                </p>
            )}
        </div>
    );
}

export default TypeDropdown;
