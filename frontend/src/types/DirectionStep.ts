interface DirectionStep {
    type: string;
    distance: number;
    time: number;
    road?: string;
    direction: string;
    index: number;
    mode: string;
    modifier?: string;
    text: string;
    exit?: number;
}

export default DirectionStep