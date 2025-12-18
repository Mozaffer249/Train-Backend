import React, { useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { User, CreditCard, Check, ArrowLeft, Train, MapPin } from 'lucide-react';
import { useLanguage } from '../contexts/LanguageContext';

const BookingPage: React.FC = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { t } = useLanguage();
  const [currentStep, setCurrentStep] = useState(1);
  const [selectedSeat, setSelectedSeat] = useState<number | null>(null);
  const [passengerInfo, setPassengerInfo] = useState({
    fullName: '',
    fullNameArabic: '',
    idNumber: '',
    nationality: 'Saudi Arabia',
    email: '',
    phone: '',
    gender: 'male',
    birthDate: ''
  });

  const { train, searchParams } = location.state || {};

  const steps = [
    { id: 1, name: t('passenger.info'), icon: User },
    { id: 2, name: t('seat.selection'), icon: Train },
    { id: 3, name: t('payment'), icon: CreditCard },
    { id: 4, name: t('confirm'), icon: Check }
  ];

  const handleNext = () => {
    if (currentStep < 4) {
      setCurrentStep(currentStep + 1);
    }
  };

  const handlePrevious = () => {
    if (currentStep > 1) {
      setCurrentStep(currentStep - 1);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    handleNext();
  };

  if (!train) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <p className="text-gray-600 mb-4">{t('no.trains')}</p>
          <button
            onClick={() => navigate('/search')}
            className="bg-emerald-600 text-white px-4 py-2 rounded-lg hover:bg-emerald-700 transition-colors"
          >
            {t('back')} {t('search')}
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 py-4 sm:py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Progress Steps */}
        <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 mb-4 sm:mb-8">
          <div className="flex items-center justify-between overflow-x-auto">
            {steps.map((step, index) => (
              <div key={step.id} className="flex items-center flex-shrink-0">
                <div className={`flex items-center justify-center w-10 h-10 rounded-full ${
                  currentStep >= step.id ? 'bg-emerald-600 text-white' : 'bg-gray-200 text-gray-600'
                }`}>
                  <step.icon className="h-5 w-5" />
                </div>
                <span className={`ml-2 rtl:ml-0 rtl:mr-2 font-medium text-sm sm:text-base ${
                  currentStep >= step.id ? 'text-emerald-600' : 'text-gray-500'
                } hidden sm:inline`}>
                  {step.name}
                </span>
                {index < steps.length - 1 && (
                  <div className={`w-8 sm:w-16 h-0.5 ml-4 sm:ml-8 rtl:ml-0 rtl:mr-4 sm:rtl:mr-8 ${
                    currentStep > step.id ? 'bg-emerald-600' : 'bg-gray-200'
                  }`} />
                )}
              </div>
            ))}
          </div>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Main Content */}
          <div className="lg:col-span-2">
            <div className="bg-white rounded-lg shadow-md p-4 sm:p-6">
              {currentStep === 1 && (
                <div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-4 sm:mb-6">
                    {t('passenger.info')}
                  </h2>
                  <form onSubmit={handleSubmit} className="space-y-4 sm:space-y-6">
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('full.name.arabic')}
                        </label>
                        <input
                          type="text"
                          value={passengerInfo.fullNameArabic}
                          onChange={(e) => setPassengerInfo({...passengerInfo, fullNameArabic: e.target.value})}
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 text-sm sm:text-base"
                          placeholder="الاسم الكامل بالعربية"
                          required
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('full.name.english')}
                        </label>
                        <input
                          type="text"
                          value={passengerInfo.fullName}
                          onChange={(e) => setPassengerInfo({...passengerInfo, fullName: e.target.value})}
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 text-sm sm:text-base"
                          placeholder="Full name in English"
                          required
                        />
                      </div>
                    </div>
                    
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('id.passport.number')}
                        </label>
                        <input
                          type="text"
                          value={passengerInfo.idNumber}
                          onChange={(e) => setPassengerInfo({...passengerInfo, idNumber: e.target.value})}
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 text-sm sm:text-base"
                          placeholder="رقم الهوية أو جواز السفر"
                          required
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('birth.date')}
                        </label>
                        <input
                          type="date"
                          value={passengerInfo.birthDate}
                          onChange={(e) => setPassengerInfo({...passengerInfo, birthDate: e.target.value})}
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-emerald-500 focus:border-emerald-500 text-sm sm:text-base"
                          required
                        />
                      </div>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('gender')}
                        </label>
                        <div className="flex space-x-4 rtl:space-x-reverse">
                          <label className="flex items-center">
                            <input
                              type="radio"
                              name="gender"
                              value="male"
                              checked={passengerInfo.gender === 'male'}
                              onChange={(e) => setPassengerInfo({...passengerInfo, gender: e.target.value})}
                              className="mr-2 rtl:mr-0 rtl:ml-2"
                            />
                            {t('male')}
                          </label>
                          <label className="flex items-center">
                            <input
                              type="radio"
                              name="gender"
                              value="female"
                              checked={passengerInfo.gender === 'female'}
                              onChange={(e) => setPassengerInfo({...passengerInfo, gender: e.target.value})}
                              className="mr-2 rtl:mr-0 rtl:ml-2"
                            />
                            {t('female')}
                          </label>
                        </div>
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('nationality')}
                        </label>
                        <select
                          value={passengerInfo.nationality}
                          onChange={(e) => setPassengerInfo({...passengerInfo, nationality: e.target.value})}
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                        >
                          <option value="Sudan">Sudan</option>
                          <option value="Egypt">Egypt</option>
                          <option value="Ethiopia">Ethiopia</option>
                          <option value="Eritrea">Eritrea</option>
                          <option value="Chad">Chad</option>
                          <option value="Libya">Libya</option>
                          <option value="South Sudan">South Sudan</option>
                          <option value="Other">Other</option>
                        </select>
                      </div>
                    </div>

                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        {t('phone')}
                      </label>
                      <input
                        type="tel"
                        value={passengerInfo.phone}
                        onChange={(e) => setPassengerInfo({...passengerInfo, phone: e.target.value})}
                        className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                        placeholder="+249 123 456 789"
                        required
                      />
                    </div>
                    
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        {t('email')} ({t('optional')})
                      </label>
                      <input
                        type="email"
                        value={passengerInfo.email}
                        onChange={(e) => setPassengerInfo({...passengerInfo, email: e.target.value})}
                        className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                        placeholder="example@email.com"
                      />
                    </div>
                    
                    <button
                      type="submit"
                      className="w-full bg-blue-600 text-white py-3 rounded-lg font-medium hover:bg-blue-700 transition-colors text-sm sm:text-base"
                    >
                      {t('next')}
                    </button>
                  </form>
                </div>
              )}

              {currentStep === 2 && (
                <div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-4 sm:mb-6">
                    {t('seat.selection')}
                  </h2>
                  <div className="bg-gray-50 rounded-lg p-4 sm:p-6 mb-4 sm:mb-6">
                    <div className="grid grid-cols-4 gap-1 sm:gap-2 max-w-xs sm:max-w-md mx-auto">
                      {/* Seat Map */}
                      {Array.from({length: 40}, (_, i) => (
                        <div
                          key={i}
                          onClick={() => {
                            if (![3, 7, 12, 23, 31].includes(i)) {
                              setSelectedSeat(i);
                            }
                          }}
                          className={`w-6 h-6 sm:w-8 sm:h-8 rounded border-2 cursor-pointer transition-colors ${
                            selectedSeat === i ? 'bg-blue-600 border-blue-600 text-white' : 
                            [3, 7, 12, 23, 31].includes(i) ? 'bg-red-200 border-red-300 cursor-not-allowed' :
                            'bg-white border-gray-300 hover:bg-blue-50 hover:border-blue-300'
                          }`}
                        >
                          <span className="text-xs font-medium flex items-center justify-center h-full">
                            {i + 1}
                          </span>
                        </div>
                      ))}
                    </div>
                    <div className="flex flex-col sm:flex-row justify-center space-y-2 sm:space-y-0 sm:space-x-6 rtl:space-x-reverse mt-4 sm:mt-6">
                      <div className="flex items-center space-x-2 rtl:space-x-reverse">
                        <div className="w-4 h-4 bg-white border-2 border-gray-300 rounded"></div>
                        <span className="text-xs sm:text-sm">{t('available')}</span>
                      </div>
                      <div className="flex items-center space-x-2 rtl:space-x-reverse">
                        <div className="w-4 h-4 bg-red-200 border-2 border-red-300 rounded"></div>
                        <span className="text-xs sm:text-sm">{t('occupied')}</span>
                      </div>
                      <div className="flex items-center space-x-2 rtl:space-x-reverse">
                        <div className="w-4 h-4 bg-blue-600 border-2 border-blue-600 rounded"></div>
                        <span className="text-xs sm:text-sm">{t('selected')}</span>
                      </div>
                    </div>
                    
                    {selectedSeat !== null && (
                      <div className="mt-4 p-3 bg-blue-50 rounded-lg text-center">
                        <p className="text-sm font-medium text-blue-800">
                          {t('selected.seat')}: {selectedSeat + 1}
                        </p>
                      </div>
                    )}
                    
                    <div className="mt-4 text-center">
                      <button
                        onClick={() => setSelectedSeat(Math.floor(Math.random() * 40))}
                        className="text-blue-600 text-sm hover:text-blue-800 transition-colors"
                      >
                        {t('auto.select.seat')}
                      </button>
                    </div>
                  </div>
                  
                  <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-3 mb-4">
                    <p className="text-sm text-yellow-800">
                      <span className="font-medium">{t('add.another.passenger')}</span>
                    </p>
                    <button className="text-blue-600 text-sm mt-1 hover:text-blue-800 transition-colors">
                      + {t('add.passenger')}
                    </button>
                  </div>
                  
                  <div className="flex flex-col sm:flex-row space-y-2 sm:space-y-0 sm:space-x-4 rtl:space-x-reverse">
                    <button
                      onClick={handlePrevious}
                      className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors text-sm sm:text-base"
                    >
                      <ArrowLeft className="h-4 w-4 mr-2 rtl:mr-0 rtl:ml-2" />
                      {t('previous')}
                    </button>
                    <button
                      onClick={handleNext}
                      className="flex-1 bg-blue-600 text-white py-2 rounded-lg font-medium hover:bg-blue-700 transition-colors text-sm sm:text-base"
                      disabled={selectedSeat === null}
                    >
                      {t('next')}
                    </button>
                  </div>
                </div>
              )}

              {currentStep === 3 && (
                <div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-4 sm:mb-6">
                    {t('payment')}
                  </h2>
                  
                  {/* Payment Methods */}
                  <div className="mb-6">
                    <h3 className="text-lg font-semibold text-gray-900 mb-4">{t('choose.payment.method')}</h3>
                    <div className="space-y-3">
                      <label className="flex items-center p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                        <input type="radio" name="payment" value="card" className="mr-3 rtl:mr-0 rtl:ml-3" defaultChecked />
                        <div className="flex items-center">
                          <div className="w-8 h-6 bg-blue-600 rounded mr-3 rtl:mr-0 rtl:ml-3 flex items-center justify-center">
                            <span className="text-white text-xs font-bold">💳</span>
                          </div>
                          <div>
                            <p className="font-medium">{t('credit.debit.card')}</p>
                            <p className="text-sm text-gray-600">Visa, Mastercard</p>
                          </div>
                        </div>
                      </label>
                      
                      <label className="flex items-center p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                        <input type="radio" name="payment" value="mobile" className="mr-3 rtl:mr-0 rtl:ml-3" />
                        <div className="flex items-center">
                          <div className="w-8 h-6 bg-green-600 rounded mr-3 rtl:mr-0 rtl:ml-3 flex items-center justify-center">
                            <span className="text-white text-xs font-bold">📱</span>
                          </div>
                          <div>
                            <p className="font-medium">{t('mobile.payment')}</p>
                            <p className="text-sm text-gray-600">MTN Pay, Sudani Pay</p>
                          </div>
                        </div>
                      </label>
                      
                      <label className="flex items-center p-3 border border-gray-300 rounded-lg cursor-pointer hover:bg-gray-50">
                        <input type="radio" name="payment" value="bank" className="mr-3 rtl:mr-0 rtl:ml-3" />
                        <div className="flex items-center">
                          <div className="w-8 h-6 bg-yellow-600 rounded mr-3 rtl:mr-0 rtl:ml-3 flex items-center justify-center">
                            <span className="text-white text-xs font-bold">🏦</span>
                          </div>
                          <div>
                            <p className="font-medium">{t('bank.transfer')}</p>
                            <p className="text-sm text-gray-600">{t('pay.via.bank')}</p>
                          </div>
                        </div>
                      </label>
                    </div>
                  </div>
                  
                  <form onSubmit={(e) => { e.preventDefault(); handleNext(); }} className="space-y-4 sm:space-y-6">
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        {t('cardholder.name')}
                      </label>
                      <input
                        type="text"
                        className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                        placeholder="اسم حامل البطاقة"
                        required
                      />
                    </div>
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-2">
                        {t('card.number')}
                      </label>
                      <input
                        type="text"
                        placeholder="1234 5678 9012 3456"
                        className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                        required
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-3 sm:gap-4">
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          {t('expiry.date')}
                        </label>
                        <input
                          type="text"
                          placeholder="MM/YY"
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                          required
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                          CVV
                        </label>
                        <input
                          type="text"
                          placeholder="123"
                          className="w-full px-3 sm:px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-sm sm:text-base"
                          required
                        />
                      </div>
                    </div>
                    
                    <div className="bg-blue-50 border border-blue-200 rounded-lg p-3">
                      <label className="flex items-center">
                        <input type="checkbox" className="mr-2 rtl:mr-0 rtl:ml-2" required />
                        <span className="text-sm text-blue-800">
                          {t('agree.terms.conditions')}
                        </span>
                      </label>
                    </div>
                    
                    <div className="flex flex-col sm:flex-row space-y-2 sm:space-y-0 sm:space-x-4 rtl:space-x-reverse">
                      <button
                        type="button"
                        onClick={handlePrevious}
                        className="flex items-center justify-center px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors text-sm sm:text-base"
                      >
                        <ArrowLeft className="h-4 w-4 mr-2 rtl:mr-0 rtl:ml-2" />
                        {t('previous')}
                      </button>
                      <button
                        type="submit"
                        className="flex-1 bg-blue-600 text-white py-2 rounded-lg font-medium hover:bg-blue-700 transition-colors text-sm sm:text-base"
                      >
                        {t('continue.payment')} {train.price} {t('sdg')}
                      </button>
                    </div>
                  </form>
                </div>
              )}

              {currentStep === 4 && (
                <div className="text-center">
                  <div className="bg-green-100 rounded-full w-16 h-16 mx-auto mb-6 flex items-center justify-center">
                    <Check className="h-8 w-8 text-green-600" />
                  </div>
                  <h2 className="text-xl sm:text-2xl font-bold text-gray-900 mb-4">
                    {t('booking.confirmed')}
                  </h2>
                  <p className="text-sm sm:text-base text-gray-600 mb-6">
                    {t('ticket.booked.successfully')}
                  </p>
                  
                  <div className="bg-gray-50 rounded-lg p-4 sm:p-6 mb-6">
                    <h3 className="font-semibold mb-2 text-sm sm:text-base">{t('booking.reference')}</h3>
                    <p className="text-xl sm:text-2xl font-bold text-blue-600">SD-2024-{Math.random().toString(36).substr(2, 5).toUpperCase()}</p>
                    
                    <div className="mt-4 p-3 bg-white rounded border">
                      <div className="text-center">
                        <div className="w-24 h-24 mx-auto mb-2 bg-gray-200 rounded flex items-center justify-center">
                          <span className="text-xs text-gray-600">QR Code</span>
                        </div>
                        <p className="text-xs text-gray-600">{t('scan.at.station')}</p>
                      </div>
                    </div>
                  </div>
                  
                  <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 sm:gap-4">
                    <button
                      onClick={() => navigate('/dashboard')}
                      className="bg-blue-600 text-white py-3 rounded-lg font-medium hover:bg-blue-700 transition-colors text-sm sm:text-base"
                    >
                      {t('view.my.trips')}
                    </button>
                    <button
                      onClick={() => navigate('/')}
                      className="border border-gray-300 py-3 rounded-lg font-medium hover:bg-gray-50 transition-colors text-sm sm:text-base"
                    >
                      {t('book.another.trip')}
                    </button>
                  </div>
                  
                  <div className="mt-6 p-4 bg-blue-50 rounded-lg">
                    <h4 className="font-semibold text-blue-900 mb-2">{t('need.help')}</h4>
                    <div className="flex flex-col sm:flex-row justify-center space-y-2 sm:space-y-0 sm:space-x-4 rtl:space-x-reverse text-sm">
                      <button className="text-blue-600 hover:text-blue-800">📞 {t('call.us')} +249 123 456</button>
                      <button className="text-blue-600 hover:text-blue-800">💬 {t('whatsapp.us')}</button>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>

          {/* Booking Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 lg:sticky lg:top-24">
              <h3 className="text-base sm:text-lg font-semibold mb-4">{t('booking.summary')}</h3>
              
              <div className="space-y-3 sm:space-y-4 mb-4 sm:mb-6">
                <div className="flex items-center space-x-3 rtl:space-x-reverse">
                  <Train className="h-5 w-5 text-blue-600" />
                  <div>
                    <p className="font-medium text-sm sm:text-base">{train.name}</p>
                    <p className="text-sm text-gray-600">{train.class}</p>
                  </div>
                </div>
                
                <div className="flex items-center space-x-3 rtl:space-x-reverse">
                  <MapPin className="h-5 w-5 text-gray-400" />
                  <div>
                    <p className="text-sm text-gray-600">{t('route')}</p>
                    <p className="font-medium">{t(searchParams?.from)} → {t(searchParams?.to)}</p>
                  </div>
                </div>
                
                <div className="flex items-center space-x-3 rtl:space-x-reverse">
                  <div className="h-5 w-5 flex items-center justify-center">
                    <div className="w-2 h-2 bg-blue-600 rounded-full"></div>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">{t('departure')}</p>
                    <p className="font-medium">{train.departure}</p>
                  </div>
                </div>
                
                <div className="flex items-center space-x-3 rtl:space-x-reverse">
                  <div className="h-5 w-5 flex items-center justify-center">
                    <div className="w-2 h-2 bg-gray-400 rounded-full"></div>
                  </div>
                  <div>
                    <p className="text-sm text-gray-600">{t('arrival')}</p>
                    <p className="font-medium">{train.arrival}</p>
                  </div>
                </div>
                
                {selectedSeat !== null && (
                  <div className="flex items-center space-x-3 rtl:space-x-reverse">
                    <div className="h-5 w-5 flex items-center justify-center">
                      <div className="w-2 h-2 bg-green-600 rounded-full"></div>
                    </div>
                    <div>
                      <p className="text-sm text-gray-600">{t('seat')}</p>
                      <p className="font-medium">{selectedSeat + 1}A</p>
                    </div>
                  </div>
                )}
              </div>

              <div className="border-t pt-3 sm:pt-4">
                <div className="flex justify-between items-center mb-2">
                  <span className="text-sm text-gray-600">{t('ticket.price')}</span>
                  <span className="font-medium text-sm">{train.price} {t('sdg')}</span>
                </div>
                <div className="flex justify-between items-center mb-2">
                  <span className="text-sm text-gray-600">{t('service.fee')}</span>
                  <span className="font-medium text-sm">25 {t('sdg')}</span>
                </div>
                <div className="flex justify-between items-center text-base sm:text-lg font-bold border-t pt-2">
                  <span>{t('total')}</span>
                  <span>{train.price + 25} {t('sdg')}</span>
                </div>
                
                {currentStep >= 3 && (
                  <div className="mt-4 p-3 bg-green-50 rounded-lg">
                    <p className="text-sm text-green-800 font-medium">
                      🔒 {t('secure.payment.guaranteed')}
                    </p>
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BookingPage;