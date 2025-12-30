import { AlertCircle } from 'lucide-react';

interface StatusBadgeProps {
  isActive: boolean;
  maintenanceNote?: string;
}

const StatusBadge = ({ isActive, maintenanceNote }: StatusBadgeProps) => {
  return (
    <div className="flex items-center gap-2">
      <span
        className={`px-2 py-1 rounded text-xs font-medium ${
          isActive
            ? 'bg-green-100 text-green-800'
            : 'bg-red-100 text-red-800'
        }`}
      >
        {isActive ? 'Active' : 'Inactive'}
      </span>
      {maintenanceNote && (
        <span
          title={maintenanceNote}
          className="text-gray-400 hover:text-gray-600 cursor-help"
        >
          <AlertCircle size={16} />
        </span>
      )}
    </div>
  );
};

export default StatusBadge;
