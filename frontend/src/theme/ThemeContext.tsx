import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react';
import { ThemeProvider as MuiThemeProvider, CssBaseline } from '@mui/material';
import type { PaletteMode } from '@mui/material';
import { buildTheme } from './theme';

interface ColorModeContextType {
  mode: PaletteMode;
  toggleColorMode: () => void;
}

const ColorModeContext = createContext<ColorModeContextType | undefined>(undefined);

const STORAGE_KEY = 'gms-color-mode';

export function ColorModeProvider({ children }: { children: ReactNode }) {
  const [mode, setMode] = useState<PaletteMode>(() => {
    const stored = window.localStorage.getItem(STORAGE_KEY);
    if (stored === 'light' || stored === 'dark') return stored;
    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  });

  useEffect(() => {
    window.localStorage.setItem(STORAGE_KEY, mode);
  }, [mode]);

  const toggleColorMode = () => {
    setMode((prev) => (prev === 'light' ? 'dark' : 'light'));
  };

  const theme = useMemo(() => buildTheme(mode), [mode]);

  return (
    <ColorModeContext.Provider value={{ mode, toggleColorMode }}>
      <MuiThemeProvider theme={theme}>
        <CssBaseline />
        {children}
      </MuiThemeProvider>
    </ColorModeContext.Provider>
  );
}

export function useColorMode() {
  const context = useContext(ColorModeContext);
  if (!context) {
    throw new Error('useColorMode must be used within a ColorModeProvider');
  }
  return context;
}
