import { useEffect, useState } from 'react';
import { Box, Grid, TextField, MenuItem } from '@mui/material';
import PageHeader from '../../components/common/PageHeader';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import StatCard from '../../components/charts/StatCard';
import TrendChartCard from '../../components/charts/TrendChartCard';
import CategoryPieChartCard from '../../components/charts/CategoryPieChartCard';
import EventRoundedIcon from '@mui/icons-material/EventRounded';
import CalendarMonthRoundedIcon from '@mui/icons-material/CalendarMonthRounded';
import TaskAltRoundedIcon from '@mui/icons-material/TaskAltRounded';
import ReportProblemRoundedIcon from '@mui/icons-material/ReportProblemRounded';
import { reportApi } from '../../api/reportApi';
import { extractErrorMessage } from '../../api/axiosInstance';
import { useToast } from '../../context/ToastContext';
import { statusColors } from '../../theme/theme';
import type { ComplaintStatistics, DailyCollectionReport, MonthlyCollectionReport, WasteCategoryStatistics } from '../../types';

const MONTH_NAMES = [
  'January', 'February', 'March', 'April', 'May', 'June',
  'July', 'August', 'September', 'October', 'November', 'December',
];

function todayIso() {
  return new Date().toISOString().split('T')[0];
}

export default function ReportsPage() {
  const { showError } = useToast();

  const [selectedDate, setSelectedDate] = useState(todayIso());
  const now = new Date();
  const [selectedYear, setSelectedYear] = useState(now.getFullYear());
  const [selectedMonth, setSelectedMonth] = useState(now.getMonth() + 1);

  const [isLoading, setIsLoading] = useState(true);
  const [dailyReport, setDailyReport] = useState<DailyCollectionReport | null>(null);
  const [monthlyReport, setMonthlyReport] = useState<MonthlyCollectionReport | null>(null);
  const [complaintStats, setComplaintStats] = useState<ComplaintStatistics | null>(null);
  const [categoryStats, setCategoryStats] = useState<WasteCategoryStatistics | null>(null);

  useEffect(() => {
    let isMounted = true;

    async function loadAll() {
      setIsLoading(true);
      try {
        const [daily, monthly, complaints, categories] = await Promise.all([
          reportApi.getDailyCollections(selectedDate),
          reportApi.getMonthlyCollections(selectedYear, selectedMonth),
          reportApi.getComplaintStatistics(),
          reportApi.getWasteCategoryStatistics(),
        ]);
        if (!isMounted) return;
        setDailyReport(daily);
        setMonthlyReport(monthly);
        setComplaintStats(complaints);
        setCategoryStats(categories);
      } catch (error) {
        showError(extractErrorMessage(error));
      } finally {
        if (isMounted) setIsLoading(false);
      }
    }

    loadAll();
    return () => {
      isMounted = false;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [selectedDate, selectedYear, selectedMonth]);

  const monthlyChartData = monthlyReport?.dailyBreakdown.map((d) => ({ day: String(d.day), count: d.count })) ?? [];

  return (
    <Box>
      <PageHeader title="Reports" subtitle="Collection and complaint analytics across the whole system." />

      {isLoading ? (
        <LoadingSpinner label="Crunching the numbers..." minHeight={400} />
      ) : (
        <>
          <Grid container spacing={2.5} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Requests today" value={dailyReport?.totalRequests ?? 0} icon={<EventRoundedIcon />} color="#0F6B52" />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label={`Requests in ${monthlyReport?.monthName ?? ''}`} value={monthlyReport?.totalRequests ?? 0} icon={<CalendarMonthRoundedIcon />} color="#3E6EA8" />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Avg. complaint resolution" value={complaintStats?.averageResolutionDays ?? 0} suffix="days" icon={<TaskAltRoundedIcon />} color={statusColors.completed} />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Open complaints" value={(complaintStats?.open ?? 0) + (complaintStats?.inProgress ?? 0)} icon={<ReportProblemRoundedIcon />} color={statusColors.open} />
            </Grid>
          </Grid>

          <Grid container spacing={2.5} sx={{ mb: 3 }}>
            <Grid item xs={12} lg={8}>
              <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                <TextField
                  select
                  label="Month"
                  size="small"
                  value={selectedMonth}
                  onChange={(e) => setSelectedMonth(Number(e.target.value))}
                  sx={{ minWidth: 160 }}
                >
                  {MONTH_NAMES.map((name, index) => (
                    <MenuItem key={name} value={index + 1}>
                      {name}
                    </MenuItem>
                  ))}
                </TextField>
                <TextField
                  select
                  label="Year"
                  size="small"
                  value={selectedYear}
                  onChange={(e) => setSelectedYear(Number(e.target.value))}
                  sx={{ minWidth: 120 }}
                >
                  {[now.getFullYear(), now.getFullYear() - 1].map((year) => (
                    <MenuItem key={year} value={year}>
                      {year}
                    </MenuItem>
                  ))}
                </TextField>
                <TextField label="Daily report date" type="date" size="small" value={selectedDate} onChange={(e) => setSelectedDate(e.target.value)} InputLabelProps={{ shrink: true }} />
              </Box>

              <TrendChartCard
                title={`Requests per day — ${monthlyReport?.monthName ?? ''} ${monthlyReport?.year ?? ''}`}
                data={monthlyChartData}
                xKey="day"
                series={[{ key: 'count', label: 'Requests', color: '#0F6B52' }]}
                height={280}
              />
            </Grid>

            <Grid item xs={12} lg={4}>
              <CategoryPieChartCard title="Requests by waste category" data={categoryStats?.categories ?? []} height={260} />
            </Grid>
          </Grid>

          <Grid container spacing={2.5}>
            <Grid item xs={12} lg={6}>
              <TrendChartCard
                title="Today's collections by outcome"
                subtitle={new Date(selectedDate).toLocaleDateString(undefined, { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' })}
                data={[
                  {
                    label: 'Today',
                    completed: dailyReport?.completed ?? 0,
                    pending: (dailyReport?.pending ?? 0) + (dailyReport?.inProgress ?? 0),
                    cancelled: dailyReport?.cancelled ?? 0,
                  },
                ]}
                xKey="label"
                series={[
                  { key: 'completed', label: 'Completed', color: statusColors.completed },
                  { key: 'pending', label: 'Pending / In Progress', color: statusColors.pending },
                  { key: 'cancelled', label: 'Cancelled', color: statusColors.cancelled },
                ]}
                height={220}
              />
            </Grid>

            <Grid item xs={12} lg={6}>
              <TrendChartCard
                title="Complaints by status"
                data={[
                  {
                    label: 'All time',
                    open: complaintStats?.open ?? 0,
                    inProgress: complaintStats?.inProgress ?? 0,
                    resolved: complaintStats?.resolved ?? 0,
                    closed: complaintStats?.closed ?? 0,
                  },
                ]}
                xKey="label"
                series={[
                  { key: 'open', label: 'Open', color: statusColors.open },
                  { key: 'inProgress', label: 'In Progress', color: statusColors.inProgress },
                  { key: 'resolved', label: 'Resolved', color: statusColors.resolved },
                  { key: 'closed', label: 'Closed', color: statusColors.closed },
                ]}
                height={220}
              />
            </Grid>
          </Grid>
        </>
      )}
    </Box>
  );
}
