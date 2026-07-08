import { useEffect, useState } from 'react';
import { Grid, Box, Typography } from '@mui/material';
import GroupsRoundedIcon from '@mui/icons-material/GroupsRounded';
import PendingActionsRoundedIcon from '@mui/icons-material/PendingActionsRounded';
import LocalShippingRoundedIcon from '@mui/icons-material/LocalShippingRounded';
import TaskAltRoundedIcon from '@mui/icons-material/TaskAltRounded';
import ReportProblemRoundedIcon from '@mui/icons-material/ReportProblemRounded';
import EventAvailableRoundedIcon from '@mui/icons-material/EventAvailableRounded';
import PageHeader from '../components/common/PageHeader';
import LoadingSpinner from '../components/common/LoadingSpinner';
import StatCard from '../components/charts/StatCard';
import TrendChartCard from '../components/charts/TrendChartCard';
import CategoryPieChartCard from '../components/charts/CategoryPieChartCard';
import { useAuth } from '../hooks/useAuth';
import { dashboardApi } from '../api/dashboardApi';
import { useToast } from '../context/ToastContext';
import { extractErrorMessage } from '../api/axiosInstance';
import type { CategoryCount, DashboardStats, MonthlyTrendPoint, ResidentDashboardSummary } from '../types';
import { statusColors } from '../theme/theme';

export default function DashboardPage() {
  const { user, isAdmin } = useAuth();
  const { showError } = useToast();

  const [isLoading, setIsLoading] = useState(true);
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [trend, setTrend] = useState<MonthlyTrendPoint[]>([]);
  const [categoryBreakdown, setCategoryBreakdown] = useState<CategoryCount[]>([]);
  const [residentSummary, setResidentSummary] = useState<ResidentDashboardSummary | null>(null);

  useEffect(() => {
    let isMounted = true;

    async function load() {
      setIsLoading(true);
      try {
        if (isAdmin) {
          const [statsData, trendData, breakdownData] = await Promise.all([
            dashboardApi.getStats(),
            dashboardApi.getCollectionTrend(6),
            dashboardApi.getWasteCategoryBreakdown(),
          ]);
          if (!isMounted) return;
          setStats(statsData);
          setTrend(trendData);
          setCategoryBreakdown(breakdownData);
        } else {
          const summary = await dashboardApi.getResidentSummary();
          if (!isMounted) return;
          setResidentSummary(summary);
        }
      } catch (error) {
        showError(extractErrorMessage(error));
      } finally {
        if (isMounted) setIsLoading(false);
      }
    }

    load();
    return () => {
      isMounted = false;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAdmin]);

  if (isLoading) {
    return <LoadingSpinner label="Loading dashboard..." minHeight={400} />;
  }

  return (
    <Box>
      <PageHeader
        title={`Welcome back, ${user?.fullName?.split(' ')[0] ?? ''}`}
        subtitle={isAdmin ? "Here's what's happening across all zones today." : `Here's a quick summary for ${residentSummary?.zone ?? 'your area'}.`}
      />

      {isAdmin && stats ? (
        <>
          <Grid container spacing={2.5} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Total Residents" value={stats.totalResidents} icon={<GroupsRoundedIcon />} color="#0F6B52" />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Pending Requests" value={stats.pendingRequests} icon={<PendingActionsRoundedIcon />} color={statusColors.pending} />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Completed Collections" value={stats.completedCollections} icon={<TaskAltRoundedIcon />} color={statusColors.completed} />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Open Complaints" value={stats.openComplaints} icon={<ReportProblemRoundedIcon />} color={statusColors.open} />
            </Grid>
          </Grid>

          <Grid container spacing={2.5} sx={{ mb: 3 }}>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="In Progress Requests" value={stats.inProgressRequests} icon={<LocalShippingRoundedIcon />} color={statusColors.inProgress} />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Upcoming Schedules" value={stats.upcomingSchedules} icon={<EventAvailableRoundedIcon />} color="#3E6EA8" />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Active Residents" value={stats.activeResidents} icon={<GroupsRoundedIcon />} color="#6B4FA0" />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Total Complaints" value={stats.totalComplaints} icon={<ReportProblemRoundedIcon />} color="#C24A2E" />
            </Grid>
          </Grid>

          <Grid container spacing={2.5}>
            <Grid item xs={12} lg={7}>
              <TrendChartCard
                title="Collection trend"
                subtitle="Requests by outcome, last 6 months"
                data={trend as unknown as Record<string, string | number>[]}
                xKey="month"
                series={[
                  { key: 'completed', label: 'Completed', color: statusColors.completed },
                  { key: 'pending', label: 'Pending / In Progress', color: statusColors.pending },
                  { key: 'cancelled', label: 'Cancelled', color: statusColors.cancelled },
                ]}
              />
            </Grid>
            <Grid item xs={12} lg={5}>
              <CategoryPieChartCard title="Requests by waste category" data={categoryBreakdown} />
            </Grid>
          </Grid>
        </>
      ) : (
        residentSummary && (
          <Grid container spacing={2.5}>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="My Total Requests" value={residentSummary.myTotalRequests} icon={<LocalShippingRoundedIcon />} color="#0F6B52" />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Pending / In Progress" value={residentSummary.myPendingRequests} icon={<PendingActionsRoundedIcon />} color={statusColors.pending} />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Completed" value={residentSummary.myCompletedRequests} icon={<TaskAltRoundedIcon />} color={statusColors.completed} />
            </Grid>
            <Grid item xs={12} sm={6} lg={3}>
              <StatCard label="Open Complaints" value={residentSummary.myOpenComplaints} icon={<ReportProblemRoundedIcon />} color={statusColors.open} />
            </Grid>
            <Grid item xs={12}>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                Head over to <strong>Collection Requests</strong> to schedule a new pickup, or check <strong>Schedule</strong> to see when the next
                truck is coming to {residentSummary.zone}.
              </Typography>
            </Grid>
          </Grid>
        )
      )}
    </Box>
  );
}
