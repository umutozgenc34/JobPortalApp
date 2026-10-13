const translations: Record<string, string> = {
  "Email or Password wrong.": "E-posta ya da şifre hatalı.",
  "Too many failed login attempts. Please try again later.": "Çok fazla hatalı deneme yapıldı. Birkaç dakika sonra tekrar dene.",
  "Email already in use.": "Bu e-posta adresiyle zaten bir hesap var.",
  "Username already taken.": "Bu kullanıcı adı alınmış.",
  "You have already applied to this job.": "Bu ilana zaten başvurdun.",
  "This job posting is no longer accepting applications.": "Bu ilan artık başvuru kabul etmiyor.",
  "Job posting not found.": "İlan bulunamadı.",
  "Email is required.": "E-posta adresi gerekli.",
  "Invalid email address.": "Geçerli bir e-posta adresi gir.",
  "Enter a valid email address.": "Geçerli bir e-posta adresi gir.",
  "Password cannot be empty.": "Şifre gerekli.",
  "Password must be between 6 and 100 characters.": "Şifre 6 ile 100 karakter arasında olmalı.",
  "UserName is required.": "Kullanıcı adı gerekli.",
  "UserName must be between 3 and 50 characters.": "Kullanıcı adı 3 ile 50 karakter arasında olmalı.",
  "Cover letter cannot exceed 1000 characters.": "Ön yazı en fazla 1000 karakter olabilir.",
  "Passwords must have at least one digit ('0'-'9').": "Şifre en az bir rakam içermeli.",
  "Passwords must have at least one uppercase ('A'-'Z').": "Şifre en az bir büyük harf içermeli.",
  "Passwords must have at least one lowercase ('a'-'z').": "Şifre en az bir küçük harf içermeli.",
  "Passwords must be at least 6 characters.": "Şifre en az 6 karakter olmalı."
};

export function translateMessage(message: string) {
  if (translations[message]) return translations[message];
  if (/^Username '.*' is already taken\.$/.test(message)) return "Bu kullanıcı adı alınmış.";
  if (/^Email '.*' is already taken\.$/.test(message)) return "Bu e-posta adresiyle zaten bir hesap var.";
  if (/^Username '.*' is invalid/.test(message)) return "Kullanıcı adı yalnızca harf ve rakam içerebilir.";
  return message;
}
