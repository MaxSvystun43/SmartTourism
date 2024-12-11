import { useState } from "react";
import { Checkbox, InputNumber, Button, Typography, Space } from "antd";

const { Title } = Typography;

function Settings() {
    // State for checkboxes
    const [checkboxes, setCheckboxes] = useState({
        restaurantVisit: false,
    });

    // State for number inputs
    const [numbers, setNumbers] = useState({
        lowerLimit: null,
        upperLimit: null,
    });

    // Handler for checkboxes
    const handleCheckboxChange = (name: string, checked: boolean) => {
        setCheckboxes((prevState) => ({
            ...prevState,
            [name]: checked,
        }));
    };

    // Handler for number inputs
    const handleNumberChange = (name: string, value: number | null) => {
        if (value !== null && value >= 0) {
            setNumbers((prevState) => ({
                ...prevState,
                [name]: value,
            }));
        } else {
            setNumbers((prevState) => ({
                ...prevState,
                [name]: null, // Reset to null if invalid
            }));
        }
    };

    // Handler for update button
    const handleUpdate = () => {
        console.log("Checkboxes:", checkboxes);
        console.log("Numbers:", numbers);
        alert("Settings updated!");
    };

    return (
        <div style={{ padding: "20px" }}>
            <Title level={2}>Settings Page</Title>

            <Space direction="vertical" size="large">
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
