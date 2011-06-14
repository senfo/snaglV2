//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using Berico.SnagL.Infrastructure.Configuration;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Berico.SnagL.Infrastructure.Tests
{
    /// <summary>
    /// Contains unit tests to validate the functionality of the ConfigurationManager class
    /// </summary>
    [TestClass]
    public class ConfigurationManagerTests : SilverlightTest
    {
        /// <summary>
        /// Ensures an exception is thrown if both parameters to copy are null
        /// </summary>
        [TestMethod]
        public void Should_Throw_Exception_If_Params_Null()
        {
            bool exceptionThrown = false;
            string exceptionMessage = String.Empty;

            try
            {
                ConfigurationManager.MergeProperties(this.GetType(), null, null);
            }
            catch (InvalidOperationException e)
            {
                exceptionThrown = true;
                exceptionMessage = e.Message;
            }

            Assert.AreEqual("Unable to obtain property information from either parameter", exceptionMessage, false);
            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Ensures an exception is thrown if either of the two parameters don't match the specified type
        /// </summary>
        [TestMethod]
        public void Should_Throw_Error_If_Types_Differ()
        {
            bool exceptionThrown = false;
            string exceptionMessage = String.Empty;

            try
            {
                ConfigurationManager.MergeProperties(this.GetType(), String.Empty, String.Empty);
            }
            catch (InvalidOperationException e)
            {
                exceptionThrown = true;
                exceptionMessage = e.Message;
            }

            Assert.AreEqual("One or more parameters do not match the specified object type", exceptionMessage, false);
            Assert.IsTrue(exceptionThrown);
        }

        /// <summary>
        /// Ensures paramA is returned if paramB is null
        /// </summary>
        [TestMethod]
        public void Should_Return_ParamA_If_TypeB_Is_Null()
        {
            string expected = "ParamA";
            string actual = ConfigurationManager.MergeProperties(typeof(String), expected, null) as String;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Ensures paramB is returned if paramA is null
        /// </summary>
        [TestMethod]
        public void Should_Return_ParamB_If_TypeA_Is_Null()
        {
            string expected = "ParamB";
            string actual = ConfigurationManager.MergeProperties(typeof(String), null, expected) as String;

            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Ensures values from paramB are copied to paramA
        /// </summary>
        [TestMethod]
        public void Values_From__ParamB_Should_Be_Copied_To_ParamA()
        {
            int age = 21;
            string firstName = "Fred";
            string lastName = "Flinstone";

            ShallowClass shallow1 = new ShallowClass
            {
                LastName = lastName
            };

            ShallowClass shallow2 = new ShallowClass
            {
                Age = age,
                FirstName = firstName
            };

            ShallowClass actual = ConfigurationManager.MergeProperties(typeof(ShallowClass), shallow1, shallow2) as ShallowClass;

            Assert.AreEqual<int>(age, actual.Age);
            Assert.AreEqual(firstName, actual.FirstName, false);
            Assert.AreEqual(lastName, actual.LastName, false);
        }

        /// <summary>
        /// Ensures values from paramB override values from paramA
        /// </summary>
        [TestMethod]
        public void Values_From_ParamB_ShouldOverride_ParamA()
        {
            int age = 21;
            string firstName = "Fred";
            string lastName = "Flinstone";

            ShallowClass shallow1 = new ShallowClass
            {
                Age = 2,
                FirstName = "George",
                LastName = "Jetson"
            };

            ShallowClass shallow2 = new ShallowClass
            {
                Age = age,
                FirstName = firstName,
                LastName = lastName
            };

            ShallowClass actual = ConfigurationManager.MergeProperties(typeof(ShallowClass), shallow1, shallow2) as ShallowClass;

            Assert.AreEqual<int>(age, actual.Age);
            Assert.AreEqual(firstName, actual.FirstName, false);
            Assert.AreEqual(lastName, actual.LastName, false);
        }

        #region Subclasses

        /// <summary>
        /// Contains properties for testing the functionality of the merge method
        /// </summary>
        public class ShallowClass
        {
            #region Properties

            /// <summary>
            /// Gets or sets the age
            /// </summary>
            public int Age
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the first name
            /// </summary>
            public string FirstName
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the last name
            /// </summary>
            public string LastName
            {
                get;
                set;
            }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the ShallowClass class
            /// </summary>
            public ShallowClass()
            {
            }

            #endregion
        }

        #endregion
    }
}