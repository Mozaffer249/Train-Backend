import { useEffect, useState } from 'react';
import { usersApi } from '../../services/api';
import { AdminUser, UserFormData, ROLES } from '../../types/infrastructure';
import { AR } from '../../i18n/ar';
import { showError, showSuccess, extractErrorMessage } from '../../utils/alerts';

interface Props {
  isOpen: boolean;
  editUser: AdminUser | null;
  onClose: () => void;
  onSuccess: () => void;
}

const UserModal = ({ isOpen, editUser, onClose, onSuccess }: Props) => {
  const isEdit = !!editUser;
  const [form, setForm] = useState<UserFormData>({
    userName: '',
    email: '',
    firstName: '',
    lastName: '',
    password: '',
    phoneNumber: '',
    roles: [],
  });
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (editUser) {
      setForm({
        userName: editUser.userName,
        email: editUser.email || '',
        firstName: editUser.firstName || '',
        lastName: editUser.lastName || '',
        phoneNumber: editUser.phoneNumber || '',
        password: '',
        roles: editUser.roles,
      });
    } else {
      setForm({
        userName: '', email: '', firstName: '', lastName: '',
        password: '', phoneNumber: '', roles: [ROLES.Customer],
      });
    }
  }, [editUser, isOpen]);

  if (!isOpen) return null;

  const submit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      if (isEdit) {
        const { userName: _u, password: _p, roles: _r, ...rest } = form;
        await usersApi.update(editUser!.id, rest);
      } else {
        await usersApi.create(form);
      }
      await showSuccess(isEdit ? AR.common.updated : AR.common.created);
      onSuccess();
    } catch (err) {
      showError(AR.common.errorTitle, extractErrorMessage(err));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center">
      <form onSubmit={submit} className="bg-white rounded-lg w-full max-w-lg p-6 space-y-3">
        <h2 className="text-lg font-bold">{isEdit ? AR.users.editUser : AR.users.addUser}</h2>

        <div className="grid grid-cols-2 gap-3">
          <Field label={AR.users.firstName} value={form.firstName || ''} onChange={(v) => setForm({ ...form, firstName: v })} />
          <Field label={AR.users.lastName} value={form.lastName || ''} onChange={(v) => setForm({ ...form, lastName: v })} />
        </div>

        <Field label={AR.users.userName} value={form.userName} onChange={(v) => setForm({ ...form, userName: v })} required disabled={isEdit} />
        <Field label={AR.users.email} type="email" value={form.email} onChange={(v) => setForm({ ...form, email: v })} required />
        <Field label={AR.users.phone} value={form.phoneNumber || ''} onChange={(v) => setForm({ ...form, phoneNumber: v })} />

        {!isEdit && (
          <Field label={AR.users.password} type="password" value={form.password || ''} onChange={(v) => setForm({ ...form, password: v })} required />
        )}

        {!isEdit && (
          <div>
            <label className="text-sm text-gray-700">{AR.users.roles}</label>
            <div className="flex flex-wrap gap-2 mt-1">
              {Object.values(ROLES).map((r) => (
                <label key={r} className="flex items-center gap-1 border px-2 py-1 rounded text-sm">
                  <input
                    type="checkbox"
                    checked={form.roles?.includes(r) ?? false}
                    onChange={(e) => {
                      const next = new Set(form.roles || []);
                      if (e.target.checked) next.add(r); else next.delete(r);
                      setForm({ ...form, roles: Array.from(next) });
                    }}
                  />
                  {r}
                </label>
              ))}
            </div>
          </div>
        )}

        <div className="flex justify-end gap-2 pt-2">
          <button type="button" onClick={onClose} className="admin-button-secondary">{AR.common.cancel}</button>
          <button type="submit" disabled={submitting} className="admin-button">
            {submitting ? AR.common.processing : (isEdit ? AR.common.saveChanges : AR.common.create)}
          </button>
        </div>
      </form>
    </div>
  );
};

const Field = ({ label, value, onChange, type = 'text', required, disabled }: {
  label: string; value: string; onChange: (v: string) => void;
  type?: string; required?: boolean; disabled?: boolean;
}) => (
  <label className="block">
    <span className="text-sm text-gray-700">{label}</span>
    <input
      type={type}
      value={value}
      onChange={(e) => onChange(e.target.value)}
      required={required}
      disabled={disabled}
      className="mt-1 w-full border rounded-md px-3 py-1.5 text-sm disabled:bg-gray-100"
    />
  </label>
);

export default UserModal;
