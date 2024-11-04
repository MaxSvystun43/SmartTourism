import { Button} from "antd";
import {  MoonOutlined, SunOutlined } from '@ant-design/icons';
import React from "react";

interface ToggleThemeButtonProps {
    darkTheme: boolean;
    toggleTheme: () => void;
}

const ToggleThemeButton : React.FC<ToggleThemeButtonProps> = ({ darkTheme, toggleTheme }) =>{
    return(
        <div className="toggle-theme-btn">
            <Button onClick={toggleTheme}> {darkTheme ? <SunOutlined/> : <MoonOutlined/>}</Button>
        </div>
    )
}

export default ToggleThemeButton