import { Box, CircularProgress, Typography } from '@mui/material';

interface LoadingSpinnerProps {
  label?: string;
  minHeight?: number | string;
}

export default function LoadingSpinner({ label = 'Loading...', minHeight = 240 }: LoadingSpinnerProps) {
  return (
    <Box
      sx={{
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        gap: 1.5,
        minHeight,
        width: '100%',
      }}
    >
      <CircularProgress size={32} thickness={4} />
      <Typography variant="body2" color="text.secondary">
        {label}
      </Typography>
    </Box>
  );
}
