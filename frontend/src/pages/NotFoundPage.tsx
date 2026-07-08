import { Box, Typography, Button } from '@mui/material';
import { useNavigate } from 'react-router-dom';
import SentimentDissatisfiedRoundedIcon from '@mui/icons-material/SentimentDissatisfiedRounded';

export default function NotFoundPage() {
  const navigate = useNavigate();

  return (
    <Box sx={{ minHeight: '100dvh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', p: 3, textAlign: 'center' }}>
      <SentimentDissatisfiedRoundedIcon sx={{ fontSize: 56, color: 'text.disabled', mb: 2 }} />
      <Typography variant="h4" sx={{ mb: 1 }}>
        404 — Page not found
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3, maxWidth: 380 }}>
        The page you're looking for doesn't exist or may have been moved.
      </Typography>
      <Button variant="contained" onClick={() => navigate('/dashboard')}>
        Back to Dashboard
      </Button>
    </Box>
  );
}
