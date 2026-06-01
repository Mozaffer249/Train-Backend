// Sudanese flag stripe rendered above the layout on every admin page.

export default function BrandStripe() {
  return (
    <div className="relative h-2 w-full overflow-hidden" aria-hidden="true">
      <div className="absolute inset-0 flex flex-col">
        <div className="flex-1 bg-sudan-red-600" />
        <div className="flex-1 bg-white" />
        <div className="flex-1 bg-black" />
      </div>
      <div className="absolute inset-y-0 start-0 w-8">
        <svg viewBox="0 0 32 8" preserveAspectRatio="none" className="h-full w-full">
          <polygon points="0,0 24,4 0,8" fill="#007a3d" />
        </svg>
      </div>
    </div>
  );
}
