import { useCallback, useEffect, useState } from 'react';
import { Box, Button, TextField, InputAdornment, MenuItem, IconButton, Tooltip, Select, Avatar, Typography, Popover } from '@mui/material';
import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import DeleteRoundedIcon from '@mui/icons-material/DeleteRounded';
import ImageRoundedIcon from '@mui/icons-material/ImageRounded';
import NotesRoundedIcon from '@mui/icons-material/NotesRounded';
import PageHeader from '../../components/common/PageHeader';
import DataTable, { type DataTableColumn } from '../../components/common/DataTable';
import StatusChip from '../../components/common/StatusChip';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import ComplaintFormDialog from './ComplaintFormDialog';
import { useAuth } from '../../hooks/useAuth';
import { complaintApi } from '../../api/complaintApi';
import { extractErrorMessage, resolveImageUrl } from '../../api/axiosInstance';
import { useToast } from '../../context/ToastContext';
import { useDebounce } from '../../hooks/useDebounce';
import type { ComplaintDto, ComplaintStatus, CreateComplaintPayload } from '../../types';

const STATUS_OPTIONS: ComplaintStatus[] = ['Open', 'InProgress', 'Resolved', 'Closed'];

export default function ComplaintsPage() {
  const { isAdmin } = useAuth();
  const { showSuccess, showError } = useToast();

  const [complaints, setComplaints] = useState<ComplaintDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [searchInput, setSearchInput] = useState('');
  const debouncedSearch = useDebounce(searchInput, 400);
  const [statusFilter, setStatusFilter] = useState<ComplaintStatus | ''>('');
  const [isLoading, setIsLoading] = useState(true);

  const [formOpen, setFormOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [deletingComplaint, setDeletingComplaint] = useState<ComplaintDto | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);
  const [descriptionAnchor, setDescriptionAnchor] = useState<{ el: HTMLElement; text: string } | null>(null);

  const load = useCallback(async () => {
    setIsLoading(true);
    try {
      if (isAdmin) {
        const result = await complaintApi.getPaged(page + 1, pageSize, debouncedSearch, statusFilter || undefined);
        setComplaints(result.items);
        setTotalCount(result.totalCount);
      } else {
        const result = await complaintApi.getMy();
        setComplaints(result);
        setTotalCount(result.length);
      }
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAdmin, page, pageSize, debouncedSearch, statusFilter]);

  useEffect(() => {
    load();
  }, [load]);

  useEffect(() => {
    setPage(0);
  }, [debouncedSearch, statusFilter]);

  const handleCreate = async (payload: CreateComplaintPayload) => {
    setIsSaving(true);
    try {
      await complaintApi.create(payload);
      showSuccess('Complaint submitted.');
      setFormOpen(false);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleStatusChange = async (id: number, status: ComplaintStatus) => {
    try {
      await complaintApi.updateStatus(id, { status });
      showSuccess('Complaint status updated.');
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    }
  };

  const handleDelete = async () => {
    if (!deletingComplaint) return;
    setIsDeleting(true);
    try {
      await complaintApi.remove(deletingComplaint.id);
      showSuccess('Complaint deleted.');
      setDeletingComplaint(null);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsDeleting(false);
    }
  };

  const rows = isAdmin ? complaints : complaints.slice(page * pageSize, page * pageSize + pageSize);
  const effectiveTotal = isAdmin ? totalCount : complaints.length;

  const columns: DataTableColumn<ComplaintDto>[] = [
    {
      key: 'photo',
      header: '',
      width: 48,
      render: (c) =>
        c.imageUrl ? (
          <Avatar variant="rounded" src={resolveImageUrl(c.imageUrl)} sx={{ width: 36, height: 36 }} />
        ) : (
          <Avatar variant="rounded" sx={{ width: 36, height: 36, bgcolor: 'action.hover', color: 'text.disabled' }}>
            <ImageRoundedIcon fontSize="small" />
          </Avatar>
        ),
    },
    { key: 'subject', header: 'Subject', render: (c) => c.subject },
    ...(isAdmin
      ? [{ key: 'resident', header: 'Resident', render: (c: ComplaintDto) => c.residentName } as DataTableColumn<ComplaintDto>]
      : []),
    {
      key: 'description',
      header: 'Details',
      render: (c) => (
        <Tooltip title="View full description">
          <IconButton size="small" onClick={(e) => setDescriptionAnchor({ el: e.currentTarget, text: c.description })}>
            <NotesRoundedIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      ),
    },
    { key: 'createdAt', header: 'Submitted', render: (c) => new Date(c.createdAt).toLocaleDateString() },
    {
      key: 'status',
      header: 'Status',
      render: (c) =>
        isAdmin ? (
          <Select size="small" value={c.status} onChange={(e) => handleStatusChange(c.id, e.target.value as ComplaintStatus)} sx={{ minWidth: 130, fontSize: '0.8rem' }}>
            {STATUS_OPTIONS.map((s) => (
              <MenuItem key={s} value={s} sx={{ fontSize: '0.8rem' }}>
                {s === 'InProgress' ? 'In Progress' : s}
              </MenuItem>
            ))}
          </Select>
        ) : (
          <StatusChip status={c.status} />
        ),
    },
    ...(isAdmin
      ? [
          {
            key: 'actions',
            header: '',
            align: 'right' as const,
            render: (c: ComplaintDto) => (
              <Tooltip title="Delete">
                <IconButton size="small" color="error" onClick={() => setDeletingComplaint(c)}>
                  <DeleteRoundedIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            ),
          } as DataTableColumn<ComplaintDto>,
        ]
      : []),
  ];

  return (
    <Box>
      <PageHeader
        title={isAdmin ? 'All Complaints' : 'My Complaints'}
        subtitle={isAdmin ? 'Triage and resolve complaints raised by residents.' : 'Submit a complaint and track it through to resolution.'}
        action={
          !isAdmin && (
            <Button variant="contained" color="error" startIcon={<AddRoundedIcon />} onClick={() => setFormOpen(true)}>
              New complaint
            </Button>
          )
        }
      />

      <Box sx={{ display: 'flex', gap: 2, mb: 2.5, flexWrap: 'wrap' }}>
        {isAdmin && (
          <TextField
            placeholder="Search by subject or resident..."
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            sx={{ minWidth: 260, flex: 1, maxWidth: 380 }}
            InputProps={{
              startAdornment: (
                <InputAdornment position="start">
                  <SearchRoundedIcon fontSize="small" />
                </InputAdornment>
              ),
            }}
          />
        )}
        {isAdmin && (
          <TextField select label="Status" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value as ComplaintStatus | '')} sx={{ minWidth: 170 }}>
            <MenuItem value="">All statuses</MenuItem>
            {STATUS_OPTIONS.map((s) => (
              <MenuItem key={s} value={s}>
                {s === 'InProgress' ? 'In Progress' : s}
              </MenuItem>
            ))}
          </TextField>
        )}
      </Box>

      {!isAdmin && complaints.length === 0 && !isLoading && (
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          You haven&apos;t filed any complaints yet.
        </Typography>
      )}

      <DataTable
        columns={columns}
        rows={rows}
        getRowId={(c) => c.id}
        isLoading={isLoading}
        emptyMessage={isAdmin ? 'No complaints match your filters.' : 'Nothing to report — submit a complaint above if something comes up.'}
        page={page}
        pageSize={pageSize}
        totalCount={effectiveTotal}
        onPageChange={setPage}
        onPageSizeChange={(size) => {
          setPageSize(size);
          setPage(0);
        }}
      />

      <Popover
        open={!!descriptionAnchor}
        anchorEl={descriptionAnchor?.el}
        onClose={() => setDescriptionAnchor(null)}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Typography sx={{ p: 2, maxWidth: 320 }} variant="body2">
          {descriptionAnchor?.text}
        </Typography>
      </Popover>

      <ComplaintFormDialog open={formOpen} isSaving={isSaving} onClose={() => setFormOpen(false)} onSubmit={handleCreate} />

      <ConfirmDialog
        open={!!deletingComplaint}
        title="Delete complaint?"
        message="This will permanently remove this complaint. This cannot be undone."
        confirmLabel="Delete"
        isDestructive
        isLoading={isDeleting}
        onConfirm={handleDelete}
        onCancel={() => setDeletingComplaint(null)}
      />
    </Box>
  );
}
