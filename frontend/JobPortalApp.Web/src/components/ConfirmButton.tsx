import { useEffect, useState } from "react";

export function ConfirmButton({
  label,
  confirmLabel,
  onConfirm,
  className = "button button--danger"
}: {
  label: string;
  confirmLabel: string;
  onConfirm: () => void | Promise<void>;
  className?: string;
}) {
  const [armed, setArmed] = useState(false);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (!armed) return;
    const timer = window.setTimeout(() => setArmed(false), 4000);
    return () => window.clearTimeout(timer);
  }, [armed]);

  async function handleClick() {
    if (!armed) {
      setArmed(true);
      return;
    }
    setBusy(true);
    try {
      await onConfirm();
    } finally {
      setBusy(false);
      setArmed(false);
    }
  }

  return (
    <button type="button" className={`${className}${armed ? " is-armed" : ""}`} onClick={handleClick} disabled={busy}>
      {armed ? confirmLabel : label}
    </button>
  );
}
