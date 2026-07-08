import { useCallback, useEffect, useState } from 'react';
import { Box, Button, TextField, MenuItem, IconButton, Tooltip, Card, Grid, Typography } from '@mui/material';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import EditRoundedIcon from '@mui/icons-material/EditRounded';
import DeleteRoundedIcon from '@mui/icons-material/DeleteRounded';
import EventRoundedIcon from '@mui/icons-material/EventRounded';
import PersonPinCircleRoundedIcon from '@mui/icons-material/PersonPinCircleRounded';
import PageHeader from '../../components/common/PageHeader';
import DataTable, { type DataTableColumn } from '../../components/common/DataTable';
import StatusChip from '../../components/common/StatusChip';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import LoadingSpinner from '../../components/common/LoadingSpinner';
import ScheduleFormDialog from './ScheduleFormDialog';
import { useAuth } from '../../hooks/useAuth';
import { scheduleApi } from '../../api/scheduleApi';
import { extractErrorMessage } from '../../api/axiosInstance';
import { useToast } from '../../context/ToastContext';
import type { CreateScheduleRequest, ScheduleDto, UpdateScheduleRequest } from '../../types';

const ZONE_OPTIONS = ['Zone A', 'Zone B', 'Zone C'];

export default function SchedulesPage() {
  const { isAdmin } = useAuth();
  const { showSuccess, showError } = useToast();

  const [schedules, setSchedules] = useState<ScheduleDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [zoneFilter, setZoneFilter] = useState('');
  const [isLoading, setIsLoading] = useState(true);

  const [formOpen, setFormOpen] = useState(false);
  const [editingSchedule, setEditingSchedule] = useState<ScheduleDto | null>(null);
  const [isSaving, setIsSaving] = useState(false);
  const [deletingSchedule, setDeletingSchedule] = useState<ScheduleDto | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const load = useCallback(async () => {
    setIsLoading(true);
    try {
      if (isAdmin) {
        const result = await scheduleApi.getPaged(page + 1, pageSize, zoneFilter || undefined);
        setSchedules(result.items);
        setTotalCount(result.totalCount);
      } else {
        const result = await scheduleApi.getMy();
        setSchedules(result);
        setTotalCount(result.length);
      }
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAdmin, page, pageSize, zoneFilter]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    setPage(0);
  }, [zoneFilter]);

  const handleCreate = async (payload: CreateScheduleRequest) => {
    setIsSaving(true);
    try {
      await scheduleApi.create(payload);
      showSuccess('Schedule entry created.');
      setFormOpen(false);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleUpdate = async (id: number, payload: UpdateScheduleRequest) => {
    setIsSaving(true);
    try {
      await scheduleApi.update(id, payload);
      showSuccess('Schedule entry updated.');
      setFormOpen(false);
      setEditingSchedule(null);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deletingSchedule) return;
    setIsDeleting(true);
    try {
      await scheduleApi.remove(deletingSchedule.id);
      showSuccess('Schedule entry deleted.');
      setDeletingSchedule(null);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsDeleting(false);
    }
  };

  const columns: DataTableColumn<ScheduleDto>[] = [
    { key: 'zone', header: 'Zone', render: (s) => s.zone },
    { key: 'category', header: 'Waste category', render: (s) => s.wasteCategoryName },
    { key: 'date', header: 'Date', render: (s) => new Date(s.scheduledDate).toLocaleDateString() },
    { key: 'time', header: 'Time window', render: (s) => s.scheduledTime },
    { key: 'collector', header: 'Collector', render: (s) => s.collectorName || '—' },
    { key: 'status', header: 'Status', render: (s) => <StatusChip status={s.status} /> },
    ...(isAdmin
      ? [
          {
            key: 'actions',
            header: '',
            align: 'right' as const,
            render: (s: ScheduleDto) => (
              <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 0.5 }}>
                <Tooltip title="Edit">
                  <IconButton
                    size="small"
                    onClick={() => {
                      setEditingSchedule(s);
                      setFormOpen(true);
                    }}
                  >
                    <EditRoundedIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
                <Tooltip title="Delete">
                  <IconButton size="small" color="error" onClick={() => setDeletingSchedule(s)}>
                    <DeleteRoundedIcon fontSize="small" />
                  </IconButton>
                </Tooltip>
              </Box>
            ),
          } as DataTableColumn<ScheduleDto>,
        ]
      : []),
  ];

  return (
    <Box>
      <PageHeader
        title="Collection Schedule"
        subtitle={isAdmin ? 'Plan upcoming collection runs for each zone.' : 'See when the next collection is happening in your zone.'}
        action={
          isAdmin && (
            <Button
              variant="contained"
              startIcon={<AddRoundedIcon />}
              onClick={() => {
                setEditingSchedule(null);
                setFormOpen(true);
              }}
            >
              Add schedule
            </Button>
          )
        }
      />

      {isAdmin ? (
        <>
          <TextField
            select
            label="Filter by zone"
            value={zoneFilter}
            onChange={(e) => setZoneFilter(e.target.value)}
            sx={{ mb: 2.5, minWidth: 200 }}
          >
            <MenuItem value="">All zones</MenuItem>
            {ZONE_OPTIONS.map((zone) => (
              <MenuItem key={zone} value={zone}>
                {zone}
              </MenuItem>
            ))}
          </TextField>

          <DataTable
            columns={columns}
            rows={schedules}
            getRowId={(s) => s.id}
            isLoading={isLoading}
            emptyMessage="No schedule entries yet."
            page={page}
            pageSize={pageSize}
            totalCount={totalCount}
            onPageChange={setPage}
            onPageSizeChange={(size) => {
              setPageSize(size);
              setPage(0);
            }}
          />
        </>
      ) : isLoading ? (
        <LoadingSpinner label="Loading your schedule..." />
      ) : schedules.length === 0 ? (
        <Card variant="outlined" sx={{ p: 4, textAlign: 'center' }}>
          <EventRoundedIcon sx={{ fontSize: 32, color: 'text.disabled', mb: 1 }} />
          <Typography color="text.secondary">No collections are scheduled for your zone yet.</Typography>
        </Card>
      ) : (
        <Grid container spacing={2}>
          {schedules.map((s) => (
            <Grid item xs={12} sm={6} lg={4} key={s.id}>
              <Card variant="outlined" sx={{ p: 2.5, height: '100%' }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1.5 }}>
                  <Box>
                    <Typography variant="subtitle1">{new Date(s.scheduledDate).toLocaleDateString(undefined, { weekday: 'long', month: 'short', day: 'numeric' })}</Typography>
                    <Typography variant="body2" color="text.secondary">
                      {s.scheduledTime}
                    </Typography>
                  </Box>
                  <StatusChip status={s.status} />
                </Box>
                <Typography variant="body2" sx={{ mb: 0.5 }}>
                  <strong>{s.wasteCategoryName}</strong>
                </Typography>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.75, color: 'text.secondary' }}>
                  <PersonPinCircleRoundedIcon fontSize="small" />
                  <Typography variant="body2">{s.collectorName || 'Collector to be assigned'}</Typography>
                </Box>
                {s.notes && (
                  <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mt: 1.5 }}>
                    {s.notes}
                  </Typography>
                )}
              </Card>
            </Grid>
          ))}
        </Grid>
      )}

      <ScheduleFormDialog
        open={formOpen}
        schedule={editingSchedule}
        isSaving={isSaving}
        onClose={() => {
          setFormOpen(false);
          setEditingSchedule(null);
        }}
        onCreate={handleCreate}
        onUpdate={handleUpdate}
      />

      <ConfirmDialog
        open={!!deletingSchedule}
        title="Delete schedule entry?"
        message="This will permanently remove this schedule entry. This cannot be undone."
        confirmLabel="Delete"
        isDestructive
        isLoading={isDeleting}
        onConfirm={handleDelete}
        onCancel={() => setDeletingSchedule(null)}
      />
    </Box>
  );
}
