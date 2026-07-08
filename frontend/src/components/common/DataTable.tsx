import type { ReactNode } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Paper,
  Box,
  Typography,
} from '@mui/material';
import InboxRoundedIcon from '@mui/icons-material/InboxRounded';
import LoadingSpinner from './LoadingSpinner';

export interface DataTableColumn<T> {
  key: string;
  header: string;
  render: (row: T) => ReactNode;
  align?: 'left' | 'right' | 'center';
  width?: string | number;
}

interface DataTableProps<T> {
  columns: DataTableColumn<T>[];
  rows: T[];
  getRowId: (row: T) => string | number;
  isLoading?: boolean;
  emptyMessage?: string;
  page: number; // zero-indexed
  pageSize: number;
  totalCount: number;
  onPageChange: (page: number) => void;
  onPageSizeChange: (pageSize: number) => void;
}

export default function DataTable<T>({
  columns,
  rows,
  getRowId,
  isLoading = false,
  emptyMessage = 'No records found.',
  page,
  pageSize,
  totalCount,
  onPageChange,
  onPageSizeChange,
}: DataTableProps<T>) {
  return (
    <Paper variant="outlined" sx={{ borderRadius: 2.5, overflow: 'hidden' }}>
      <TableContainer sx={{ maxHeight: 620 }}>
        <Table stickyHeader size="small">
          <TableHead>
            <TableRow>
              {columns.map((col) => (
                <TableCell key={col.key} align={col.align ?? 'left'} sx={{ width: col.width }}>
                  {col.header}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {isLoading ? (
              <TableRow>
                <TableCell colSpan={columns.length} sx={{ border: 'none' }}>
                  <LoadingSpinner label="Loading records..." minHeight={200} />
                </TableCell>
              </TableRow>
            ) : rows.length === 0 ? (
              <TableRow>
                <TableCell colSpan={columns.length} sx={{ border: 'none' }}>
                  <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', py: 6, gap: 1, color: 'text.secondary' }}>
                    <InboxRoundedIcon sx={{ fontSize: 36, opacity: 0.5 }} />
                    <Typography variant="body2">{emptyMessage}</Typography>
                  </Box>
                </TableCell>
              </TableRow>
            ) : (
              rows.map((row) => (
                <TableRow key={getRowId(row)} hover>
                  {columns.map((col) => (
                    <TableCell key={col.key} align={col.align ?? 'left'}>
                      {col.render(row)}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={totalCount}
        page={page}
        onPageChange={(_, newPage) => onPageChange(newPage)}
        rowsPerPage={pageSize}
        onRowsPerPageChange={(e) => onPageSizeChange(parseInt(e.target.value, 10))}
        rowsPerPageOptions={[5, 10, 25, 50]}
      />
    </Paper>
  );
}
