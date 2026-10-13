import { BrowserRouter, Route, Routes } from "react-router-dom";
import { AuthProvider } from "./auth/AuthContext";
import { ThemeProvider } from "./theme/ThemeContext";
import { Layout } from "./components/Layout";
import { Home } from "./pages/Home";
import { Jobs } from "./pages/Jobs";
import { JobDetail } from "./pages/JobDetail";
import { AuthPage } from "./pages/AuthPage";
import { NotFound } from "./pages/NotFound";

export default function App() {
  return (
    <ThemeProvider>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route element={<Layout />}>
              <Route index element={<Home />} />
              <Route path="ilanlar" element={<Jobs />} />
              <Route path="ilanlar/:id" element={<JobDetail />} />
              <Route path="giris" element={<AuthPage mode="login" />} />
              <Route path="kayit" element={<AuthPage mode="register" />} />
              <Route path="*" element={<NotFound />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </ThemeProvider>
  );
}
