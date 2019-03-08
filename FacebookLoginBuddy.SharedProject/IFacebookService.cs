using System;
using System.Collections.Generic;
using System.Text;

namespace FacebookLoginLib
{
	public interface IFacebookService
	{
		event FacebookLoginSuccessEvent OnLoginSuccess;

		event FacebookLoginErrorEvent OnLoginError;

		bool LoggedIn { get; }

		FacebookUser User { get; }

		void Login();

		void Logout();
	}
}
