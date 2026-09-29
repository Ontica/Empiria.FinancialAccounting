/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounting Transactions                    Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Web api client                          *
*  Type     : AccountingClassificationsServices          License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides financial accounting classifications services using a web proxy.                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

using Empiria.FinancialAccounting.Transactions.Adapters;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting classifications services using a web proxy.</summary>
  public class AccountingClassificationsServices : BaseService {

    static private FixedList<AccountingClassificationDto> _accountingClassifications;

    public async Task<FixedList<AccountingClassificationDto>> GetAccountingClassifications() {

      if (_accountingClassifications != null) {
        return await Task.FromResult(_accountingClassifications);
      }

      string path = "v2/financial-accounting/rules/accounting-classifications";

      _accountingClassifications = await WebApiClient.GetAsync<FixedList<AccountingClassificationDto>>(path);

      return _accountingClassifications;
    }

  }  // class AccountingClassificationsServices

}  // namespace Empiria.FinancialAccounting.ClientServices
