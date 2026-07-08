import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import {
  Box,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Grid,
  TextField,
  MenuItem,
  FormControlLabel,
  Switch,
  CircularProgress,
} from '@mui/material';
import type { CreateResidentRequest, ResidentDto, UpdateResidentRequest } from '../../types';

const ZONE_OPTIONS = ['Zone A', 'Zone B', 'Zone C'];

interface ResidentFormValues {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  address: string;
  city: string;
  state: string;
  zipCode: string;
  zone: string;
  isActive: boolean;
}

interface ResidentFormDialogProps {
  open: boolean;
  resident: ResidentDto | null;
  isSaving: boolean;
  onClose: () => void;
  onCreate: (payload: CreateResidentRequest) => Promise<void>;
  onUpdate: (id: number, payload: UpdateResidentRequest) => Promise<void>;
}

export default function ResidentFormDialog({ open, resident, isSaving, onClose, onCreate, onUpdate }: ResidentFormDialogProps) {
  const isEditMode = !!resident;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ResidentFormValues>();

  useEffect(() => {
    if (open) {
      reset(
        resident
          ? {
              fullName: resident.fullName,
              email: resident.email,
              password: '',
              phoneNumber: resident.phoneNumber ?? '',
              address: resident.address,
              city: resident.city,
              state: resident.state,
              zipCode: resident.zipCode,
              zone: resident.zone,
              isActive: resident.isActive,
            }
          : {
              fullName: '',
              email: '',
              password: '',
              phoneNumber: '',
              address: '',
              city: '',
              state: '',
              zipCode: '',
              zone: ZONE_OPTIONS[0],
              isActive: true,
            },
      );
    }
  }, [open, resident, reset]);

  const onSubmit = async (data: ResidentFormValues) => {
    if (isEditMode && resident) {
      await onUpdate(resident.id, {
        fullName: data.fullName,
        phoneNumber: data.phoneNumber,
        address: data.address,
        city: data.city,
        state: data.state,
        zipCode: data.zipCode,
        zone: data.zone,
        isActive: data.isActive,
      });
    } else {
      await onCreate({
        fullName: data.fullName,
        email: data.email,
        password: data.password,
        phoneNumber: data.phoneNumber,
        address: data.address,
        city: data.city,
        state: data.state,
        zipCode: data.zipCode,
        zone: data.zone,
      });
    }
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ fontWeight: 700 }}>{isEditMode ? 'Edit resident' : 'Add resident'}</DialogTitle>
      <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Full name"
                fullWidth
                error={!!errors.fullName}
                helperText={errors.fullName?.message}
                {...register('fullName', { required: 'Full name is required' })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Email address"
                type="email"
                fullWidth
                disabled={isEditMode}
                error={!!errors.email}
                helperText={errors.email?.message || (isEditMode ? 'Email cannot be changed' : undefined)}
                {...register('email', { required: !isEditMode && 'Email is required' })}
              />
            </Grid>

            {!isEditMode && (
              <Grid item xs={12} sm={6}>
                <TextField
                  label="Temporary password"
                  type="text"
                  fullWidth
                  error={!!errors.password}
                  helperText={errors.password?.message || 'Shared with the resident to log in the first time'}
                  {...register('password', { required: 'Password is required', minLength: { value: 6, message: 'At least 6 characters' } })}
                />
              </Grid>
            )}

            <Grid item xs={12} sm={isEditMode ? 6 : 6}>
              <TextField
                label="Phone number"
                fullWidth
                error={!!errors.phoneNumber}
                helperText={errors.phoneNumber?.message}
                {...register('phoneNumber', { required: 'Phone number is required' })}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                select
                label="Collection zone"
                fullWidth
                defaultValue={resident?.zone ?? ZONE_OPTIONS[0]}
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

            {isEditMode && (
              <Grid item xs={12} sm={6} sx={{ display: 'flex', alignItems: 'center' }}>
                <FormControlLabel control={<Switch defaultChecked={resident?.isActive} {...register('isActive')} />} label="Active resident" />
              </Grid>
            )}

            <Grid item xs={12}>
              <TextField
                label="Address"
                fullWidth
                error={!!errors.address}
                helperText={errors.address?.message}
                {...register('address', { required: 'Address is required' })}
              />
            </Grid>

            <Grid item xs={12} sm={4}>
              <TextField
                label="City"
                fullWidth
                error={!!errors.city}
                helperText={errors.city?.message}
                {...register('city', { required: 'City is required' })}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="State / Province"
                fullWidth
                error={!!errors.state}
                helperText={errors.state?.message}
                {...register('state', { required: 'State is required' })}
              />
            </Grid>
            <Grid item xs={12} sm={4}>
              <TextField
                label="Zip / Postal code"
                fullWidth
                error={!!errors.zipCode}
                helperText={errors.zipCode?.message}
                {...register('zipCode', { required: 'Zip code is required' })}
              />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={onClose} color="inherit" disabled={isSaving}>
            Cancel
          </Button>
          <Button
            type="submit"
            variant="contained"
            disabled={isSaving}
            startIcon={isSaving ? <CircularProgress size={16} color="inherit" /> : undefined}
          >
            {isEditMode ? 'Save changes' : 'Add resident'}
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
