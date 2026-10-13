import { Link } from "react-router-dom";

export function NotFound() {
  return (
    <div className="container page state">
      <p className="state__code">404</p>
      <h1>Sayfa bulunamadı</h1>
      <p className="muted">Aradığın sayfa taşınmış ya da hiç var olmamış olabilir.</p>
      <Link to="/" className="button">Ana sayfaya dön</Link>
    </div>
  );
}
