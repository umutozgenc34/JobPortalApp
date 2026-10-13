import { useState, type FormEvent } from "react";

export function SearchBar({
  initialSearch = "",
  initialLocation = "",
  onSearch,
  size = "regular"
}: {
  initialSearch?: string;
  initialLocation?: string;
  onSearch: (search: string, location: string) => void;
  size?: "regular" | "large";
}) {
  const [search, setSearch] = useState(initialSearch);
  const [location, setLocation] = useState(initialLocation);

  function handleSubmit(event: FormEvent) {
    event.preventDefault();
    onSearch(search.trim(), location.trim());
  }

  return (
    <form className={`search-bar search-bar--${size}`} role="search" onSubmit={handleSubmit}>
      <label className="search-bar__field">
        <span className="search-bar__label">Pozisyon ya da şirket</span>
        <input
          className="search-bar__input"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          placeholder="Örn. Backend Geliştirici"
        />
      </label>
      <label className="search-bar__field">
        <span className="search-bar__label">Konum</span>
        <input
          className="search-bar__input"
          value={location}
          onChange={(e) => setLocation(e.target.value)}
          placeholder="Örn. İstanbul ya da Uzaktan"
        />
      </label>
      <button type="submit" className="button button--primary search-bar__submit">İlan ara</button>
    </form>
  );
}
