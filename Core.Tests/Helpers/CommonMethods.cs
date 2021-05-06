/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Core                  Component : Test Helpers                            *
*  Assembly : Empiria.FinancialAccounting.Tests.dll      Pattern   : Common Testing Methods                  *
*  Type     : CommonMethods                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides common testing methods.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using System.Threading;

using Empiria.Contacts;
using Empiria.Security;

namespace Empiria.FinancialAccounting.Tests {

  /// <summary>Provides common testing methods.</summary>
  static public class CommonMethods {

    #region Auxiliary methods

    static public void Authenticate() {
      string sessionToken = TestingConstants.SESSION_TOKEN;

      Thread.CurrentPrincipal = AuthenticationService.Authenticate(sessionToken);
    }


    static public Contact GetCurrentUser() {
      return Contact.Parse(ExecutionServer.CurrentUserId);
    }

    #endregion Auxiliary methods

  }  // CommonMethods

}  // namespace Empiria.FinancialAccounting.Tests
