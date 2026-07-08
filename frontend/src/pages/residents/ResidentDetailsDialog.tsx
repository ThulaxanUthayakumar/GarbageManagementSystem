import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Grid, Typography, Box, Chip, Divider } from '@mui/material';
import type { ResidentDto } from '../../types';

interface ResidentDetailsDialogProps {
  open: boolean;
  resident: ResidentDto | null;
  onClose: () => void;
}

function Field({ label, value }: { label: string; value: string }) {
  return (
    <Box>
      <Typography variant="caption" color="text.secondary" sx={{ display: 'block' }}>
        {label}
      </Typography>
      <Typography variant="body2" sx={{ fontWeight: 500 }}>
        {value || '—'}
      </Typography>
    </Box>
  );
}

export default function ResidentDetailsDialog({ open, resident, onClose }: ResidentDetailsDialogProps) {
  if (!resident) return null;

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ fontWeight: 700, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
        {resident.fullName}
        <Chip
          size="small"
          label={resident.isActive ? 'Active' : 'Inactive'}
          color={resident.isActive ? 'success' : 'default'}
          sx={{ fontWeight: 700 }}
        />
      </DialogTitle>
      <DialogContent dividers>
        <Grid container spacing={2.5}>
          <Grid item xs={12} sm={6}>
            <Field label="Email" value={resident.email} />
          </Grid>
          <Grid item xs={12} sm={6}>
            <Field label="Phone number" value={resident.phoneNumber ?? ''} />
          </Grid>
          <Grid item xs={12} sm={6}>
            <Field label="Collection zone" value={resident.zone} />
          </Grid>
          <Grid item xs={12} sm={6}>
            <Field label="Date joined" value={new Date(resident.dateJoined).toLocaleDateString()} />
          </Grid>
          <Grid item xs={12}>
            <Field label="Address" value={`${resident.address}, ${resident.city}, ${resident.state} ${resident.zipCode}`} />
          </Grid>
        </Grid>

        <Divider sx={{ my: 2.5 }} />

        <Grid container spacing={2.5}>
          <Grid item xs={6}>
            <Typography variant="h5" className="tabular-nums">
              {resident.totalRequests}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Collection requests
            </Typography>
          </Grid>
          <Grid item xs={6}>
            <Typography variant="h5" className="tabular-nums">
              {resident.totalComplaints}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Complaints filed
            </Typography>
          </Grid>
        </Grid>
      </DialogContent>
      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button onClick={onClose} variant="contained">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
}
