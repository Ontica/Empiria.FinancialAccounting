/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounts                                   Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Web api client                          *
*  Type     : AccountsServices                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides financial accounting accounts services using a web proxy.                             *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting accounts services using a web proxy.</summary>
  public class AccountsServices : BaseService {

    public Task<FixedList<NamedEntityDto>> SearchSuppliersSubledgerAccounts(NamedEntityFields fields) {
      Assertion.Require(fields, nameof(fields));

      string path = "v2/financial-accounting/subledger-accounts/search-suppliers";

      return WebApiClient.PostAsync<FixedList<NamedEntityDto>>(fields, path);
    }

  }  // class AccountsServices

}  // namespace Empiria.FinancialAccounting.ClientServices
