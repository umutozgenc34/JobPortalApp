import { createContext, useContext, useEffect, useState, type ReactNode } from "react";

export const themes = [
  { id: "light", label: "Açık tema", swatch: "#FFFFFF", meta: "#F5F5F2" },
  { id: "dark", label: "Koyu tema", swatch: "#15171C", meta: "#0F1114" }
] as const;

export type ThemeId = (typeof themes)[number]["id"];

const STORAGE_KEY = "jp.theme";

const ThemeContext = createContext<{ theme: ThemeId; setTheme: (theme: ThemeId) => void } | null>(null);

function readTheme(): ThemeId {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    return themes.some((t) => t.id === stored) ? (stored as ThemeId) : "light";
  } catch {
    return "light";
  }
}

export function ThemeProvider({ children }: { children: ReactNode }) {
  const [theme, setTheme] = useState<ThemeId>(readTheme);

  useEffect(() => {
    document.documentElement.dataset.theme = theme;
    const meta = document.querySelector('meta[name="theme-color"]');
    meta?.setAttribute("content", themes.find((t) => t.id === theme)!.meta);
    try {
      localStorage.setItem(STORAGE_KEY, theme);
    } catch {
      return;
    }
  }, [theme]);

  return <ThemeContext.Provider value={{ theme, setTheme }}>{children}</ThemeContext.Provider>;
}

export function useTheme() {
  const context = useContext(ThemeContext);
  if (!context) throw new Error("useTheme must be used inside ThemeProvider");
  return context;
}
