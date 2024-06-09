import { Outlet } from "react-router-dom";
import { NavDrawer } from "../../components";
import './Layout.css';
import Box from "@mui/material/Box";
export function Layout() {
  const navDrawer = NavDrawer(
    [
      {
        label: 'MatchQueue',
        link: '/'
      },
      {
        label: 'Stats',
        link: '/stats'
      }
    ]
  )
    return (
      <Box sx={{ display: 'flex' }}>
        <Box
          component="nav"
          sx={{ width: { sm: navDrawer.drawerWidth }, flexShrink: { sm: 0 } }}
          aria-label="mailbox folders"
        >
          <nav>{navDrawer.drawer}</nav>
        </Box>
        <Box
          component="main"
          sx={{ flexGrow: 1, p: 3, width: { sm: `calc(100% - ${navDrawer.drawerWidth}px)` } }}
          >
          <Outlet/>
        </Box>
      </Box>
    );
}
