import { useEffect, useState, type FormEvent } from "react";
import { ApiError } from "../api/client";
import { authApi, profileApi } from "../api/endpoints";
import type { UserProfile } from "../api/types";
import { useAuth } from "../auth/AuthContext";
import { ConfirmButton } from "../components/ConfirmButton";

function errorText(e: unknown) {
  return e instanceof ApiError ? e.messages.join(" ") : "İşlem tamamlanamadı. Biraz sonra tekrar dene.";
}

function AccountForm() {
  const { user, refreshUser } = useAuth();
  const [userName, setUserName] = useState(user?.userName ?? "");
  const [email, setEmail] = useState(user?.email ?? "");
  const [status, setStatus] = useState<{ type: "success" | "error"; text: string } | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setStatus(null);
    try {
      await authApi.updateMe(userName.trim(), email.trim());
      await refreshUser();
      setStatus({ type: "success", text: "Hesap bilgilerin güncellendi." });
    } catch (e) {
      setStatus({ type: "error", text: errorText(e) });
    } finally {
      setSaving(false);
    }
  }

  return (
    <form className="panel form-card" onSubmit={handleSubmit}>
      <div className="form-card__head">
        <h2>Hesap bilgileri</h2>
        <p className="muted">Giriş yaparken kullandığın bilgiler.</p>
      </div>
      <div className="form-grid">
        <label className="field">
          <span className="field__label">Kullanıcı adı</span>
          <input className="input" value={userName} onChange={(e) => setUserName(e.target.value)} />
        </label>
        <label className="field">
          <span className="field__label">E-posta</span>
          <input className="input" type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
        </label>
      </div>
      {status && <p className={`alert alert--${status.type}`}>{status.text}</p>}
      <div className="form-card__actions">
        <button type="submit" className="button button--primary" disabled={saving}>{saving ? "Kaydediliyor…" : "Hesabı güncelle"}</button>
      </div>
    </form>
  );
}

type Status = { type: "success" | "error"; text: string } | null;

function ProfileForm({
  profile,
  status,
  setStatus,
  onSaved,
  onDeleted
}: {
  profile: UserProfile | null;
  status: Status;
  setStatus: (status: Status) => void;
  onSaved: () => void;
  onDeleted: () => void;
}) {
  const [fullName, setFullName] = useState(profile?.fullName ?? "");
  const [summary, setSummary] = useState(profile?.summary ?? "");
  const [email, setEmail] = useState(profile?.email ?? "");
  const [image, setImage] = useState<File | null>(null);
  const [cv, setCv] = useState<File | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setSaving(true);
    setStatus(null);
    const form = new FormData();
    form.append("FullName", fullName.trim());
    form.append("Summary", summary.trim());
    if (email.trim()) form.append("Email", email.trim());
    if (image) form.append("ProfileImage", image);
    if (cv) form.append("CvFile", cv);
    try {
      if (profile) await profileApi.update(form);
      else await profileApi.create(form);
      setStatus({ type: "success", text: profile ? "Profilin güncellendi." : "Profilin oluşturuldu." });
      setImage(null);
      setCv(null);
      onSaved();
    } catch (e) {
      setStatus({ type: "error", text: errorText(e) });
    } finally {
      setSaving(false);
    }
  }

  return (
    <form className="panel form-card" onSubmit={handleSubmit}>
      <div className="form-card__head">
        <h2>{profile ? "Özgeçmiş bilgileri" : "Özgeçmiş profilini oluştur"}</h2>
        <p className="muted">Başvurduğun şirketler bu bilgileri görür.</p>
      </div>
      <div className="form-grid">
        <label className="field">
          <span className="field__label">Ad soyad</span>
          <input className="input" value={fullName} onChange={(e) => setFullName(e.target.value)} maxLength={100} />
        </label>
        <label className="field">
          <span className="field__label">İletişim e-postası</span>
          <input className="input" type="email" value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Boş bırakırsan hesap e-postan kullanılır" />
        </label>
        <label className="field form-grid__wide">
          <span className="field__label">Kısa özet</span>
          <textarea className="input" value={summary} onChange={(e) => setSummary(e.target.value)} maxLength={500} placeholder="Deneyimini, uzmanlık alanlarını ve aradığın pozisyonu birkaç cümleyle anlat." />
          <span className="field__hint">{summary.length} / 500</span>
        </label>
        <label className="field">
          <span className="field__label">Profil fotoğrafı</span>
          <input className="input input--file" type="file" accept=".jpg,.jpeg,.png,.webp" onChange={(e) => setImage(e.target.files?.[0] ?? null)} />
          <span className="field__hint">jpg, png ya da webp; en fazla 2 MB.</span>
        </label>
        <label className="field">
          <span className="field__label">CV</span>
          <input className="input input--file" type="file" accept=".pdf" onChange={(e) => setCv(e.target.files?.[0] ?? null)} />
          <span className="field__hint">Yalnızca PDF; en fazla 5 MB.</span>
        </label>
      </div>
      {status && <p className={`alert alert--${status.type}`}>{status.text}</p>}
      <div className="form-card__actions">
        <button type="submit" className="button button--primary" disabled={saving}>
          {saving ? "Kaydediliyor…" : profile ? "Profili güncelle" : "Profili oluştur"}
        </button>
        {profile && (
          <ConfirmButton
            label="Profili sil"
            confirmLabel="Silmek için tekrar tıkla"
            onConfirm={async () => {
              await profileApi.remove();
              setStatus({ type: "success", text: "Profilin silindi." });
              onDeleted();
            }}
          />
        )}
      </div>
    </form>
  );
}

export function Profile() {
  const [profile, setProfile] = useState<UserProfile | null | undefined>(undefined);
  const [status, setStatus] = useState<Status>(null);

  function load() {
    profileApi
      .mine()
      .then(setProfile)
      .catch(() => setProfile(null));
  }

  useEffect(load, []);

  return (
    <div className="container page">
      <header className="page-header">
        <h1>Profilim</h1>
        <p className="muted">Hesap bilgilerini ve şirketlerin göreceği özgeçmiş profilini yönet.</p>
      </header>

      <div className="profile-layout">
        <aside className="profile-card panel">
          {profile === undefined ? (
            <div className="skeleton" style={{ height: 200 }} />
          ) : profile ? (
            <>
              {profile.profileImageUrl ? (
                <img className="profile-card__photo" src={profile.profileImageUrl} alt="" />
              ) : (
                <span className="profile-card__photo profile-card__photo--empty" aria-hidden="true">
                  {profile.fullName.slice(0, 1).toLocaleUpperCase("tr-TR")}
                </span>
              )}
              <h2>{profile.fullName}</h2>
              <p className="muted">{profile.email}</p>
              {profile.summary && <p className="profile-card__summary">{profile.summary}</p>}
              {profile.cvFilePath ? (
                <a className="button button--block" href={profile.cvFilePath} target="_blank" rel="noreferrer">CV'yi görüntüle</a>
              ) : (
                <p className="muted profile-card__hint">Henüz CV yüklemedin.</p>
              )}
            </>
          ) : (
            <>
              <h2>Profilin yok</h2>
              <p className="muted">Şirketlerin seni tanıyabilmesi için özgeçmiş profilini oluştur.</p>
            </>
          )}
        </aside>

        <div className="profile-forms">
          <AccountForm />
          {profile !== undefined && (
            <ProfileForm
              key={profile ? `edit-${profile.id}` : "new"}
              profile={profile}
              status={status}
              setStatus={setStatus}
              onSaved={load}
              onDeleted={() => setProfile(null)}
            />
          )}
        </div>
      </div>
    </div>
  );
}
