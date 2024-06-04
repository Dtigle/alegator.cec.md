using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CEC.SRV.BLL;
using CEC.SRV.BLL.Dto;
using CEC.Web.SRV.Infrastructure.Export;
using CEC.Web.SRV.Models.Account;
using Microsoft.AspNet.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.Web.SRV.Controllers;
using Moq;
using Lib.Web.Mvc.JQuery.JqGrid;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using Amdaris.Domain.Paging;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Infrastructure;
using NHibernate;
using System.Reflection;
using System.Security.Principal;
using System.Web.Routing;
using Amdaris.NHibernateProvider.Identity;

namespace CEC.Web.SRV.Tests.Controllers
{
    [TestClass]
    public class AccountControllerTests : BaseControllerTests
    {
        private static Mock<UserManager<SRVIdentityUser>> _userManager;
        private static Mock<ISRVUserBll> _bll;
        private static Mock<ISessionFactory> _sessionFactory;
        private static Mock<IRepository> _repository;
        private static AccountController _controller;
        
        [TestInitialize]
        public void Startup()
        {
            var identityMock = new Mock<IIdentity>();
           
            var userMock = new Mock<IPrincipal>();
            userMock.Setup(p => p.Identity).Returns(identityMock.Object);
            
            var mockHttpContext = new Mock<HttpContextBase>();
            mockHttpContext.SetupGet(x => x.User).Returns(userMock.Object);
           
            _sessionFactory = new Mock<ISessionFactory>();
            _repository = new Mock<IRepository>();
            _bll = new Mock<ISRVUserBll>();

            _controller = new AccountController(_sessionFactory.Object, _repository.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                    Controller = _controller,
                    RequestContext = new RequestContext
                    {
                        HttpContext = mockHttpContext.Object
                    }
                }
            };

            _userManager = new Mock<UserManager<SRVIdentityUser>>(new UserRepository<SRVIdentityUser>(_sessionFactory.Object));

            var fieldInfo = _controller.GetType().GetField("_userBll", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null) fieldInfo.SetValue(_controller, _bll.Object);
            
            _controller.GetType().GetProperty("UserManager").SetValue(_controller, _userManager.Object);

            BaseController = _controller;
        }

        [TestMethod]
        public void Login_returns_correct_view()
        {
            // Arrange

            const string url = "url";

            // Act
            var result = _controller.Login(url);
          
            // Assert

            Assert.IsNotNull(result);
        }
        
