import { createContext, useContext, useEffect, useState, ReactNode } from 'react';

// Customer UI supports Arabic (default, RTL) and English (LTR). The `t()` API is
// unchanged; missing English keys gracefully fall back to the Arabic string.

type Language = 'ar' | 'en';

interface LanguageContextType {
  language: Language;
  setLanguage: (lang: Language) => void;
  toggleLanguage: () => void;
  t: (key: string) => string;
}

const LanguageContext = createContext<LanguageContextType | undefined>(undefined);

const STORAGE_KEY = 'app.language';

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
  'language.switch': 'English',

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
  'seat.hold.timer': 'الوقت المتبقي لإتمام الحجز',
  'seat.hold.expired': 'انتهت مهلة حجز المقاعد (5 دقائق). يرجى اختيار المقاعد من جديد.',
  'seat.hold.syncing': 'جاري حجز المقاعد مؤقتاً…',
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
  'validation.card.cvv': 'رمز الأمان يجب أن يكون ٣ أرقام.',
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

  // Network / offline
  'network.offline': 'لا يوجد اتصال بالإنترنت. يرجى التحقق من الشبكة والمحاولة مرة أخرى.',
  'network.offline.banner': 'أنت غير متصل بالإنترنت — بعض الميزات غير متاحة حتى يعود الاتصال.',
  'network.connection.failed': 'تعذّر الاتصال بالخادم. يرجى المحاولة مرة أخرى.',
  'network.cached.data.notice': 'يتم عرض بيانات محفوظة مؤقتاً. قد لا تكون محدّثة.',
  'network.booking.requires.connection': 'يتطلب إتمام الحجز اتصالاً بالإنترنت.',

  // Payment providers (Visa active; others prepared for future integration)
  'payment.visa.description': 'الدفع الإلكتروني الآمن عبر بطاقة فيزا.',
  'payment.simulated.notice': 'نموذج أولي للدفع الإلكتروني — سيتم ربط بوابات الدفع المحلية لاحقاً.',
  'payment.provider.bankak': 'بنكك',
  'payment.provider.fawry': 'فوري',
  'payment.provider.mobile': 'محفظة إلكترونية',
  'payment.provider.coming.soon': 'قريباً',
  'payment.cvv': 'رمز الأمان',
  'payment.card.brand': 'فيزا',

  // System governance (research gap — multi-role model)
  'governance.title': 'نظام متكامل لإدارة السكك الحديدية',
  'governance.subtitle': 'منصة رقمية تخدم المسافرين والإدارة وهيئة السكك الحديدية في السودان',
  'governance.passenger.title': 'المسافر',
  'governance.passenger.desc': 'البحث عن الرحلات، حجز المقاعد، الدفع الإلكتروني، وعرض التذاكر.',
  'governance.admin.title': 'الإدارة',
  'governance.admin.desc': 'إدارة القطارات والمسارات والأسعار والحجوزات والتقارير.',
  'governance.authority.title': 'هيئة السكك الحديدية',
  'governance.authority.desc': 'الموظفون في المحطات: بيع التذاكر، الصعود إلى القطار، ومتابعة الرحلات.',
};

