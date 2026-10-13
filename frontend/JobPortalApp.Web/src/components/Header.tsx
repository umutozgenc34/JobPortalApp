import { Link, NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { useTheme } from "../theme/ThemeContext";

export function Logo() {
  return (
    <span className="logo">
      <span className="logo__mark" aria-hidden="true">J</span>
      <span className="logo__text">JobPortal</span>
    </span>
  );
}

export function Header() {
  const { user, logout } = useAuth();
  const { theme, setTheme } = useTheme();
  const navigate = useNavigate();

  async function handleLogout() {
    await logout();
    navigate("/");
  }

  return (
    <header className="header">
      <div className="container header__inner">
        <Link to="/" aria-label="JobPortal ana sayfa">
          <Logo />
        </Link>

        <nav className="header__nav" aria-label="Ana menü">
          <NavLink to="/ilanlar" className="header__link">İş ilanları</NavLink>
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
              <span className="header__user">
                <span className="avatar" aria-hidden="true">{user.userName.slice(0, 1).toLocaleUpperCase("tr-TR")}</span>
                <span className="header__user-name">{user.userName}</span>
              </span>
              <button type="button" className="button button--ghost" onClick={handleLogout}>Çıkış yap</button>
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
