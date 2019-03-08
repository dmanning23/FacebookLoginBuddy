using Android.OS;
using Org.Json;
using Plugin.CurrentActivity;
using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace FacebookLoginLib.Android
{
	public class FacebookService : Java.Lang.Object, IFacebookService, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
	{
		#region Properties

		public ICallbackManager CallbackManager;

		public bool LoggedIn { get; private set; }

		public FacebookUser User { get; private set; }

		public event FacebookLoginSuccessEvent OnLoginSuccess;
		public event FacebookLoginErrorEvent OnLoginError;

		#endregion //Properties

		#region Methods

		public FacebookService()
		{
			CallbackManager = CallbackManagerFactory.Create();
			LoginManager.Instance.RegisterCallback(CallbackManager, this);
		}

		public void Login()
		{
			LoginManager.Instance.SetLoginBehavior(LoginBehavior.NativeWithFallback);
			LoginManager.Instance.LogInWithReadPermissions(CrossCurrentActivity.Current.Activity, new List<string> { "public_profile", "email" });
		}

		public void Logout()
		{
			LoggedIn = false;
			User = null;
			LoginManager.Instance.LogOut();
		}

		#region IFacebookCallback

		public void OnCancel()
		{
			OnLoginError?.Invoke(new System.Exception("Login Canceled."));
		}

		public void OnError(FacebookException error)
		{
			OnLoginError?.Invoke(new System.Exception(error.LocalizedMessage));
		}

		public void OnSuccess(Java.Lang.Object result)
		{
			var n = result as LoginResult;
			if (n != null)
			{
				var request = GraphRequest.NewMeRequest(n.AccessToken, this);
				var bundle = new Bundle();
				bundle.PutString("fields", "id, first_name, email, last_name, picture.width(500).height(500)");
				request.Parameters = bundle;
				request.ExecuteAsync();
			}
		}

		#endregion

		#region IGraphJSONObjectCallback

		public void OnCompleted(JSONObject p0, GraphResponse p1)
		{
			var id = string.Empty;
			var first_name = string.Empty;
			var email = string.Empty;
			var last_name = string.Empty;
			var pictureUrl = string.Empty;

			if (p0.Has("id"))
				id = p0.GetString("id");

			if (p0.Has("first_name"))
				first_name = p0.GetString("first_name");

			if (p0.Has("email"))
				email = p0.GetString("email");

			if (p0.Has("last_name"))
				last_name = p0.GetString("last_name");

			if (p0.Has("picture"))
			{
				var p2 = p0.GetJSONObject("picture");
				if (p2.Has("data"))
				{
					var p3 = p2.GetJSONObject("data");
					if (p3.Has("url"))
					{
						pictureUrl = p3.GetString("url");
					}
				}
			}

			LoggedIn = true;
			User = new FacebookUser(id, AccessToken.CurrentAccessToken.Token, first_name, last_name, email, pictureUrl);
			OnLoginSuccess?.Invoke(User);
		}

		#endregion

		#endregion //Methods
	}
}
