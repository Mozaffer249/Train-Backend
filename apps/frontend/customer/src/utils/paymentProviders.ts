// Payment provider registry — Visa is active today; local providers can be
// enabled later without rewriting BookingPage.

export type PaymentProviderId = 'visa' | 'bankak' | 'fawry' | 'mobile_wallet';

export interface PaymentProvider {
  id: PaymentProviderId;
  labelKey: string;
  descriptionKey: string;
  enabled: boolean;
}

export const PAYMENT_PROVIDERS: PaymentProvider[] = [
  {
    id: 'visa',
    labelKey: 'payment.visa.only',
    descriptionKey: 'payment.visa.description',
    enabled: true,
  },
  {
    id: 'bankak',
    labelKey: 'payment.provider.bankak',
    descriptionKey: 'payment.provider.coming.soon',
    enabled: false,
  },
  {
    id: 'fawry',
    labelKey: 'payment.provider.fawry',
    descriptionKey: 'payment.provider.coming.soon',
    enabled: false,
  },
  {
    id: 'mobile_wallet',
    labelKey: 'payment.provider.mobile',
    descriptionKey: 'payment.provider.coming.soon',
    enabled: false,
  },
];

export function activePaymentProvider(): PaymentProvider {
  return PAYMENT_PROVIDERS.find((p) => p.enabled) ?? PAYMENT_PROVIDERS[0];
}
