import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  AppBar,
  Toolbar,
  IconButton,
  Typography,
  Box,
  Avatar,
  Menu,
  MenuItem,
  ListItemIcon,
  Divider,
  Chip,
  Tooltip,
} from '@mui/material';
import MenuRoundedIcon from '@mui/icons-material/MenuRounded';
import LightModeRoundedIcon from '@mui/icons-material/LightModeRounded';
import DarkModeRoundedIcon from '@mui/icons-material/DarkModeRounded';
import PersonRoundedIcon from '@mui/icons-material/PersonRounded';
import LogoutRoundedIcon from '@mui/icons-material/LogoutRounded';
import { useColorMode } from '../../theme/ThemeContext';
import { useAuth } from '../../hooks/useAuth';
import { SIDEBAR_WIDTH } from './Sidebar';

interface NavbarProps {
  onMenuClick: () => void;
  title: string;
}

function getInitials(name: string) {
  return name
    .split(' ')
    .map((part) => part[0])
    .slice(0, 2)
    .join('')
    .toUpperCase();
}

export default function Navbar({ onMenuClick, title }: NavbarProps) {
  const { mode, toggleColorMode } = useColorMode();
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  const handleLogout = () => {
    setAnchorEl(null);
    logout();
    navigate('/login');
  };

  return (
    <AppBar
      position="fixed"
      elevation={0}
      sx={{
        width: { md: `calc(100% - ${SIDEBAR_WIDTH}px)` },
        ml: { md: `${SIDEBAR_WIDTH}px` },
        bgcolor: 'background.paper',
        color: 'text.primary',
        borderBottom: '1px solid',
        borderColor: 'divider',
      }}
    >
      <Toolbar sx={{ gap: 1 }}>
        <IconButton edge="start" onClick={onMenuClick} sx={{ display: { md: 'none' } }}>
          <MenuRoundedIcon />
        </IconButton>

        <Typography variant="h6" sx={{ flexGrow: 1, fontSize: '1.05rem' }}>
          {title}
        </Typography>

        <Tooltip title={mode === 'light' ? 'Switch to dark mode' : 'Switch to light mode'}>
          <IconButton onClick={toggleColorMode} aria-label="Toggle color mode">
            {mode === 'light' ? <DarkModeRoundedIcon /> : <LightModeRoundedIcon />}
          </IconButton>
        </Tooltip>

        <Chip
          size="small"
          label={isAdmin ? 'Admin' : 'Resident'}
          color={isAdmin ? 'secondary' : 'primary'}
          sx={{ display: { xs: 'none', sm: 'inline-flex' }, fontWeight: 700 }}
        />

        <IconButton onClick={(e) => setAnchorEl(e.currentTarget)} sx={{ ml: 0.5 }} aria-label="Account menu">
          <Avatar sx={{ width: 34, height: 34, bgcolor: 'primary.main', fontSize: '0.85rem', fontWeight: 700 }}>
            {user ? getInitials(user.fullName) : '?'}
          </Avatar>
        </IconButton>

        <Menu anchorEl={anchorEl} open={!!anchorEl} onClose={() => setAnchorEl(null)}>
          <Box sx={{ px: 2, py: 1 }}>
            <Typography variant="subtitle2" noWrap>
              {user?.fullName}
            </Typography>
            <Typography variant="caption" color="text.secondary" noWrap>
              {user?.email}
            </Typography>
          </Box>
          <Divider />
          <MenuItem
            onClick={() => {
              setAnchorEl(null);
              navigate('/profile');
            }}
          >
            <ListItemIcon>
              <PersonRoundedIcon fontSize="small" />
            </ListItemIcon>
            Profile
          </MenuItem>
          <MenuItem onClick={handleLogout}>
            <ListItemIcon>
              <LogoutRoundedIcon fontSize="small" />
            </ListItemIcon>
            Logout
          </MenuItem>
        </Menu>
      </Toolbar>
    </AppBar>
  );
}
