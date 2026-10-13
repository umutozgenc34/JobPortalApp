export function Stars({ value, size = 16 }: { value: number; size?: number }) {
  const rounded = Math.round(value * 2) / 2;
  return (
    <span className="stars" style={{ fontSize: size }} aria-label={`5 üzerinden ${value.toFixed(1)} puan`}>
      {[1, 2, 3, 4, 5].map((i) => (
        <span key={i} className={i <= rounded ? "stars__on" : i - 0.5 === rounded ? "stars__half" : "stars__off"} aria-hidden="true">
          ★
        </span>
      ))}
    </span>
  );
}

export function RatingInput({ value, onChange }: { value: number; onChange: (value: number) => void }) {
  return (
    <div className="rating-input" role="radiogroup" aria-label="Puan">
      {[1, 2, 3, 4, 5].map((i) => (
        <button
          key={i}
          type="button"
          role="radio"
          aria-checked={value === i}
          aria-label={`${i} puan`}
          className={i <= value ? "rating-input__star is-on" : "rating-input__star"}
          onClick={() => onChange(i)}
        >
          ★
        </button>
      ))}
    </div>
  );
}
