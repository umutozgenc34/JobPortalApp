import { NavLink, Navigate, Route, Routes } from "react-router-dom";
import { AdminJobs } from "./AdminJobs";
import { AdminCompanies } from "./AdminCompanies";
import { AdminCategories } from "./AdminCategories";

export function Admin() {
  return (
    <div className="container page">
      <header className="page-header">
        <h1>Yönetim paneli</h1>
        <p className="muted">İlanları, şirketleri ve çalışma alanlarını yönet.</p>
      </header>

      <nav className="tabs" aria-label="Yönetim bölümleri">
        <NavLink to="/yonetim/ilanlar" className="tabs__item">İlanlar</NavLink>
        <NavLink to="/yonetim/sirketler" className="tabs__item">Şirketler</NavLink>
        <NavLink to="/yonetim/alanlar" className="tabs__item">Alanlar</NavLink>
      </nav>

      <Routes>
        <Route index element={<Navigate to="ilanlar" replace />} />
        <Route path="ilanlar" element={<AdminJobs />} />
        <Route path="sirketler" element={<AdminCompanies />} />
        <Route path="alanlar" element={<AdminCategories />} />
      </Routes>
    </div>
  );
}
