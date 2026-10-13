import { useCallback, useEffect, useState, type FormEvent } from "react";
import { Link } from "react-router-dom";
import { ApiError } from "../../api/client";
import { companiesApi } from "../../api/endpoints";
import type { Company } from "../../api/types";
import { CompanyMark } from "../../components/CompanyMark";
import { ConfirmButton } from "../../components/ConfirmButton";
import { Modal } from "../../components/Modal";

function errorText(e: unknown) {
  return e instanceof ApiError ? e.messages.join(" ") : "İşlem tamamlanamadı.";
}

function CompanyForm({ company, onSaved }: { company: Company | null; onSaved: () => void }) {
  const [values, setValues] = useState({
    name: company?.name ?? "",
    industry: company?.industry ?? "",
    location: company?.location ?? "",
    websiteUrl: company?.websiteUrl ?? "",
    email: company?.email ?? "",
    phoneNumber: company?.phoneNumber ?? "",
    about: company?.about ?? ""
  });
  const [logo, setLogo] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  function set(key: keyof typeof values, value: string) {
    setValues((current) => ({ ...current, [key]: value }));
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setError(null);
    const form = new FormData();
    form.append("Name", values.name.trim());
    form.append("Industry", values.industry.trim());
    form.append("About", values.about.trim());
    if (values.location.trim()) form.append("Location", values.location.trim());
    if (values.websiteUrl.trim()) form.append("WebsiteUrl", values.websiteUrl.trim());
    if (values.email.trim()) form.append("Email", values.email.trim());
    if (values.phoneNumber.trim()) form.append("PhoneNumber", values.phoneNumber.trim());
    if (logo) form.append("Logo", logo);
    try {
      if (company) await companiesApi.update(company.id, form);
      else await companiesApi.create(form);
      onSaved();
    } catch (e) {
      setError(errorText(e));
    } finally {
      setSaving(false);
    }
  }

  return (
    <form className="modal-form" onSubmit={handleSubmit}>
      <div className="form-grid">
        <label className="field">
          <span className="field__label">Şirket adı</span>
          <input className="input" value={values.name} maxLength={100} onChange={(e) => set("name", e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Sektör</span>
          <input className="input" value={values.industry} maxLength={50} onChange={(e) => set("industry", e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Konum</span>
          <input className="input" value={values.location} maxLength={150} onChange={(e) => set("location", e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Web sitesi</span>
          <input className="input" value={values.websiteUrl} maxLength={200} onChange={(e) => set("websiteUrl", e.target.value)} placeholder="ornek.com" />
        </label>
        <label className="field">
          <span className="field__label">E-posta</span>
          <input className="input" type="email" value={values.email} maxLength={100} onChange={(e) => set("email", e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">Telefon</span>
          <input className="input" value={values.phoneNumber} maxLength={20} onChange={(e) => set("phoneNumber", e.target.value)} placeholder="+905551112233" />
        </label>
        <label className="field form-grid__wide">
          <span className="field__label">Hakkında</span>
          <textarea className="input" value={values.about} maxLength={500} onChange={(e) => set("about", e.target.value)} />
          <span className="field__hint">{values.about.length} / 500</span>
        </label>
        <label className="field form-grid__wide">
          <span className="field__label">Logo <span className="muted">(isteğe bağlı)</span></span>
          <input className="input input--file" type="file" accept=".jpg,.jpeg,.png,.webp" onChange={(e) => setLogo(e.target.files?.[0] ?? null)} />
          <span className="field__hint">{company?.logoUrl ? "Yeni dosya seçmezsen mevcut logo korunur. " : ""}jpg, png ya da webp; en fazla 2 MB.</span>
        </label>
      </div>
      {error && <p className="alert alert--error">{error}</p>}
      <div className="form-card__actions">
        <button type="submit" className="button button--primary" disabled={saving}>{saving ? "Kaydediliyor…" : company ? "Değişiklikleri kaydet" : "Şirketi ekle"}</button>
      </div>
    </form>
  );
}

export function AdminCompanies() {
  const [companies, setCompanies] = useState<Company[] | null>(null);
  const [editing, setEditing] = useState<Company | null | "new">(null);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    companiesApi.list().then(setCompanies).catch((e) => setError(errorText(e)));
  }, []);

  useEffect(load, [load]);

  return (
    <section>
      <div className="admin-toolbar">
        <p className="muted">{companies ? `${companies.length} şirket` : "Yükleniyor"}</p>
        <button type="button" className="button button--primary" onClick={() => setEditing("new")}>Yeni şirket</button>
      </div>

      {error && <p className="alert alert--error">{error}</p>}

      <div className="panel table-wrap">
        <table className="table">
          <thead>
            <tr>
              <th>Şirket</th>
              <th>Sektör</th>
              <th>Konum</th>
              <th><span className="visually-hidden">İşlemler</span></th>
            </tr>
          </thead>
          <tbody>
            {companies?.map((company) => (
              <tr key={company.id}>
                <td>
                  <span className="table__company">
                    <CompanyMark name={company.name} logoUrl={company.logoUrl} size={36} />
                    <Link to={`/sirketler/${company.id}`} className="table__primary">{company.name}</Link>
                  </span>
                </td>
                <td>{company.industry}</td>
                <td>{company.location ?? <span className="muted">Belirtilmemiş</span>}</td>
                <td>
                  <div className="table__actions">
                    <button type="button" className="button button--small" onClick={() => setEditing(company)}>Düzenle</button>
                    <ConfirmButton
                      className="button button--small button--danger-text"
                      label="Sil"
                      confirmLabel="İlanlarıyla silinsin mi?"
                      onConfirm={async () => {
                        try {
                          await companiesApi.remove(company.id);
                          load();
                        } catch (e) {
                          setError(errorText(e));
                        }
                      }}
                    />
                  </div>
                </td>
              </tr>
            ))}
            {companies?.length === 0 && (
              <tr>
                <td colSpan={4} className="table__empty muted">Henüz şirket yok.</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <Modal title={editing === "new" ? "Yeni şirket" : "Şirketi düzenle"} open={editing !== null} onClose={() => setEditing(null)}>
        <CompanyForm
          company={editing === "new" ? null : editing}
          onSaved={() => {
            setEditing(null);
            load();
          }}
        />
      </Modal>
    </section>
  );
}
