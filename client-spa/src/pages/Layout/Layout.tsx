import { Outlet } from "react-router-dom";
import { NavDrawer, MenuAppBar } from "../../components";
import './Layout.css';
import Box from "@mui/material/Box";
import { RouteConstants } from "../../constants";
export function Layout() {
  const navDrawer = NavDrawer(
    [
      {
        label: 'MatchQueue',
        link: '/'
      },
      {
        label: 'Stats',
        link: RouteConstants.Stats
      },
      {
        label: 'Match History',
        link: RouteConstants.MatchHistory
      }
    ]
  );
  const appBar = MenuAppBar(navDrawer.toggleDrawer());
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        flexWrap: 'nowrap'
      }}
    >
      <Box
        sx={{ marginLeft: { sm: `${navDrawer.drawerWidth}px` } }}
      >
        {appBar.bar}
      </Box>
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
          sx={{ flexGrow: 1, p: 3 }}
          >
          <Outlet/>
        </Box>
      </Box>
    </Box>
  );
}
