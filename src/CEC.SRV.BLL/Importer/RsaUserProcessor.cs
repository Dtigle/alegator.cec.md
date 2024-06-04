using System;
using Amdaris;
using Amdaris.Domain;
using Amdaris.Domain.Identity;
using CEC.SRV.BLL.Exceptions;
using CEC.SRV.Domain;
using CEC.SRV.Domain.Importer;
using CEC.SRV.Domain.Lookup;
using CEC.Web.SRV.Resources;
using Microsoft.AspNet.Identity;

namespace CEC.SRV.BLL.Importer
{
    public class RsaUserProcessor : ImportProcesser<RsaUser>
    {
        private readonly UserManager<SRVIdentityUser> _userManager;

        private readonly IRepository _repository;


        public RsaUserProcessor(IRepository repository, UserManager<SRVIdentityUser> userManager, ILogger logger, int batchSize)
            : base(repository, logger, batchSize)
        {
            _repository = repository;
            _userManager = userManager;
        }

        protected override void ProcessInternal(RsaUser rawData)
        {
            var userValidation = _userManager.FindByNameAsync(rawData.LoginName).Result;
            if (userValidation != null)
                throw new SrvException("Create_UniqueError", MUI.Create_UniqueError);

            var role = _repository.Get<IdentityRole, string>("2");
            if (role == null)
            {
                throw new Exception("Can not find userRole with id = (2)");
            }

            var user = new SRVIdentityUser
            {
                UserName = rawData.LoginName,
                AdditionalInfo = new AdditionalUserInfo
                {
                    FirstName = rawData.LoginName,
                    LastName = rawData.LoginName,
                    
                    Gender = _repository.Get<Gender>(1),
                },
                Comments = "Importat din RSA"
            };
            user.Roles.Add(role);
            var result = _userManager.CreateAsync(user, rawData.Password).Result;
            if (!result.Succeeded)
            {
                throw new Exception(string.Format("Unknow error occurs during creating user"));
            }

            user.AddRegion(rawData.Region);

            _repository.SaveOrUpdate(user);
        }

        protected override void NotifySuccess(RsaUser rawData)
        {
            Delete(rawData);
        }

        protected override void NotifyFailure(RsaUser rawData)
        {
            
        }
    }
}