import { useEffect, useState, useCallback } from 'react';
import { Box, Button, TextField, InputAdornment, IconButton, Tooltip, Chip } from '@mui/material';
import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import EditRoundedIcon from '@mui/icons-material/EditRounded';
import DeleteRoundedIcon from '@mui/icons-material/DeleteRounded';
import VisibilityRoundedIcon from '@mui/icons-material/VisibilityRounded';
import PageHeader from '../../components/common/PageHeader';
import DataTable, { type DataTableColumn } from '../../components/common/DataTable';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import ResidentFormDialog from './ResidentFormDialog';
import ResidentDetailsDialog from './ResidentDetailsDialog';
import { residentApi } from '../../api/residentApi';
import { extractErrorMessage } from '../../api/axiosInstance';
import { useToast } from '../../context/ToastContext';
import { useDebounce } from '../../hooks/useDebounce';
import type { CreateResidentRequest, ResidentDto, UpdateResidentRequest } from '../../types';

export default function ResidentsPage() {
  const { showSuccess, showError } = useToast();

  const [residents, setResidents] = useState<ResidentDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [searchInput, setSearchInput] = useState('');
  const debouncedSearch = useDebounce(searchInput, 400);
  const [isLoading, setIsLoading] = useState(true);

  const [formOpen, setFormOpen] = useState(false);
  const [editingResident, setEditingResident] = useState<ResidentDto | null>(null);
  const [isSaving, setIsSaving] = useState(false);

  const [viewingResident, setViewingResident] = useState<ResidentDto | null>(null);
  const [deletingResident, setDeletingResident] = useState<ResidentDto | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const loadResidents = useCallback(async () => {
    setIsLoading(true);
    try {
      const result = await residentApi.getPaged(page + 1, pageSize, debouncedSearch);
      setResidents(result.items);
      setTotalCount(result.totalCount);
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page, pageSize, debouncedSearch]);

  useEffect(() => {
    loadResidents();
  }, [loadResidents]);

  useEffect(() => {
    setPage(0);
  }, [debouncedSearch]);

  const handleCreate = async (payload: CreateResidentRequest) => {
    setIsSaving(true);
    try {
      await residentApi.create(payload);
      showSuccess('Resident added successfully.');
      setFormOpen(false);
      loadResidents();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleUpdate = async (id: number, payload: UpdateResidentRequest) => {
    setIsSaving(true);
    try {
      await residentApi.update(id, payload);
      showSuccess('Resident updated successfully.');
      setFormOpen(false);
      setEditingResident(null);
      loadResidents();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!deletingResident) return;
    setIsDeleting(true);
    try {
      await residentApi.remove(deletingResident.id);
      showSuccess('Resident deleted successfully.');
      setDeletingResident(null);
      loadResidents();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsDeleting(false);
    }
  };

  const columns: DataTableColumn<ResidentDto>[] = [
    { key: 'fullName', header: 'Name', render: (r) => r.fullName },
    { key: 'email', header: 'Email', render: (r) => r.email },
    { key: 'zone', header: 'Zone', render: (r) => r.zone },
    { key: 'address', header: 'City', render: (r) => r.city },
    {
      key: 'status',
      header: 'Status',
      render: (r) => (
        <Chip size="small" label={r.isActive ? 'Active' : 'Inactive'} color={r.isActive ? 'success' : 'default'} sx={{ fontWeight: 700 }} />
      ),
    },
    {
      key: 'actions',
      header: '',
      align: 'right',
      render: (r) => (
        <Box sx={{ display: 'flex', justifyContent: 'flex-end', gap: 0.5 }}>
          <Tooltip title="View details">
            <IconButton size="small" onClick={() => setViewingResident(r)}>
              <VisibilityRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Edit">
            <IconButton
              size="small"
              onClick={() => {
                setEditingResident(r);
                setFormOpen(true);
              }}
            >
              <EditRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete">
            <IconButton size="small" color="error" onClick={() => setDeletingResident(r)}>
              <DeleteRoundedIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
      ),
    },
  ];

  return (
    <Box>
      <PageHeader
        title="Resident Management"
        subtitle="Add, search, and manage every resident registered in the system."
        action={
          <Button
            variant="contained"
            startIcon={<AddRoundedIcon />}
            onClick={() => {
              setEditingResident(null);
              setFormOpen(true);
            }}
          >
            Add resident
          </Button>
        }
      />

      <TextField
        placeholder="Search by name, email, city or zone..."
        fullWidth
        value={searchInput}
        onChange={(e) => setSearchInput(e.target.value)}
        sx={{ mb: 2.5, maxWidth: 420 }}
        InputProps={{
          startAdornment: (
            <InputAdornment position="start">
              <SearchRoundedIcon fontSize="small" />
            </InputAdornment>
          ),
        }}
      />

      <DataTable
        columns={columns}
        rows={residents}
        getRowId={(r) => r.id}
        isLoading={isLoading}
        emptyMessage="No residents match your search yet."
        page={page}
        pageSize={pageSize}
        totalCount={totalCount}
        onPageChange={setPage}
        onPageSizeChange={(size) => {
          setPageSize(size);
          setPage(0);
        }}
      />

      <ResidentFormDialog
        open={formOpen}
        resident={editingResident}
        isSaving={isSaving}
        onClose={() => {
          setFormOpen(false);
          setEditingResident(null);
        }}
        onCreate={handleCreate}
        onUpdate={handleUpdate}
      />

      <ResidentDetailsDialog open={!!viewingResident} resident={viewingResident} onClose={() => setViewingResident(null)} />

      <ConfirmDialog
        open={!!deletingResident}
        title="Delete resident?"
        message={`This will permanently delete ${deletingResident?.fullName ?? 'this resident'}, along with their login account, requests and complaints. This cannot be undone.`}
        confirmLabel="Delete"
        isDestructive
        isLoading={isDeleting}
        onConfirm={handleDelete}
        onCancel={() => setDeletingResident(null)}
      />
    </Box>
  );
}
