import { createContext, useContext, ReactNode } from 'react';

// Arabic-only customer UI. The language toggle has been removed; the provider
// keeps the same `t()` API so existing pages continue to compile unchanged.

interface LanguageContextType {
  language: 'ar';
  setLanguage: (lang: 'ar') => void; // kept for API compatibility — no-op.
  t: (key: string) => string;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

const ar: Record<string, string> = {
  // Navigation
  'home': 'الرئيسية',
  'search': 'البحث',
  'dashboard': 'لوحة التحكم',
  'admin': 'الإدارة',
  'login': 'تسجيل الدخول',
  'logout': 'تسجيل الخروج',

  // Auth pages
  'register': 'تسجيل',
  'sign.in': 'تسجيل الدخول',
  'sign.up': 'إنشاء حساب',
  'create.account': 'إنشاء حساب جديد',
  'first.name': 'الاسم الأول',
  'last.name': 'اسم العائلة',
  'password': 'كلمة المرور',
  'confirm.password': 'تأكيد كلمة المرور',
  'phone.number': 'رقم الهاتف',
  'email.address': 'البريد الإلكتروني',
  'already.have.account': 'لديك حساب بالفعل؟',
  'dont.have.account': 'ليس لديك حساب؟',
  'forgot.password': 'نسيت كلمة المرور؟',
  'confirm.email.title': 'تأكيد البريد الإلكتروني',
  'confirm.email.desc': 'أدخل الرمز المكوّن من 4 أرقام المُرسل إلى بريدك الإلكتروني.',
  'confirm.email.no.session': 'لم نعثر على طلب تسجيل حديث. يُرجى إنشاء حساب أولاً.',
  'user.id': 'معرّف المستخدم',
  'verification.code': 'رمز التحقق',
  'verify': 'تحقق',
  'reset.password.title': 'إعادة تعيين كلمة المرور',
  'send.reset.code': 'إرسال رمز الاستعادة',
  'reset.code': 'رمز الاستعادة',
  'new.password': 'كلمة المرور الجديدة',
  'reset.code.sent': 'تم إرسال رمز الاستعادة إلى بريدك الإلكتروني.',
  'registration.success': 'تم التسجيل! تحقق من بريدك الإلكتروني للحصول على رمز التأكيد.',
  'registration.resume': 'هذا البريد مسجَّل مسبقاً ولم يتم تفعيله بعد. أرسلنا لك رمز تأكيد جديد.',
  'email.confirmed.login': 'تم تأكيد البريد الإلكتروني. يمكنك تسجيل الدخول الآن.',
  'processing': 'جاري المعالجة...',
  'login.required': 'يرجى تسجيل الدخول للمتابعة',

  // Homepage
  'hero.title': 'احجز رحلتك على قطارات السودان',
  'hero.subtitle': 'استكشف السودان بأمان وراحة مع شبكة السكك الحديدية السودانية',
  'book.your.journey': 'احجز رحلتك',
  'my.bookings': 'حجوزاتي',
  'view.manage.trips': 'عرض وإدارة الرحلات',
  'train.status': 'حالة القطار',
  'check.live.status': 'تحقق من الحالة المباشرة',
  'customer.support': 'خدمة العملاء',
  '24.7.support': 'دعم على مدار الساعة',
  'need.help': 'تحتاج مساعدة؟',
  'contact.support.team': 'تواصل مع فريق الدعم الخاص بنا',
  'call.us': 'اتصل بنا',
  'email.us': 'راسلنا',
  'live.chat': 'محادثة مباشرة',
  'available.24.7': 'متاح 24/7',
  'from': 'من',
  'to': 'إلى',
  'date': 'التاريخ',
  'passengers': 'المسافرون',
  'increase.passengers': 'زيادة عدد المسافرين',
  'decrease.passengers': 'إنقاص عدد المسافرين',
  'class': 'الدرجة',
  'search.trains': 'البحث عن الرحلات',
  'why.choose.atbara.rail': 'لماذا تختار قطارات السودان؟',
  'experience.future.travel': 'اختبر مستقبل السفر مع خدماتنا الحديثة والفعالة والمريحة',
  'popular.routes': 'الوجهات الأكثر شعبية',
  'discover.most.traveled': 'اكتشف الوجهات الأكثر سفراً عبر جمهورية السودان',
  'safe.secure': 'آمن ومضمون',
  'modern.safety.systems': 'أنظمة أمان حديثة وحجز آمن',
  'on.time.performance': 'الالتزام بالمواعيد',
  'reliable.schedules': 'جداول موثوقة وخدمات دقيقة',
  'comfort.quality': 'راحة وجودة',
  'premium.seats': 'مقاعد فاخرة وخدمة ممتازة',
  'award.winning': 'حائز على جوائز',
  'recognized.excellence': 'معترف به للتميز في النقل',

  // Search Results
  'available.trains': 'الرحلات المتاحة',
  'departure': 'المغادرة',
  'arrival': 'الوصول',
  'duration': 'المدة',
  'price': 'السعر',
  'book.now': 'احجز الآن',
  'no.trains': 'لا توجد رحلات متاحة',
  'trains.found': 'رحلة متاحة',
  'modify.search': 'تعديل البحث',
  'filters': 'المرشحات',
  'all.classes': 'جميع الدرجات',
  'price.range.sdg': 'نطاق السعر (جنيه سوداني)',
  'price.range.sar': 'نطاق السعر (ريال)',
  'sort.by': 'ترتيب حسب',
  'departure.time': 'وقت المغادرة',
  'seats.available': 'مقعد متاح',
  'per.person': 'للشخص الواحد',

  // Booking
  'passenger.info': 'معلومات المسافر',
  'full.name': 'الاسم الكامل',
  'full.name.arabic': 'الاسم الكامل بالعربية',
  'full.name.english': 'الاسم الكامل بالإنجليزية',
  'id.number': 'رقم الهوية',
  'id.passport.number': 'رقم الهوية/جواز السفر',
  'birth.date': 'تاريخ الميلاد',
  'gender': 'الجنس',
  'male': 'ذكر',
  'female': 'أنثى',
  'nationality': 'الجنسية',
  'optional': 'اختياري',
  'seat.selection': 'اختيار المقعد',
  'selected.seat': 'المقعد المختار',
  'auto.select.seat': 'اختيار تلقائي للمقعد',
  'add.another.passenger': 'إضافة مسافر آخر',
  'add.passenger': 'إضافة مسافر',
  'payment.summary': 'ملخص الدفع',
  'choose.payment.method': 'اختر طريقة الدفع',
  'payment.visa.only': 'الدفع ببطاقة فيزا فقط',
  'payment.declined': 'تم رفض الدفع من قبل البنك. يرجى التحقق من بيانات البطاقة أو استخدام بطاقة أخرى.',
  'credit.debit.card': 'بطاقة ائتمان/خصم',
  'mobile.payment': 'الدفع عبر الهاتف',
  'bank.transfer': 'التحويل المصرفي',
  'pay.via.bank': 'ادفع عن طريق البنك',
  'agree.terms.conditions': 'أوافق على الشروط والأحكام',
  'continue.payment': 'متابعة الدفع',
  'booking.summary': 'ملخص الحجز',
  'route': 'المسار',
  'ticket.price': 'سعر التذكرة',
  'service.fee': 'رسوم الخدمة',
  'total': 'المجموع',
  'passenger.information': 'معلومات المسافر',
  'email': 'البريد الإلكتروني',
  'phone': 'رقم الهاتف',
  'available': 'متاح',
  'occupied': 'محجوز',
  'selected': 'مختار',
  'payment': 'الدفع',
  'card.number': 'رقم البطاقة',
  'expiry.date': 'تاريخ الانتهاء',
  'cardholder.name': 'اسم حامل البطاقة',
  'booking.confirmed': 'تم تأكيد الحجز!',
  'ticket.booked.successfully': 'تم حجز تذكرتك بنجاح. ستتلقى رسالة تأكيد قريباً.',
  'booking.reference': 'رقم الحجز',
  'scan.at.station': 'امسح في المحطة',
  'whatsapp.us': 'واتساب',
  'secure.payment.guaranteed': 'دفع آمن ومضمون',
  'view.my.trips': 'عرض رحلاتي',
  'book.another.trip': 'حجز رحلة أخرى',

  // Dashboard
  'my.trips': 'رحلاتي',
  'upcoming': 'القادمة',
  'past': 'السابقة',
  'download.ticket': 'تحميل التذكرة',
  'cancel.trip': 'إلغاء الرحلة',
  'cancel.booking': 'إلغاء الحجز',
  'cancel.booking.title': 'تأكيد إلغاء الحجز',
  'cancel.booking.warning': 'هذا الإجراء لا يمكن التراجع عنه. سيتم إلغاء جميع التذاكر المرتبطة بهذا الحجز.',
  'cancel.booking.confirm': 'نعم، ألغِ الحجز',
  'cancel.booking.keep': 'الاحتفاظ بالحجز',
  'cancel.reason.label': 'سبب الإلغاء',
  'cancel.reason.placeholder': 'اختياري — يساعدنا على تحسين الخدمة.',
  'reschedule': 'إعادة جدولة',
  'welcome.back': 'مرحباً بعودتك',
  'no.upcoming.trips': 'لا توجد رحلات قادمة',
  'no.past.trips': 'لا توجد رحلات سابقة',
  'e.ticket': 'التذكرة الإلكترونية',
  'scan.qr.code': 'امسح رمز الاستجابة السريعة في المحطة',
  'booking.ref': 'رقم الحجز:',
  'train': 'القطار:',
  'seat': 'المقعد:',
  'download.pdf': 'تحميل PDF',

  // Common
  'loading': 'جاري التحميل...',
  'error': 'خطأ',
  'success': 'تم بنجاح',
  'cancel': 'إلغاء',
  'confirm': 'تأكيد',
  'save': 'حفظ',
  'edit': 'تعديل',
  'delete': 'حذف',
  'submit': 'إرسال',
  'back': 'رجوع',
  'next': 'التالي',
  'previous': 'السابق',
  'close': 'إغلاق',
  'select': 'اختر',
  'min': 'أدنى',
  'max': 'أقصى',
  'minutes.ago': 'دقائق مضت',
  'hour.ago': 'ساعة مضت',
  'hours.ago': 'ساعات مضت',

  // Brand & Places
  'atbara.rail': 'قطارات السودان',
  'brand.name': 'قطارات السودان',
  'brand.tagline': 'شبكة السكك الحديدية السودانية',
  'khartoum': 'الخرطوم',
  'atbara': 'عطبرة',
  'port.sudan': 'بورت سودان',
  'kassala': 'كسلا',
  'wad.medani': 'ود مدني',
  'nyala': 'نيالا',
  'el.obeid': 'الأبيض',
  'dongola': 'دنقلا',
  'sennar': 'سنار',

  // Nationalities
  'sudan': 'جمهورية السودان',
  'egypt': 'جمهورية مصر العربية',
  'ethiopia': 'إثيوبيا',
  'eritrea': 'إريتريا',
  'chad': 'تشاد',
  'libya': 'ليبيا',
  'south.sudan': 'جنوب السودان',
  'other': 'أخرى',

  // Train Classes
  'economy': 'الاقتصادية',
  'business': 'رجال الأعمال',
  'vip': 'كبار الشخصيات',

  // Currency / Time
  'sdg': 'جنيه سوداني',
  'duration.hours': 'ساعات',
  'duration.minutes': 'دقائق',
  'cancelled': 'ملغي',
  'confirmed': 'مؤكد',
  'completed': 'مكتملة',
  'pending': 'قيد الانتظار',

  // Booking flow (Option B per-segment)
  'select.seat.first': 'يرجى اختيار مقعد أولاً.',
  'seat.just.taken': 'تم حجز هذا المقعد للتو من قبل مسافر آخر — يرجى اختيار مقعد آخر.',
  'no.seats.available': 'لا توجد مقاعد متاحة لهذا المسار.',
  'retry': 'إعادة المحاولة',
  'coach': 'العربة',
  'window.seat': 'بجانب النافذة',
  'boarding': 'محطة الصعود',
  'alighting': 'محطة النزول',

  // Coach classes (server returns "First"/"Second"/"Third")
  'first': 'الدرجة الأولى',
  'second': 'الدرجة الثانية',
  'third': 'الدرجة الثالثة',
  'first.class': 'الدرجة الأولى',
  'second.class': 'الدرجة الثانية',
  'third.class': 'الدرجة الثالثة',
  'any.class': 'أي درجة',
  'no.coaches.in.class': 'لا توجد عربات في هذه الدرجة على هذه الرحلة. جرّب درجة أخرى.',
  'passenger': 'المسافر',
  'next.passenger': 'التالي: بيانات المسافر التالي',
  'use.my.data': 'استخدام بياناتي',
  'use.my.data.loading': 'جارٍ تعبئة بياناتك...',
  'use.my.data.error': 'تعذّر جلب بياناتك، يرجى إدخالها يدوياً.',
  'use.my.data.empty': 'لا توجد بيانات محفوظة لتعبئتها، يرجى إدخالها يدوياً.',

  // Client-side validation messages for the passenger info forms.
  'validation.required': 'هذا الحقل مطلوب.',
  'validation.arabic.only': 'يرجى استخدام حروف عربية فقط.',
  'validation.english.only': 'يرجى استخدام حروف إنجليزية فقط.',
  'validation.id.format': 'الرقم الوطني / الجواز يجب أن يحتوي على ٥-٣٠ حرفاً أو رقماً.',
  'validation.date.invalid': 'التاريخ غير صحيح.',
  'validation.birthdate.future': 'تاريخ الميلاد يجب أن يكون في الماضي.',
  'validation.birthdate.too.old': 'تاريخ الميلاد غير معقول.',
  'validation.phone.format': 'رقم الهاتف يجب أن يكون من ٨ إلى ١٥ رقماً.',
  'validation.email.format': 'صيغة البريد الإلكتروني غير صحيحة.',
  'validation.fix.errors': 'يرجى تصحيح الحقول المظللة بالأحمر قبل المتابعة.',
  'validation.card.number': 'رقم البطاقة غير صحيح. يجب أن يكون ١٦ رقماً ويبدأ بـ ٤ (فيزا).',
  'validation.card.visa.only': 'يُقبل الدفع ببطاقات فيزا فقط (تبدأ الرقم بـ ٤).',
  'validation.card.expiry': 'تاريخ الانتهاء يجب أن يكون بصيغة MM/YY.',
  'validation.card.expired': 'انتهت صلاحية البطاقة.',
  'validation.card.cvv': 'رمز CVV يجب أن يكون ٣ أرقام.',
  'passengers.count.note': 'يرجى تعبئة بيانات {n} مسافر/مسافرين',
  'seats.picked': 'تم اختيار',
  'select.all.seats': 'يرجى اختيار مقعد لكل مسافر.',
  'selected.seats': 'المقاعد المختارة',
  'seats': 'المقاعد',

  // Fare breakdown
  'base.fare': 'الأجرة الأساسية',
  'discount': 'خصم',

  // Fare-scope chips (shown when an override price applies)
  'fare.scope.trip': 'سعر خاص بهذه الرحلة',
  'fare.scope.segment': 'سعر خاص بهذا المقطع',

  // Search-card "starting from {class}" hint when the cheapest available fare
  // is shown but the customer may want a different class at booking time.
  'starting.from': 'ابتداءً من',

  // Ticket statuses surfaced on Dashboard + e-ticket modal.
  'ticket.status.Issued': 'صادرة',
  'ticket.status.Boarded': 'تم الصعود',
  'ticket.status.NoShow': 'لم يحضر',
  'ticket.status.Cancelled': 'ملغاة',

  // Notifications drawer.
  'notifications': 'الإشعارات',
  'notifications.empty': 'لا توجد إشعارات',
  'notifications.markRead': 'تحديد كمقروء',
  'notification.type.BookingConfirmation': 'تأكيد الحجز',
  'notification.type.BookingCancellation': 'إلغاء الحجز',
  'notification.type.TripCancellation': 'إلغاء الرحلة',
  'notification.type.TripDelay': 'تأخير الرحلة',
  'notification.type.PaymentReceived': 'تم استلام الدفع',
  'notification.type.SystemAlert': 'إشعار النظام',
  'notification.type.PromotionalOffer': 'عرض ترويجي',
};

const noop = () => undefined;

export const LanguageProvider = ({ children }: { children: ReactNode }) => {
  const t = (key: string): string => ar[key] || key;
  return (
    <LanguageContext.Provider value={{ language: 'ar', setLanguage: noop, t }}>
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguage = () => {
  const context = useContext(LanguageContext);
  if (!context) {
    throw new Error('useLanguage must be used within a LanguageProvider');
  }
  return context;
};
