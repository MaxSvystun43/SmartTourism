// GeoApiContext.tsx
import React, { createContext, useState, useContext, ReactNode } from "react";
import {Bias, Filter} from "../../types/PlacesRequest.tsx";

interface GeoApiContextType {
    filter: Filter;
    setFilter: React.Dispatch<React.SetStateAction<Filter>>;
    bias: Bias;
    setBias: React.Dispatch<React.SetStateAction<Bias>>;
}

// Default values
const defaultFilter: Filter = { longitude: 0, latitude: 0, radiusInMeters: 0 };
const defaultBias: Bias = { longitude: 0, latitude: 0 };

// Create context
const GeoApiContext = createContext<GeoApiContextType | undefined>(undefined);

// Provider component
export const GeoApiProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
    const [filter, setFilter] = useState<Filter>(defaultFilter);
    const [bias, setBias] = useState<Bias>(defaultBias);

    return (
        <GeoApiContext.Provider value={{ filter, setFilter, bias, setBias }}>
            {children}
        </GeoApiContext.Provider>
    );
};

// Custom hook for easy access to context
export const useGeoApiContext = (): GeoApiContextType => {
    const context = useContext(GeoApiContext);
    if (!context) {
        throw new Error("useGeoApiContext must be used within a GeoApiProvider");
    }
    return context;
};
