import { createContext, useCallback, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { getTokens, onTokensChange, setTokens } from "../api/client";
import { authApi } from "../api/endpoints";
import type { CurrentUser } from "../api/types";

interface AuthState {
  user: CurrentUser | null;
  loading: boolean;
  isAdmin: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (userName: string, email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [loading, setLoading] = useState(() => Boolean(getTokens()));

  const loadUser = useCallback(async () => {
    if (!getTokens()) {
      setUser(null);
      setLoading(false);
      return;
    }
    try {
      setUser(await authApi.me());
    } catch {
      setUser(null);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadUser();
    return onTokensChange((tokens) => {
      if (!tokens) setUser(null);
    });
  }, [loadUser]);

  const login = useCallback(
    async (email: string, password: string) => {
      const tokens = await authApi.login(email, password);
      setTokens(tokens);
      await loadUser();
    },
    [loadUser]
  );

  const register = useCallback(
    async (userName: string, email: string, password: string) => {
      await authApi.register(userName, email, password);
      await login(email, password);
    },
    [login]
  );

  const logout = useCallback(async () => {
    const tokens = getTokens();
    setTokens(null);
    setUser(null);
    if (tokens) {
      try {
        await authApi.revoke(tokens.refreshToken);
      } catch {
        return;
      }
    }
  }, []);

  const value = useMemo<AuthState>(
    () => ({
      user,
      loading,
      isAdmin: Boolean(user?.roles.some((role) => role === "Admin" || role === "SuperAdmin")),
      login,
      register,
      logout,
      refreshUser: loadUser
    }),
    [user, loading, login, register, logout, loadUser]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider");
  return context;
}
