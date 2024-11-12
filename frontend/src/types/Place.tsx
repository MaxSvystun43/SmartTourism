import Category from "./Categories.tsx";

interface Place{
    name : string;
    lon : number;
    lat : number;
    categories : Category[];
    details : string[];
    openingHours : string;
}

export default Place;