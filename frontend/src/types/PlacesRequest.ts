import Category from "./Categories.ts";

export interface PlacesRequest{
    limit : number;
    bias : Bias;
    filter : Filter;
    categories : Category[];
}


export interface Filter extends Bias{
    radiusInMeters : number;
}

export interface Bias {
    longitude : number;
    latitude : number;
}


export default PlacesRequest;