        [TestMethod]
        public void Login_returns_correct_url()
        {
            // Arrange

            const string url = "url";

            // Act
            var result = _controller.Login(url);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));

            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.ViewBag);
            Assert.IsNotNull(viewResult.ViewBag.ReturnUrl);
            Assert.AreEqual(viewResult.ViewBag.ReturnUrl, url);
        }

        [TestMethod]
        public void LoginByNonValidModel_returns_correct_model()
        {
            // Arrange

            const string url = "url";

            var model = GetLoginViewModel();
            _controller.ModelState.AddModelError("", "Error");
            
            // Act
            var result = _controller.Login(model, url);

            // Assert
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void LoginByValidModelAndNullUser_has_errors()
        {
            // Arrange
            
            const string url = "url";

            var model = GetLoginViewModel();
          
            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(null);
           
            // Pre Assert
            
            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act
           
            var result = _controller.Login(model, url);

            // Assert
            
            Assert.IsNotNull(result);
            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
        }

        [TestMethod]
        public void LoginByValidModelAndBlockedUser_returns_correct_model()
        {
            // Arrange

            const string url = "url";

            var model = GetLoginViewModel();

            var user = GetSrvIdentityUser();
            user.Block();
            
            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(user);

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.Login(model, url);

            // Assert

            Assert.IsTrue(user.IsBlocked);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void LoginByValidModelAndActiveUserWithoutRoles_returns_correct_model()
        {
            // Arrange

            const string url = "url";

            var model = GetLoginViewModel();

            var user = GetSrvIdentityUser();
            user.UnBlock();
            user.Roles.Clear();

            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(user);

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.Login(model, url);

            // Assert

            Assert.IsFalse(user.IsBlocked);
            Assert.AreEqual(0, user.Roles.Count);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void LoginByValidModelAndActiveUserWithRolesAndIncorrectPassword_returns_correct_model()
        {
            // Arrange

            const string url = "url";

            var model = GetLoginViewModel();

            var user = GetSrvIdentityUser();
            user.UnBlock();
            
            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(user);
            _userManager.Setup(x => x.CheckPasswordAsync(user, model.Password)).Returns(Task.FromResult(false));
            _repository.Setup(x => x.SaveOrUpdate(user));

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.Login(model, url);

            // Assert

            Assert.IsFalse(user.IsBlocked);
            Assert.AreNotEqual(0, user.Roles.Count);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            _repository.Verify(x => x.SaveOrUpdate(user), Times.Once);

            var viewResult = result.Result as ViewResult;

            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void Register_returns_correct_view()
        {
            // Act
            var result = _controller.Register();

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void RegisterByNonValidModel_returns_correct_model()
        {
            // Arrange

            var model = GetRegisterViewModel();
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = _controller.Register(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void RegisterByValidModelAndNotValidUser_returns_correct_model()
        {
            // Arrange
            
            var model = GetRegisterViewModel();

            var user = GetSrvIdentityUser();

            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(user);

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.Register(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["UserName"].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void RegisterByValidModelAndValidUserWithNotValidInfo_returns_correct_model()
        {
            // Arrange

            var model = GetRegisterViewModel();

            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(null);
            _userManager.Setup(x => x.CreateAsync(It.IsAny<SRVIdentityUser>(), model.Password)).ReturnsAsync(new IdentityResult(new List<string> { "Error" }));

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.Register(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void ChangePassword_returns_correct_view()
        {
            // Act
            var result = _controller.ChangePassword();

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ChangePassword_returns_correct_model()
        {
            // Act
            var result = _controller.ChangePassword();

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = result as PartialViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(ChangePasswordViewModel));
        }

        [TestMethod]
        public void ChangePasswordByNonValidModel_returns_correct_model()
        {
            // Arrange

            var model = GetChangePasswordViewModel();
            _controller.ModelState.AddModelError("", "Error");

            // Act
            
            var result = _controller.ChangePassword(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as PartialViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void ChangePasswordByValidModelWithNotValidInfo_returns_correct_model()
        {
            // Arrange

            var model = GetChangePasswordViewModel();

            _userManager.Setup(x => x.ChangePasswordAsync(It.IsAny<string>(), model.OldPassword, model.NewPassword)).ReturnsAsync(new IdentityResult(new List<string> { "Error" }));


            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.ChangePassword(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as PartialViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["OldPassword"].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }
        
        [TestMethod]
        public void ResetPassword_returns_correct_view()
        {
            // Arrange

            const string userId = "admin";

            // Act
            var result = _controller.ResetPassword(userId);

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void ResetPassword_returns_correct_model()
        {
            // Arrange

            const string userId = "admin";

            // Act
            var result = _controller.ResetPassword(userId);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = result as PartialViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.IsInstanceOfType(viewResult.Model, typeof(ResetPasswordViewModel));

            var model = viewResult.Model as ResetPasswordViewModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.UserId, userId);
        }

        [TestMethod]
        public void ResetPasswordByNonValidModel_returns_correct_model()
        {
            // Arrange

            var model = GetResetPasswordViewModel();
            _controller.ModelState.AddModelError("", "Error");

            // Act
            var result = _controller.ResetPassword(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as PartialViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void ResetPasswordByValidModelAndNotValidUser_returns_correct_model()
        {
            // Arrange

            var model = GetResetPasswordViewModel();

            _userManager.Setup(x => x.FindByIdAsync(model.UserId)).ReturnsAsync(null);

            // Pre Assert

            Assert.IsTrue(_controller.ModelState.IsValid);

            // Act

            var result = _controller.ResetPassword(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as PartialViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }
   
        [TestMethod]
        public void Users_returns_correct_view()
        {
            // Act

            var result = _controller.Users();

            // Assert

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void CreateUser_returns_correct_view_model()
        {
            // Arrange

            var genders = GetGenders();
            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable());

            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

            // Act

            var result = _controller.CreateUser() as ViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
        }

        [TestMethod]
        public void CreateUser_returns_correct_view_data()
        {
            // Arrange

            var genders = GetGenders();
            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable());

            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());
            
            // Act

            var result = _controller.CreateUser() as ViewResult;

            // Assert
            
            AssertViewData(result, "GenderId", x => x.Name, x => x.Id.ToString(), genders);
            AssertViewData(result, "RoleId", x => x.Name, x => x.Id, roles);
        }

        [TestMethod]
        public void CreateUserByModel_returns_correct_view_data()
        {
            // Arrange

            var genders = GetGenders();
            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable());

            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

            var model = GetCreateUserViewModel();

            var user = GetSrvIdentityUser();

            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(user);
            _repository.Setup(x => x.Get<IdentityRole, string>(model.RoleId)).Returns(GetIdentityRole());

            // Act

            var result = _controller.CreateUser(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            AssertViewData(result.Result as ViewResult, "GenderId", x => x.Name, x => x.Id.ToString(), genders);
            AssertViewData(result.Result as ViewResult, "RoleId", x => x.Name, x => x.Id, roles);
        }

        [TestMethod]
        public void CreateUserByModelAndNotValidUser_returns_correct_view_model()
        {
            // Arrange

            var genders = GetGenders();
            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable());

            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

            var model = GetCreateUserViewModel();

            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(GetSrvIdentityUser());
            _repository.Setup(x => x.Get<IdentityRole, string>(model.RoleId)).Returns(GetIdentityRole());

            // Act

            var result = _controller.CreateUser(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["UserName"].Errors.Count > 0);

            Assert.IsNotNull(viewResult);
            Assert.AreSame(model, viewResult.Model);
        }

        [TestMethod]
        public void CreateUserByModelAndValidUserWithNullRole_returns_correct_view_model()
        {
            // Arrange

            var genders = GetGenders();
            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable());

            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

            var model = GetCreateUserViewModel();
            
            _userManager.Setup(x => x.FindByNameAsync(model.UserName)).ReturnsAsync(null);
            _repository.Setup(x => x.Get<IdentityRole, string>(model.RoleId)).Returns((IdentityRole)null);

            // Act

            var result = _controller.CreateUser(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Result);

            var viewResult = result.Result as ViewResult;

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState["RoleId"].Errors.Count > 0);

            Assert.IsNotNull(viewResult);
            Assert.AreSame(model, viewResult.Model);
        }
        
        [TestMethod]
        public void ListUsersAjax_returns_correct_format()
        {
            // Arrange

            var request = new JqGridRequest { PageIndex = 0, RecordsCount = 20 };

            _bll.Setup(x => x.ListUsers(It.IsAny<PageRequest>()))
                .Returns(new PageResponse<SRVIdentityUser> { Items = new List<SRVIdentityUser>(), PageSize = 20, StartIndex = 1, Total = 2 });

            // Act

            var result = _controller.ListUsersAjax(request);

            // Assert

            _bll.Verify(x => x.ListUsers(It.IsAny<PageRequest>()), Times.Once());
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(JqGridJsonResult));
        }

       [TestMethod]
        public void UpdateUser_Active_returns_correct_view_model()
        {
            // Arrange

           const string userId = "user";

           var user = GetSrvIdentityUser();
           user.UnBlock();
           _bll.Setup(x => x.Get<SRVIdentityUser, string>(userId)).Returns(user);
           
            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

            // Act

            var result = _controller.UpdateUser(userId) as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
           Assert.IsInstanceOfType(result.Model, typeof(UpdateUserModel));

           var model = result.Model as UpdateUserModel;
           Assert.IsNotNull(model);
           Assert.AreEqual(user.Id, model.Id);
           Assert.AreEqual(user.Comments, model.Comments);
           Assert.AreEqual(Convert.ToInt64(user.Roles.Select(x => x.Id).FirstOrDefault()), model.RoleId);
           
           Assert.IsFalse(user.IsBlocked);
           Assert.AreEqual(model.StatusId, Convert.ToInt64(AccountStatus.Active));
        }

       [TestMethod]
       public void UpdateUser_Blocked_returns_correct_view_model()
       {
           // Arrange

           const string userId = "user";

           var user = GetSrvIdentityUser();
           user.Block();
           _bll.Setup(x => x.Get<SRVIdentityUser, string>(userId)).Returns(user);

           var roles = GetIdentityRoles();
           _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

           // Act

           var result = _controller.UpdateUser(userId) as PartialViewResult;

           // Assert

           Assert.IsNotNull(result);
           Assert.IsNotNull(result.Model);
           Assert.IsInstanceOfType(result.Model, typeof(UpdateUserModel));

           var model = result.Model as UpdateUserModel;
           Assert.IsNotNull(model);
           Assert.AreEqual(user.Id, model.Id);
           Assert.AreEqual(user.Comments, model.Comments);
           Assert.AreEqual(Convert.ToInt64(user.Roles.Select(x => x.Id).FirstOrDefault()), model.RoleId);

           Assert.IsTrue(user.IsBlocked);
           Assert.AreEqual(model.StatusId, Convert.ToInt64(AccountStatus.Blocked));
       }
       
       [TestMethod]
       public void UpdateUser_returns_correct_view_data()
       {
           // Arrange

           const string userId = "user";

           var user = GetSrvIdentityUser();
           user.UnBlock();
           _bll.Setup(x => x.Get<SRVIdentityUser, string>(userId)).Returns(user);

           var roles = GetIdentityRoles();
           _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());
           
           // Act

           var result = _controller.UpdateUser(userId) as PartialViewResult;

           // Assert

           AssertViewData(result, "StatusId", x => x.GetEnumDescription(), x => ((int)x).ToString(), 
               Enum.GetValues(typeof(AccountStatus)).Cast<AccountStatus>().ToList());
           AssertViewData(result, "RoleId", x => x.Name, x => x.Id, roles);
       }

       [TestMethod]
       public void UpdateUserByModel_returns_correct_view_data()
       {
           // Arrange

           var model = GetUpdateUserModel();

           var roles = GetIdentityRoles();
           _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

           _bll.Setup(x => x.UpdateAccount(model.Id, It.IsAny<string>(), It.IsAny<bool>(), model.Comments)).Throws(new Exception());
          
           // Act

           var result = _controller.UpdateUser(model) as PartialViewResult;

           // Assert

           AssertViewData(result, "StatusId", x => x.GetEnumDescription(), x => ((int)x).ToString(), Enum.GetValues(typeof(AccountStatus)).Cast<AccountStatus>().ToList());
           AssertViewData(result, "RoleId", x => x.Name, x => x.Id, roles);
       }

       [TestMethod]
       public void UpdateUserByModel_returns_correct_view_model()
       {
           // Arrange

           var model = GetUpdateUserModel();

           var roles = GetIdentityRoles();
           _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

           _bll.Setup(x => x.UpdateAccount(model.Id, It.IsAny<string>(), It.IsAny<bool>(), model.Comments)).Throws(new Exception());

           // Act

           var result = _controller.UpdateUser(model) as PartialViewResult;

           // Assert

           Assert.IsNotNull(result);
           Assert.IsNotNull(result.Model);
           Assert.AreEqual(model, result.Model);
       }

       [TestMethod]
       public void UpdateUserByModel_returns_correct_view_content()
       {
           // Arrange

           var model = GetUpdateUserModel();

           var roles = GetIdentityRoles();
           _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

           _bll.Setup(x => x.UpdateAccount(model.Id, It.IsAny<string>(), It.IsAny<bool>(), model.Comments));

           // Act

           var result = _controller.UpdateUser(model) as ContentResult;

           // Assert

           Assert.IsNotNull(result);
           
           Assert.AreEqual(result.Content, Const.CloseWindowContent);
       }

       [TestMethod]
       public void UpdateAccounts_returns_correct_view_model()
       {
           // Act

           var result = _controller.UpdateAccounts() as PartialViewResult;

           // Assert

           Assert.IsNotNull(result);
           Assert.IsNotNull(result.Model);
           Assert.IsInstanceOfType(result.Model, typeof(MultiSelectAccountEdit));
       }

       [TestMethod]
       public void UpdateAccounts_returns_correct_view_data()
       {
          // Act

           var result = _controller.UpdateAccounts() as PartialViewResult;

           // Assert

           AssertViewData(result, "Status", x => x.GetEnumDescription(), x => ((int)x).ToString(), Enum.GetValues(typeof(AccountStatus)).Cast<AccountStatus>().ToList());
       }

       [TestMethod]
       public void UpdateAccountsByZeroStatus_returns_correct_view_model()
       {
           // Arrange

           var model = GetMultiSelectAccountEdit();
           model.Status = 0;

           // Act

           var result = _controller.UpdateAccounts(model);

           // Assert
           
           Assert.IsFalse(_controller.ModelState.IsValid);
           Assert.IsTrue(_controller.ModelState["Status"].Errors.Count > 0);

           Assert.IsNotNull(result);
           Assert.IsInstanceOfType(result, typeof(PartialViewResult));

           var viewResult = result as PartialViewResult;

           Assert.IsNotNull(viewResult);
           Assert.IsNotNull(viewResult.Model);
           Assert.IsInstanceOfType(viewResult.Model, typeof(MultiSelectAccountEdit));

           var viewModel = viewResult.Model as MultiSelectAccountEdit;

           Assert.IsNotNull(viewModel);
           Assert.AreEqual((int)viewModel.Status, 0);
           Assert.AreSame(model, viewModel);
       }

       [TestMethod]
       public void UpdateAccountsByNotValidModelAndNotZeroStatus_returns_correct_view_model()
       {
           // Arrange

           var model = GetMultiSelectAccountEdit();
           _controller.ModelState.AddModelError("", "Error");
           
           // Act

           var result = _controller.UpdateAccounts(model);

           // Assert

           Assert.IsFalse(_controller.ModelState.IsValid);
           Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);

           Assert.IsNotNull(result);
           Assert.IsInstanceOfType(result, typeof(PartialViewResult));

           var viewResult = result as PartialViewResult;

           Assert.IsNotNull(viewResult);
           Assert.IsNotNull(viewResult.Model);
           Assert.IsInstanceOfType(viewResult.Model, typeof(MultiSelectAccountEdit));

           var viewModel = viewResult.Model as MultiSelectAccountEdit;

           Assert.IsNotNull(viewModel);
           Assert.AreNotEqual((int)viewModel.Status, 0);
           Assert.AreSame(model, viewModel);
       }

       [TestMethod]
       public void UpdateAccountsByValidModelAndNotZeroStatus_returns_correct_content()
       {
           // Arrange

           var model = GetMultiSelectAccountEdit();
           _bll.Setup(x => x.ChangeStatus(model.Accounts, model.Status == AccountStatus.Blocked, model.Comments));

           // Act

           var result = _controller.UpdateAccounts(model);

           // Assert

           Assert.IsNotNull(result);
           Assert.IsInstanceOfType(result, typeof(ContentResult));
           Assert.IsTrue(_controller.ViewData.ModelState.IsValid);

           _bll.Verify(x => x.ChangeStatus(model.Accounts, model.Status == AccountStatus.Blocked, model.Comments), Times.Once);

           Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
       }

        [TestMethod]
        public void DeleteUser_returns_correct_content()
        {
            // Arrange

            const string userId = "user";
            _bll.Setup(x => x.DeleteAccount(userId));

            // Act

            var result = _controller.DeleteUser(userId);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ContentResult));
           
            _bll.Verify(x => x.DeleteAccount(userId), Times.Once);

            Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
        }

        [TestMethod]
        public void SetRegions_returns_correct_view_model()
        {
            // Arrange

            const string userId = "user";

            var user = GetSrvIdentityUser();

            _bll.Setup(x => x.GetRegistratorUser(userId)).Returns(user);

            // Act

			var result = _controller.SetRegionsForUser(userId) as PartialViewResult;
            
            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(AllocateRegionModel));
            
            var model = result.Model as AllocateRegionModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(userId, model.UserId);
            Assert.IsNotNull(user.Regions);
            AssertViewModel(result, model.AllocatedRegions, x => x.GetFullName(), x => x.Id.ToString(), user.Regions.ToList());
        }

        [TestMethod]
        public void SetRegions_returns_correct_content()
        {
            // Arrange

            var model = GetAllocateRegionModel();
            _bll.Setup(x => x.SetUserRegions(model.UserId, It.IsAny<IEnumerable<long>>()));

            // Act

            var result = _controller.SetRegions(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ContentResult));
            _bll.Verify(x => x.SetUserRegions(model.UserId, It.IsAny<IEnumerable<long>>()), Times.Once);
            Assert.AreEqual(((ContentResult)result).Content, Const.CloseWindowContent);
        }

        [TestMethod]
        public void SetRegions_returns_correct_model()
        {
            // Arrange

            var model = GetAllocateRegionModel();
            _bll.Setup(x => x.SetUserRegions(model.UserId, It.IsAny<IEnumerable<long>>())).Throws(new Exception());

            // Act

            var result = _controller.SetRegions(model);

            // Assert

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));

            var viewResult = result as PartialViewResult;

            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            Assert.IsNotNull(viewResult);
            Assert.IsNotNull(viewResult.Model);
            Assert.AreSame(model, viewResult.Model);
        }
        
        [TestMethod]
        public void SelectRoles_returns_correct_view_model()
        {
            // Arrange

            var roles = GetIdentityRoles();
            _repository.Setup(x => x.Query<IdentityRole>()).Returns(roles.AsQueryable());

            // Act

            var result = _controller.SelectRoles() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);

            AssertViewModel(result, result.Model, x => x.Name, x => x.Id, roles);
        }
        
        [TestMethod]
        public void AccountStatuses_returns_correct_model()
        {
            // Act

            var result = _controller.AccountStatuses() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            AssertViewModel(result, result.Model, x => x.GetEnumDescription(), x => ((int)x).ToString(), Enum.GetValues(typeof(AccountStatus)).Cast<AccountStatus>().ToList());
        }

        [TestMethod]
        public void AccountStatusesForSearch_returns_correct_model()
        {
            // Act

            var result = _controller.AccountStatusesForSearch() as PartialViewResult;

            // Assert

            Assert.IsNotNull(result);
            AssertViewModel(result, result.Model, x => x.GetEnumDescription(), x => x.GetFilterValue().ToString(), Enum.GetValues(typeof(AccountStatus)).Cast<AccountStatus>().ToList());
        }

        [TestMethod]
        public void GetUserProfile_returns_correct_model()
        {
            // Arrange

            const string userId = "user";
            var profile = GetUserProfileDto();
            _bll.Setup(x => x.GetUserProfile(userId)).Returns(profile);
            
            // Act

            var model = _controller.GetUserProfile(userId);

            // Assert

            Assert.IsNotNull(model);
            Assert.AreEqual(model.UserId, profile.Id);
            Assert.AreEqual(model.LoginName, profile.LoginName);
            Assert.AreEqual(model.LastLogin, profile.LastLogin);
            Assert.AreEqual(model.LoginCreationDate, profile.LoginCreationDate);
            Assert.AreEqual(model.FirstName, profile.FirstName);
            Assert.AreEqual(model.LastName, profile.LastName);
            Assert.AreEqual(model.DateOfBirth, profile.DateOfBirth);
            Assert.AreEqual(model.Email, profile.Email);
            Assert.AreEqual(model.LandlinePhone, profile.LandlinePhone);
            Assert.AreEqual(model.MobilePhone, profile.MobilePhone);
            Assert.AreEqual(model.WorkInfo, profile.WorkInfo);
            Assert.AreEqual(model.IsCurrentUser, _controller.User.Identity.GetUserId() == userId);
            Assert.AreEqual(model.GenderId, profile.Gender.Id);
        }

        [TestMethod]
        public void ViewProfile_returns_correct_view_model()
        {
            // Arrange

            const string userId = "user";
            
            var profile = GetUserProfileDto();
            _bll.Setup(x => x.GetUserProfile(userId)).Returns(profile);

            var expectedModel = GetUserProfileModel(profile);

            // Act

            var result = _controller.ViewProfile(userId) as ViewResult;

            // Assert

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOfType(result.Model, typeof(UserProfileModel));

            var model = result.Model as UserProfileModel;

            Assert.IsNotNull(model);
            Assert.AreEqual(model.UserId, expectedModel.UserId);
            Assert.AreEqual(model.LoginName, expectedModel.LoginName);
            Assert.AreEqual(model.LastLogin, expectedModel.LastLogin);
            Assert.AreEqual(model.LoginCreationDate, expectedModel.LoginCreationDate);
            Assert.AreEqual(model.FirstName, expectedModel.FirstName);
            Assert.AreEqual(model.LastName, expectedModel.LastName);
            Assert.AreEqual(model.DateOfBirth, expectedModel.DateOfBirth);
            Assert.AreEqual(model.Email, expectedModel.Email);
            Assert.AreEqual(model.LandlinePhone, expectedModel.LandlinePhone);
            Assert.AreEqual(model.MobilePhone, expectedModel.MobilePhone);
            Assert.AreEqual(model.WorkInfo, expectedModel.WorkInfo);
            Assert.AreEqual(model.GenderId, expectedModel.GenderId);
        }

        [TestMethod]
        public void ViewProfile_returns_correct_view_data()
        {
            // Arrange

            const string userId = "user";

            var profile = GetUserProfileDto();
            _bll.Setup(x => x.GetUserProfile(userId)).Returns(profile);

            var genders = GetGenders();

            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable);

            // Act

            var result = _controller.ViewProfile(userId) as ViewResult;

            // Assert

            AssertViewData(result, "GenderId", x => x.Name, x => x.Id.ToString(), genders);
        }

        [TestMethod]
        public void UpdateProfileByNotValidModel_returns_correct_view_data()
        {
            // Arrange

            var model = GetUserProfileModel(GetUserProfileDto());

            var genders = GetGenders();
            _repository.Setup(x => x.Query<Gender>()).Returns(genders.AsQueryable);

            _controller.ModelState.AddModelError("", "Error");

            // Act

            var result = _controller.UpdateProfile(model) as ViewResult;

            // Assert

            Assert.IsFalse(_controller.ModelState.IsValid);
            Assert.IsTrue(_controller.ModelState[""].Errors.Count > 0);
            AssertViewData(result, "GenderId", x => x.Name, x => x.Id.ToString(), genders);
        }

        [TestMethod]
        public void ExportUsersAllData_has_correct_logic()
        {
            ExportGridDataTest<UserGridModel>(ExportType.AllData, "Users");
        }

        [TestMethod]
        public void ExportUsersCurrentPage_has_correct_logic()
        {
            ExportGridDataTest<UserGridModel>(ExportType.CurrentPage, "Users");
        }

        private static void AssertViewData<TView, T>(TView view, string viewDataKey, Func<T, string> textFunc, Func<T, string> idFunc, List<T> expectedViewData) where TView : ViewResultBase
        {
            AssertViewModel(view, view.ViewData[viewDataKey], textFunc, idFunc, expectedViewData);
        }

        private static void AssertViewModel<TView, T>(TView view, object viewModel, Func<T, string> textFunc, Func<T, string> idFunc, List<T> expectedModel) where TView : ViewResultBase
        {
            Assert.IsNotNull(view);
            Assert.IsNotNull(viewModel);
            Assert.IsInstanceOfType(viewModel, typeof(List<SelectListItem>));

            var model = viewModel as List<SelectListItem>;

            AssertListsAreEqual(model, expectedModel, textFunc, idFunc);
        }
        
        private static void AssertListsAreEqual<T>(IEnumerable<SelectListItem> list1, List<T> list2, Func<T, string> textFunc, Func<T, string> valueFunc)
        {
            Assert.AreEqual(list1.Count(), list2.Count);
            Assert.IsTrue(list1.All(item => list2.Exists(x => string.Equals(textFunc(x), item.Text) && string.Equals(valueFunc(x), item.Value))));
        }
        
        private static List<Gender> GetGenders()
        {
            return new List<Gender>
            {
                GetGender0(),
                GetGender1()
            };
        }

        private static Gender GetGender0()
        {
            return new Gender
            {
                Description = "Masculin",
                Name = "M",
                Created = new DateTime(2003, 11, 11),
                CreatedBy = GetSrvIdentityUser()
            };
        }

        private static Gender GetGender1()
        {
            return new Gender
            {
                Description = "Feminin",
                Name = "F"
            };
        }

        private static List<IdentityRole> GetIdentityRoles()
        {
            return new List<IdentityRole>
            {
                GetIdentityRole()
            };
        }

        private static IdentityRole GetIdentityRole()
        {
            return new IdentityRole
            {
                Name = "role0"
             };
        }

        private static LoginViewModel GetLoginViewModel()
        {
            return new LoginViewModel
            {
                Password = "pass",
                UserName = "user"
            };
        }

        private static SRVIdentityUser GetSrvIdentityUser()
        {
            var user = new SRVIdentityUser
            {
                Comments = "Comentarii"
            };
            user.SetId("user");
            user.Roles.Clear();
            GetIdentityRoles().ForEach(x => user.Roles.Add(x));
            GetRegions().ForEach(user.AddRegion);
            return user;
        }

        private static MultiSelectAccountEdit GetMultiSelectAccountEdit()
        {
            return new MultiSelectAccountEdit
            {
                Accounts = new List<string> {"account1", "account2"},
                Comments = "comentarii",
                Status = AccountStatus.Active
            };
        }

        private static List<Region> GetRegions()
        {
            return new List<Region>
            {
                GetRegion0(),
                GetRegion1()
            };
        }

        private static Region GetRegion0()
        {
            return new Region(
                new RegionType
                {
                    Description = "oras mare care are in subordine alte orase",
                    Name = "municipiu",
                    Rank = 1
                })
            {
                Circumscription = 1,
                Description = "capitala RM",
                GeoLocation = new GeoLocation { Latitude = 10, Longitude = 30 },
                HasStreets = true,
                Name = "Chisinau"
            };
        }

        private static Region GetRegion1()
        {
            return new Region(
                new RegionType
                {
                    Description = "oras mare care are in subordine alte orase",
                    Name = "municipiu",
                    Rank = 1
                })
            {
                Circumscription = 2,
                Description = "capitala de nord a RM",
                GeoLocation = new GeoLocation { Latitude = 30, Longitude = 10 },
                HasStreets = true,
                Name = "Balti"
            };
        }

        private static AllocateRegionModel GetAllocateRegionModel()
        {
            return new AllocateRegionModel
            {
                UserId = "user",
                AllocatedRegions = GetRegions().Select(x => new SelectListItem { Text = x.GetFullName(), Value = x.Id.ToString() }).ToList()
            };
        }

        private static UserProfileDto GetUserProfileDto()
        {
            var profile = new UserProfileDto
            {
                DateOfBirth = new DateTime(1983, 11, 11),
                Email = "a@a.am",
                FirstName = "Alex",
                LastName = "Ionescu",
                Gender = GetGender0(),
                Id = "user",
                LandlinePhone = "12222343",
                LastLogin = DateTime.Now,
                LoginCreationDate = DateTime.Now,
                LoginName = "user",
                MobilePhone = "0691234455",
                WorkInfo = "info"
            };
            
            return profile;
        }

        private static UserProfileModel GetUserProfileModel(UserProfileDto profile)
        {
            return new UserProfileModel
            {
                DateOfBirth = profile.DateOfBirth,
                Email = profile.Email,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                GenderId = profile.Gender.Id,
                LandlinePhone = profile.LandlinePhone,
                LastLogin = profile.LastLogin.HasValue ? profile.LastLogin.Value.DateTime : (DateTime?) null,
                LoginCreationDate = profile.LoginCreationDate.DateTime,
                LoginName = profile.LoginName,
                MobilePhone = profile.MobilePhone,
                UserId = profile.Id,
                WorkInfo = profile.WorkInfo
            };
        }

        private static RegisterViewModel GetRegisterViewModel()
        {
            return new RegisterViewModel
            {
                Password = "pass",
                ConfirmPassword = "pass",
                DisplayName = "user",
                UserName = "user",
                PreferredEmail = "a@a.am"
            };
        }

        private static ChangePasswordViewModel GetChangePasswordViewModel()
        {
            return new ChangePasswordViewModel
            {
                ConfirmPassword = "pass",
                NewPassword = "pass",
                OldPassword = "pass"
            };
        }

        private static ResetPasswordViewModel GetResetPasswordViewModel()
        {
            return new ResetPasswordViewModel
            {
                ConfirmPassword = "pass",
                NewPassword = "pass",
                UserId = "user"
            };
        }

        private static CreateUserViewModel GetCreateUserViewModel()
        {
            return new CreateUserViewModel
            {
                Comments = "Comments",
                ConfirmPassword = "pass",
                Email = "a@a.am",
                FirstName = "firstname",
                LastName = "lastname",
                GenderId = 1,
                Password = "pass",
                UserName = "username",
                RoleId = "roleId"
            };
        }

        private static UpdateUserModel GetUpdateUserModel()
        {
            return new UpdateUserModel
            {
                Comments = "comments",
                Id = "id",
                RoleId = 1,
                StatusId = 1
            };
        }
    }
}

