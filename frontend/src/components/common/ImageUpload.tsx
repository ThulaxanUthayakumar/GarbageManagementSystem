import { useEffect, useRef, useState } from 'react';
import { Box, Button, IconButton, Typography } from '@mui/material';
import CloudUploadRoundedIcon from '@mui/icons-material/CloudUploadRounded';
import CloseRoundedIcon from '@mui/icons-material/CloseRounded';

interface ImageUploadProps {
  label?: string;
  value: File | null;
  existingImageUrl?: string | null;
  onChange: (file: File | null) => void;
}

export default function ImageUpload({ label = 'Attach a photo (optional)', value, existingImageUrl, onChange }: ImageUploadProps) {
  const inputRef = useRef<HTMLInputElement>(null);
  const [preview, setPreview] = useState<string | null>(existingImageUrl ?? null);

  useEffect(() => {
    if (!value) {
      setPreview(existingImageUrl ?? null);
      return;
    }
    const objectUrl = URL.createObjectURL(value);
    setPreview(objectUrl);
    return () => URL.revokeObjectURL(objectUrl);
  }, [value, existingImageUrl]);

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) onChange(file);
  };

  const handleClear = () => {
    onChange(null);
    if (inputRef.current) inputRef.current.value = '';
  };

  return (
    <Box>
      <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 0.75 }}>
        {label}
      </Typography>

      {preview ? (
        <Box
          sx={{
            position: 'relative',
            width: '100%',
            maxWidth: 260,
            borderRadius: 2,
            overflow: 'hidden',
            border: '1px solid',
            borderColor: 'divider',
          }}
        >
          <Box component="img" src={preview} alt="Preview" sx={{ width: '100%', height: 160, objectFit: 'cover', display: 'block' }} />
          <IconButton
            size="small"
            onClick={handleClear}
            sx={{ position: 'absolute', top: 6, right: 6, bgcolor: 'rgba(0,0,0,0.55)', color: '#fff', '&:hover': { bgcolor: 'rgba(0,0,0,0.75)' } }}
          >
            <CloseRoundedIcon fontSize="small" />
          </IconButton>
        </Box>
      ) : (
        <Button
          component="label"
          variant="outlined"
          color="inherit"
          startIcon={<CloudUploadRoundedIcon />}
          sx={{ borderStyle: 'dashed', py: 1.4, px: 2.5 }}
        >
          Choose image
          <input ref={inputRef} type="file" accept="image/*" hidden onChange={handleFileSelect} />
        </Button>
      )}
    </Box>
  );
}
