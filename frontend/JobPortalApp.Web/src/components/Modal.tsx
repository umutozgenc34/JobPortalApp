import { useEffect, useRef, type ReactNode } from "react";

export function Modal({ title, open, onClose, children }: { title: string; open: boolean; onClose: () => void; children: ReactNode }) {
  const ref = useRef<HTMLDialogElement>(null);

  useEffect(() => {
    const dialog = ref.current;
    if (!dialog) return;
    if (open && !dialog.open) dialog.showModal();
    if (!open && dialog.open) dialog.close();
  }, [open]);

  return (
    <dialog
      ref={ref}
      className="modal"
      onClose={onClose}
      onClick={(event) => {
        if (event.target === ref.current) onClose();
      }}
    >
      <div className="modal__inner">
        <header className="modal__head">
          <h2>{title}</h2>
          <button type="button" className="modal__close" aria-label="Kapat" onClick={onClose}>×</button>
        </header>
        {open && children}
      </div>
    </dialog>
  );
}
