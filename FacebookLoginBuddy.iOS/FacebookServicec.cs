using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Foundation;
using UIKit;
using Facebook.LoginKit;
using Facebook.CoreKit;
using System.Diagnostics;

namespace FacebookLoginLib.iOS
{
	public class FacebookService : IFacebookService
	{
		#region Properties

		public event FacebookLoginSuccessEvent OnLoginSuccess;
		public event FacebookLoginErrorEvent OnLoginError;

		private LoginManager LoginManager { get; set; }

		public bool LoggedIn => null == User;

		public FacebookUser User { get; private set; }

		#endregion //Properties

		#region Methods

		public FacebookService()
		{
		}

		private void OnRequestHandler(GraphRequestConnection connection, NSObject result, NSError error)
		{
			if (error != null || result == null)
			{
				OnLoginError?.Invoke(new Exception(error.LocalizedDescription));
			}
			else
			{
				var id = string.Empty;
				var first_name = string.Empty;
				var email = string.Empty;
				var last_name = string.Empty;
				var url = string.Empty;

				try
				{
					id = result.ValueForKey(new NSString("id"))?.ToString();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				try
				{
					first_name = result.ValueForKey(new NSString("first_name"))?.ToString();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				try
				{
					email = result.ValueForKey(new NSString("email"))?.ToString();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				try
				{
					last_name = result.ValueForKey(new NSString("last_name"))?.ToString();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				try
				{
					url = ((result.ValueForKey(new NSString("picture")) as NSDictionary).ValueForKey(new NSString("data")) as NSDictionary).ValueForKey(new NSString("url")).ToString();
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.Message);
				}

				//Grab the user and send the success event
				User = new FacebookUser(id, AccessToken.CurrentAccessToken.TokenString, first_name, last_name, email, url);
				OnLoginSuccess?.Invoke(User);
			}
		}

		private void OnLoginHandler(LoginManagerLoginResult result, NSError error)
		{
			if (error != null || result == null || result.IsCancelled)
			{
				if (result != null && result.IsCancelled)
				{
					OnLoginError?.Invoke(new Exception("Login Canceled."));
				}

				if (error != null)
				{
					OnLoginError?.Invoke(new Exception(error.LocalizedDescription));
				}
			}
			else
			{
				var request = new GraphRequest("me", new NSDictionary("fields", "id, first_name, email, last_name, picture.width(500).height(500)"));
				request.Start(OnRequestHandler);
			}
		}

		public void Login()
		{
			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}
			if (LoginManager == null)
			{
				LoginManager = new LoginManager();
			}

			LoginManager.LogOut();
			LoginManager.LoginBehavior = LoginBehavior.Browser;
			LoginManager.LogIn(new string[] { "public_profile", "email" }, vc, OnLoginHandler);
		}

		public void Logout()
		{
			User = null;
			LoginManager.LogOut();
		}

		#endregion //Methods
	}
}