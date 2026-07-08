import { useEffect, useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Grid, TextField, MenuItem, CircularProgress, Box } from '@mui/material';
import ImageUpload from '../../components/common/ImageUpload';
import { wasteCategoryApi } from '../../api/wasteCategoryApi';
import type { CreateCollectionRequestPayload, WasteCategoryDto } from '../../types';

interface RequestFormValues {
  wasteCategoryId: number;
  pickupDate: string;
  description: string;
}

interface RequestFormDialogProps {
  open: boolean;
  isSaving: boolean;
  onClose: () => void;
  onSubmit: (payload: CreateCollectionRequestPayload) => Promise<void>;
}

function todayIso() {
  return new Date().toISOString().split('T')[0];
}

export default function RequestFormDialog({ open, isSaving, onClose, onSubmit }: RequestFormDialogProps) {
  const [categories, setCategories] = useState<WasteCategoryDto[]>([]);
  const [image, setImage] = useState<File | null>(null);

  const {
    register,
    handleSubmit,
    control,
    reset,
    formState: { errors },
  } = useForm<RequestFormValues>({ defaultValues: { pickupDate: todayIso() } });

  useEffect(() => {
    if (open) {
      wasteCategoryApi.getAll().then(setCategories).catch(() => setCategories([]));
      reset({ wasteCategoryId: undefined, pickupDate: todayIso(), description: '' });
      setImage(null);
    }
  }, [open, reset]);

  const submit = async (data: RequestFormValues) => {
    await onSubmit({
      wasteCategoryId: Number(data.wasteCategoryId),
      pickupDate: data.pickupDate,
      description: data.description,
      image,
    });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ fontWeight: 700 }}>New collection request</DialogTitle>
      <Box component="form" onSubmit={handleSubmit(submit)} noValidate>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <Controller
                name="wasteCategoryId"
                control={control}
                rules={{ required: 'Please select a waste category' }}
                render={({ field }) => (
                  <TextField
                    {...field}
                    select
                    label="Waste category"
                    fullWidth
                    value={field.value ?? ''}
                    error={!!errors.wasteCategoryId}
                    helperText={errors.wasteCategoryId?.message}
                  >
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
                label="Pickup date"
                type="date"
                fullWidth
                InputLabelProps={{ shrink: true }}
                inputProps={{ min: todayIso() }}
                error={!!errors.pickupDate}
                helperText={errors.pickupDate?.message}
                {...register('pickupDate', { required: 'Pickup date is required' })}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Description (optional)"
                fullWidth
                multiline
                minRows={2}
                placeholder="Anything the collection team should know?"
                {...register('description')}
              />
            </Grid>
            <Grid item xs={12}>
              <ImageUpload value={image} onChange={setImage} />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={onClose} color="inherit" disabled={isSaving}>
            Cancel
          </Button>
          <Button type="submit" variant="contained" disabled={isSaving} startIcon={isSaving ? <CircularProgress size={16} color="inherit" /> : undefined}>
            Submit request
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
