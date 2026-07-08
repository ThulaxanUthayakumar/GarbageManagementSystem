import { Card, Box, Typography, useTheme } from '@mui/material';
import { ResponsiveContainer, PieChart, Pie, Cell, Tooltip, Legend } from 'recharts';
import type { CategoryCount } from '../../types';

const PALETTE = ['#0F6B52', '#3E6EA8', '#B8790F', '#C24A2E', '#6B4FA0', '#1E8E5A'];

interface CategoryPieChartCardProps {
  title: string;
  data: CategoryCount[];
  height?: number;
}

export default function CategoryPieChartCard({ title, data, height = 280 }: CategoryPieChartCardProps) {
  const theme = useTheme();
  const hasData = data.some((d) => d.count > 0);

  return (
    <Card variant="outlined" sx={{ p: 2.5, height: '100%' }}>
      <Typography variant="subtitle1" sx={{ mb: 1.5 }}>
        {title}
      </Typography>

      {hasData ? (
        <ResponsiveContainer width="100%" height={height}>
          <PieChart>
            <Pie
              data={data}
              dataKey="count"
              nameKey="category"
              innerRadius="55%"
              outerRadius="82%"
              paddingAngle={2}
            >
              {data.map((_, index) => (
                <Cell key={index} fill={PALETTE[index % PALETTE.length]} stroke="none" />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                borderRadius: 8,
                border: `1px solid ${theme.palette.divider}`,
                background: theme.palette.background.paper,
                fontSize: 13,
              }}
            />
            <Legend wrapperStyle={{ fontSize: 12 }} layout="horizontal" verticalAlign="bottom" />
          </PieChart>
        </ResponsiveContainer>
      ) : (
        <Box sx={{ height, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            No data yet.
          </Typography>
        </Box>
      )}
    </Card>
  );
}
