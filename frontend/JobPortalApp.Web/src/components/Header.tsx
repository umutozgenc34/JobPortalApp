import { useEffect, useRef, useState } from "react";
import { Link, NavLink, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { useNotifications } from "../notifications/NotificationsContext";
import { useTheme } from "../theme/ThemeContext";

export function Logo() {
  return (
    <span className="logo">
      <span className="logo__mark" aria-hidden="true">J</span>
      <span className="logo__text">JobPortal</span>
    </span>
  );
}

function UserMenu() {
  const { user, isAdmin, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => setOpen(false), [location.pathname]);

  useEffect(() => {
    if (!open) return;
    const onPointer = (event: PointerEvent) => {
      if (!ref.current?.contains(event.target as Node)) setOpen(false);
    };
    const onKey = (event: KeyboardEvent) => event.key === "Escape" && setOpen(false);
    document.addEventListener("pointerdown", onPointer);
    document.addEventListener("keydown", onKey);
    return () => {
      document.removeEventListener("pointerdown", onPointer);
      document.removeEventListener("keydown", onKey);
    };
  }, [open]);

  if (!user) return null;

  async function handleLogout() {
    await logout();
    navigate("/");
  }

  return (
    <div className="user-menu" ref={ref}>
      <button type="button" className="user-menu__trigger" aria-expanded={open} aria-haspopup="menu" onClick={() => setOpen((v) => !v)}>
        <span className="avatar" aria-hidden="true">{user.userName.slice(0, 1).toLocaleUpperCase("tr-TR")}</span>
        <span className="header__user-name">{user.userName}</span>
      </button>
      {open && (
        <div className="user-menu__panel panel" role="menu">
          <div className="user-menu__who">
            <strong>{user.userName}</strong>
            <span className="muted">{user.email}</span>
          </div>
          <Link to="/profil" role="menuitem">Profilim</Link>
          <Link to="/basvurularim" role="menuitem">Başvurularım</Link>
          <Link to="/bildirimler" role="menuitem">Bildirimler</Link>
          {isAdmin && <Link to="/yonetim" role="menuitem">Yönetim paneli</Link>}
          <button type="button" role="menuitem" onClick={handleLogout}>Çıkış yap</button>
        </div>
      )}
    </div>
  );
}

export function Header() {
  const { user } = useAuth();
  const { unread } = useNotifications();
  const { theme, setTheme } = useTheme();

  return (
    <header className="header">
      <div className="container header__inner">
        <Link to="/" aria-label="JobPortal ana sayfa">
          <Logo />
        </Link>

        <nav className="header__nav" aria-label="Ana menü">
          <NavLink to="/ilanlar" className="header__link">İş ilanları</NavLink>
          <NavLink to="/sirketler" className="header__link">Şirketler</NavLink>
        </nav>

        <div className="header__actions">
          <button
            type="button"
            className="theme-toggle"
            aria-label={theme === "light" ? "Koyu temaya geç" : "Açık temaya geç"}
            title={theme === "light" ? "Koyu tema" : "Açık tema"}
            onClick={() => setTheme(theme === "light" ? "dark" : "light")}
          >
            <span className="theme-toggle__dot" data-active={theme === "light"} />
            <span className="theme-toggle__dot theme-toggle__dot--dark" data-active={theme === "dark"} />
          </button>

          {user ? (
            <>
              <Link to="/bildirimler" className="bell" aria-label={unread ? `Bildirimler, ${unread} okunmamış` : "Bildirimler"}>
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
                  <path d="M6 8a6 6 0 1 1 12 0c0 7 3 9 3 9H3s3-2 3-9" />
                  <path d="M10.3 21a1.94 1.94 0 0 0 3.4 0" />
                </svg>
                {unread > 0 && <span className="bell__count">{unread > 99 ? "99+" : unread}</span>}
              </Link>
              <UserMenu />
            </>
          ) : (
            <>
              <Link to="/giris" className="button button--ghost">Giriş yap</Link>
              <Link to="/kayit" className="button button--primary header__register">Kayıt ol</Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
