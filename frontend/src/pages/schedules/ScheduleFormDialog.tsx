import { useEffect, useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Grid, TextField, MenuItem, CircularProgress, Box } from '@mui/material';
import { wasteCategoryApi } from '../../api/wasteCategoryApi';
import type { CreateScheduleRequest, ScheduleDto, UpdateScheduleRequest, WasteCategoryDto, ScheduleStatus } from '../../types';

const ZONE_OPTIONS = ['Zone A', 'Zone B', 'Zone C'];
const STATUS_OPTIONS: ScheduleStatus[] = ['Scheduled', 'Completed', 'Missed', 'Cancelled'];

interface ScheduleFormValues {
  zone: string;
  wasteCategoryId: number | '';
  scheduledDate: string;
  scheduledTime: string;
  collectorName: string;
  notes: string;
  status: ScheduleStatus;
}

interface ScheduleFormDialogProps {
  open: boolean;
  schedule: ScheduleDto | null;
  isSaving: boolean;
  onClose: () => void;
  onCreate: (payload: CreateScheduleRequest) => Promise<void>;
  onUpdate: (id: number, payload: UpdateScheduleRequest) => Promise<void>;
}

export default function ScheduleFormDialog({ open, schedule, isSaving, onClose, onCreate, onUpdate }: ScheduleFormDialogProps) {
  const isEditMode = !!schedule;
  const [categories, setCategories] = useState<WasteCategoryDto[]>([]);

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm<ScheduleFormValues>();

  useEffect(() => {
    if (open) {
      wasteCategoryApi.getAll().then(setCategories).catch(() => setCategories([]));
      reset(
        schedule
          ? {
              zone: schedule.zone,
              wasteCategoryId: schedule.wasteCategoryId ?? '',
              scheduledDate: schedule.scheduledDate.split('T')[0],
              scheduledTime: schedule.scheduledTime,
              collectorName: schedule.collectorName ?? '',
              notes: schedule.notes ?? '',
              status: schedule.status,
            }
          : {
              zone: ZONE_OPTIONS[0],
              wasteCategoryId: '',
              scheduledDate: new Date().toISOString().split('T')[0],
              scheduledTime: '08:00 AM - 11:00 AM',
              collectorName: '',
              notes: '',
              status: 'Scheduled',
            },
      );
    }
  }, [open, schedule, reset]);

  const submit = async (data: ScheduleFormValues) => {
    const basePayload = {
      zone: data.zone,
      wasteCategoryId: data.wasteCategoryId === '' ? null : Number(data.wasteCategoryId),
      scheduledDate: data.scheduledDate,
      scheduledTime: data.scheduledTime,
      collectorName: data.collectorName || undefined,
      notes: data.notes || undefined,
    };

    if (isEditMode && schedule) {
      await onUpdate(schedule.id, { ...basePayload, status: data.status });
    } else {
      await onCreate(basePayload);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ fontWeight: 700 }}>{isEditMode ? 'Edit schedule entry' : 'Add schedule entry'}</DialogTitle>
      <Box component="form" onSubmit={handleSubmit(submit)} noValidate>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Zone"
                fullWidth
                defaultValue={schedule?.zone ?? ZONE_OPTIONS[0]}
                error={!!errors.zone}
                helperText={errors.zone?.message}
                {...register('zone', { required: 'Zone is required' })}
              >
                {ZONE_OPTIONS.map((zone) => (
                  <MenuItem key={zone} value={zone}>
                    {zone}
                  </MenuItem>
                ))}
              </TextField>
            </Grid>

            <Grid item xs={12} sm={6}>
              <Controller
                name="wasteCategoryId"
                control={control}
                render={({ field }) => (
                  <TextField {...field} select label="Waste category (optional)" fullWidth value={field.value ?? ''}>
                    <MenuItem value="">All categories</MenuItem>
                    {categories.map((cat) => (
                      <MenuItem key={cat.id} value={cat.id}>
                        {cat.name}
                      </MenuItem>
                    ))}
                  </TextField>
                )}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Scheduled date"
                type="date"
                fullWidth
                InputLabelProps={{ shrink: true }}
                error={!!errors.scheduledDate}
                helperText={errors.scheduledDate?.message}
                {...register('scheduledDate', { required: 'Scheduled date is required' })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Time window"
                fullWidth
                placeholder="e.g. 08:00 AM - 11:00 AM"
                error={!!errors.scheduledTime}
                helperText={errors.scheduledTime?.message}
                {...register('scheduledTime', { required: 'Time window is required' })}
              />
            </Grid>

            <Grid item xs={12} sm={isEditMode ? 6 : 12}>
              <TextField label="Collector / crew name (optional)" fullWidth {...register('collectorName')} />
            </Grid>

            {isEditMode && (
              <Grid item xs={12} sm={6}>
                <TextField select label="Status" fullWidth defaultValue={schedule?.status} {...register('status')}>
                  {STATUS_OPTIONS.map((s) => (
                    <MenuItem key={s} value={s}>
                      {s}
                    </MenuItem>
                  ))}
                </TextField>
              </Grid>
            )}

            <Grid item xs={12}>
              <TextField label="Notes (optional)" fullWidth multiline minRows={2} {...register('notes')} />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={onClose} color="inherit" disabled={isSaving}>
            Cancel
          </Button>
          <Button type="submit" variant="contained" disabled={isSaving} startIcon={isSaving ? <CircularProgress size={16} color="inherit" /> : undefined}>
            {isEditMode ? 'Save changes' : 'Add schedule'}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
