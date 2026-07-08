import { useState } from 'react';
import { Link as RouterLink, useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Link,
  Alert,
  Grid,
  MenuItem,
  CircularProgress,
} from '@mui/material';
import RecyclingRoundedIcon from '@mui/icons-material/RecyclingRounded';
import { useAuth } from '../../hooks/useAuth';
import { extractErrorMessage } from '../../api/axiosInstance';
import type { RegisterRequest } from '../../types';

const ZONE_OPTIONS = ['Zone A', 'Zone B', 'Zone C'];

export default function RegisterPage() {
  const { register: registerUser } = useAuth();
  const navigate = useNavigate();
  const [serverError, setServerError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm<RegisterRequest>({ defaultValues: { zone: ZONE_OPTIONS[0] } });

  const password = watch('password');

  const onSubmit = async (data: RegisterRequest) => {
    setServerError(null);
    setIsSubmitting(true);
    try {
      await registerUser(data);
      navigate('/dashboard', { replace: true });
    } catch (error) {
      setServerError(extractErrorMessage(error));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Box sx={{ minHeight: '100dvh', display: 'flex', alignItems: 'center', justifyContent: 'center', bgcolor: 'background.default', p: 3 }}>
      <Paper variant="outlined" sx={{ width: '100%', maxWidth: 640, p: { xs: 3, sm: 4.5 }, borderRadius: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.25, mb: 2.5 }}>
          <Box sx={{ width: 38, height: 38, borderRadius: '10px', bgcolor: 'primary.main', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <RecyclingRoundedIcon sx={{ color: '#fff', fontSize: 20 }} />
          </Box>
          <Typography sx={{ fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 800 }}>GarbageMS</Typography>
        </Box>

        <Typography variant="h5" sx={{ mb: 0.5 }}>
          Create your resident account
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          Register to submit collection requests and track complaints for your address.
        </Typography>

        {serverError && (
          <Alert severity="error" sx={{ mb: 2.5 }}>
            {serverError}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
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
                error={!!errors.email}
                helperText={errors.email?.message}
                {...register('email', { required: 'Email is required' })}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
              <TextField
                label="Password"
                type="password"
                fullWidth
                error={!!errors.password}
                helperText={errors.password?.message}
                {...register('password', {
                  required: 'Password is required',
                  minLength: { value: 6, message: 'At least 6 characters' },
                })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="Confirm password"
                type="password"
                fullWidth
                error={!!errors.confirmPassword}
                helperText={errors.confirmPassword?.message}
                {...register('confirmPassword', {
                  required: 'Please confirm your password',
                  validate: (value) => value === password || 'Passwords do not match',
                })}
              />
            </Grid>

            <Grid item xs={12} sm={6}>
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
                defaultValue={ZONE_OPTIONS[0]}
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

          <Button
            type="submit"
            fullWidth
            variant="contained"
            size="large"
            disabled={isSubmitting}
            sx={{ mt: 3, mb: 2, py: 1.2 }}
            startIcon={isSubmitting ? <CircularProgress size={18} color="inherit" /> : undefined}
          >
            {isSubmitting ? 'Creating account...' : 'Create account'}
          </Button>

          <Typography variant="body2" color="text.secondary" align="center">
            Already have an account?{' '}
            <Link component={RouterLink} to="/login" underline="hover" fontWeight={600}>
              Log in
            </Link>
          </Typography>
        </Box>
      </Paper>
    </Box>
  );
}
