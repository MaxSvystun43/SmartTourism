import PlacesRequest, {Bias, Filter} from "./PlacesRequest.ts";

interface GeoApiFormProps {
    initialBias: Bias;
    initialFilter: Filter;
    onSubmit: (data: PlacesRequest) => void;
}

export default GeoApiFormProps;