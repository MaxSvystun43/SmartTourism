﻿import {Menu} from "antd";
import { AppstoreOutlined, SettingOutlined} from '@ant-design/icons';
import React from "react";
import { Link } from "react-router-dom";

interface MenuListProps {
    darkTheme: boolean;
}
const MenuList : React.FC<MenuListProps> = ({darkTheme}) => {
    return(
        <Menu theme={darkTheme ? 'dark' : 'light'} className="menu-bar">
            <Menu.Item key="map" icon={<AppstoreOutlined />}>
                <Link to="/">
                    Map
                </Link>
            </Menu.Item>
            <Menu.Item key="dijstra-test" icon={<AppstoreOutlined />}>
                <Link to="/dijstra-test">
                    Shortest Path
                </Link>
            </Menu.Item>
            <Menu.Item key="settings" icon={<SettingOutlined />}>
                <Link to="/settings">
                    Settings
                </Link>
            </Menu.Item>
        </Menu>
    )
}

export default MenuList