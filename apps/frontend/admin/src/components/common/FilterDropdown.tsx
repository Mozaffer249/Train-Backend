interface FilterDropdownProps {
  label: string;
  value: string | number;
  onChange: (value: string | number) => void;
  options: { value: string | number; label: string }[];
  className?: string;
}

const FilterDropdown = ({
  label,
  value,
  onChange,
  options,
  className = '',
}: FilterDropdownProps) => {
  return (
    <div className={`flex flex-col gap-1 ${className}`}>
      <label className="text-sm font-medium text-gray-700">{label}</label>
      <select
        value={value}
        onChange={(e) => {
          const val = e.target.value;
          // Try to convert to number if it's a numeric string
          const numVal = Number(val);
          onChange(isNaN(numVal) ? val : numVal);
        }}
        className="px-3 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-admin-primary-500 bg-white"
      >
        {options.map((option) => (
          <option key={option.value} value={option.value}>
            {option.label}
          </option>
        ))}
      </select>
    </div>
  );
};

export default FilterDropdown;
