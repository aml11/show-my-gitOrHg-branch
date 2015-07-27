﻿using System;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace ShowMyGitBranch_IntegrationTests.IntegrationTests {
    [TestClass]
    public class CPPProjectTests {
        #region fields

        private delegate void ThreadInvoker();

        private TestContext _testContext;

        #endregion

        #region properties

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get { return _testContext; }
            set { _testContext = value; }
        }

        #endregion

        #region ctors

        public CPPProjectTests() {}

        #endregion

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion

        [HostType("VS IDE")]
        [TestMethod]
        public void CPPWinformsApplication() {
            UIThreadInvoker.Invoke((ThreadInvoker) delegate() {
                //Solution and project creation parameters
                var solutionName = "CPPWinApp";
                var projectName = "CPPWinApp";

                //Template parameters
                var projectType = "{8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942}";
                var projectTemplateName = Path.Combine("vcNet", "mc++appwiz.vsz");

                var itemTemplateName = "newc++file.cpp";
                var newFileName = "Test.cpp";

                var dte = (DTE) VsIdeTestHostContext.ServiceProvider.GetService(typeof (DTE));

                var testUtils = new TestUtils();

                testUtils.CreateEmptySolution(TestContext.TestDir, solutionName);
                Assert.AreEqual<int>(0, testUtils.ProjectCount());

                //Add new CPP Windows application project to existing solution
                var solutionDirectory = Directory.GetParent(dte.Solution.FullName).FullName;
                var projectDirectory = TestUtils.GetNewDirectoryName(solutionDirectory, projectName);
                var projectTemplatePath = Path.Combine(dte.Solution.get_TemplatePath(projectType), projectTemplateName);
                Assert.IsTrue(File.Exists(projectTemplatePath), string.Format("Could not find template file: {0}", projectTemplatePath));
                dte.Solution.AddFromTemplate(projectTemplatePath, projectDirectory, projectName, false);

                //Verify that the new project has been added to the solution
                Assert.AreEqual<int>(1, testUtils.ProjectCount());

                //Get the project
                var project = dte.Solution.Item(1);
                Assert.IsNotNull(project);
                Assert.IsTrue(string.Compare(project.Name, projectName, StringComparison.InvariantCultureIgnoreCase) == 0);

                //Verify Adding new code file to project
                var newItemTemplatePath = Path.Combine(dte.Solution.ProjectItemsTemplatePath(projectType), itemTemplateName);
                Assert.IsTrue(File.Exists(newItemTemplatePath));
                var item = project.ProjectItems.AddFromTemplate(newItemTemplatePath, newFileName);
                Assert.IsNotNull(item);
            });
        }
    }
}