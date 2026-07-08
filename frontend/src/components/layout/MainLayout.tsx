import { useState } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import { Box, Toolbar } from '@mui/material';
import Sidebar, { SIDEBAR_WIDTH } from './Sidebar';
import Navbar from './Navbar';

const TITLES: Record<string, string> = {
  '/dashboard': 'Dashboard',
  '/residents': 'Resident Management',
  '/requests': 'Collection Requests',
  '/schedules': 'Collection Schedule',
  '/complaints': 'Complaints',
  '/reports': 'Reports',
  '/profile': 'My Profile',
};

function resolveTitle(pathname: string): string {
  const match = Object.keys(TITLES).find((path) => pathname.startsWith(path));
  return match ? TITLES[match] : 'Garbage Management System';
}

export default function MainLayout() {
  const [mobileOpen, setMobileOpen] = useState(false);
  const location = useLocation();

  return (
    <Box sx={{ display: 'flex', minHeight: '100dvh' }}>
      <Navbar onMenuClick={() => setMobileOpen(true)} title={resolveTitle(location.pathname)} />
      <Sidebar mobileOpen={mobileOpen} onClose={() => setMobileOpen(false)} />

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          width: { md: `calc(100% - ${SIDEBAR_WIDTH}px)` },
          bgcolor: 'background.default',
          minHeight: '100dvh',
        }}
      >
        <Toolbar />
        <Box sx={{ p: { xs: 2, sm: 3 }, maxWidth: 1400, mx: 'auto' }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}
