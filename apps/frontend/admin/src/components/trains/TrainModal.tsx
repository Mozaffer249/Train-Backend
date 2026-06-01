import { useState, useEffect } from 'react';
import { X } from 'lucide-react';
import { Train, TrainFormData } from '../../types/infrastructure';
import { trainsApi } from '../../services/api';
import { showSuccess, showError, extractErrorMessage } from '../../utils/alerts';

interface TrainModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSuccess: () => void;
  train?: Train | null;
}

const TrainModal = ({ isOpen, onClose, onSuccess, train }: TrainModalProps) => {
  const [formData, setFormData] = useState<TrainFormData>({
    trainNumber: '',
    nameEn: '',
    nameAr: '',
  });
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!isOpen) return;
    if (train) {
      setFormData({
        trainNumber: train.trainNumber,
        nameEn: train.nameEn,
        nameAr: train.nameAr,
      });
    } else {
      setFormData({ trainNumber: '', nameEn: '', nameAr: '' });
    }
    setError('');
  }, [isOpen, train]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setIsSubmitting(true);
    try {
      if (train) {
        await trainsApi.update(train.id, formData);
        showSuccess('تم تحديث القطار');
      } else {
        await trainsApi.create(formData);
        showSuccess('تم إنشاء القطار');
      }
      onSuccess();
      onClose();
    } catch (err) {
      const msg = extractErrorMessage(err);
      setError(msg);
      showError('تعذّر حفظ القطار', msg);
    } finally {
      setIsSubmitting(false);
    }
  };

  if (!isOpen) return null;

  const inputClass = 'w-full px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500';

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={onClose} />
        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <div className="bg-white px-6 pt-5 pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-medium text-gray-900">{train ? 'تعديل قطار' : 'إضافة قطار'}</h3>
              <button onClick={onClose} className="text-gray-400 hover:text-gray-500">
                <X size={24} />
              </button>
            </div>

            {error && <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">{error}</div>}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">رقم القطار *</label>
                <input
                  type="text"
                  value={formData.trainNumber}
                  onChange={(e) => setFormData({ ...formData, trainNumber: e.target.value })}
                  required
                  className={inputClass}
                />
              </div>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الاسم (بالإنجليزية) *</label>
                  <input type="text" value={formData.nameEn} onChange={(e) => setFormData({ ...formData, nameEn: e.target.value })} required className={inputClass} />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">الاسم (بالعربية) *</label>
                  <input type="text" dir="rtl" value={formData.nameAr} onChange={(e) => setFormData({ ...formData, nameAr: e.target.value })} required className={inputClass} />
                </div>
              </div>
              <p className="text-xs text-gray-500">
                درجات العربات تُحدَّد لكل عربة على حدة من شاشة "العربات".
              </p>

              <div className="flex justify-end gap-3 pt-4 border-t">
                <button type="button" onClick={onClose} disabled={isSubmitting} className="px-4 py-2 text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50">إلغاء</button>
                <button type="submit" disabled={isSubmitting} className="admin-button">
                  {isSubmitting ? 'جاري الحفظ…' : train ? 'حفظ التغييرات' : 'إنشاء قطار'}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TrainModal;
