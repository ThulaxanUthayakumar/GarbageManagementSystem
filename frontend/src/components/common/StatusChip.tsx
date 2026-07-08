import { Chip } from '@mui/material';
import { statusColors } from '../../theme/theme';

type AnyStatus = 'Pending' | 'InProgress' | 'Completed' | 'Cancelled' | 'Scheduled' | 'Missed' | 'Open' | 'Resolved' | 'Closed';

const LABELS: Record<AnyStatus, string> = {
  Pending: 'Pending',
  InProgress: 'In Progress',
  Completed: 'Completed',
  Cancelled: 'Cancelled',
  Scheduled: 'Scheduled',
  Missed: 'Missed',
  Open: 'Open',
  Resolved: 'Resolved',
  Closed: 'Closed',
};

const COLOR_MAP: Record<AnyStatus, string> = {
  Pending: statusColors.pending,
  InProgress: statusColors.inProgress,
  Completed: statusColors.completed,
  Cancelled: statusColors.cancelled,
  Scheduled: statusColors.scheduled,
  Missed: statusColors.missed,
  Open: statusColors.open,
  Resolved: statusColors.resolved,
  Closed: statusColors.closed,
};

interface StatusChipProps {
  status: string;
}

export default function StatusChip({ status }: StatusChipProps) {
  const key = status as AnyStatus;
  const color = COLOR_MAP[key] ?? '#6B7280';
  const label = LABELS[key] ?? status;

  return (
    <Chip
      size="small"
      label={label}
      sx={{
        bgcolor: `${color}1A`,
        color,
        border: `1px solid ${color}40`,
        fontWeight: 700,
        fontSize: '0.72rem',
      }}
    />
  );
}
