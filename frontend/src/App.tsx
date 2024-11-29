import './App.css'
import {Button, Layout, theme} from "antd";
import Logo from "./components/Logo.tsx";
import MenuList from "./components/MenuList.tsx";
import {useState} from "react";
import ToggleThemeButton from "./components/ToggleThemeButton.tsx";
import { MenuUnfoldOutlined, MenuFoldOutlined} from '@ant-design/icons'
import {Route, Routes} from "react-router-dom";
import Home from "./pages/Home.tsx";
import Settings from "./pages/Settings.tsx";
import Map from "./pages/Map.tsx";
import {DiIntellij} from "react-icons/di";
import DijstraTest from "./pages/DijstraTest.tsx";


const { Header, Sider} = Layout;




function App() {
    const [darkTheme, setDarkTheme] = useState(true);
    const [collapsed, setCollapsed] = useState(false);

   
    const toggleTheme = () => {
        setDarkTheme(!darkTheme);
    };
    const { 
        token : { colorBgContainer },
        } = theme.useToken();


    
    return (
    <>
        <Layout>
            <Sider 
                collapsed={collapsed} 
                theme={darkTheme ? 'dark' : 'light'} 
                className='sidebar'>
                <Logo />
                <MenuList darkTheme={darkTheme}/>
                <ToggleThemeButton darkTheme={darkTheme} toggleTheme={toggleTheme}/>
            </Sider>
            <Layout>
                <Header style={{padding:0, background : colorBgContainer}}>
                    <Button 
                        className="toggle" 
                        onClick={() => setCollapsed(!collapsed)} 
                        type='text' 
                        icon={collapsed ? <MenuUnfoldOutlined/> : <MenuFoldOutlined/>} />
                </Header>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/map" element={<Map/>} />
                    <Route path="/dijstra-test" element={<DijstraTest/>} />
                    {/*<Route path="/find-route" element={<FindRoute/>} />*/}
                    <Route path="/settings" element={<Settings />} />
                </Routes>

            </Layout>
        </Layout>
        
    </>
    )
}

export default App
