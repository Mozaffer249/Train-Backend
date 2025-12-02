using MediatR;
using Microsoft.AspNetCore.Identity;
using Sudan_Train.Core.Wrappers;
using Sudan_Train.Data.Entity.Identity;
using Sudan_Train.Data.Results;

namespace Sudan_Train.Core.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Response<JwtAuthResult>>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<Response<JwtAuthResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Check if user exists
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new Response<JwtAuthResult>("User not found");
            }

            // Try to sign in
            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!signInResult.Succeeded)
            {
                return new Response<JwtAuthResult>("Invalid username or password");
            }

            // TODO: Generate JWT token
            // This will be implemented when we add authentication service
            var result = new JwtAuthResult
            {
                AccessToken = "token_placeholder",
                refreshToken = new RefreshToken
                {
                    UserName = user.UserName!,
                    TokenString = "refresh_token_placeholder",
                    ExpireAt = DateTime.UtcNow.AddDays(30)
                }
            };

            return new Response<JwtAuthResult>(result, "Login successful");
        }
    }
}

