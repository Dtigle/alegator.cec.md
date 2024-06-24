using System;
using System.Linq;
using CEC.SRV.BLL.Impl;
using CEC.SRV.Domain.Importer.ToSaise;
using CEC.SRV.Domain.Print;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CEC.SRV.Domain;
using CEC.Web.SRV.Resources;

namespace CEC.SRV.BLL.Tests.Impl
{
    [TestClass]
    public class ElectionBllTests : BaseBllTests
    {
        private ElectionBll _bll;
        
        [TestInitialize]
        public void Startup2()
        {
            _bll = CreateBll<ElectionBll>();
        }
        
        [TestMethod]
        public void SaveOrUpdateByFalseAcceptAbroadDeclarationAndZeroElectionId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var electionType = GetFirstObjectFromDbTable(GetElectionType);
            var electionTypeId = electionType.Id;
            var electionDate = new DateTime(2015, 1, 1);
            const long saiseId = 0;
            const string comments = "comentarii";

            // Act
            
            //SafeExec(_bll.SaveOrUpdate, 0L, electionTypeId, electionDate, saiseId, comments, false);
            
            // Assert

            var election = GetLastCreatedObject<Election>();

            Assert.IsNotNull(election);
            Assert.AreSame(electionType, election.ElectionType);
            Assert.AreEqual(electionDate, election.ElectionRounds.LastOrDefault());
            //Assert.AreEqual(saiseId, election.SaiseId);
            Assert.AreEqual(comments, election.Description);
            Assert.IsFalse(election.ElectionType.AcceptAbroadDeclaration);
        }

        [TestMethod]
        public void SaveOrUpdateByTrueAcceptAbroadDeclarationAndZeroElectionId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            var electionType = GetFirstObjectFromDbTable(GetElectionType);
            var oldElections = GetAllObjectsFromDbTable<Election>();
            var electionTypeId = electionType.Id;
            var electionDate = new DateTime(2015, 1, 1);
            const long saiseId = 0;
            const string comments = "comentarii";

            // Act

            //SafeExec(_bll.SaveOrUpdate, 0L, electionTypeId, electionDate, saiseId, comments, true);
            
            // Assert

            var election = GetLastCreatedObject<Election>();

            Assert.IsNotNull(election);
            Assert.AreSame(electionType, election.ElectionType);
            Assert.AreEqual(electionDate, election.ElectionRounds.LastOrDefault().ElectionDate);
            //Assert.AreEqual(saiseId, election.SaiseId);
            Assert.AreEqual(comments, election.Description);
            Assert.IsTrue(election.ElectionType.AcceptAbroadDeclaration);