const en: Record<string, string> = {
  // Navigation
  'home': 'Home',
  'search': 'Search',
  'dashboard': 'Dashboard',
  'admin': 'Admin',
  'login': 'Sign In',
  'logout': 'Sign Out',

  // Auth pages
  'register': 'Register',
  'sign.in': 'Sign In',
  'sign.up': 'Sign Up',
  'create.account': 'Create a new account',
  'first.name': 'First Name',
  'last.name': 'Last Name',
  'password': 'Password',
  'confirm.password': 'Confirm Password',
  'phone.number': 'Phone Number',
  'email.address': 'Email Address',
  'already.have.account': 'Already have an account?',
  'dont.have.account': "Don't have an account?",
  'forgot.password': 'Forgot your password?',
  'confirm.email.title': 'Confirm Email',
  'confirm.email.desc': 'Enter the 4-digit code sent to your email.',
  'confirm.email.no.session': 'We could not find a recent registration. Please create an account first.',
  'user.id': 'User ID',
  'verification.code': 'Verification Code',
  'verify': 'Verify',
  'reset.password.title': 'Reset Password',
  'send.reset.code': 'Send Reset Code',
  'reset.code': 'Reset Code',
  'new.password': 'New Password',
  'reset.code.sent': 'A reset code has been sent to your email.',
  'registration.success': 'Registered! Check your email for the confirmation code.',
  'registration.resume': 'This email is already registered but not yet activated. We sent you a new confirmation code.',
  'email.confirmed.login': 'Email confirmed. You can sign in now.',
  'processing': 'Processing...',
  'login.required': 'Please sign in to continue',

  // Homepage
  'hero.title': 'Book your journey on Sudan Trains',
  'hero.subtitle': 'Explore Sudan safely and comfortably with the Sudanese railway network',
  'book.your.journey': 'Book your journey',
  'my.bookings': 'My Bookings',
  'view.manage.trips': 'View and manage trips',
  'train.status': 'Train Status',
  'check.live.status': 'Check live status',
  'customer.support': 'Customer Support',
  '24.7.support': 'Round-the-clock support',
  'need.help': 'Need help?',
  'contact.support.team': 'Contact our support team',
  'call.us': 'Call Us',
  'email.us': 'Email Us',
  'live.chat': 'Live Chat',
  'available.24.7': 'Available 24/7',
  'from': 'From',
  'to': 'To',
  'date': 'Date',
  'passengers': 'Passengers',
  'increase.passengers': 'Increase passengers',
  'decrease.passengers': 'Decrease passengers',
  'class': 'Class',
  'search.trains': 'Search Trains',
  'why.choose.atbara.rail': 'Why choose Sudan Trains?',
  'experience.future.travel': 'Experience the future of travel with our modern, efficient and comfortable services',
  'popular.routes': 'Popular Destinations',
  'discover.most.traveled': 'Discover the most traveled destinations across Sudan',
  'safe.secure': 'Safe & Secure',
  'modern.safety.systems': 'Modern safety systems and secure booking',
  'on.time.performance': 'On-Time Performance',
  'reliable.schedules': 'Reliable schedules and punctual service',
  'comfort.quality': 'Comfort & Quality',
  'premium.seats': 'Premium seats and excellent service',
  'award.winning': 'Award Winning',
  'recognized.excellence': 'Recognized for excellence in transport',

  // Search Results
  'available.trains': 'Available Trains',
  'departure': 'Departure',
  'arrival': 'Arrival',
  'duration': 'Duration',
  'price': 'Price',
  'book.now': 'Book Now',
  'no.trains': 'No trains available',
  'trains.found': 'trains available',
  'modify.search': 'Modify Search',
  'filters': 'Filters',
  'all.classes': 'All Classes',
  'price.range.sdg': 'Price range (SDG)',
  'price.range.sar': 'Price range (SAR)',
  'sort.by': 'Sort by',
  'departure.time': 'Departure Time',
  'seats.available': 'seats available',
  'per.person': 'per person',

  // Booking
  'passenger.info': 'Passenger Information',
  'full.name': 'Full Name',
  'full.name.arabic': 'Full Name (Arabic)',
  'full.name.english': 'Full Name (English)',
  'id.number': 'ID Number',
  'id.passport.number': 'ID / Passport Number',
  'birth.date': 'Date of Birth',
  'gender': 'Gender',
  'male': 'Male',
  'female': 'Female',
  'nationality': 'Nationality',
  'optional': 'Optional',
  'seat.selection': 'Seat Selection',
  'selected.seat': 'Selected Seat',
  'auto.select.seat': 'Auto-select seat',
  'add.another.passenger': 'Add another passenger',
  'add.passenger': 'Add passenger',
  'payment.summary': 'Payment Summary',
  'choose.payment.method': 'Choose payment method',
  'payment.visa.only': 'Visa cards only',
  'payment.declined': 'Payment was declined by the bank. Please check your card details or use another card.',
  'credit.debit.card': 'Credit / Debit Card',
  'mobile.payment': 'Mobile Payment',
  'bank.transfer': 'Bank Transfer',
  'pay.via.bank': 'Pay via bank',
  'agree.terms.conditions': 'I agree to the terms and conditions',
  'continue.payment': 'Continue to Payment',
  'booking.summary': 'Booking Summary',
  'route': 'Route',
  'ticket.price': 'Ticket Price',
  'service.fee': 'Service Fee',
  'total': 'Total',
  'passenger.information': 'Passenger Information',
  'email': 'Email',
  'phone': 'Phone',
  'available': 'Available',
  'occupied': 'Occupied',
  'selected': 'Selected',
  'payment': 'Payment',
  'card.number': 'Card Number',
  'expiry.date': 'Expiry Date',
  'cardholder.name': 'Cardholder Name',
  'booking.confirmed': 'Booking Confirmed!',
  'ticket.booked.successfully': 'Your ticket was booked successfully. You will receive a confirmation message shortly.',
  'booking.reference': 'Booking Reference',
  'scan.at.station': 'Scan at the station',
  'whatsapp.us': 'WhatsApp',
  'secure.payment.guaranteed': 'Secure payment guaranteed',
  'view.my.trips': 'View my trips',
  'book.another.trip': 'Book another trip',

  // Dashboard
  'my.trips': 'My Trips',
  'upcoming': 'Upcoming',
  'past': 'Past',
  'download.ticket': 'Download Ticket',
  'cancel.trip': 'Cancel Trip',
  'cancel.booking': 'Cancel Booking',
  'cancel.booking.title': 'Confirm Booking Cancellation',
  'cancel.booking.warning': 'This action cannot be undone. All tickets associated with this booking will be cancelled.',
  'cancel.booking.confirm': 'Yes, cancel the booking',
  'cancel.booking.keep': 'Keep the booking',
  'cancel.reason.label': 'Cancellation reason',
  'cancel.reason.placeholder': 'Optional — helps us improve the service.',
  'reschedule': 'Reschedule',
  'welcome.back': 'Welcome back',
  'no.upcoming.trips': 'No upcoming trips',
  'no.past.trips': 'No past trips',
  'e.ticket': 'E-Ticket',
  'scan.qr.code': 'Scan the QR code at the station',
  'booking.ref': 'Booking Ref:',
  'train': 'Train:',
  'seat': 'Seat:',
  'download.pdf': 'Download PDF',

  // Common
  'loading': 'Loading...',
  'error': 'Error',
  'success': 'Success',
  'cancel': 'Cancel',
  'confirm': 'Confirm',
  'save': 'Save',
  'edit': 'Edit',
  'delete': 'Delete',
  'submit': 'Submit',
  'back': 'Back',
  'next': 'Next',
  'previous': 'Previous',
  'close': 'Close',
  'select': 'Select',
  'min': 'Min',
  'max': 'Max',
  'minutes.ago': 'minutes ago',
  'hour.ago': 'an hour ago',
  'hours.ago': 'hours ago',
  'language.switch': 'العربية',

  // Brand & Places
  'atbara.rail': 'Sudan Trains',
  'brand.name': 'Sudan Trains',
  'brand.tagline': 'Sudanese Railway Network',
  'khartoum': 'Khartoum',
  'atbara': 'Atbara',
  'port.sudan': 'Port Sudan',
  'kassala': 'Kassala',
  'wad.medani': 'Wad Madani',
  'nyala': 'Nyala',
  'el.obeid': 'El Obeid',
  'dongola': 'Dongola',
  'sennar': 'Sennar',

  // Nationalities
  'sudan': 'Republic of Sudan',
  'egypt': 'Arab Republic of Egypt',
  'ethiopia': 'Ethiopia',
  'eritrea': 'Eritrea',
  'chad': 'Chad',
  'libya': 'Libya',
  'south.sudan': 'South Sudan',
  'other': 'Other',

  // Train Classes
  'economy': 'Economy',
  'business': 'Business',
  'vip': 'VIP',

  // Currency / Time
  'sdg': 'SDG',
  'duration.hours': 'hours',
  'duration.minutes': 'minutes',
  'cancelled': 'Cancelled',
  'confirmed': 'Confirmed',
  'completed': 'Completed',
  'pending': 'Pending',

  // Booking flow (Option B per-segment)
  'select.seat.first': 'Please select a seat first.',
  'seat.just.taken': 'This seat was just taken by another passenger — please select another seat.',
  'seat.hold.timer': 'Time remaining to complete the booking',
  'seat.hold.expired': 'The seat hold has expired (5 minutes). Please select your seats again.',
  'seat.hold.syncing': 'Holding your seats temporarily…',
  'no.seats.available': 'No seats available for this route.',
  'retry': 'Retry',
  'coach': 'Coach',
  'window.seat': 'Window seat',
  'boarding': 'Boarding Station',
  'alighting': 'Alighting Station',

  // Coach classes (server returns "First"/"Second"/"Third")
  'first': 'First Class',
  'second': 'Second Class',
  'third': 'Third Class',
  'first.class': 'First Class',
  'second.class': 'Second Class',
  'third.class': 'Third Class',
  'any.class': 'Any Class',
  'no.coaches.in.class': 'There are no coaches in this class on this trip. Try another class.',
  'passenger': 'Passenger',
  'next.passenger': 'Next: next passenger details',
  'use.my.data': 'Use my data',
  'use.my.data.loading': 'Filling in your data...',
  'use.my.data.error': 'Could not fetch your data, please enter it manually.',
  'use.my.data.empty': 'No saved data to fill in, please enter it manually.',

  // Client-side validation messages for the passenger info forms.
  'validation.required': 'This field is required.',
  'validation.arabic.only': 'Please use Arabic letters only.',
  'validation.english.only': 'Please use English letters only.',
  'validation.id.format': 'National ID / passport must contain 5-30 letters or digits.',
  'validation.date.invalid': 'The date is invalid.',
  'validation.birthdate.future': 'Date of birth must be in the past.',
  'validation.birthdate.too.old': 'Date of birth is not reasonable.',
  'validation.phone.format': 'Phone number must be 8 to 15 digits.',
  'validation.email.format': 'Email format is invalid.',
  'validation.fix.errors': 'Please fix the fields highlighted in red before continuing.',
  'validation.card.number': 'Card number is invalid. It must be 16 digits and start with 4 (Visa).',
  'validation.card.visa.only': 'Only Visa cards are accepted (number starts with 4).',
  'validation.card.expiry': 'Expiry date must be in MM/YY format.',
  'validation.card.expired': 'The card has expired.',
  'validation.card.cvv': 'The security code must be 3 digits.',
  'passengers.count.note': 'Please fill in details for {n} passenger(s)',
  'seats.picked': 'Selected',
  'select.all.seats': 'Please select a seat for each passenger.',
  'selected.seats': 'Selected Seats',
  'seats': 'Seats',

  // Fare breakdown
  'base.fare': 'Base Fare',
  'discount': 'Discount',

  // Fare-scope chips (shown when an override price applies)
  'fare.scope.trip': 'Price specific to this trip',
  'fare.scope.segment': 'Price specific to this segment',

  // Search-card "starting from {class}" hint when the cheapest available fare
  // is shown but the customer may want a different class at booking time.
  'starting.from': 'Starting from',

  // Ticket statuses surfaced on Dashboard + e-ticket modal.
  'ticket.status.Issued': 'Issued',
  'ticket.status.Boarded': 'Boarded',
  'ticket.status.NoShow': 'No Show',
  'ticket.status.Cancelled': 'Cancelled',

  // Notifications drawer.
  'notifications': 'Notifications',
  'notifications.empty': 'No notifications',
  'notifications.markRead': 'Mark as read',
  'notification.type.BookingConfirmation': 'Booking Confirmation',
  'notification.type.BookingCancellation': 'Booking Cancellation',
  'notification.type.TripCancellation': 'Trip Cancellation',
  'notification.type.TripDelay': 'Trip Delay',
  'notification.type.PaymentReceived': 'Payment Received',
  'notification.type.SystemAlert': 'System Alert',
  'notification.type.PromotionalOffer': 'Promotional Offer',

  // Network / offline
  'network.offline': 'No internet connection. Please check your network and try again.',
  'network.offline.banner': 'You are offline — some features are unavailable until the connection returns.',
  'network.connection.failed': 'Could not connect to the server. Please try again.',
  'network.cached.data.notice': 'Showing cached data. It may not be up to date.',
  'network.booking.requires.connection': 'Completing the booking requires an internet connection.',

  // Payment providers (Visa active; others prepared for future integration)
  'payment.visa.description': 'Secure electronic payment via Visa card.',
  'payment.simulated.notice': 'Electronic payment prototype — local payment gateways will be integrated later.',
  'payment.provider.bankak': 'Bankak',
  'payment.provider.fawry': 'Fawry',
  'payment.provider.mobile': 'E-Wallet',
  'payment.provider.coming.soon': 'Coming soon',
  'payment.cvv': 'Security Code',
  'payment.card.brand': 'Visa',

  // System governance (research gap — multi-role model)
  'governance.title': 'An integrated railway management system',
  'governance.subtitle': 'A digital platform serving travelers, management and the railway authority in Sudan',
  'governance.passenger.title': 'Passenger',
  'governance.passenger.desc': 'Search trips, book seats, pay electronically, and view tickets.',
  'governance.admin.title': 'Management',
  'governance.admin.desc': 'Manage trains, routes, fares, bookings and reports.',
  'governance.authority.title': 'Railway Authority',
  'governance.authority.desc': 'Station staff: sell tickets, board the train, and track trips.',
};

const dictionaries: Record<Language, Record<string, string>> = { ar, en };

const getInitialLanguage = (): Language => {
  if (typeof window === 'undefined') return 'ar';
  const stored = window.localStorage.getItem(STORAGE_KEY);
  return stored === 'en' ? 'en' : 'ar';
};

const applyDocumentLanguage = (lang: Language) => {
  if (typeof document === 'undefined') return;
  document.documentElement.lang = lang;
  document.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
};

export const LanguageProvider = ({ children }: { children: ReactNode }) => {
  const [language, setLanguageState] = useState<Language>(getInitialLanguage);

  useEffect(() => {
    applyDocumentLanguage(language);
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(STORAGE_KEY, language);
    }
  }, [language]);

  const setLanguage = (lang: Language) => setLanguageState(lang);
  const toggleLanguage = () => setLanguageState((prev) => (prev === 'ar' ? 'en' : 'ar'));

  const t = (key: string): string => dictionaries[language][key] ?? ar[key] ?? key;

  return (
    <LanguageContext.Provider value={{ language, setLanguage, toggleLanguage, t }}>
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
