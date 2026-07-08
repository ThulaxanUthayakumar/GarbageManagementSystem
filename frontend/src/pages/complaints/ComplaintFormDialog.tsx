import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, Grid, TextField, CircularProgress, Box } from '@mui/material';
import ImageUpload from '../../components/common/ImageUpload';
import type { CreateComplaintPayload } from '../../types';

interface ComplaintFormValues {
  subject: string;
  description: string;
}

interface ComplaintFormDialogProps {
  open: boolean;
  isSaving: boolean;
  onClose: () => void;
  onSubmit: (payload: CreateComplaintPayload) => Promise<void>;
}

export default function ComplaintFormDialog({ open, isSaving, onClose, onSubmit }: ComplaintFormDialogProps) {
  const [image, setImage] = useState<File | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<ComplaintFormValues>();

  useEffect(() => {
    if (open) {
      reset({ subject: '', description: '' });
      setImage(null);
    }
  }, [open, reset]);

  const submit = async (data: ComplaintFormValues) => {
    await onSubmit({ subject: data.subject, description: data.description, image });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle sx={{ fontWeight: 700 }}>Submit a complaint</DialogTitle>
      <Box component="form" onSubmit={handleSubmit(submit)} noValidate>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid item xs={12}>
              <TextField
                label="Subject"
                fullWidth
                placeholder="e.g. Missed pickup on Tuesday"
                error={!!errors.subject}
                helperText={errors.subject?.message}
                {...register('subject', { required: 'Subject is required', maxLength: { value: 200, message: 'Keep it under 200 characters' } })}
              />
            </Grid>
            <Grid item xs={12}>
              <TextField
                label="Description"
                fullWidth
                multiline
                minRows={3}
                placeholder="Tell us what happened, and when."
                error={!!errors.description}
                helperText={errors.description?.message}
                {...register('description', { required: 'Please describe the issue' })}
              />
            </Grid>
            <Grid item xs={12}>
              <ImageUpload label="Attach a supporting photo (optional)" value={image} onChange={setImage} />
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions sx={{ px: 3, py: 2 }}>
          <Button onClick={onClose} color="inherit" disabled={isSaving}>
            Cancel
          </Button>
          <Button type="submit" variant="contained" color="error" disabled={isSaving} startIcon={isSaving ? <CircularProgress size={16} color="inherit" /> : undefined}>
            Submit complaint
          </Button>
        </DialogActions>
      </Box>
    </Dialog>
  );
}