            var elections = GetAllObjectsFromDbTable<Election>(x => x.Id != election.Id);
            Assert.IsFalse(elections.Exists(x => x.ElectionType.AcceptAbroadDeclaration));
        }

        [TestMethod]
        public void SaveOrUpdateByFalseAcceptAbroadDeclarationAndNonZeroElectionId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var electionType = GetFirstObjectFromDbTable(GetElectionType);
            var oldElection = GetFirstObjectFromDbTable(GetElection);
            var electionTypeId = electionType.Id;
            var electionId = oldElection.Id;
            var electionDate = new DateTime(2015, 1, 1);
            const long saiseId = 0;
            const string comments = "comentarii";
            const bool acceptAbroadDeclaration = false;

            // Act

            //SafeExec(_bll.SaveOrUpdate, electionId, electionTypeId, electionDate, saiseId, comments, acceptAbroadDeclaration);

            // Assert

            var election = GetFirstObjectFromDbTable<Election>(x => x.Id == electionId);

            Assert.IsNotNull(election);
            Assert.AreSame(electionType, election.ElectionType);
            Assert.AreEqual(electionDate, election.ElectionRounds.LastOrDefault().ElectionDate);
            //Assert.AreEqual(saiseId, election.SaiseId);
            Assert.AreEqual(comments, election.Description);
            Assert.IsFalse(election.ElectionType.AcceptAbroadDeclaration);
            Assert.AreEqual(electionId, election.Id);
        }

        [TestMethod]
        public void SaveOrUpdateByTrueAcceptAbroadDeclarationFalseElectionAcceptAbroadDeclarationAndNonZeroElectionId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var electionType = GetFirstObjectFromDbTable(GetElectionType);
            var oldElection = GetFirstObjectFromDbTable(x => !x.ElectionType.AcceptAbroadDeclaration, GetElection);
            var oldElections = GetAllObjectsFromDbTable<Election>();
            var electionTypeId = electionType.Id;
            var electionId = oldElection.Id;
            var electionDate = new DateTime(2015, 1, 1);
            const long saiseId = 0;
            const string comments = "comentarii";
            const bool acceptAbroadDeclaration = true;

            // Act

            //SafeExec(_bll.SaveOrUpdate, electionId, electionTypeId, electionDate, saiseId, comments, acceptAbroadDeclaration);

            // Assert

            var election = GetFirstObjectFromDbTable<Election>(x => x.Id == electionId);

            Assert.IsNotNull(election);
            Assert.AreSame(electionType, election.ElectionType);
            Assert.AreEqual(electionDate, election.ElectionRounds.LastOrDefault().ElectionDate);
            //Assert.AreEqual(saiseId, election.SaiseId);
            Assert.AreEqual(comments, election.Description);
            Assert.IsTrue(election.ElectionType.AcceptAbroadDeclaration);

            var elections = GetAllObjectsFromDbTable<Election>(x => x.Id != electionId);
            Assert.IsFalse(elections.Exists(x => x.ElectionType.AcceptAbroadDeclaration));
        }

        [TestMethod]
        public void SaveOrUpdateByTrueAcceptAbroadDeclarationTrueElectionAcceptAbroadDeclarationAndNonZeroElectionId_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            var electionType = GetFirstObjectFromDbTable(GetElectionType);
            var oldElection = GetFirstObjectFromDbTable(x => x.ElectionType.AcceptAbroadDeclaration, GetElectionWithAbroadDeclarations);
            var electionTypeId = electionType.Id;
            var electionId = oldElection.Id;
            var electionDate = new DateTime(2015, 1, 1);
            const long saiseId = 0;
            const string comments = "comentarii";
            const bool acceptAbroadDeclaration = true;

            // Act

            //SafeExec(_bll.SaveOrUpdate, electionId, electionTypeId, electionDate, saiseId, comments, acceptAbroadDeclaration);

            // Assert

            var election = GetFirstObjectFromDbTable<Election>(x => x.Id == electionId);

            Assert.IsNotNull(election);
            Assert.AreSame(electionType, election.ElectionType);
            Assert.AreEqual(electionDate, election.ElectionRounds.LastOrDefault().ElectionDate);
            //Assert.AreEqual(saiseId, election.SaiseId);
            Assert.AreEqual(comments, election.Description);
            Assert.IsTrue(election.ElectionType.AcceptAbroadDeclaration);
        }

        [TestMethod]
        public void RemoveOtherAcceptAbroadDeclarations_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();

            GetFirstObjectFromDbTable(GetElection);
            GetFirstObjectFromDbTable(GetElectionWithAbroadDeclarations);

            var oldElections = GetAllObjectsFromDbTable<Election>();

            // Act

            SafeExec("RemoveOtherAcceptAbroadDeclarations");
            
            // Assert

            var elections = GetAllObjectsFromDbTable<Election>();
            Assert.IsFalse(elections.Exists(x => x.ElectionType.AcceptAbroadDeclaration));
        }

        [TestMethod]
        public void GetCurrentElection_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();
            
            var expectedElection = GetFirstObjectFromDbTable(x => x.Deleted == null && x.ElectionType.AcceptAbroadDeclaration == true, GetElectionWithAbroadDeclarations);
            
            // Act

            var election = SafeExecFunc(_bll.GetCurrentElection);
            
            // Assert

            Assert.IsNotNull(election);
            Assert.AreSame(expectedElection, election);
        }

        [TestMethod]
        public void GetActiveElections_returns_correct_result()
        {
            // Arrange

            SetAdministratorRole();

            var expectedElection = GetFirstObjectFromDbTable(x => x.Deleted == null, GetElection);
            var expectedElections = GetAllObjectsFromDbTable<Election>(x => x.Deleted == null);

            // Act

            var result = SafeExecFunc(_bll.GetActiveElections);
            
            // Assert

            Assert.IsNotNull(result);

            var elections = result.ToList();

            Assert.IsTrue(elections.Contains(expectedElection));
            AssertListsAreEqual(expectedElections, elections);
        }

        [TestMethod]
        public void VerificationIfElectionHasReferenceByExistingSaiseExporterWithGivenElection_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var saiseExporter = GetFirstObjectFromDbTable(x => (x.Deleted == null), GetSaiseExporter);
            var electionId = saiseExporter.ElectionDayId;

            // Act & Assert

            SafeExec(_bll.VerificationIfElectionHasReference, electionId, true, false, string.Empty, 
                String.Format(MUI.HasReference_Error, saiseExporter.ElectionDayId, saiseExporter.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfElectionHasReferenceByExistingPrintSessionWithGivenElection_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var printSession = GetFirstObjectFromDbTable(x => (x.Election != null) && (x.Deleted == null), GetPrintSession);
            var electionId = printSession.Election.Id;

            var saiseExporters = GetAllObjectsFromDbTable<SaiseExporter>(x => (x.ElectionDayId == electionId) && (x.Deleted == null));
            saiseExporters.ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfElectionHasReference, electionId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, printSession.Election.GetObjectType(), printSession.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfElectionHasReferenceByExistingStayStatementWithGivenElection_throws_an_exception()
        {
            // Arrange

            SetAdministratorRole();

            var stayStatement = GetFirstObjectFromDbTable(x => (x.ElectionInstance != null) && (x.Deleted == null), GetStayStatement);
            var electionId = stayStatement.ElectionInstance.Id;

            var saiseExporters = GetAllObjectsFromDbTable<SaiseExporter>(x => (x.ElectionDayId == electionId) && (x.Deleted == null));
            saiseExporters.ForEach(Repository.Delete);

            var printSessions = GetAllObjectsFromDbTable<PrintSession>(x => (x.Election.Id == electionId) && (x.Deleted == null));
            printSessions.ForEach(Repository.Delete);

            // Act & Assert

            SafeExec(_bll.VerificationIfElectionHasReference, electionId, true, false, string.Empty,
                String.Format(MUI.HasReference_Error, stayStatement.ElectionInstance.GetObjectType(), stayStatement.GetObjectType()));
        }

        [TestMethod]
        public void VerificationIfElectionHasReferenceByNonExistingReferences_has_correct_logic()
        {
            // Arrange

            SetAdministratorRole();
            
            // Act & Assert

            SafeExec(_bll.VerificationIfElectionHasReference, -1L);
        }
    }
}
