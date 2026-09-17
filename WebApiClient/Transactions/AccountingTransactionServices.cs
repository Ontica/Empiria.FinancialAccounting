/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Accounting Transactions                    Component : Data Layer                              *
*  Assembly : FinancialAccounting.WebApiClient.dll       Pattern   : Web api client                          *
*  Type     : AccountingTransactionServices              License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Provides financial accounting transactions services using a web proxy.                         *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System.Threading.Tasks;

using Empiria.Financial.Transactions.Adapters;

namespace Empiria.FinancialAccounting.ClientServices {

  /// <summary>Provides financial accounting transactions services using a web proxy.</summary>
  public class AccountingTransactionServices : BaseService {

    public Task<int> NotifyTransaction(FinancialTransactionDto transactionDto) {

      string path = "v2/financial-accounting/transactions";

      return WebApiClient.PostAsync<int>(transactionDto, path);
    }

  }  // class AccountingTransactionServices

}  // namespace Empiria.FinancialAccounting.ClientServices
