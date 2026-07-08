import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { Box, Card, Grid, Typography, Avatar, TextField, Button, Divider, CircularProgress } from '@mui/material';
import LockResetRoundedIcon from '@mui/icons-material/LockResetRounded';
import PageHeader from '../../components/common/PageHeader';
import { useAuth } from '../../hooks/useAuth';
import { extractErrorMessage } from '../../api/axiosInstance';
import { useToast } from '../../context/ToastContext';
import type { ChangePasswordRequest } from '../../types';

function getInitials(name: string) {
  return name
    .split(' ')
    .map((part) => part[0])
    .slice(0, 2)
    .join('')
    .toUpperCase();
}

export default function ProfilePage() {
  const { user, changePassword, isAdmin } = useAuth();
  const { showSuccess, showError } = useToast();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    watch,
    reset,
    formState: { errors },
  } = useForm<ChangePasswordRequest>();

  const newPassword = watch('newPassword');

  const onSubmit = async (data: ChangePasswordRequest) => {
    setIsSubmitting(true);
    try {
      await changePassword(data);
      showSuccess('Password changed successfully.');
      reset();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Box>
      <PageHeader title="My Profile" subtitle="Manage your account details and password." />

      <Grid container spacing={2.5}>
        <Grid item xs={12} md={5}>
          <Card variant="outlined" sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2.5 }}>
              <Avatar sx={{ width: 56, height: 56, bgcolor: 'primary.main', fontSize: '1.1rem', fontWeight: 700 }}>
                {user ? getInitials(user.fullName) : '?'}
              </Avatar>
              <Box>
                <Typography variant="subtitle1">{user?.fullName}</Typography>
                <Typography variant="body2" color="text.secondary">
                  {isAdmin ? 'Administrator' : 'Resident'}
                </Typography>
              </Box>
            </Box>

            <Divider sx={{ mb: 2.5 }} />

            <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1.75 }}>
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Email address
                </Typography>
                <Typography variant="body2">{user?.email}</Typography>
              </Box>
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Phone number
                </Typography>
                <Typography variant="body2">{user?.phoneNumber || '—'}</Typography>
              </Box>
              <Box>
                <Typography variant="caption" color="text.secondary">
                  Member since
                </Typography>
                <Typography variant="body2">{user ? new Date(user.createdAt).toLocaleDateString() : '—'}</Typography>
              </Box>
            </Box>
          </Card>
        </Grid>

        <Grid item xs={12} md={7}>
          <Card variant="outlined" sx={{ p: 3 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
              <LockResetRoundedIcon color="action" />
              <Typography variant="subtitle1">Change password</Typography>
            </Box>

            <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
              <TextField
                label="Current password"
                type="password"
                fullWidth
                margin="normal"
                error={!!errors.currentPassword}
                helperText={errors.currentPassword?.message}
                {...register('currentPassword', { required: 'Current password is required' })}
              />
              <TextField
                label="New password"
                type="password"
                fullWidth
                margin="normal"
                error={!!errors.newPassword}
                helperText={errors.newPassword?.message}
                {...register('newPassword', {
                  required: 'New password is required',
                  minLength: { value: 6, message: 'At least 6 characters' },
                })}
              />
              <TextField
                label="Confirm new password"
                type="password"
                fullWidth
                margin="normal"
                error={!!errors.confirmNewPassword}
                helperText={errors.confirmNewPassword?.message}
                {...register('confirmNewPassword', {
                  required: 'Please confirm your new password',
                  validate: (value) => value === newPassword || 'Passwords do not match',
                })}
              />

              <Button
                type="submit"
                variant="contained"
                disabled={isSubmitting}
                sx={{ mt: 2 }}
                startIcon={isSubmitting ? <CircularProgress size={16} color="inherit" /> : undefined}
              >
                Update password
              </Button>
            </Box>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}
