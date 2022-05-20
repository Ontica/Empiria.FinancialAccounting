/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Balance Engine                             Component : Test Helpers                            *
*  Assembly : FinancialAccounting.BalanceEngine.Tests    Pattern   : Common Testing Methods                  *
*  Type     : CommonMethods                              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides common testing methods.                                                               *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;
using System.IO;
using System.Threading;

using Empiria.Contacts;
using Empiria.Json;
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


    static public void SetDefaultJsonSettings() {
      Newtonsoft.Json.JsonConvert.DefaultSettings = () => JsonConverter.JsonSerializerDefaultSettings();
    }


    static public T ReadTestDataFromFile<T>(string fileNamePrefix) {
      var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

      string path = Path.Combine(directory.Parent.FullName,
                                @"tests-data",
                                $"{fileNamePrefix}.test-data.json");

      var jsonString = File.ReadAllText(path);

      return JsonConverter.ToObject<T>(jsonString);
    }


    #endregion Auxiliary methods

  }  // CommonMethods

}  // namespace Empiria.FinancialAccounting.Tests
