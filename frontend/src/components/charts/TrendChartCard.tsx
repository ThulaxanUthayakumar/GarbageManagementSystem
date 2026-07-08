import { Card, Box, Typography, useTheme } from '@mui/material';
import {
  ResponsiveContainer,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  Legend,
} from 'recharts';

interface SeriesDef {
  key: string;
  label: string;
  color: string;
}

interface TrendChartCardProps {
  title: string;
  subtitle?: string;
  data: Record<string, string | number>[];
  xKey: string;
  series: SeriesDef[];
  height?: number;
}

export default function TrendChartCard({ title, subtitle, data, xKey, series, height = 300 }: TrendChartCardProps) {
  const theme = useTheme();
  const hasData = data.length > 0 && data.some((d) => series.some((s) => Number(d[s.key]) > 0));

  return (
    <Card variant="outlined" sx={{ p: 2.5, height: '100%' }}>
      <Typography variant="subtitle1" sx={{ mb: subtitle ? 0.25 : 1.5 }}>
        {title}
      </Typography>
      {subtitle && (
        <Typography variant="body2" color="text.secondary" sx={{ mb: 1.5 }}>
          {subtitle}
        </Typography>
      )}

      {hasData ? (
        <ResponsiveContainer width="100%" height={height}>
          <BarChart data={data} margin={{ top: 4, right: 8, left: -16, bottom: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke={theme.palette.divider} vertical={false} />
            <XAxis dataKey={xKey} tick={{ fontSize: 12, fill: theme.palette.text.secondary }} axisLine={false} tickLine={false} />
            <YAxis tick={{ fontSize: 12, fill: theme.palette.text.secondary }} axisLine={false} tickLine={false} allowDecimals={false} />
            <Tooltip
              contentStyle={{
                borderRadius: 8,
                border: `1px solid ${theme.palette.divider}`,
                background: theme.palette.background.paper,
                fontSize: 13,
              }}
            />
            <Legend wrapperStyle={{ fontSize: 12 }} />
            {series.map((s) => (
              <Bar key={s.key} dataKey={s.key} name={s.label} fill={s.color} radius={[4, 4, 0, 0]} maxBarSize={36} />
            ))}
          </BarChart>
        </ResponsiveContainer>
      ) : (
        <Box sx={{ height, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            No data yet — this will fill in as collections happen.
          </Typography>
        </Box>
      )}
    </Card>
  );
}
