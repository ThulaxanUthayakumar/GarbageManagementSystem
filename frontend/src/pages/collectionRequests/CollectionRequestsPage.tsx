import { useCallback, useEffect, useState } from 'react';
import {
  Box,
  Button,
  TextField,
  InputAdornment,
  MenuItem,
  IconButton,
  Tooltip,
  Select,
  Avatar,
  Typography,
} from '@mui/material';
import SearchRoundedIcon from '@mui/icons-material/SearchRounded';
import AddRoundedIcon from '@mui/icons-material/AddRounded';
import DeleteRoundedIcon from '@mui/icons-material/DeleteRounded';
import ImageRoundedIcon from '@mui/icons-material/ImageRounded';
import PageHeader from '../../components/common/PageHeader';
import DataTable, { type DataTableColumn } from '../../components/common/DataTable';
import StatusChip from '../../components/common/StatusChip';
import ConfirmDialog from '../../components/common/ConfirmDialog';
import RequestFormDialog from './RequestFormDialog';
import { useAuth } from '../../hooks/useAuth';
import { collectionRequestApi } from '../../api/collectionRequestApi';
import { extractErrorMessage, resolveImageUrl } from '../../api/axiosInstance';
import { useToast } from '../../context/ToastContext';
import { useDebounce } from '../../hooks/useDebounce';
import type { CollectionRequestDto, CreateCollectionRequestPayload, RequestStatus } from '../../types';

const STATUS_OPTIONS: RequestStatus[] = ['Pending', 'InProgress', 'Completed', 'Cancelled'];

