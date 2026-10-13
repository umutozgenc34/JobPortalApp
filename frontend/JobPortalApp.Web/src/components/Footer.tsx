import { Link } from "react-router-dom";
import { Logo } from "./Header";

export function Footer() {
  return (
    <footer className="footer">
      <div className="container footer__inner">
        <div className="footer__brand">
          <Logo />
          <p className="muted">Açık pozisyonlar, şirketler ve başvurular tek yerde.</p>
        </div>
        <nav className="footer__links" aria-label="Alt menü">
          <Link to="/ilanlar">İş ilanları</Link>
          <Link to="/giris">Giriş yap</Link>
          <Link to="/kayit">Kayıt ol</Link>
        </nav>
        <p className="footer__copy muted">© {new Date().getFullYear()} JobPortal</p>
      </div>
    </footer>
  );
}
