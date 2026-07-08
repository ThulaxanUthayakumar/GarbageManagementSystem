import { useState } from 'react';
import { Link as RouterLink, useNavigate, useLocation } from 'react-router-dom';
import { useForm } from 'react-hook-form';
import {
  Box,
  Paper,
  TextField,
  Button,
  Typography,
  Link,
  Alert,
  InputAdornment,
  IconButton,
  CircularProgress,
} from '@mui/material';
import VisibilityRoundedIcon from '@mui/icons-material/VisibilityRounded';
import VisibilityOffRoundedIcon from '@mui/icons-material/VisibilityOffRounded';
import RecyclingRoundedIcon from '@mui/icons-material/RecyclingRounded';
import { useAuth } from '../../hooks/useAuth';
import { extractErrorMessage } from '../../api/axiosInstance';
import type { LoginRequest } from '../../types';

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [showPassword, setShowPassword] = useState(false);
  const [serverError, setServerError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginRequest>();

  const onSubmit = async (data: LoginRequest) => {
    setServerError(null);
    setIsSubmitting(true);
    try {
      await login(data);
      const redirectTo = (location.state as { from?: Location })?.from?.pathname || '/dashboard';
      navigate(redirectTo, { replace: true });
    } catch (error) {
      setServerError(extractErrorMessage(error));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <Box sx={{ display: 'flex', minHeight: '100dvh' }}>
      {/* Branded panel - desktop only */}
      <Box
        sx={{
          display: { xs: 'none', md: 'flex' },
          flexDirection: 'column',
          justifyContent: 'space-between',
          width: '42%',
          bgcolor: '#0F6B52',
          color: '#fff',
          p: 6,
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
          <Box sx={{ width: 40, height: 40, borderRadius: '10px', bgcolor: 'rgba(255,255,255,0.15)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
            <RecyclingRoundedIcon />
          </Box>
          <Typography sx={{ fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 800 }}>GarbageMS</Typography>
        </Box>

        <Box>
          <Typography variant="h4" sx={{ mb: 2, maxWidth: 380 }}>
            Keep every pickup, schedule and complaint in one place.
          </Typography>
          <Typography sx={{ opacity: 0.8, maxWidth: 360 }}>
            Residents request collections and track complaints. Admins manage zones, schedules and reporting — all from a single dashboard.
          </Typography>
        </Box>

        <Typography variant="caption" sx={{ opacity: 0.6 }}>
          &copy; {new Date().getFullYear()} Garbage Management System
        </Typography>
      </Box>

      {/* Form panel */}
      <Box sx={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center', p: 3, bgcolor: 'background.default' }}>
        <Paper variant="outlined" sx={{ width: '100%', maxWidth: 420, p: { xs: 3, sm: 4.5 }, borderRadius: 3 }}>
          <Typography variant="h5" sx={{ mb: 0.5 }}>
            Welcome back
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
            Log in to continue to your dashboard.
          </Typography>

          {serverError && (
            <Alert severity="error" sx={{ mb: 2.5 }}>
              {serverError}
            </Alert>
          )}

          <Box component="form" onSubmit={handleSubmit(onSubmit)} noValidate>
            <TextField
              label="Email address"
              type="email"
              fullWidth
              margin="normal"
              autoComplete="email"
              error={!!errors.email}
              helperText={errors.email?.message}
              {...register('email', { required: 'Email is required' })}
            />

            <TextField
              label="Password"
              type={showPassword ? 'text' : 'password'}
              fullWidth
              margin="normal"
              autoComplete="current-password"
              error={!!errors.password}
              helperText={errors.password?.message}
              {...register('password', { required: 'Password is required' })}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton onClick={() => setShowPassword((p) => !p)} edge="end" size="small">
                      {showPassword ? <VisibilityOffRoundedIcon fontSize="small" /> : <VisibilityRoundedIcon fontSize="small" />}
                    </IconButton>
                  </InputAdornment>
                ),
              }}
            />

            <Button
              type="submit"
              fullWidth
              variant="contained"
              size="large"
              disabled={isSubmitting}
              sx={{ mt: 2.5, mb: 2, py: 1.2 }}
              startIcon={isSubmitting ? <CircularProgress size={18} color="inherit" /> : undefined}
            >
              {isSubmitting ? 'Logging in...' : 'Log in'}
            </Button>

            <Typography variant="body2" color="text.secondary" align="center">
              Don&apos;t have an account?{' '}
              <Link component={RouterLink} to="/register" underline="hover" fontWeight={600}>
                Register as a resident
              </Link>
            </Typography>
          </Box>
        </Paper>
      </Box>
    </Box>
  );
}
