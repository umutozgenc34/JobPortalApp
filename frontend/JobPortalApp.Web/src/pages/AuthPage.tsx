import { useState, type FormEvent } from "react";
import { Link, Navigate, useLocation, useNavigate } from "react-router-dom";
import { ApiError } from "../api/client";
import { useAuth } from "../auth/AuthContext";

export function AuthPage({ mode }: { mode: "login" | "register" }) {
  const { user, login, register } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = (location.state as { from?: string } | null)?.from ?? "/ilanlar";

  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [errors, setErrors] = useState<string[]>([]);
  const [submitting, setSubmitting] = useState(false);

  if (user) return <Navigate to={from} replace />;

  const isLogin = mode === "login";

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setSubmitting(true);
    setErrors([]);
    try {
      if (isLogin) await login(email.trim(), password);
      else await register(userName.trim(), email.trim(), password);
      navigate(from, { replace: true });
    } catch (e) {
      setErrors(e instanceof ApiError ? e.messages : ["Sunucuya ulaşılamadı. Biraz sonra tekrar dene."]);
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <div className="container page auth">
      <div className="auth__card panel">
        <h1 className="auth__title">{isLogin ? "Hesabına giriş yap" : "Hesap oluştur"}</h1>
        <p className="muted auth__lead">
          {isLogin ? "Başvurularını ve bildirimlerini takip etmek için giriş yap." : "Ücretsiz hesap oluştur, ilanlara tek adımda başvur."}
        </p>

        <form className="auth__form" onSubmit={handleSubmit} noValidate>
          {!isLogin && (
            <label className="field">
              <span className="field__label">Kullanıcı adı</span>
              <input className="input" autoComplete="username" value={userName} onChange={(e) => setUserName(e.target.value)} />
              <span className="field__hint">3–50 karakter; harf, rakam ve . _ - kullanılabilir.</span>
            </label>
          )}
          <label className="field">
            <span className="field__label">E-posta</span>
            <input className="input" type="email" autoComplete="email" value={email} onChange={(e) => setEmail(e.target.value)} />
          </label>
          <label className="field">
            <span className="field__label">Şifre</span>
            <input
              className="input"
              type="password"
              autoComplete={isLogin ? "current-password" : "new-password"}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            {!isLogin && <span className="field__hint">En az 6 karakter; büyük harf, küçük harf ve rakam içermeli.</span>}
          </label>

          {errors.length > 0 && (
            <div className="alert alert--error" role="alert">
              {errors.map((message) => (
                <p key={message}>{message}</p>
              ))}
            </div>
          )}

          <button type="submit" className="button button--primary button--block" disabled={submitting}>
            {submitting ? "Lütfen bekle…" : isLogin ? "Giriş yap" : "Hesap oluştur"}
          </button>
        </form>

        <p className="auth__switch muted">
          {isLogin ? "Hesabın yok mu? " : "Zaten hesabın var mı? "}
          <Link to={isLogin ? "/kayit" : "/giris"} state={location.state} className="text-link">
            {isLogin ? "Kayıt ol" : "Giriş yap"}
          </Link>
        </p>
      </div>
    </div>
  );
}
