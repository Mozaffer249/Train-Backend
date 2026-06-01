import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Train, BulkCoachesFormData, CoachClass } from '../../types/infrastructure';
import { trainsApi } from '../../services/api';
import { showSuccess, showError, extractErrorMessage } from '../../utils/alerts';

interface BulkCoachesModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  train: Train | null;
}

const BulkCoachesModal = ({ isOpen, onClose, onSuccess, train }: BulkCoachesModalProps) => {
  const [formData, setFormData] = useState<BulkCoachesFormData>({
    numberOfCoaches: 1,
    class: CoachClass.Second,
    capacityPerCoach: 40,
    autoGenerateSeats: true,
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (isOpen) {
      setFormData({ numberOfCoaches: 1, class: CoachClass.Second, capacityPerCoach: 40, autoGenerateSeats: true });
      setError('');
    }
  }, [isOpen]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!train) return;
    setError('');
    setIsSubmitting(true);
    try {
      await trainsApi.bulkCreateCoaches(train.id, formData);
      showSuccess('تم إنشاء العربات');
      onSuccess();
      onClose();
    } catch (err) {
      const msg = extractErrorMessage(err);
      setError(msg);
      showError('تعذّر إنشاء العربات', msg);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen || !train) return null;

  const inputClass = 'w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500';

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />
        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">Add Coaches — {train.nameEn}</h3>
              <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>

            {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">عدد العربات *</label>
                  <input type="number" min={1} value={formData.numberOfCoaches} onChange={(e) => setFormData({ ...formData, numberOfCoaches: Number(e.target.value) })} required className={inputClass} />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">السعة لكل عربة *</label>
                  <input type="number" min={1} value={formData.capacityPerCoach} onChange={(e) => setFormData({ ...formData, capacityPerCoach: Number(e.target.value) })} required className={inputClass} />
                </div>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">فئة العربة *</label>
                <select value={formData.class} onChange={(e) => setFormData({ ...formData, class: Number(e.target.value) })} className={inputClass}>
                  <option value={CoachClass.First}>الدرجة الأولى</option>
                  <option value={CoachClass.Second}>الدرجة الثانية</option>
                  <option value={CoachClass.Third}>الدرجة الثالثة</option>
                </select>
              </div>
              <label className="flex items-center gap-2">
                <input type="checkbox" checked={formData.autoGenerateSeats} onChange={(e) => setFormData({ ...formData, autoGenerateSeats: e.target.checked })} />
                <span className="text-sm text-gray-700">إنشاء المقاعد تلقائياً</span>
              </label>

              <div className="flex justify-end gap-3 pt-4 border-t">
                <button type="button" onClick={onClose} disabled={isSubmitting} className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">إلغاء</button>
                <button type="submit" disabled={isSubmitting} className="admin-button">
                  {isSubmitting ? 'جاري الإنشاء…' : 'إنشاء العربات'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BulkCoachesModal;
