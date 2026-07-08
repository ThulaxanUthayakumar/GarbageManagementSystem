import { Card, Box, Typography, useTheme } from '@mui/material';
import type { ReactNode } from 'react';

interface StatCardProps {
  label: string;
  value: number | string;
  icon: ReactNode;
  color?: string;
  suffix?: string;
}

export default function StatCard({ label, value, icon, color, suffix }: StatCardProps) {
  const theme = useTheme();
  const accent = color ?? theme.palette.primary.main;

  return (
    <Card
      variant="outlined"
      sx={{
        p: 2.25,
        display: 'flex',
        alignItems: 'center',
        gap: 2,
        height: '100%',
        borderLeft: `3px solid ${accent}`,
      }}
    >
      <Box
        sx={{
          width: 44,
          height: 44,
          borderRadius: '12px',
          bgcolor: `${accent}18`,
          color: accent,
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
          flexShrink: 0,
        }}
      >
        {icon}
      </Box>
      <Box sx={{ minWidth: 0 }}>
        <Typography variant="h5" className="tabular-nums" sx={{ lineHeight: 1.1 }}>
          {value}
          {suffix && (
            <Typography component="span" variant="body2" color="text.secondary" sx={{ ml: 0.5 }}>
              {suffix}
            </Typography>
          )}
        </Typography>
        <Typography variant="body2" color="text.secondary" noWrap>
          {label}
        </Typography>
      </Box>
    </Card>
  );
}
