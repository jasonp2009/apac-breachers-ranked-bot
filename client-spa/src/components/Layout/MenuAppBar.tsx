import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import Typography from '@mui/material/Typography';
import IconButton from '@mui/material/IconButton';
import MenuIcon from '@mui/icons-material/Menu';
import MenuItem from '@mui/material/MenuItem';
import Menu from '@mui/material/Menu';
import { useLogin } from "../../hooks";
import { getProfilePicture } from "../../hooks/Login/getProfilePicture";
import { Avatar, Button } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { RouteConstants } from "../../constants";

export function MenuAppBar(onClickHamburger?: () => void) {
  const [ anchorEl, setAnchorEl ] = React.useState<null | HTMLElement>(null);
  const [ label, setLabel ] = React.useState<string>("Home");
  const { isLoggedIn, user, login, logout } = useLogin();
  const navigate = useNavigate();
  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const bar = (
    <Box sx={{ flexGrow: 1 }}>
      <AppBar position="static">
        <Toolbar>
          <IconButton
            size="large"
            edge="start"
            color="inherit"
            aria-label="menu"
            sx={{ display: { xs: 'block', sm: 'none' }, mr: 2 }}
            onClick={() => {onClickHamburger && onClickHamburger()}}
          >
            <MenuIcon/>
          </IconButton>
          <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
            {label}
          </Typography>
          {isLoggedIn && (
            <div>
              <IconButton
                size="large"
                aria-label="account of current user"
                aria-controls="menu-appbar"
                aria-haspopup="true"
                onClick={handleMenu}
                color="inherit"
              >
                <Avatar src={getProfilePicture(user)}> </Avatar>
              </IconButton>
              <Menu
                id="menu-appbar"
                anchorEl={anchorEl}
                anchorOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                keepMounted
                transformOrigin={{
                  vertical: 'top',
                  horizontal: 'right',
                }}
                open={Boolean(anchorEl)}
                onClose={handleClose}
              >
                <MenuItem onClick={() => navigate(RouteConstants.Account)}>My account</MenuItem>
                <MenuItem onClick={logout}>Logout</MenuItem>
              </Menu>
            </div>
          )}
          {!isLoggedIn && (
            <Button color="secondary" variant="contained" onClick={login}>Login</Button>
          )}
        </Toolbar>
      </AppBar>
    </Box>
  );

  return {
    bar,
    setLabel
  }
}
