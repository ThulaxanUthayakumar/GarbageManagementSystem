import { Box, Typography, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import BlockRoundedIcon from '@mui/icons-material/BlockRounded';

export default function UnauthorizedPage() {
  const navigate = useNavigate();

  return (
    <Box sx={{ minHeight: '100dvh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', p: 3, textAlign: 'center' }}>
      <BlockRoundedIcon sx={{ fontSize: 56, color: 'error.main', mb: 2 }} />
      <Typography variant="h4" sx={{ mb: 1 }}>
        403 — Access restricted
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3, maxWidth: 380 }}>
        You don't have permission to view this page with your current account role.
      </Typography>
      <Button variant="contained" onClick={() => navigate('/dashboard')}>
        Back to Dashboard
      </Button>
    </Box>
  );
}
