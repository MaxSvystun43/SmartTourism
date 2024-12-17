import { useState, useEffect } from "react";
import { Checkbox, InputNumber, Button, Typography, Space, message } from "antd";

const { Title } = Typography;

interface Setting {
    visitRestaurant: boolean;
    lowerLimit: number | null;
    upperLimit: number | null;
}

interface NumberState {
    lowerLimit: number | null;
    upperLimit: number | null;
}

function Settings() {
    const [checkboxes, setCheckboxes] = useState({
        restaurantVisit: false,
    });

    const [numbers, setNumbers] = useState<NumberState>({
        lowerLimit: null,
        upperLimit: null,
    });

    const [loading, setLoading] = useState(false);

    // Fetch settings on component mount
    useEffect(() => {
        const fetchSettings = async () => {
            setLoading(true);
            try {
                const response = await fetch("https://localhost:7148/settings");
                if (!response.ok) throw new Error("Failed to fetch settings");

                const data: Setting = await response.json();

                // Set fetched data into state
                setCheckboxes({
                    restaurantVisit: data.visitRestaurant,
                });
                setNumbers({
                    lowerLimit: data.lowerLimit,
                    upperLimit: data.upperLimit,
                });
                message.success("Settings loaded successfully!");
            } catch (error) {
                message.error("Error loading settings.");
                console.error(error);
            } finally {
                setLoading(false);
            }
        };

        fetchSettings();
    }, []);

    // Handler for checkboxes
    const handleCheckboxChange = (name: string, checked: boolean) => {
        setCheckboxes((prevState) => ({
            ...prevState,
            [name]: checked,
        }));
    };

    // Handler for number inputs
    const handleNumberChange = (name: string, value: number | null) => {
        setNumbers((prevState) => ({
            ...prevState,
            [name]: value !== null && value >= 0 ? value : null,
        }));
    };

    // Handler for update button
    const handleUpdate = async () => {
        const updatedSettings: Setting = {
            visitRestaurant: checkboxes.restaurantVisit,
            lowerLimit: numbers.lowerLimit,
            upperLimit: numbers.upperLimit,
        };

        try {
            const response = await fetch("https://localhost:7148/add-settings", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(updatedSettings),
            });

            if (!response.ok) throw new Error("Failed to update settings");

            message.success("Settings updated successfully!");
        } catch (error) {
            message.error("Error updating settings.");
            console.error(error);
        }
    };

    return (
        <div style={{ padding: "20px" }}>
            <Title level={2}>Settings Page</Title>

            <Space direction="vertical" size="large">
                {/* Loading Indicator */}
                {loading ? <div>Loading settings...</div> : null}

                {/* Checkboxes */}
                <div>
                    <Checkbox
                        checked={checkboxes.restaurantVisit}
                        onChange={(e) => handleCheckboxChange("restaurantVisit", e.target.checked)}
                    >
                        Visit at least one restaurant
                    </Checkbox>
                </div>

                {/* Number inputs */}
                <div>
                    <label style={{ display: "block", marginBottom: "8px" }}>
                        Lower limit
                        <InputNumber
                            value={numbers.lowerLimit}
                            onChange={(value) => handleNumberChange("lowerLimit", value)}
                            min={0}
                            style={{ width: "100%", marginTop: "8px" }}
                        />
                    </label>
                    <label style={{ display: "block", marginBottom: "8px" }}>
                        Upper limit
                        <InputNumber
                            value={numbers.upperLimit}
                            onChange={(value) => handleNumberChange("upperLimit", value)}
                            min={0}
                            style={{ width: "100%", marginTop: "8px" }}
                        />
                    </label>
                </div>

                {/* Update button */}
                <Button type="primary" onClick={handleUpdate}>
                    Update
                </Button>
            </Space>
        </div>
    );
}

export default Settings;
