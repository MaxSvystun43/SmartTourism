import { useState } from "react";
import { Modal, Button } from "antd";
import GeoApiForm from "./GeoApiForm";
import GeoApiFormProps from "../types/GeoApiFormProp.ts"; // Adjust the import path as needed

const GeoApiModal: React.FC<GeoApiFormProps> = ({ initialBias, initialFilter, onSubmit }) => {
    const [isModalVisible, setIsModalVisible] = useState(false);

    const showModal = () => {
        setIsModalVisible(true);
    };

    const handleCancel = () => {
        setIsModalVisible(false);
    };

    const handleFormSubmit = (data: any) => {
        onSubmit(data); // Pass the submitted data to the parent component
        setIsModalVisible(false); // Close the modal
    };

    return (
        <>
            <Button type="primary" onClick={showModal}>
                Open GeoApi Form
            </Button>
            <Modal
                title="GeoApi Request Form"
                visible={isModalVisible}
                onCancel={handleCancel}
                footer={null} // Footer is handled by the form's submit button
            >
                <GeoApiForm   initialBias={initialBias}       // Pass bias as initial values
                              initialFilter={initialFilter}    // Pass filter as initial values
                              onSubmit={handleFormSubmit} // Handle data when form is submitted 
                />
            </Modal>
        </>
    );
};

export default GeoApiModal;