export default function CollectionRequestsPage() {
  const { isAdmin } = useAuth();
  const { showSuccess, showError } = useToast();

  const [requests, setRequests] = useState<CollectionRequestDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  const [searchInput, setSearchInput] = useState('');
  const debouncedSearch = useDebounce(searchInput, 400);
  const [statusFilter, setStatusFilter] = useState<RequestStatus | ''>('');
  const [isLoading, setIsLoading] = useState(true);

  const [formOpen, setFormOpen] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [deletingRequest, setDeletingRequest] = useState<CollectionRequestDto | null>(null);
  const [isDeleting, setIsDeleting] = useState(false);

  const load = useCallback(async () => {
    setIsLoading(true);
    try {
      if (isAdmin) {
        const result = await collectionRequestApi.getPaged(page + 1, pageSize, debouncedSearch, statusFilter || undefined);
        setRequests(result.items);
        setTotalCount(result.totalCount);
      } else {
        const result = await collectionRequestApi.getMy();
        setRequests(result);
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

  const handleCreate = async (payload: CreateCollectionRequestPayload) => {
    setIsSaving(true);
    try {
      await collectionRequestApi.create(payload);
      showSuccess('Collection request submitted.');
      setFormOpen(false);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsSaving(false);
    }
  };

  const handleStatusChange = async (id: number, status: RequestStatus) => {
    try {
      await collectionRequestApi.updateStatus(id, status);
      showSuccess('Request status updated.');
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    }
  };

  const handleDelete = async () => {
    if (!deletingRequest) return;
    setIsDeleting(true);
    try {
      await collectionRequestApi.remove(deletingRequest.id);
      showSuccess('Request deleted.');
      setDeletingRequest(null);
      load();
    } catch (error) {
      showError(extractErrorMessage(error));
    } finally {
      setIsDeleting(false);
    }
  };

  const rows = isAdmin ? requests : requests.slice(page * pageSize, page * pageSize + pageSize);
  const effectiveTotal = isAdmin ? totalCount : requests.length;

  const columns: DataTableColumn<CollectionRequestDto>[] = [
    {
      key: 'photo',
      header: '',
      width: 48,
      render: (r) =>
        r.imageUrl ? (
          <Avatar variant="rounded" src={resolveImageUrl(r.imageUrl)} sx={{ width: 36, height: 36 }} />
        ) : (
          <Avatar variant="rounded" sx={{ width: 36, height: 36, bgcolor: 'action.hover', color: 'text.disabled' }}>
            <ImageRoundedIcon fontSize="small" />
          </Avatar>
        ),
    },
    ...(isAdmin
      ? [{ key: 'resident', header: 'Resident', render: (r: CollectionRequestDto) => r.residentName } as DataTableColumn<CollectionRequestDto>]
      : []),
    { key: 'category', header: 'Waste category', render: (r) => r.wasteCategoryName },
    ...(isAdmin ? [{ key: 'zone', header: 'Zone', render: (r: CollectionRequestDto) => r.zone } as DataTableColumn<CollectionRequestDto>] : []),
    { key: 'pickupDate', header: 'Pickup date', render: (r) => new Date(r.pickupDate).toLocaleDateString() },
    {
      key: 'status',
      header: 'Status',
      render: (r) =>
        isAdmin ? (
          <Select
            size="small"
            value={r.status}
            onChange={(e) => handleStatusChange(r.id, e.target.value as RequestStatus)}
            sx={{ minWidth: 130, fontSize: '0.8rem' }}
          >
            {STATUS_OPTIONS.map((s) => (
              <MenuItem key={s} value={s} sx={{ fontSize: '0.8rem' }}>
                {s === 'InProgress' ? 'In Progress' : s}
              </MenuItem>
            ))}
          </Select>
        ) : (
          <StatusChip status={r.status} />
        ),
    },
    ...(isAdmin
      ? [
          {
            key: 'actions',
            header: '',
            align: 'right' as const,
            render: (r: CollectionRequestDto) => (
              <Tooltip title="Delete">
                <IconButton size="small" color="error" onClick={() => setDeletingRequest(r)}>
                  <DeleteRoundedIcon fontSize="small" />
                </IconButton>
              </Tooltip>
            ),
          } as DataTableColumn<CollectionRequestDto>,
        ]
      : []),
  ];

  return (
    <Box>
      <PageHeader
        title={isAdmin ? 'All Collection Requests' : 'My Collection Requests'}
        subtitle={isAdmin ? 'Review and update the status of every pickup request.' : 'Request a pickup and track its progress.'}
        action={
          !isAdmin && (
            <Button variant="contained" startIcon={<AddRoundedIcon />} onClick={() => setFormOpen(true)}>
              New request
            </Button>
          )
        }
      />

      <Box sx={{ display: 'flex', gap: 2, mb: 2.5, flexWrap: 'wrap' }}>
        {isAdmin && (
          <TextField
            placeholder="Search by resident or category..."
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
          <TextField select label="Status" value={statusFilter} onChange={(e) => setStatusFilter(e.target.value as RequestStatus | '')} sx={{ minWidth: 170 }}>
            <MenuItem value="">All statuses</MenuItem>
            {STATUS_OPTIONS.map((s) => (
              <MenuItem key={s} value={s}>
                {s === 'InProgress' ? 'In Progress' : s}
              </MenuItem>
            ))}
          </TextField>
        )}
      </Box>

      {!isAdmin && requests.length === 0 && !isLoading && (
        <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
          You haven&apos;t submitted any collection requests yet.
        </Typography>
      )}

      <DataTable
        columns={columns}
        rows={rows}
        getRowId={(r) => r.id}
        isLoading={isLoading}
        emptyMessage={isAdmin ? 'No collection requests match your filters.' : 'No requests yet — create your first one above.'}
        page={page}
        pageSize={pageSize}
        totalCount={effectiveTotal}
        onPageChange={setPage}
        onPageSizeChange={(size) => {
          setPageSize(size);
          setPage(0);
        }}
      />

      <RequestFormDialog open={formOpen} isSaving={isSaving} onClose={() => setFormOpen(false)} onSubmit={handleCreate} />

      <ConfirmDialog
        open={!!deletingRequest}
        title="Delete collection request?"
        message="This will permanently remove this request. This cannot be undone."
        confirmLabel="Delete"
        isDestructive
        isLoading={isDeleting}
        onConfirm={handleDelete}
        onCancel={() => setDeletingRequest(null)}
      />
    </Box>
  );
}
