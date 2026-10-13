import type { ReactNode } from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "./AuthContext";

export function RequireAuth({ children, admin = false }: { children: ReactNode; admin?: boolean }) {
  const { user, loading, isAdmin } = useAuth();
  const location = useLocation();

  if (loading) {
    return (
      <div className="container page">
        <div className="skeleton" style={{ height: 160 }} />
      </div>
    );
  }

  if (!user) return <Navigate to="/giris" replace state={{ from: location.pathname }} />;

  if (admin && !isAdmin) {
    return (
      <div className="container page state">
        <h1>Bu sayfaya erişimin yok</h1>
        <p className="muted">Yönetim paneli yalnızca yönetici hesaplarına açık.</p>
      </div>
    );
  }

  return <>{children}</>;
}
