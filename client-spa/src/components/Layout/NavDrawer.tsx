import React from 'react';
import Box from '@mui/material/Box';
import Drawer from '@mui/material/Drawer';
import List from '@mui/material/List';
import ListItem from '@mui/material/ListItem';
import ListItemButton from '@mui/material/ListItemButton';
import ListItemText from '@mui/material/ListItemText';
import { To, useNavigate } from 'react-router-dom';
import { SwipeableDrawer } from "@mui/material";

export type NavItem = {
  label: string;
  link: To;
}

const drawerWidth = 240;

export function NavDrawer(navItems: NavItem[]) {
  const [isOpen, setIsOpen] = React.useState(false);
  const navigate = useNavigate();
  const toggleDrawer = (newOpen?: boolean) => () => {
    if (newOpen === undefined) {
      newOpen = !isOpen;
    }
    setIsOpen(newOpen);
  };

  const DrawerList = (
    <Box sx={{ width: drawerWidth }} role="presentation" onClick={toggleDrawer(false)}>
      <List>
        {navItems.map((navItem) => (
          <ListItem key={navItem.label} disablePadding>
            <ListItemButton
              onClick={() => navigate(navItem.link)}
            >
              <ListItemText primary={navItem.label}/>
            </ListItemButton>
          </ListItem>
        ))}
      </List>
    </Box>
  );
  
  const drawer = (
    <div>
      <SwipeableDrawer
        open={isOpen}
        onClose={toggleDrawer(false)}
        onOpen={toggleDrawer(true)}
        variant="temporary"
        sx={{
          display: { xs: 'block', sm: 'none' },
          '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
        }}>
        {DrawerList}
      </SwipeableDrawer>
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: 'none', sm: 'block' },
          '& .MuiDrawer-paper': { boxSizing: 'border-box', width: drawerWidth },
        }}
        open
      >
        {DrawerList}
      </Drawer>
    </div>
  )

  return {
    isOpen,
    toggleDrawer,
    drawer,
    drawerWidth
  };
}
