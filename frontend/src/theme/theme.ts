import { createTheme, type ThemeOptions, type PaletteMode } from '@mui/material/styles';

/**
 * Design tokens for the Garbage Management System.
 *
 * Palette: deep emerald/teal as the brand primary (sustainability, waste
 * management), slate blue as a secondary accent, and status colors that are
 * deliberately distinct from the brand green so "brand" and "completed" never
 * get confused at a glance.
 */
const brand = {
  primaryMain: '#0F6B52',
  primaryDark: '#0A4E3B',
  primaryLight: '#3D9679',
  secondaryMain: '#3E6EA8',
  secondaryDark: '#2C5480',
};

export const statusColors = {
  pending: '#B8790F',
  inProgress: '#3E6EA8',
  completed: '#1E8E5A',
  scheduled: '#3E6EA8',
  missed: '#B8790F',
  cancelled: '#6B7280',
  open: '#C24A2E',
  resolved: '#1E8E5A',
  closed: '#6B7280',
};

function getDesignTokens(mode: PaletteMode): ThemeOptions {
  const isLight = mode === 'light';

  return {
    palette: {
      mode,
      primary: {
        main: brand.primaryMain,
        dark: brand.primaryDark,
        light: brand.primaryLight,
        contrastText: '#FFFFFF',
      },
      secondary: {
        main: brand.secondaryMain,
        dark: brand.secondaryDark,
        contrastText: '#FFFFFF',
      },
      background: {
        default: isLight ? '#F5F8F6' : '#0F1614',
        paper: isLight ? '#FFFFFF' : '#161F1C',
      },
      text: {
        primary: isLight ? '#151E1B' : '#EAF1EE',
        secondary: isLight ? '#4B5A55' : '#A9B8B2',
      },
      divider: isLight ? 'rgba(15, 30, 27, 0.09)' : 'rgba(234, 241, 238, 0.10)',
      error: { main: '#C24A2E' },
      warning: { main: '#B8790F' },
      success: { main: '#1E8E5A' },
      info: { main: '#3E6EA8' },
    },
    typography: {
      fontFamily: '"Inter", "Segoe UI", system-ui, sans-serif',
      h1: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
      h2: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
      h3: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700 },
      h4: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 700, letterSpacing: '-0.01em' },
      h5: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 600 },
      h6: { fontFamily: '"Plus Jakarta Sans", sans-serif', fontWeight: 600 },
      subtitle1: { fontWeight: 500 },
      subtitle2: { fontWeight: 500 },
      button: { fontWeight: 600, textTransform: 'none' },
    },
    shape: {
      borderRadius: 10,
    },
    spacing: 8,
    components: {
      MuiButton: {
        styleOverrides: {
          root: {
            borderRadius: 8,
            paddingTop: 8,
            paddingBottom: 8,
            boxShadow: 'none',
          },
          contained: {
            '&:hover': { boxShadow: 'none' },
          },
        },
        defaultProps: {
          disableElevation: true,
        },
      },
      MuiPaper: {
        styleOverrides: {
          root: {
            backgroundImage: 'none',
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            borderRadius: 14,
            border: `1px solid ${isLight ? 'rgba(15, 30, 27, 0.08)' : 'rgba(234, 241, 238, 0.08)'}`,
          },
        },
      },
      MuiChip: {
        styleOverrides: {
          root: {
            fontWeight: 600,
          },
        },
      },
      MuiTableCell: {
        styleOverrides: {
          head: {
            fontWeight: 700,
            fontSize: '0.75rem',
            textTransform: 'uppercase',
            letterSpacing: '0.04em',
            color: isLight ? '#4B5A55' : '#A9B8B2',
          },
        },
      },
      MuiTextField: {
        defaultProps: {
          size: 'small',
        },
      },
      MuiFormControl: {
        defaultProps: {
          size: 'small',
        },
      },
    },
  };
}

export function buildTheme(mode: PaletteMode) {
  return createTheme(getDesignTokens(mode));
}
