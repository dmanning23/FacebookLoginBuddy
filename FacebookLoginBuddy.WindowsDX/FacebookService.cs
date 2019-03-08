using System;

namespace FacebookLoginLib
{
	public class FacebookService : IFacebookService
	{
		public bool LoggedIn => false;

		public FacebookUser User => null;

		public event FacebookLoginSuccessEvent OnLoginSuccess;
		public event FacebookLoginErrorEvent OnLoginError;

		public void Login()
		{
			OnLoginError?.Invoke(new NotImplementedException());
		}

		public void Logout()
		{
			OnLoginError?.Invoke(new NotImplementedException());
		}
	}
}
