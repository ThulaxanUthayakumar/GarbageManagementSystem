import { useNavigate, useLocation } from 'react-router-dom';
import {
  Box,
  Drawer,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Typography,
  Divider,
  useTheme,
} from '@mui/material';
import DashboardRoundedIcon from '@mui/icons-material/DashboardRounded';
import GroupsRoundedIcon from '@mui/icons-material/GroupsRounded';
import LocalShippingRoundedIcon from '@mui/icons-material/LocalShippingRounded';
import EventRoundedIcon from '@mui/icons-material/EventRounded';
import ReportProblemRoundedIcon from '@mui/icons-material/ReportProblemRounded';
import BarChartRoundedIcon from '@mui/icons-material/BarChartRounded';
import PersonRoundedIcon from '@mui/icons-material/PersonRounded';
import RecyclingRoundedIcon from '@mui/icons-material/RecyclingRounded';
import { useAuth } from '../../hooks/useAuth';

export const SIDEBAR_WIDTH = 260;

interface NavItem {
  label: string;
  path: string;
  icon: React.ReactNode;
  adminOnly?: boolean;
  residentOnly?: boolean;
}

const navItems: NavItem[] = [
  { label: 'Dashboard', path: '/dashboard', icon: <DashboardRoundedIcon /> },
  { label: 'Residents', path: '/residents', icon: <GroupsRoundedIcon />, adminOnly: true },
  { label: 'Collection Requests', path: '/requests', icon: <LocalShippingRoundedIcon /> },
  { label: 'Schedule', path: '/schedules', icon: <EventRoundedIcon /> },
  { label: 'Complaints', path: '/complaints', icon: <ReportProblemRoundedIcon /> },
  { label: 'Reports', path: '/reports', icon: <BarChartRoundedIcon />, adminOnly: true },
  { label: 'Profile', path: '/profile', icon: <PersonRoundedIcon /> },
];

interface SidebarProps {
  mobileOpen: boolean;
  onClose: () => void;
}

export default function Sidebar({ mobileOpen, onClose }: SidebarProps) {
  const theme = useTheme();
  const navigate = useNavigate();
  const location = useLocation();
  const { isAdmin } = useAuth();

  const visibleItems = navItems.filter((item) => {
    if (item.adminOnly && !isAdmin) return false;
    if (item.residentOnly && isAdmin) return false;
    return true;
  });

  const content = (
    <Box
      sx={{
        height: '100%',
        display: 'flex',
        flexDirection: 'column',
        bgcolor: theme.palette.mode === 'light' ? '#0F6B52' : '#0A211B',
        color: '#FFFFFF',
      }}
    >
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.25, px: 3, py: 3 }}>
        <Box
          sx={{
            width: 38,
            height: 38,
            borderRadius: '10px',
            bgcolor: 'rgba(255,255,255,0.14)',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
          }}
        >
          <RecyclingRoundedIcon sx={{ color: '#fff', fontSize: 22 }} />
        </Box>
        <Box>
          <Typography sx={{ fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 800, fontSize: '1.02rem', lineHeight: 1.15 }}>
            GarbageMS
          </Typography>
          <Typography sx={{ fontSize: '0.68rem', opacity: 0.7, letterSpacing: '0.03em' }}>
            Collection & Complaints
          </Typography>
        </Box>
      </Box>

      <Divider sx={{ borderColor: 'rgba(255,255,255,0.12)', mx: 2 }} />

      <List sx={{ px: 1.5, py: 2, flex: 1 }}>
        {visibleItems.map((item) => {
          const isActive = location.pathname.startsWith(item.path);
          return (
            <ListItemButton
              key={item.path}
              onClick={() => {
                navigate(item.path);
                onClose();
              }}
              sx={{
                borderRadius: 2,
                mb: 0.5,
                py: 1.1,
                color: isActive ? '#FFFFFF' : 'rgba(255,255,255,0.72)',
                bgcolor: isActive ? 'rgba(255,255,255,0.14)' : 'transparent',
                '&:hover': { bgcolor: 'rgba(255,255,255,0.10)' },
              }}
            >
              <ListItemIcon sx={{ minWidth: 38, color: 'inherit' }}>{item.icon}</ListItemIcon>
              <ListItemText
                primary={item.label}
                primaryTypographyProps={{ fontSize: '0.875rem', fontWeight: isActive ? 700 : 500 }}
              />
            </ListItemButton>
          );
        })}
      </List>

      <Box sx={{ px: 3, py: 2.5, opacity: 0.6 }}>
        <Typography sx={{ fontSize: '0.7rem' }}>v1.0.0 &middot; Junior Developer Portfolio</Typography>
      </Box>
    </Box>
  );

  return (
    <Box component="nav" sx={{ width: { md: SIDEBAR_WIDTH }, flexShrink: { md: 0 } }}>
      {/* Mobile: temporary overlay drawer */}
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={onClose}
        ModalProps={{ keepMounted: true }}
        sx={{
          display: { xs: 'block', md: 'none' },
          '& .MuiDrawer-paper': { width: SIDEBAR_WIDTH, border: 'none' },
        }}
      >
        {content}
      </Drawer>

      {/* Desktop: permanent drawer */}
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: 'none', md: 'block' },
          '& .MuiDrawer-paper': { width: SIDEBAR_WIDTH, border: 'none' },
        }}
        open
      >
        {content}
      </Drawer>
    </Box>
  );
}
