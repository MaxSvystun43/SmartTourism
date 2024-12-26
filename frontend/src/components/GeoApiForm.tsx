import React from "react";
import {Form, Button, InputNumber, Select} from "antd";
import Category from "../types/Categories";
import PlacesRequest from "../types/PlacesRequest.ts";
import GeoApiFormProps from "../types/GeoApiFormProp.ts";

// Define types for Filter, Bias, and GeoApiRequest

const GeoApiForm: React.FC<GeoApiFormProps> = ({ initialBias, initialFilter, onSubmit }) => {
    const [form] = Form.useForm();
    const categories = Object.values(Category);

    const onFinish = (values: any) => {
        // Map form values to GeoApiRequest structure
        const geoApiRequest: PlacesRequest = {
            categories: values.Categories,
            filter: initialFilter,
            bias: initialBias,
            limit: values.Limit,
        };

        console.log("GeoApiRequest:", geoApiRequest);
        // Send geoApiRequest to backend API as needed
        onSubmit(geoApiRequest);
    };

    return (
        <Form
            form={form}
            layout="vertical"
            onFinish={onFinish}
            initialValues={{
                Filter: initialFilter,
                Bias: initialBias,
                Limit: 10,
                Categories: [],
            }}
        >
            <Form.Item label="Select Categories" name="Categories">
                <Select
                    mode="multiple"
                    placeholder="Select categories"
                    options={categories.map((category) => ({
                        label: category,
                        value: category,
                    }))}
                />
            </Form.Item>
            
            <h3>Limit</h3>
            <Form.Item
                label="Limit"
                name="Limit"
                rules={[{ required: true, message: "Please enter limit" }]}
            >
                <InputNumber min={1} placeholder="Enter limit" />
            </Form.Item>

            <Form.Item>
                <Button type="primary" htmlType="submit">
                    Submit
                </Button>
            </Form.Item>
        </Form>
    );
};

export default GeoApiForm;
