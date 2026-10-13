import { useCallback, useEffect, useState, type FormEvent } from "react";
import { ApiError } from "../../api/client";
import { categoriesApi } from "../../api/endpoints";
import type { Category } from "../../api/types";
import { ConfirmButton } from "../../components/ConfirmButton";

function errorText(e: unknown) {
  return e instanceof ApiError ? e.messages.join(" ") : "İşlem tamamlanamadı.";
}

function CategoryRow({ category, onChanged, onError }: { category: Category; onChanged: () => void; onError: (message: string) => void }) {
  const [editing, setEditing] = useState(false);
  const [name, setName] = useState(category.name);

  async function save(event: FormEvent) {
    event.preventDefault();
    try {
      await categoriesApi.update(category.id, name.trim());
      setEditing(false);
      onChanged();
    } catch (e) {
      onError(errorText(e));
    }
  }

  return (
    <li className="list__item category-row">
      {editing ? (
        <form className="category-row__form" onSubmit={save}>
          <input className="input" value={name} onChange={(e) => setName(e.target.value)} autoFocus aria-label="Alan adı" />
          <button type="submit" className="button button--primary button--small">Kaydet</button>
          <button type="button" className="button button--ghost button--small" onClick={() => { setEditing(false); setName(category.name); }}>Vazgeç</button>
        </form>
      ) : (
        <>
          <span className="category-row__name">{category.name}</span>
          <div className="table__actions">
            <button type="button" className="button button--small" onClick={() => setEditing(true)}>Yeniden adlandır</button>
            <ConfirmButton
              className="button button--small button--danger-text"
              label="Sil"
              confirmLabel="Emin misin?"
              onConfirm={async () => {
                try {
                  await categoriesApi.remove(category.id);
                  onChanged();
                } catch (e) {
                  onError(errorText(e));
                }
              }}
            />
          </div>
        </>
      )}
    </li>
  );
}

export function AdminCategories() {
  const [categories, setCategories] = useState<Category[] | null>(null);
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(() => {
    categoriesApi.list().then(setCategories).catch((e) => setError(errorText(e)));
  }, []);

  useEffect(load, [load]);

  async function handleAdd(event: FormEvent) {
    event.preventDefault();
    if (!name.trim()) return;
    setError(null);
    try {
      await categoriesApi.create(name.trim());
      setName("");
      load();
    } catch (e) {
      setError(errorText(e));
    }
  }

  return (
    <section>
      <form className="admin-toolbar admin-toolbar--form" onSubmit={handleAdd}>
        <label className="field">
          <span className="visually-hidden">Yeni alan adı</span>
          <input className="input" value={name} onChange={(e) => setName(e.target.value)} placeholder="Yeni alan adı, örn. Finans" maxLength={256} />
        </label>
        <button type="submit" className="button button--primary">Alan ekle</button>
      </form>

      {error && <p className="alert alert--error">{error}</p>}

      <ul className="panel list">
        {categories?.map((category) => (
          <CategoryRow key={category.id} category={category} onChanged={load} onError={setError} />
        ))}
        {categories?.length === 0 && <li className="list__item muted">Henüz alan yok.</li>}
      </ul>
    </section>
  );
}
