using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OtpNet;
using QRCoder;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Infrastructure.InfrastructureBases;
using Sudan_Train.Service.Abstracts;

namespace Sudan_Train.Service.Implementations
{
    public class TwoFactorAuthenticationService : ITwoFactorAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IGenericRepositoryAsync<TwoFactorRecoveryCode> _recoveryCodeRepository;
        private const string ApplicationName = "Sudan Train System";

        public TwoFactorAuthenticationService(
            UserManager<User> userManager,
            IGenericRepositoryAsync<TwoFactorRecoveryCode> recoveryCodeRepository)
        {
            _userManager = userManager;
            _recoveryCodeRepository = recoveryCodeRepository;
        }

        public async Task<(string QrCodeUrl, string ManualEntryKey)> EnableTwoFactorAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            // Generate a new authenticator key
            var key = KeyGeneration.GenerateRandomKey(20);
            var base32Key = Base32Encoding.ToString(key);

            // Store the key in user's SecurityStamp (or use a custom field)
            // For ASP.NET Core Identity, we'll generate and use the authenticator token
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            // Generate QR code URL for authenticator apps
            var qrCodeUrl = GenerateQrCodeUri(user.Email!, unformattedKey!);

            return (qrCodeUrl, unformattedKey!);
        }

        public async Task<bool> VerifyAndEnableTwoFactorAsync(int userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Verify the code
            var isValid = await ValidateTwoFactorCodeAsync(userId, code);
            if (!isValid)
                return false;

            // Enable 2FA
            var result = await _userManager.SetTwoFactorEnabledAsync(user, true);
            return result.Succeeded;
        }

        public async Task<bool> DisableTwoFactorAsync(int userId, string password)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Verify password for security
            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!isValidPassword)
                return false;

            // Disable 2FA
            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            // Delete all recovery codes
            if (result.Succeeded)
            {
                var codes = await _recoveryCodeRepository.GetTableNoTracking()
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                foreach (var code in codes)
                {
                    await _recoveryCodeRepository.DeleteAsync(code);
                }

                // Reset authenticator key
                await _userManager.ResetAuthenticatorKeyAsync(user);
            }

            return result.Succeeded;
        }

        public async Task<List<string>> GenerateRecoveryCodesAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new Exception("User not found");

            // Delete old recovery codes
            var oldCodes = await _recoveryCodeRepository.GetTableNoTracking()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var oldCode in oldCodes)
            {
                await _recoveryCodeRepository.DeleteAsync(oldCode);
            }

            // Generate 10 new recovery codes
            var codes = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var code = GenerateRecoveryCode();
                codes.Add(code);

                var recoveryCode = new TwoFactorRecoveryCode
                {
                    UserId = userId,
                    Code = code,
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _recoveryCodeRepository.AddAsync(recoveryCode);
            }

            return codes;
        }

        public async Task<bool> ValidateTwoFactorCodeAsync(int userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return false;

            // Get the authenticator key
            var key = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(key))
                return false;

            // Validate TOTP code with time window (allows for clock skew)
            var totp = new Totp(Base32Encoding.ToBytes(key));
            return totp.VerifyTotp(code, out _, new VerificationWindow(2, 2));
        }

        public async Task<bool> UseRecoveryCodeAsync(int userId, string code)
        {
            var recoveryCode = await _recoveryCodeRepository.GetTableNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Code == code && !x.IsUsed);

            if (recoveryCode == null)
                return false;

            // Mark as used
            recoveryCode.IsUsed = true;
            recoveryCode.UsedAt = DateTime.UtcNow;
            await _recoveryCodeRepository.UpdateAsync(recoveryCode);

            return true;
        }

        private string GenerateQrCodeUri(string email, string key)
        {
            return $"otpauth://totp/{Uri.EscapeDataString(ApplicationName)}:{Uri.EscapeDataString(email)}?secret={key}&issuer={Uri.EscapeDataString(ApplicationName)}";
        }

        private string GenerateRecoveryCode()
        {
            var random = new Random();
            return $"{random.Next(1000, 9999)}-{random.Next(1000, 9999)}";
        }
    }
}
