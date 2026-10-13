import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import { RequireAuth } from "./auth/RequireAuth";
import { NotificationsProvider } from "./notifications/NotificationsContext";
import { ThemeProvider } from "./theme/ThemeContext";
import { Layout } from "./components/Layout";
import { Home } from "./pages/Home";
import { Jobs } from "./pages/Jobs";
import { JobDetail } from "./pages/JobDetail";
import { AuthPage } from "./pages/AuthPage";
import { Companies } from "./pages/Companies";
import { CompanyDetail } from "./pages/CompanyDetail";
import { Profile } from "./pages/Profile";
import { Applications } from "./pages/Applications";
import { Notifications } from "./pages/Notifications";
import { Admin } from "./pages/admin/Admin";
import { NotFound } from "./pages/NotFound";

export default function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <NotificationsProvider>
          <BrowserRouter>
            <Routes>
              <Route element={<Layout />}>
                <Route index element={<Home />} />
                <Route path="ilanlar" element={<Jobs />} />
                <Route path="ilanlar/:id" element={<JobDetail />} />
                <Route path="sirketler" element={<Companies />} />
                <Route path="sirketler/:id" element={<CompanyDetail />} />
                <Route path="giris" element={<AuthPage mode="login" />} />
                <Route path="kayit" element={<AuthPage mode="register" />} />
                <Route path="profil" element={<RequireAuth><Profile /></RequireAuth>} />
                <Route path="basvurularim" element={<RequireAuth><Applications /></RequireAuth>} />
                <Route path="bildirimler" element={<RequireAuth><Notifications /></RequireAuth>} />
                <Route path="yonetim/*" element={<RequireAuth admin><Admin /></RequireAuth>} />
                <Route path="*" element={<NotFound />} />
              </Route>
            </Routes>
          </BrowserRouter>
        </NotificationsProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}
