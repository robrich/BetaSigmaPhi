namespace BetaSigmaPhi.Web.Controllers {
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web.Mvc;
	using BetaSigmaPhi.Entity;
	using BetaSigmaPhi.Repository;
	using BetaSigmaPhi.Service;
	using BetaSigmaPhi.Web.Filters;
	using BetaSigmaPhi.Web.Models;

	[RequireAdmin]
	public class UserController : Controller {

		private readonly IUserRepository userRepository;
		private readonly ILoginService loginService;

		public UserController(IUserRepository UserRepository, ILoginService LoginService) {
			if (UserRepository == null) {
				throw new ArgumentNullException("UserRepository");
			}
			if (LoginService == null) {
				throw new ArgumentNullException("LoginService");
			}
			this.userRepository = UserRepository;
			this.loginService = LoginService;
		}

		public ActionResult Index(bool Hidden = false) {
			List<User> users;
			if (Hidden) {
				users = this.userRepository.GetAll();
			} else {
				users = this.userRepository.GetActive();
			}
			List<UserModel> models = (
				from u in users
				select this.UserToModel(u)
				).ToList();
			return this.View(models);
		}

		public ActionResult Edit(int id) {
			User user = this.userRepository.GetById(id);
			if (user == null && id > 0) {
				return this.View("NotFound");
			}
			UserModel model = this.UserToModel(user ?? new User());
			return this.View(model);
		}

		[HttpPost]
		public ActionResult Edit(int id, UserModel Model) {
			if (Model == null) {
				return this.RedirectToAction("Edit", new {id});
			}
			if (this.ModelState.IsValid && id < 1) {
				if (string.IsNullOrEmpty(Model.Email)) {
					this.ModelState.AddModelError("Email", "Email is required to create a new user");
				}
				if (string.IsNullOrEmpty(Model.Password)) {
					this.ModelState.AddModelError("Password", "Password is required for new users");
				}
			}
			if (!this.userRepository.EmailAvailable(id, Model.Email)) {
				this.ModelState.AddModelError("Email", "Email is in use, please choose a new username");
			}
			// TODO: Validate that the password is complex enough

			if (this.ModelState.IsValid) {
				User user = this.userRepository.GetById(id);
				if (user == null) {
					if (id > 0) {
						return this.View("NotFound");
					}
					user = new User {
						Email = Model.Email,
						Salt = "notnull",
						Password = "notnull"
					};
				}
				this.UserFromModel(user, Model);
				this.userRepository.Save(user);
				this.loginService.SaveEmailPasswordChanges(user, Model.Email, Model.Password);
				return this.RedirectToAction("Index"); // Success
			}
			return this.View(Model); // Fix your errors
		}

		public ActionResult Delete(int id) {
			User user = this.userRepository.GetById(id);
			if (user == null) {
				return this.View("NotFound");
			}
			user.IsActive = false;
			this.userRepository.Save(user);
			return this.RedirectToAction("Index");
		}

		private UserModel UserToModel(User User) {
			UserModel model = new UserModel {
				UserId = User.UserId,
				Email = User.Email,
				FirstName = User.FirstName,
				LastName = User.LastName,
				IsAdmin = User.IsAdmin,
				IsActive = User.IsActive,
			};
			return model;
		}

		private void UserFromModel(User User, UserModel Model) {
			User.FirstName = Model.FirstName;
			User.LastName = Model.LastName;
			User.IsAdmin = Model.IsAdmin;
			User.IsActive = Model.IsActive;
			// Specifically don't set Email and Password -- those are set differently
		}

	}
}
